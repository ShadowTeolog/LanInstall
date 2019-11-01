using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms.VisualStyles;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using NLog;
using RedistDto;

namespace RedistServ
{
    internal partial class RedistributionNode : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _basefolder;
        private readonly Role _role;
        private Process _nodeprocess;
        private volatile bool _taskNeedToBeRunning;
        

        public RedistributionNode(Role role, string basefolder)
        {
            _role = role;
            _basefolder = basefolder;
        }


        public void Dispose()
        {
            try
            {
                StopProcess();
            }
            catch (Exception e)
            {
                Error(e.Message);
            }

            _nodeprocess = null;
        }

        private void StopProcess()
        {
            _taskNeedToBeRunning = false;
            if (_nodeprocess == null)
                return;
            var processtostop = _nodeprocess;
            _nodeprocess = null;
            if (!processtostop.HasExited)
            {
                processtostop.CloseMainWindow();
                if (!processtostop.WaitForExit(2000))
                    processtostop.Kill();
                processtostop.Close();
            }

            
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

                Logger.Trace("try deleting repository directory");
                DirectoryUtils.DeleteDirectory(path,Trace);

                GitClone(ip);
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
                GitHardReset();
                GitPull();
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
                StopProcess();
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
                StopProcess();
                GitHardReset();
                Start();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return false;
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
                var startporcess = new Process
                {
                    StartInfo = settings
                };
                _taskNeedToBeRunning = true;
                startporcess.Exited += Startporcess_Exited;
                startporcess.Start();
                _nodeprocess = startporcess;
            }
        }

        private void Startporcess_Exited(object sender, EventArgs e)
        {
            if (_taskNeedToBeRunning)
                Start();
        }


        private string RepositoryPath() => Path.Combine(_basefolder, _role.Repository);
    }
}