namespace RedistClient
{
    partial class RedistClient
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RedistClient));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ServiceMode = new System.Windows.Forms.ToolStripMenuItem();
            this.RestartAll = new System.Windows.Forms.ToolStripMenuItem();
            this.DryRunUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.Status = new System.Windows.Forms.Label();
            this.nlogtarget = new System.Windows.Forms.RichTextBox();
            this.TrayContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.TrayContextMenu;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Redistribution client";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
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
            // ServiceMode
            // 
            this.ServiceMode.Name = "ServiceMode";
            this.ServiceMode.Size = new System.Drawing.Size(153, 22);
            this.ServiceMode.Text = "Service mode";
            this.ServiceMode.Click += new System.EventHandler(this.ServiceMode_Click);
            // 
            // RestartAll
            // 
            this.RestartAll.Name = "RestartAll";
            this.RestartAll.Size = new System.Drawing.Size(153, 22);
            this.RestartAll.Text = "Restart roles";
            this.RestartAll.Click += new System.EventHandler(this.RestartAll_Click);
            // 
            // DryRunUpdate
            // 
            this.DryRunUpdate.Name = "DryRunUpdate";
            this.DryRunUpdate.Size = new System.Drawing.Size(153, 22);
            this.DryRunUpdate.Text = "Dry run update";
            this.DryRunUpdate.Click += new System.EventHandler(this.DryRunUpdate_Click);
            // 
            // Exit
            // 
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(153, 22);
            this.Exit.Text = "Stop and exit";
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // Status
            // 
            this.Status.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Status.Location = new System.Drawing.Point(2, 14);
            this.Status.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(1194, 20);
            this.Status.TabIndex = 1;
            this.Status.Text = "Searching for server";
            this.Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // nlogtarget
            // 
            this.nlogtarget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nlogtarget.Location = new System.Drawing.Point(6, 37);
            this.nlogtarget.Name = "nlogtarget";
            this.nlogtarget.Size = new System.Drawing.Size(1200, 341);
            this.nlogtarget.TabIndex = 3;
            this.nlogtarget.Text = "";
            // 
            // RedistClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1209, 380);
            this.Controls.Add(this.nlogtarget);
            this.Controls.Add(this.Status);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RedistClient";
            this.Text = "Lan redistribution client";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RedistClient_FormClosing);
            this.TrayContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label Status;
        private System.Windows.Forms.ContextMenuStrip TrayContextMenu;
        private System.Windows.Forms.ToolStripMenuItem ServiceMode;
        private System.Windows.Forms.ToolStripMenuItem RestartAll;
        private System.Windows.Forms.ToolStripMenuItem DryRunUpdate;
        private System.Windows.Forms.ToolStripMenuItem Exit;
        private System.Windows.Forms.RichTextBox nlogtarget;
    }
}

