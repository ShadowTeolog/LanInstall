using System;
using System.Windows.Forms;

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
        private void ServiceMode_Click(object sender, EventArgs e) => Show();
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason==CloseReason.UserClosing)
                e.Cancel = true;
        }
    }
}
