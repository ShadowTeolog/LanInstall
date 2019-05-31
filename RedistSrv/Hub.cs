using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RedistDto;

namespace RedistServ
{
    
    public static class Hub
    {
        private const int standartlogcapacity = 500;
        private static List<string> loglines=new List<string>(standartlogcapacity*2);
        private static volatile string[] _lastlogresult;
        private static string RepositoryPath => Directory.GetCurrentDirectory();
        private static readonly IRedistributionServer redistribution = new RedistributionServer(RepositoryPath);
        private static readonly INetwork network = NetworkHelper.CreateNetwork();
        private static readonly Configuration config = Configuration.Load();
        
        private static Server Server;
        
        public static void Start()
        {
            redistribution.HandleLogEvent += _client_HandleLogEvent;
            redistribution.HandleErrorEvent += _client_HandleErrorEvent;
            redistribution.Start();

            network.Start(config.MulticastInterface);
            Server=new Server(network,config);
        }

        public static void Shutdown() => redistribution.Shutdown();

        public static void SendCommand(CommandId command) => Server.SendCommandToAll(command);

        private static void _client_HandleErrorEvent(string message)
        {
            lock (loglines)
            {
                loglines?.Add("Error:" + message);
                if(loglines.Count>=standartlogcapacity*2-1)
                    loglines.RemoveRange(0,loglines.Count-standartlogcapacity);
                _lastlogresult = null;
            }
        }

        private static void _client_HandleLogEvent(string message)
        {
            lock (loglines)
            {
                loglines?.Add(message);
                if(loglines.Count>=standartlogcapacity*2-1)
                    loglines.RemoveRange(0,loglines.Count-standartlogcapacity);
                _lastlogresult = null;
            }
        }

        
        public static string[] RequestLogLines(int count)
        {
            if (_lastlogresult != null)
                return _lastlogresult;
            lock (loglines)
            {
                var realcount = Math.Min(loglines.Count, count);
                var realoffset = loglines.Count - realcount;
                var newresult=loglines.Skip(realoffset).ToArray();
                _lastlogresult = newresult;
                return newresult;
            }
            
        }
    }
}