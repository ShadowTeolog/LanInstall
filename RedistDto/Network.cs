using System;
using System.Net;

namespace RedistServ
{
    public static class NetworkHelper
    {
        public static INetwork CreateNetwork() => new Network();
    }

    public interface INetwork : IDisposable
    {
        void Start(string targetip);
        void Shutdown();
        void Send(byte[] data, long length);
        event Action<byte[], long, IPAddress> DataReceivedEventHandler;
    }

    internal class Network : INetwork
    {
        private const string MulticastNetwork = "224.5.6.7";
        private MulticastUdpExchange exchange;
        public event Action<byte[], long, IPAddress> DataReceivedEventHandler;

        public void Start(string targetip)
        {
            var setip = string.IsNullOrWhiteSpace(targetip)
                ? MulticastUdpExchange.DefaultBroadcastInterface()
                : IPAddress.Parse(targetip);
            exchange = new MulticastUdpExchange(setip, 9415, IPAddress.Parse(MulticastNetwork));
            exchange.ReciveHandler += Exchange_ReciveHandler;
            exchange.Initialize();
        }

        public void Shutdown()
        {
            var old = exchange;
            exchange = null;
            old?.Dispose();
        }

        public void Send(byte[] data, long length) => exchange?.SendMulticast(data, (int) length);

        public void Dispose()
        {
            Shutdown();
        }

        private void Exchange_ReciveHandler(byte[] data, int len, IPEndPoint arg3) =>
            DataReceivedEventHandler?.Invoke(data, len, arg3.Address);
    }
}