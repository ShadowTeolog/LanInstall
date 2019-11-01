using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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
        static void Main(string[] args)
        {
            Startup(args);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainfowm = new Form1();
            //mainfowm.Hide();
            Application.Run(mainfowm);
            ExitCleanup();
        }

        private const string startupdelay = "/STARTUPDELAY:";
        public static void Startup(string[] args)
        {
            float statupdelay = 0;
            foreach (var key in args)
            {
                try
                {
                    if (key.StartsWith(startupdelay,StringComparison.InvariantCultureIgnoreCase))
                    {
                        var value = key.Substring(startupdelay.Length);
                        float result = 0;
                        if (float.TryParse(value, System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out result))
                        {
                            statupdelay = result;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
            if (statupdelay > 0)
            {
                Thread.Sleep((int)(statupdelay*1000));
            }
            Hub.Start();
        } 
        private static void ExitCleanup()
        {

            Hub.Shutdown();
        }
    }
}

