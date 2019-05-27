using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RedistServ;

namespace RedistSrv
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Startup();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainfowm = new Form1();
            mainfowm.Hide();
            Application.Run(mainfowm);
            ExitCleanup();
        }

        public static void Startup()
        {
            Hub.Start();
        } 
        private static void ExitCleanup()
        {
            Hub.Shutdown();
        }
    }
}
