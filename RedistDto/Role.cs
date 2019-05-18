using System;
using ProtoBuf;

namespace RedistDto
{
    [ProtoContract]
    public class Role
    {
        [ProtoMember(1)] public string Name;
        [ProtoMember(2)] public string Repository;
        [ProtoMember(3,IsRequired = false)]  public string Branch;
        [ProtoMember(4,IsRequired = false)]  public string StartupFile;
        [ProtoMember(5, IsRequired = false)] public string Parameters;
    }
    [ProtoContract]
    public class RoleSet
    {
        [ProtoMember(1)] public Role[] Roles;
    }
}
