﻿using System;
using ProtoBuf;
namespace RedistDto
{
    public enum EClientToServer
    {
        EConfigRequest=0,       //request for configuration
        EStatistics,            //notify about current statistics
    }
    [ProtoContract]
    public class ConfigRequest
    {

    }
    [ProtoContract]
    public class ClientStat
    {

    }
    [ProtoContract]
    public class ClientMessage
    {
        [ProtoMember(1)]
        public UInt64 SourceId;

        [ProtoMember(2,IsRequired =false)]
        public ConfigRequest ConfigRequest;

        [ProtoMember(3, IsRequired = false)]
        public ClientStat Stat;

    }
}
