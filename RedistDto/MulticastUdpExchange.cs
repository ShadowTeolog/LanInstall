using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NLog;

internal class MulticastUdpExchange : IDisposable
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public event Action<byte[],int,IPEndPoint> ReciveHandler;

    private uint BindPort;
    private readonly IPAddress _group;
    private IPAddress _interface;
    private IPEndPoint MulticastTarget;
    protected UdpClient Socket;
    private const int largestPossibleDatagram = 100000;

    public MulticastUdpExchange(IPAddress Interface, uint port, IPAddress group)
    {
        _interface = Interface;
        BindPort = port;
        _group = group;

    }

    public bool Initialize()
    {
        if (!Bind(_interface, BindPort))
            return false;
        if (!JoinMulticast(_group, _interface))
            return false;
        return StartRecive();
    }


    private bool Bind(IPAddress Interface, uint port)
    {
        try
        {
            Socket = new UdpClient
            {
                EnableBroadcast = true,
                ExclusiveAddressUse = false,
                MulticastLoopback = true
            };
            BindPort = port;
            var local = new IPEndPoint(Interface, (int) port);
            Socket.Client.ExclusiveAddressUse = false;
            Socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            Socket.Client.ReceiveBufferSize = largestPossibleDatagram;
            Socket.Client.SendBufferSize = largestPossibleDatagram;
            Socket.Client.Bind(local);
            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
        return false;
    }

    public bool Send(byte[] message, int len, IPEndPoint endPoint)
    {
        return Socket?.Client.SendTo(message, 0, len, SocketFlags.None, endPoint) == len;
    }
    public bool SendMulticast(byte[] message, int len)
    {
        return Send(message, len, MulticastTarget);
    }

    private bool StartRecive()
    {
        Socket?.BeginReceive(OnRecive, this);
        return true;
    }

    private void OnRecive(IAsyncResult result)
    {
        try
        {
            if (Socket != null)
            {
                var endpoint = new IPEndPoint(IPAddress.Any, 0);
                var receiveBytes = Socket?.EndReceive(result, ref endpoint);
                if (receiveBytes != null && receiveBytes.Length > 0)
                    ReciveHandler?.Invoke(receiveBytes, receiveBytes.Length, endpoint);
                StartRecive();
            }
        }
        catch (ObjectDisposedException e)
        {
            //ok
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private bool JoinMulticast(IPAddress group, IPAddress Interface)
    {
        try
        {
            MulticastTarget = new IPEndPoint(group, (int) BindPort);
            Socket.JoinMulticastGroup(group, Interface);
            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
        return false;
    }
    public void Dispose()
    {
        
        if (Socket != null)
        {
            var client = Socket;
            Socket = null;

            client?.DropMulticastGroup(MulticastTarget.Address);
            client?.Dispose();
            
        }
    }
    public static IPAddress DefaultBroadcastInterface() {
        var adapters = NetworkInterface.GetAllNetworkInterfaces();
        foreach (var adapter in adapters) {
            if (adapter.NetworkInterfaceType!=NetworkInterfaceType.Loopback &&
                adapter.Supports(NetworkInterfaceComponent.IPv4) && adapter.SupportsMulticast)
                if (adapter.OperationalStatus == OperationalStatus.Up ||
                    adapter.OperationalStatus == OperationalStatus.Testing) {
                    var unicasts = adapter.GetIPProperties().UnicastAddresses;
                    if(unicasts.Count>0)
                        foreach (var addr in unicasts) {
                            if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                                return addr.Address;
                        }
                }
        }
        return IPAddress.Loopback;
    }
    
}