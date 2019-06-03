namespace RedistSrv
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ServiceMode = new System.Windows.Forms.ToolStripMenuItem();
            this.RestartAll = new System.Windows.Forms.ToolStripMenuItem();
            this.DryRunUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.restartNetworkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shutdownNetworkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.mLog = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.TrayContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.TrayContextMenu;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "RedistServer";
            this.notifyIcon1.Visible = true;
            // 
            // TrayContextMenu
            // 
            this.TrayContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ServiceMode,
            this.RestartAll,
            this.DryRunUpdate,
            this.toolStripSeparator2,
            this.restartNetworkToolStripMenuItem,
            this.shutdownNetworkToolStripMenuItem,
            this.toolStripSeparator1,
            this.Exit});
            this.TrayContextMenu.Name = "TrayContextMenu";
            this.TrayContextMenu.Size = new System.Drawing.Size(175, 170);
            // 
            // ServiceMode
            // 
            this.ServiceMode.Name = "ServiceMode";
            this.ServiceMode.Size = new System.Drawing.Size(174, 22);
            this.ServiceMode.Text = "Service mode";
            this.ServiceMode.Click += new System.EventHandler(this.ServiceMode_Click);
            // 
            // RestartAll
            // 
            this.RestartAll.Name = "RestartAll";
            this.RestartAll.Size = new System.Drawing.Size(174, 22);
            this.RestartAll.Text = "Restart roles";
            this.RestartAll.Click += new System.EventHandler(this.RestartAll_Click);
            // 
            // DryRunUpdate
            // 
            this.DryRunUpdate.Name = "DryRunUpdate";
            this.DryRunUpdate.Size = new System.Drawing.Size(174, 22);
            this.DryRunUpdate.Text = "Dry run update";
            this.DryRunUpdate.Click += new System.EventHandler(this.DryRunUpdate_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(171, 6);
            // 
            // restartNetworkToolStripMenuItem
            // 
            this.restartNetworkToolStripMenuItem.Name = "restartNetworkToolStripMenuItem";
            this.restartNetworkToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.restartNetworkToolStripMenuItem.Text = "Restart network";
            this.restartNetworkToolStripMenuItem.Click += new System.EventHandler(this.restartNetworkToolStripMenuItem_Click);
            // 
            // shutdownNetworkToolStripMenuItem
            // 
            this.shutdownNetworkToolStripMenuItem.Name = "shutdownNetworkToolStripMenuItem";
            this.shutdownNetworkToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.shutdownNetworkToolStripMenuItem.Text = "Shutdown network";
            this.shutdownNetworkToolStripMenuItem.Click += new System.EventHandler(this.shutdownNetworkToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(171, 6);
            // 
            // Exit
            // 
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(174, 22);
            this.Exit.Text = "Stop and exit";
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // mLog
            // 
            this.mLog.Location = new System.Drawing.Point(3, 3);
            this.mLog.Multiline = true;
            this.mLog.Name = "mLog";
            this.mLog.ReadOnly = true;
            this.mLog.Size = new System.Drawing.Size(902, 375);
            this.mLog.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(907, 376);
            this.Controls.Add(this.mLog);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Redistribution service configuration";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.TrayContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip TrayContextMenu;
        private System.Windows.Forms.ToolStripMenuItem RestartAll;
        private System.Windows.Forms.ToolStripMenuItem DryRunUpdate;
        private System.Windows.Forms.ToolStripMenuItem ServiceMode;
        private System.Windows.Forms.ToolStripMenuItem Exit;
        private System.Windows.Forms.ToolStripMenuItem restartNetworkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shutdownNetworkToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TextBox mLog;
        private System.Windows.Forms.Timer timer1;
    }
}

