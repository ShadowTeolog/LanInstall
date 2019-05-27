using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;

namespace RedistServ
{
    
    public static class Hub
    {
        public static string RepositoryPath => Path.Combine(Directory.GetCurrentDirectory(), "repository");
        private static readonly IRedistributionServer redistribution = new RedistributionServer(RepositoryPath);
        private static readonly INetwork network = NetworkHelper.CreateNetwork();
        private static readonly Configuration config = Configuration.Load();
        
        private static Server Server;
        
        public static void Start()
        {
            redistribution.Start();
            network.Start();
            Server=new Server(network);
        }

        public static void Shutdown() => redistribution.Shutdown();
    }
}