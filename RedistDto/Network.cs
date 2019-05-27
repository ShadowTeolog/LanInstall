using System;
using System.Net;

namespace RedistServ
{
    public static class NetworkHelper
    {
        public static INetwork CreateNetwork()
        {
            return new Network();
        }
    }
    public interface INetwork
    {
        void Start();
        void Shutdown();
        void Send(byte[] data, long length);
        event Action<byte[], long,IPAddress> DataReceivedEventHandler;
    }

    internal class Network : INetwork
    {
        private const string MulticastNetwork = "224.5.6.7";
        public event Action<byte[], long,IPAddress> DataReceivedEventHandler;
        private MulticastUdpExchange exchange;
        public void Start()
        {
            var defip = MulticastUdpExchange.DefaultBroadcastInterface();
            exchange=new MulticastUdpExchange(defip,9415,IPAddress.Parse(MulticastNetwork));
            exchange.ReciveHandler += Exchange_ReciveHandler;
            exchange.Initialize();
        }

        private void Exchange_ReciveHandler(byte[] data, int len, IPEndPoint arg3)
        {
            DataReceivedEventHandler?.Invoke(data,len,arg3.Address);
        }

        public void Shutdown()
        {

            exchange.Dispose();
            exchange = null;
        }

        public void Send(byte[] data, long length) => exchange?.SendMulticast(data, (int) length);
    }
}