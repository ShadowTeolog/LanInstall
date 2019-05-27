using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using RedistDto;

namespace RedistServ
{
    interface IRedistributionClient
    {
        void UpdateRoles(CommandId currentcommand, RoleSet roles, IPAddress ip);
    }

    public static class Hub
    {

        public static string RepositoryPath => Path.Combine(Directory.GetCurrentDirectory(), "repository");
        private static readonly IRedistributionClient redistribution = new RedistributionClient(RepositoryPath);
        private static readonly INetwork network = NetworkHelper.CreateNetwork();
        private static readonly Configuration config = Configuration.Load();
        
        private static Client _client;
        
        public static void Start()
        {
            network.Start();
            _client=new Client(network,config,redistribution);
        }

        public static void Shutdown()
        {
            _client.Shutdown();
            network.Shutdown();
        }
        
    }
}