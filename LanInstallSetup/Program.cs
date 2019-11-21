using System;
using System.Windows.Forms;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Forms;

// NuGet console: Install-Package WixSharp
// NuGet Manager UI: browse tab

namespace LanInstallSetup
{
    
    class Program
    {
        const string Product = "RedistServer";
        const string Company = "STSTC";
        static Version ProductVersion=new Version(1,DateTime.Now.Month,DateTime.Now.DayOfYear);
        
        static void Main()
        {
            var project = new ManagedProject(Product,
                                //new LaunchCondition("NETRELEASE>\"#461808\"", "Please install .NET 4.7.2+ first."),
                             new Dir($@"%ProgramFiles%\{Company}\{Product}",
                                    new File("Program.cs"))
                                //,new RegValueProperty("NETRELEASE", RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full", "Release", "0")
                                );

            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
            project.Version=ProductVersion;
            project.InstallPrivileges = InstallPrivileges.elevated;
            project.InstallScope = InstallScope.perMachine;
            project.Platform = Platform.x64;
            project.InstallerVersion = 200;
            project.ScheduleReboot = new ScheduleReboot();
            project.SetNetFxPrerequisite("NETFRAMEWORK45 >= '#4618080'", "Please install .NET 4.7.2+ first.");
            //custom set of standard UI dialogs
            project.ManagedUI = new ManagedUI();
            

            project.ManagedUI.InstallDialogs.Add(Dialogs.Welcome)
//                                          .Add(Dialogs.Features)
                                            .Add(Dialogs.InstallDir)
                                            .Add(Dialogs.Progress)
                                            .Add(Dialogs.Exit);

            project.ManagedUI.ModifyDialogs.Add(Dialogs.MaintenanceType)
                                           //.Add(Dialogs.Features)
                                           .Add(Dialogs.Progress)
                                           .Add(Dialogs.Exit);

            project.Load += Msi_Load;
            project.BeforeInstall += Msi_BeforeInstall;
            project.AfterInstall += Msi_AfterInstall;

            //project.SourceBaseDir = "<input dir path>";
            //project.OutDir = "<output dir path>";

            project.BuildMsi();
        }

        static void Msi_Load(SetupEventArgs e)
        {
            if (!e.IsUISupressed && !e.IsUninstalling)
                MessageBox.Show(e.ToString(), "Load");
        }

        static void Msi_BeforeInstall(SetupEventArgs e)
        {
            if (!e.IsUISupressed && !e.IsUninstalling)
                MessageBox.Show(e.ToString(), "BeforeInstall");
        }

        static void Msi_AfterInstall(SetupEventArgs e)
        {
            if (!e.IsUISupressed && !e.IsUninstalling)
                MessageBox.Show(e.ToString(), "AfterExecute");
        }
    }
}