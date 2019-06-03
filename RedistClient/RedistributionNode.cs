using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using NLog;
using RedistDto;

namespace RedistServ
{
    internal class RedistributionNode : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _basefolder;
        private readonly Role _role;
        private Process _nodeprocess;
        public RedistributionNode(Role role, string basefolder)
        {
            _role = role;
            _basefolder = basefolder;
        }


        public void Dispose()
        {
            try
            {
                if (_nodeprocess != null)
                {
                    if (!_nodeprocess.HasExited)
                    {
                        _nodeprocess.CloseMainWindow();
                        
                        if(!_nodeprocess.WaitForExit(1000))
                            _nodeprocess.Kill();
                        _nodeprocess.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Error(e.Message);
            }
            
            _nodeprocess = null;
        }

        public event Action<string> TraceEvent;
        public event Action<string> ErrorEvent;

        private void Trace(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                TraceEvent?.Invoke(message);
                Logger.Trace(message);
            }
        }

        private void Error(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ErrorEvent?.Invoke(message);
                Logger.Error(message);
            }
        }

        public void RunCommand(CommandId currentcommand, IPAddress ip)
        {
            switch (currentcommand)
            {
                case CommandId.SimpleRestart:
                    if (!TryStart())
                    {
                        Logger.Trace("Fail to clean and run, failback to CleanAndRestart");
                        RunCommand(CommandId.CleanAndRestart, ip);
                    }

                    break;
                case CommandId.CleanAndRestart:
                    if (!TryCleanAndUpdate())
                    {
                        Logger.Trace("Fail to clean and run, failback to DryRun");
                        RunCommand(CommandId.DryRun, ip);
                    }
                    Logger.Trace("Cleanup command complete");
                    break;
                case CommandId.UpdateEndRestart:
                    if (!TryUpdateAndRun())
                    {
                        Logger.Trace("Fail to clean and run, failback to DryRun");
                        RunCommand(CommandId.DryRun, ip);
                    }
                    Logger.Trace("Update and restart command complete");
                    break;
                case CommandId.DryRun:
                    if (!TryDryRun(ip))
                    {
                        //can't do anything here
                    }
                    Logger.Trace("Dry run complete");
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
                if (Directory.Exists(path))
                {
                    Logger.Trace("Deleting repository directory");
                    DirectoryUtils.DeleteDirectory(path);
                }
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
                Pull();
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


        private void HardReset() => RunRepositoryCommand(RepositoryPath(), "reset --hard");
        private void Pull()
        {
            Trace("Pulling current branch");
            RunRepositoryCommand(RepositoryPath(), "pull");
            Trace("Pulling lfs");
            RunRepositoryCommand(RepositoryPath(), "lfs pull");
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(_role.StartupFile))
            {
                var workingdir = RepositoryPath();
                var fullpath = Path.Combine(workingdir, _role.StartupFile);
                var settings = new ProcessStartInfo(fullpath, _role.Parameters)
                {
                    WorkingDirectory = workingdir
                };
                _nodeprocess = Process.Start(settings);
            }
        }

        private void Clone(IPAddress ip)
        {
            Trace("Cloning from git");
            Directory.CreateDirectory(_basefolder);
            var branch = string.IsNullOrEmpty(_role.Branch) ? "master" : _role.Branch;
            var command = $"clone http://{ip}:3000/service/{_role.Repository}  --depth 1 --branch {branch}";
            Trace($"Cloning : {command}");
            RunRepositoryCommand(_basefolder, command);
            Trace($"do lfs install");
            RunRepositoryCommand(RepositoryPath(), "lfs install");
            Trace($"do lfs fetch");
            RunRepositoryCommand(RepositoryPath(), "lfs fetch");
            Trace("Pulling lfs");
            RunRepositoryCommand(RepositoryPath(), "lfs pull");
            Trace("Cloning finished");
        }

        private void RunRepositoryCommand(string workdir, string command)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo("git", command)
                {
                    WorkingDirectory = workdir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                    //WindowStyle = ProcessWindowStyle.Hidden
                };
                process.OutputDataReceived += (s, e) => Trace(e.Data);
                process.ErrorDataReceived += (s, e) => Error(e.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }


        private string RepositoryPath() => Path.Combine(_basefolder, _role.Repository);
        
    }
}