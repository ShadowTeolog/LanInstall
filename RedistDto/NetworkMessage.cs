﻿using System;
using ProtoBuf;
namespace RedistDto
{
    [ProtoContract]
    public class NetworkMessage
    {
        [ProtoMember(1)] public UInt64 UnicalMessageTargetId;

        [ProtoMember(2,IsRequired =false)]
        public NetworkCommand ServerCommand;

        [ProtoMember(3, IsRequired = false)]
        public ClientMessage ClientMessage;
    }
}
