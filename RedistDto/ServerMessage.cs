using System;
using ProtoBuf;
namespace RedistDto
{
    public enum CommandId
    {
        SimpleRestart=0, //restart all nodes without any file actions
        CleanAndRestart, //do else directory content reset
        UpdateEndRestart, //also check for server repository for updates
        DryRun,           //also cleanup all downloaded files before all  
        RestartNetwork,    //do update for RedistClient repository and restart 
        ShutdownNetwork
    }
    [ProtoContract]
    public class NetworkCommand
    {

        [ProtoMember(2)] public CommandId Id;
        [ProtoMember(3,IsRequired =false)] public RoleSet Roles;
        [ProtoMember(4,IsRequired =false)] public bool Echo;
    }
}
