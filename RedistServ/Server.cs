﻿using System;
using System.IO;
using RedistDto;

namespace RedistServ
{
    public interface INetwork
    {
        void Send(byte[] data, long length);
    }
    public class Server
    {
        INetwork _network;
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
        public Server(INetwork network)
        {
            _network = network;
        }
        public void OnRequest(byte[] data, long datalen)
        {
            using (var stream = new MemoryStream(data, (int)datalen))
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
        private static RoleSet test = new RoleSet() {Roles=new Role[] { new Role() {Name="VisualsNodeWin",Repository="VisualsNodeWin", StartupFile="VisualNode2.exe" } }
         };
        private void HandleConfigRequest(ulong sourceId, ConfigRequest configRequest)
        {
            SendCommand(sourceId, new NetworkCommand()
            {
                Id = RedistDto.CommandId.UpdateEndRestart,
                Roles = test
            }
            );
        }
    }
}
