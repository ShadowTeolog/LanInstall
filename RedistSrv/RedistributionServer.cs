using System.Diagnostics;

namespace RedistServ
{
    internal class RedistributionServer : IRedistributionServer
    {
        private readonly string _path;
        private Process gitdaemon;
        public RedistributionServer(string path)
        {
            _path = path;
        }

        public void Start()
        {
            var executablename = ".\\gitea\\gitea-1.8-windows-4.0-amd64.exe";
            var command =$"web";
            var startupinfo = new ProcessStartInfo(executablename, command)
            {
                UseShellExecute = false
            };
            startupinfo.EnvironmentVariables["GITEA_WORK_DIR"] = _path;
            gitdaemon=Process.Start(startupinfo);
        }

        public void Restart()
        {
            Shutdown();
        }
        public void Shutdown()
        {
            gitdaemon?.Kill();
            gitdaemon = null;
        }
    }
}