using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using NLog;
using RedistDto;

namespace RedistServ
{
    class RedistributionNode : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Role _role;
        private readonly string _basefolder;
        private Process _nodeprocess=null;
        public RedistributionNode(Role role, string basefolder)
        {
            _role = role;
            _basefolder = basefolder;
        }

        public void Dispose()
        {
            _nodeprocess?.Kill();
            _nodeprocess = null;
        }

        public void RunCommand(CommandId currentcommand, IPAddress ip)
        {
            switch (currentcommand)
            {
                case CommandId.SimpleRestart:
                    if(!TryStart())
                    {
                        Logger.Trace("Fail to clean and run, failback to CleanAndRestart");
                        RunCommand(CommandId.CleanAndRestart,ip);
                    }
                    break;
                case CommandId.CleanAndRestart:
                    if (!TryCleanAndUpdate())
                    {
                        Logger.Trace("Fail to clean and run, failback to DryRun");
                        RunCommand(CommandId.DryRun,ip);
                    }
                    break;
                case CommandId.UpdateEndRestart:
                    if (!TryUpdateAndRun())
                    {
                        Logger.Trace("Fail to clean and run, failback to DryRun");
                        RunCommand(CommandId.DryRun,ip);
                    }
                    break;
                case CommandId.DryRun:
                    if (!TryDryRun(ip))
                    {
                        //can't do anything here
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentcommand), currentcommand, null);
            }
        }

        private bool TryDryRun(IPAddress ip)
        {
            try
            {
                var path = RepositoryPath();
                if(Directory.Exists(path))
                    Directory.Delete(path,true);
                Clone(ip);
                Start();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }

        

        private bool TryUpdateAndRun()
        {
            try
            {
                HardReset();
                Update();
                Start();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }

        

        private bool TryStart()
        {
            try
            {
                Start();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }

        

        private bool TryCleanAndUpdate()
        {
            try
            {
                HardReset();
                Start();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }


        
        private void HardReset() => RunRepositoryCommand(RepositoryPath(),"reset --hard");
        private void Update() => RunRepositoryCommand(RepositoryPath(),"pull");
        private void Start()
        {
            var workingdir = RepositoryPath();
            var fullpath = Path.Combine(workingdir, _role.StartupFile);
            var settings=new ProcessStartInfo(fullpath,_role.Parameters)
            {
                WorkingDirectory = workingdir
            };
            _nodeprocess=Process.Start(settings);
        }
        private void Clone(IPAddress ip)
        {
            Directory.CreateDirectory(_basefolder);
            var branch = string.IsNullOrEmpty(_role.Branch) ? "master" : _role.Branch;
            var command = $"clone http://{ip}:3000/service/{_role.Repository}  --depth 1 --branch {branch}";
            RunRepositoryCommand(_basefolder, command);
        }

        private void RedurectCommand(object sender, DataReceivedEventArgs e,ref string target)
        {
            target += e.Data;
        }
        private void RunRepositoryCommand(string workdir,string command)
        {
            string result=string.Empty;
            string error=string.Empty;
            using (var process=new Process())
            {
                process.StartInfo=new ProcessStartInfo("git",command)
                {
                    WorkingDirectory = workdir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    //WindowStyle = ProcessWindowStyle.Hidden
                };
                process.OutputDataReceived += (s,e)=>RedurectCommand(s,e,ref result);
                process.ErrorDataReceived += (s, e) =>RedurectCommand(s,e,ref error);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }


        string RepositoryPath() => Path.Combine(_basefolder, _role.Repository);
    }
    class RedistributionClient : IRedistributionClient
    {
        private readonly string _repositorystoragepath;
        private RedistributionNode[] _redistributionNodes=new RedistributionNode[0];
        public RedistributionClient(string repositorystoragepath)
        {
            _repositorystoragepath = repositorystoragepath;
            Directory.CreateDirectory(repositorystoragepath);
        }
        public void UpdateRoles(CommandId currentcommand, RoleSet roles, IPAddress ip)
        {
            Stop();
            BuildFromConfig(roles);
            if (currentcommand == CommandId.DryRun) //do total repositori cleanup
            {
                Directory.Delete(_repositorystoragepath,true);
                Directory.CreateDirectory(_repositorystoragepath);
                currentcommand = CommandId.UpdateEndRestart;
            }
            RunCommandOnNodes(currentcommand,ip);
        }

        private void RunCommandOnNodes(CommandId currentcommand, IPAddress ip)
        {
            foreach (var node in _redistributionNodes)
                node.RunCommand(currentcommand,ip);
        }

        private void BuildFromConfig(RoleSet roles)
        {
            _redistributionNodes = roles.Roles?.Select(e => new RedistributionNode(e, _repositorystoragepath)).ToArray();
        }
        private void Stop()
        {
            foreach (var node in _redistributionNodes)
                node.Dispose();
            _redistributionNodes = null;
        }
    }
}