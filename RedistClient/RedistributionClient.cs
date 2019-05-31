using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RedistDto;

namespace RedistServ
{
    internal class RedistributionClient : IRedistributionClient
    {
        private readonly string _repositorystoragepath;


        private readonly Mutex _requeststatemutex = new Mutex();
        private RedistributionNode[] _redistributionNodes = new RedistributionNode[0];
        private Tuple<CommandId, RoleSet, IPAddress> requeststate;
        private volatile bool taskinprogress;

        public RedistributionClient(string repositorystoragepath)
        {
            _repositorystoragepath = repositorystoragepath;
            Directory.CreateDirectory(repositorystoragepath);
        }

        public event Action<string> HandleLogEvent;
        public event Action<string> HandleErrorEvent;
        public event Action UpdateStart;
        public event Action UpdateComplete;
        public event Action<string> StateChangeEvent;

        public void InvokeUpdateRoles(CommandId servercommantrId, RoleSet servercommantrRoles, IPAddress ip)
        {
            lock (_requeststatemutex)
            {
                requeststate = new Tuple<CommandId, RoleSet, IPAddress>(servercommantrId, servercommantrRoles, ip);
                if (!taskinprogress) Task.Run(() => UpdateRoles());
            }
        }
       
        private void UpdateRoles()
        {
            try
            {
                taskinprogress = true;
                UpdateStart?.Invoke();
                do
                {
                    Tuple<CommandId, RoleSet, IPAddress> request;
                    lock (_requeststatemutex)
                    {
                        request = requeststate;
                    }

                    UpdateStart?.Invoke();
                    StateChangeEvent?.Invoke("Perform update operation");
                    Stop();
                    BuildFromConfig(request.Item2);
                    if (request.Item1 == CommandId.DryRun) //do total repositori cleanup
                    {
                        try
                        {
                            DirectoryUtils.DeleteDirectory(_repositorystoragepath);
                        }
                        catch (Exception e)
                        {
                            OnErrorEvent(e.Message);
                        }
                        Directory.CreateDirectory(_repositorystoragepath);
                    }

                    RunCommandOnNodes(request.Item1, request.Item3);
                    UpdateComplete?.Invoke();
                    StateChangeEvent?.Invoke("update operation finished");
                    lock (_requeststatemutex)
                    {
                        var checkrequest = requeststate;
                        if (checkrequest.Item1 == requeststate.Item1 && checkrequest.Item2 == requeststate.Item2 &&
                            Equals(checkrequest.Item3, requeststate.Item3))
                            taskinprogress = false; //finish operation
                    }
                } while (taskinprogress);
            }
            catch (Exception e)
            {
                taskinprogress = false;
            }
           
        }


        private void RunCommandOnNodes(CommandId currentcommand, IPAddress ip)
        {
            foreach (var node in _redistributionNodes)
                node.RunCommand(currentcommand, ip);
        }

        private void BuildFromConfig(RoleSet roles)
        {
            _redistributionNodes =
                roles.Roles?.Select(e => new RedistributionNode(e, _repositorystoragepath)).ToArray();
            foreach (var node in _redistributionNodes)
            {
                node.TraceEvent += OnLogEvent;
                node.ErrorEvent += OnErrorEvent;
            }
        }
        void OnLogEvent(string message) => HandleLogEvent?.Invoke(message);
        void OnErrorEvent(string message) => HandleErrorEvent?.Invoke(message);

        private void Stop()
        {
            foreach (var node in _redistributionNodes)
                node.Dispose();
            _redistributionNodes = null;
        }
    }
}