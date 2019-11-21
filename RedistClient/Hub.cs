using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using NLog;
using RedistDto;

namespace RedistServ
{
    interface IRedistributionClient
    {
        event Action<string> HandleLogEvent;
        event Action<string> HandleErrorEvent;
        event Action         UpdateStart;
        event Action         UpdateComplete;
        event Action<string> StateChangeEvent;
        void InvokeUpdateRoles(CommandId servercommantrId, RoleSet servercommantrRoles, IPAddress ip);
        void InvokeRestart();
    }

    public static class Hub
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static event Action<string> StateChangeEvent;
        public static event Action         UpdateStart;
        public static event Action         UpdateComplete;
        private static string RepositoryPath => Path.Combine(Directory.GetCurrentDirectory(), "repository");
        private static readonly IRedistributionClient redistribution = new RedistributionClient(RepositoryPath);
        private static readonly INetwork network = NetworkHelper.CreateNetwork();
        private static readonly Configuration config = Configuration.Load();
        
        private static Client _client;
        
        public static void Start()
        {
            network.Start(config.MulticastInterface);
            redistribution.UpdateStart += ()=>UpdateStart?.Invoke();
            redistribution.UpdateComplete += ()=>UpdateComplete?.Invoke();
            redistribution.StateChangeEvent += (s) => StateChangeEvent?.Invoke(s);

            redistribution.HandleLogEvent += _client_HandleLogEvent;
            redistribution.HandleErrorEvent += _client_HandleErrorEvent;

            _client=new Client(network,config,redistribution);
            _client.StateChangeEvent += (s) => StateChangeEvent?.Invoke(s);
        }

        

        public static void Shutdown()
        {
            _client.Shutdown();
            network.Shutdown();
        }

        private static void _client_HandleErrorEvent(string message) => Logger.Error(message);

        private static void _client_HandleLogEvent(string message) => Logger.Trace(message);

        public static void RequestSystemShutdown()
        {
            using (var process=new Process())
            {
                process.StartInfo = new ProcessStartInfo("shutdown", "-s -t 5 -f")
                    {CreateNoWindow = true, UseShellExecute = false};
                process.Start();
                process.WaitForExit();
            }
        }
        public static void RequestSystemReboot()
        {
            using (var process=new Process())
            {
                process.StartInfo = new ProcessStartInfo("shutdown", "-s -r -t 5 -f")
                    {CreateNoWindow = true, UseShellExecute = false};
                process.Start();
                process.WaitForExit();
            }
        }

        public static void RestartRoles()
        {
            redistribution.InvokeRestart();
        }
    }
}