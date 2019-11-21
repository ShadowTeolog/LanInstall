using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using RedistServ;
using NDesk.Options;
namespace RedistClient
{
    static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (RunCommand(args))
            {
                Logger.Trace("Exit after command execution");
                return;
            }
            Logger.Trace("Starting..");

            Hub.Start();
            
            Application.Run(new RedistClient());
            Hub.Shutdown();
        }

        private static bool RunCommand(string[] args)
        {
            
            var wantinstall = false;
            var show_help = false;
            var mydirectory = Directory.GetCurrentDirectory();
            var installpath = Directory.GetParent(mydirectory).Parent; //delault install path is .\..\..\

            var p = new OptionSet () {
                { "p|path=", "the {NAME} of someone to greet.",
                    v => installpath=new DirectoryInfo(v)},
                { "i|install", "install program to specified directory",
                    v => wantinstall=true },
                { "h|help",  "show this message and exit", 
                    v => show_help = v != null },
            };
            List<string> extra;
            try {
                extra = p.Parse (args);
            }
            catch (OptionException e) {
                Console.Write ("greet: ");
                Console.WriteLine (e.Message);
                Console.WriteLine ("Try `greet --help' for more information.");
                return true;
            }
            if (show_help) {
                ShowHelp (p);
                return true;
            }
            if (wantinstall )
            {
                Logger.Trace("Trying update client application");
                if (installpath == null)
                {
                    var message = "Can't find install directory , install path not found";
                    Logger.Error(message);
                }
                else                
                    DirectoryUtils.Copy(mydirectory,installpath.FullName,e=>Logger.Trace(e));
                return true;
            }
            return false;
        }
        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: greet [OPTIONS]+ message");
            Console.WriteLine ("Greet a list of individuals with an optional message.");
            Console.WriteLine ("If no message is specified, a generic greeting is used.");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }


        
    }
}
