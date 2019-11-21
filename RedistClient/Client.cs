using System;
using System.IO;
using System.Net;
using System.Threading;
using NLog;
using RedistDto;

namespace RedistServ
{
    internal class Client 
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public event Action<string> StateChangeEvent;
        private CommandId? currentcommand=null; 
        readonly INetwork _network;
        private readonly Configuration _config;
        private readonly IRedistributionClient _redistribution;
        private readonly Timer _timer;
        private DateTime _lastserverRectTime;
        public Client(INetwork network, Configuration config, IRedistributionClient redistribution)
        {
            _network = network;
            _config = config;
            _redistribution = redistribution;
            _network.DataReceivedEventHandler += OnRequest;
            UpdateLastRectFromserverTime();
            _timer=new Timer(UpdateTimer,this,100,1000);
        }

        

        private void UpdateTimer(object state)
        {
            if (Monitor.TryEnter(this))
            {
                try
                {
                    if (!currentcommand.HasValue)
                        SendState();
                }
                finally
                {
                    Monitor.Exit(this);
                }
                if (IfNoServMessageAtLeast(10))
                {
                    Logger.Trace("Reconnect network after to long silent interval");
                    UpdateLastRectFromserverTime();
                    _network.Shutdown();
                    _network.Start(_config.MulticastInterface);
                }
            }
            
            SendMessage(new NetworkMessage() {ClientMessage = new ClientMessage() {Echo = true}});
        }

        void SendState()
        {
            var message = new NetworkMessage()
            {
                ClientMessage = new ClientMessage()
                {
                    SourceId = _config.UnicalId,
                    ConfigRequest = new ConfigRequest()
                    {
                        NeedConfig = true
                    }
                }
            };
            SendMessage(message);
        }

        void SendMessage(NetworkMessage message)
        {
            using (var stream = new MemoryStream(1500))
            {
                ProtoBuf.Serializer.Serialize(stream, message);
                _network.Send(stream.GetBuffer(), stream.Length);
            }
        }
        
        private void OnRequest(byte[] data, long datalen,IPAddress ip)
        {
            using (var stream = new MemoryStream(data, 0,(int)datalen))
            {
                var message = ProtoBuf.Serializer.Deserialize<RedistDto.NetworkMessage>(stream);
                if (message.ClientMessage == null)
                    UpdateLastRectFromserverTime(); //this is server message,reset server connetivity
                
                if (message.UnicalMessageTargetId == _config.UnicalId)
                {
                    var servercommantr = message.ServerCommand;
                    HandleServerCommand(servercommantr, ip);
                }
            }
        }

        private void UpdateLastRectFromserverTime() => _lastserverRectTime=DateTime.UtcNow;

        private bool IfNoServMessageAtLeast(double seconds)
        {
            var period = (DateTime.UtcNow - _lastserverRectTime).TotalSeconds;
            return (period < 0 || period > seconds);
        }

        private void HandleServerCommand(NetworkCommand servercommantr, IPAddress ip)
        {
            currentcommand = servercommantr.Id;
            switch (currentcommand)
            {
                case CommandId.RestartNetwork:
                    Hub.RequestSystemShutdown();
                    return;
                case CommandId.ShutdownNetwork:
                    Hub.RequestSystemReboot();
                    return;
            }
            _redistribution.InvokeUpdateRoles(servercommantr.Id,servercommantr.Roles,ip);
        }
        public void Shutdown()
        {
            _timer.Dispose();
            _network.DataReceivedEventHandler -= OnRequest;
        }
    }
}
