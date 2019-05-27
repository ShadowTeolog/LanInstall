using System;
using System.IO;
using System.Net;
using System.Threading;
using RedistDto;

namespace RedistServ
{
    internal class Client
    {
        private CommandId? currentcommand=null; 
        readonly INetwork _network;
        private readonly Configuration _config;
        private readonly IRedistributionClient _redistribution;
        private readonly System.Threading.Timer timer; 
        public Client(INetwork network, Configuration config, IRedistributionClient redistribution)
        {
            _network = network;
            _config = config;
            _redistribution = redistribution;
            _network.DataReceivedEventHandler += OnRequest;
            timer=new Timer(UpdateTimer,this,100,1000);
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
            }

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

                    }
                }
            };

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
                if (message.Target == _config.UnicalId)
                {
                    var servercommantr = message.ServerCommand;
                    HandleServerCommand(servercommantr, ip);
                }
                
                
            }
        }

        private void HandleServerCommand(NetworkCommand servercommantr, IPAddress ip)
        {
            currentcommand = servercommantr.Id;
            _redistribution.UpdateRoles(servercommantr.Id,servercommantr.Roles,ip);
        }
        public void Shutdown()
        {
            _network.DataReceivedEventHandler -= OnRequest;
        }
    }
}
