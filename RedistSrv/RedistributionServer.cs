using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RedistServ
{
    internal class RedistributionServer : IRedistributionServer
    {
        
        private readonly string _path;
        private Process _gitdaemon;

        public RedistributionServer(string path)
        {
            _path = path;
        }

        public void Start()
        {
            var giteaname=Directory.GetFiles(_path, "gitea-*.exe").FirstOrDefault();
            var command = "web";
            var startupinfo = new ProcessStartInfo(giteaname, command)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = _path
            };
            //startupinfo.EnvironmentVariables["GITEA_WORK_DIR"] = _path;
            startupinfo.EnvironmentVariables["USER"] = Environment.UserName;
            HandleLogEvent($"run: {giteaname} {command} in dir {_path} and user {Environment.UserName}");
            var process = new Process {StartInfo = startupinfo};
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
             process.Start();
             process.BeginOutputReadLine();
             process.BeginErrorReadLine();
            _gitdaemon = process;

        }

        

        public void Restart()
        {
            Shutdown();
        }

        public void Shutdown()
        {
            if (_gitdaemon != null)
            {
                if (!_gitdaemon.HasExited)
                {
                    _gitdaemon.CloseMainWindow();
                    if(!_gitdaemon.WaitForExit(1000))
                        _gitdaemon.Kill();
                    _gitdaemon.Close();
                }

                _gitdaemon = null;
            }
        }

        public event Action<string> HandleLogEvent;
        public event Action<string> HandleErrorEvent;

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                if (e.Data != null)
                {
                    HandleErrorEvent?.Invoke(e.Data);
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                if (e.Data != null)
                {
                    HandleLogEvent?.Invoke(e.Data);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            
        }
    }
}