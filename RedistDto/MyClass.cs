using System;
using ProtoBuf;
namespace RedistDto
{
    [ProtoContract]
    public class NodeRole
    {
        [ProtoMember(1)]
        public string RoleName;
        [ProtoMember(1)] public string Repository;
        [ProtoMember(1)] public string Branch;
        [ProtoMember(1)] public string RunCommand;
    }
    [ProtoContract]
    public class NodeConfig
    {
        public NodeRole[] Roles;
    }
}
