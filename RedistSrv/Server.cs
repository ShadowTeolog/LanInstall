using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using RedistDto;

namespace RedistServ
{
    public interface IRedistributionServer
    {
        void Start();
        void Restart();
        void Shutdown();
    }

    public class Server
    {
        readonly INetwork _network;
        public Server(INetwork network)
        {
            _network = network;
            _network.DataReceivedEventHandler += OnRequest;
        }
        
        void SendCommand(UInt64 target, RedistDto.NetworkCommand command)
        {
            var message = new RedistDto.NetworkMessage()
            {
                Target = target,
                ServerCommand = command
            };
            using (var stream = new MemoryStream(1500))
            {
                ProtoBuf.Serializer.Serialize(stream, message);
                _network.Send(stream.GetBuffer(), stream.Length);
            }

        }
        
        private void OnRequest(byte[] data, long datalen,IPAddress address)
        {
            using (var stream = new MemoryStream(data, 0,(int)datalen))
            {
                var message = ProtoBuf.Serializer.Deserialize<RedistDto.NetworkMessage>(stream);
                if (message?.ClientMessage != null)
                    HandleClientRequest(message?.ClientMessage);
            }
        }

        private void HandleClientRequest(ClientMessage clientMessage)
        {
            if (clientMessage.ConfigRequest != null)
                HandleConfigRequest(clientMessage.SourceId, clientMessage.ConfigRequest);
        }
        private static RoleSet test = new RoleSet() {Roles=new Role[] { new Role() {Name="Test",Repository="TestRep", StartupFile="UsbKDeviceTest.exe" } }
         };
        private void HandleConfigRequest(ulong sourceId, ConfigRequest configRequest)
        {
            SendCommand(sourceId, new NetworkCommand()
            {
                Id = RedistDto.CommandId.UpdateEndRestart,
                Roles = test
            });
        }
    }
}
