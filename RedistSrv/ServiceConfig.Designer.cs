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
            this.RestartAll = new System.Windows.Forms.ToolStripMenuItem();
            this.DryRunUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.ServiceMode = new System.Windows.Forms.ToolStripMenuItem();
            this.Exit = new System.Windows.Forms.ToolStripMenuItem();
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
            this.Exit});
            this.TrayContextMenu.Name = "TrayContextMenu";
            this.TrayContextMenu.Size = new System.Drawing.Size(154, 92);
            // 
            // RestartAll
            // 
            this.RestartAll.Name = "RestartAll";
            this.RestartAll.Size = new System.Drawing.Size(153, 22);
            this.RestartAll.Text = "Restart all";
            // 
            // DryRunUpdate
            // 
            this.DryRunUpdate.Name = "DryRunUpdate";
            this.DryRunUpdate.Size = new System.Drawing.Size(153, 22);
            this.DryRunUpdate.Text = "Dry run update";
            // 
            // ServiceMode
            // 
            this.ServiceMode.Name = "ServiceMode";
            this.ServiceMode.Size = new System.Drawing.Size(153, 22);
            this.ServiceMode.Text = "Service mode";
            this.ServiceMode.Click += new System.EventHandler(this.ServiceMode_Click);
            // 
            // Exit
            // 
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(153, 22);
            this.Exit.Text = "Stop and exit";
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Redistribution service configuration";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.TrayContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip TrayContextMenu;
        private System.Windows.Forms.ToolStripMenuItem RestartAll;
        private System.Windows.Forms.ToolStripMenuItem DryRunUpdate;
        private System.Windows.Forms.ToolStripMenuItem ServiceMode;
        private System.Windows.Forms.ToolStripMenuItem Exit;
    }
}

