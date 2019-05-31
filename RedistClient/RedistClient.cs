using System;
using System.Windows.Forms;
using RedistServ;

namespace RedistClient
{
    public partial class RedistClient : Form
    {
        public RedistClient()
        {
            InitializeComponent();
            Hub.StateChangeEvent += Hub_StateChangeEvent;
            Hub.UpdateStart += Hub_UpdateStart;
            Hub.UpdateComplete += Hub_UpdateComplete;
        }

        private void Hub_UpdateComplete()
        {
            if (this.InvokeRequired)
                Invoke((MethodInvoker)Hide);
            else
                Hide();
        }

        private void Hub_UpdateStart()
        {
            if (this.InvokeRequired)
                Invoke((MethodInvoker)Show);
            else
                Show();
        }

        private void Hub_StateChangeEvent(string status)
        {
            if (this.InvokeRequired)
                Status.Invoke((MethodInvoker) (() => { Status.Text = status; }));
            else
                Status.Text = status;
        }

        private void RedistClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        private void notifyIcon1_Click(object sender, EventArgs e) => Show();

        private void mLogUpdateTimer_Tick(object sender, EventArgs e)
        {
            logWindow.Lines = Hub.RequestLogLines(100);
        }
    }
}
