namespace WaDesktop.Client.Views
{
    partial class ShellView
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem dashboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem companyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem phoneNumbersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wabaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem templatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem billingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem appSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TabControl tabWorkspace;
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.NotifyIcon notifyIcon;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShellView));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.toolStripSystem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSoftwareUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.dashboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.companyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wabaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.phoneNumbersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.templatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.billingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.tabWorkspace = new System.Windows.Forms.TabControl();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.footerView1 = new Shared.Views.Footer.FooterView();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSystem,
            this.dashboardToolStripMenuItem,
            this.companyToolStripMenuItem,
            this.usersToolStripMenuItem,
            this.wabaToolStripMenuItem,
            this.phoneNumbersToolStripMenuItem,
            this.templatesToolStripMenuItem,
            this.billingToolStripMenuItem,
            this.appSettingsToolStripMenuItem,
            this.logoutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1423, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // toolStripSystem
            // 
            this.toolStripSystem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSoftwareUpdate});
            this.toolStripSystem.Name = "toolStripSystem";
            this.toolStripSystem.Size = new System.Drawing.Size(57, 20);
            this.toolStripSystem.Text = "&System";
            // 
            // toolStripSoftwareUpdate
            // 
            this.toolStripSoftwareUpdate.Name = "toolStripSoftwareUpdate";
            this.toolStripSoftwareUpdate.Size = new System.Drawing.Size(161, 22);
            this.toolStripSoftwareUpdate.Text = "Software Update";
            this.toolStripSoftwareUpdate.Click += new System.EventHandler(this.toolStripSoftwareUpdate_Click);
            // 
            // dashboardToolStripMenuItem
            // 
            this.dashboardToolStripMenuItem.Name = "dashboardToolStripMenuItem";
            this.dashboardToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.dashboardToolStripMenuItem.Text = "Messages";
            this.dashboardToolStripMenuItem.Click += new System.EventHandler(this.dashboardToolStripMenuItem_Click);
            // 
            // companyToolStripMenuItem
            // 
            this.companyToolStripMenuItem.Name = "companyToolStripMenuItem";
            this.companyToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.companyToolStripMenuItem.Text = "Server";
            this.companyToolStripMenuItem.Click += new System.EventHandler(this.companyToolStripMenuItem_Click);
            // 
            // usersToolStripMenuItem
            // 
            this.usersToolStripMenuItem.Name = "usersToolStripMenuItem";
            this.usersToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.usersToolStripMenuItem.Text = "Users";
            this.usersToolStripMenuItem.Click += new System.EventHandler(this.usersToolStripMenuItem_Click);
            // 
            // wabaToolStripMenuItem
            // 
            this.wabaToolStripMenuItem.Name = "wabaToolStripMenuItem";
            this.wabaToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.wabaToolStripMenuItem.Text = "WABA";
            this.wabaToolStripMenuItem.Click += new System.EventHandler(this.wabaToolStripMenuItem_Click);
            // 
            // phoneNumbersToolStripMenuItem
            // 
            this.phoneNumbersToolStripMenuItem.Name = "phoneNumbersToolStripMenuItem";
            this.phoneNumbersToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.phoneNumbersToolStripMenuItem.Text = "Nomor HP";
            this.phoneNumbersToolStripMenuItem.Click += new System.EventHandler(this.phoneNumbersToolStripMenuItem_Click);
            // 
            // templatesToolStripMenuItem
            // 
            this.templatesToolStripMenuItem.Name = "templatesToolStripMenuItem";
            this.templatesToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.templatesToolStripMenuItem.Text = "Templates";
            this.templatesToolStripMenuItem.Click += new System.EventHandler(this.templatesToolStripMenuItem_Click);
            // 
            // billingToolStripMenuItem
            // 
            this.billingToolStripMenuItem.Name = "billingToolStripMenuItem";
            this.billingToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.billingToolStripMenuItem.Text = "Tagihan";
            this.billingToolStripMenuItem.Click += new System.EventHandler(this.billingToolStripMenuItem_Click);
            // 
            // appSettingsToolStripMenuItem
            // 
            this.appSettingsToolStripMenuItem.Name = "appSettingsToolStripMenuItem";
            this.appSettingsToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.appSettingsToolStripMenuItem.Text = "App Settings";
            this.appSettingsToolStripMenuItem.Visible = false;
            this.appSettingsToolStripMenuItem.Click += new System.EventHandler(this.appSettingsToolStripMenuItem_Click);
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.logoutToolStripMenuItem.Text = "Logout";
            this.logoutToolStripMenuItem.Visible = false;
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 24);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.panelSidebar);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tabWorkspace);
            this.splitContainer.Size = new System.Drawing.Size(1423, 703);
            this.splitContainer.SplitterDistance = 262;
            this.splitContainer.TabIndex = 1;
            // 
            // panelSidebar
            // 
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(262, 703);
            this.panelSidebar.TabIndex = 0;
            // 
            // tabWorkspace
            // 
            this.tabWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabWorkspace.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabWorkspace.Location = new System.Drawing.Point(0, 0);
            this.tabWorkspace.Name = "tabWorkspace";
            this.tabWorkspace.Padding = new System.Drawing.Point(22, 5);
            this.tabWorkspace.SelectedIndex = 0;
            this.tabWorkspace.Size = new System.Drawing.Size(1157, 703);
            this.tabWorkspace.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabWorkspace.TabIndex = 0;
            this.tabWorkspace.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TabWorkspace_DrawItem);
            this.tabWorkspace.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TabWorkspace_MouseDown);
            this.tabWorkspace.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TabWorkspace_MouseMove);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "WA Desktop";
            this.notifyIcon.Visible = true;
            this.notifyIcon.BalloonTipClicked += new System.EventHandler(this.NotifyIcon_BalloonTipClicked);
            // 
            // footerView1
            // 
            this.footerView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerView1.Location = new System.Drawing.Point(0, 727);
            this.footerView1.Name = "footerView1";
            this.footerView1.Size = new System.Drawing.Size(1423, 25);
            this.footerView1.TabIndex = 3;
            // 
            // ShellView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1423, 752);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.footerView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(1439, 791);
            this.Name = "ShellView";
            this.Text = "Whatsapp Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ShellView_FormClosing);
            this.Shown += new System.EventHandler(this.ShellView_Shown);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Shared.Views.Footer.FooterView footerView1;
        private System.Windows.Forms.ToolStripMenuItem toolStripSystem;
        private System.Windows.Forms.ToolStripMenuItem toolStripSoftwareUpdate;
    }
}
