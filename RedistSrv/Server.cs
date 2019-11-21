using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using ProtoBuf;
using RedistDto;

namespace RedistServ
{
    public interface IRedistributionServer
    {
        void Start();
        void Restart();
        void Shutdown();
        event Action<string> HandleLogEvent;
        event Action<string> HandleErrorEvent;
    }

    internal class NodeInfo
    {
        public ulong Id;
        public RoleSet roles;
    }

    internal class Server
    {
        private readonly Timer _timer;
        private DateTime _lastserverRectTime;
        private static readonly RoleSet DefaultRoles = new RoleSet
        {
            Roles = new[]
            {
                new Role {
                    Name = "NewVisualsLTS",
                    Repository = "NewVisualsLFS",
                    StartupFile = "Visualisation.exe",
                    Branch = "master"},
                new Role {Name = "RedistClient", Repository = "RedistClient",Branch = "master"}
            }
        };

        private readonly INetwork _network;
        private readonly Configuration _config;
        private readonly ConcurrentDictionary<ulong, NodeInfo> _states = new ConcurrentDictionary<ulong, NodeInfo>();

        public Server(INetwork network, Configuration config)
        {
            _network = network;
            _config = config;
            _network.DataReceivedEventHandler += OnRequest;
            UpdateLastRectFromserverTime();
            _timer=new Timer(UpdateTimer,this,100,1000);
        }

        private void UpdateTimer(object state)
        {
            if (IfNoServMessageAtLeast(10))
            {
                _network.Shutdown();
                _network.Start(_config.MulticastInterface);
            }
            SendCommand(0,new NetworkCommand() {Echo=true});
        }
        private void UpdateLastRectFromserverTime() => _lastserverRectTime=DateTime.UtcNow;

        private bool IfNoServMessageAtLeast(double seconds)
        {
            var period = (DateTime.UtcNow - _lastserverRectTime).TotalSeconds;
            return (period < 0 || period > seconds);
        }

        private void SendCommand(ulong target, NetworkCommand command)
        {
            var message = new NetworkMessage
            {
                UnicalMessageTargetId = target,
                ServerCommand = command
            };
            using (var stream = new MemoryStream(1500))
            {
                Serializer.Serialize(stream, message);
                _network.Send(stream.GetBuffer(), stream.Length);
            }
        }

        private void OnRequest(byte[] data, long datalen, IPAddress address)
        {
            using (var stream = new MemoryStream(data, 0, (int) datalen))
            {
                var message = Serializer.Deserialize<NetworkMessage>(stream);
                if (message?.ClientMessage != null)
                {
                    UpdateLastRectFromserverTime();
                    HandleClientRequest(message.ClientMessage);
                }
            }
        }

        private void HandleClientRequest(ClientMessage clientMessage)
        {
            if (clientMessage.ConfigRequest?.NeedConfig==true)
                HandleConfigRequest(clientMessage.SourceId, clientMessage.ConfigRequest);
        }

        private void HandleConfigRequest(ulong sourceId, ConfigRequest configRequest)
        {
            NodeInfo node;
            if (!_states.TryGetValue(sourceId, out node))
            {
                node = new NodeInfo
                {
                    Id = sourceId,
                    roles = DefaultRoles
                };
                _states[sourceId] = node;
            }

            SendCommand(sourceId, new NetworkCommand
            {
                Id = CommandId.UpdateEndRestart,
                Roles = node.roles
            });
        }

        public void SendCommandToAll(CommandId command)
        {
            var set = _states.Values.ToArray();
            foreach (var target in set)
            {
                SendCommand(target.Id, new NetworkCommand
                {
                    Id = command,
                    Roles = target.roles
                });
            }
        }
    }
}