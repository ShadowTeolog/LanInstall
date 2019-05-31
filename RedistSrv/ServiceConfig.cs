using System;
using System.Windows.Forms;
using RedistDto;
using RedistServ;

namespace RedistSrv
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            Visible = false; 
            ShowInTaskbar = false; 
            base.OnLoad(e);
        }

        private void Exit_Click(object sender, EventArgs e) => Application.Exit();
        private void RestartAll_Click(object sender, EventArgs e) => Hub.SendCommand(CommandId.SimpleRestart);
        private void restartNetworkToolStripMenuItem_Click(object sender, EventArgs e) => Hub.SendCommand(CommandId.RestartNetwork);
        private void shutdownNetworkToolStripMenuItem_Click(object sender, EventArgs e) => Hub.SendCommand(CommandId.ShutdownNetwork);
        private void ServiceMode_Click(object sender, EventArgs e) => Show();
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void DryRunUpdate_Click(object sender, EventArgs e) => Hub.SendCommand(CommandId.DryRun);

        private void timer1_Tick(object sender, EventArgs e)
        {
            mLog.Lines = Hub.RequestLogLines(100);
        }
    }
}
