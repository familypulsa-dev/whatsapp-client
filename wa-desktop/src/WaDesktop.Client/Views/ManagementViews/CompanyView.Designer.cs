namespace WaDesktop.Client.Views.ManagementViews
{
    partial class CompanyView
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel panelToolbar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.IdServer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LimitMarketing = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LimitUtility = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LimitAuthentication = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LimitService = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UsageMarketing = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UsageUtility = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UsageAuthentication = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UsageService = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrentCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MetaCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxEstimatedCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.panelToolbar.SuspendLayout();
            this.panelFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdServer,
            this.dgName,
            this.dataGridViewTextBoxColumn3,
            this.LimitMarketing,
            this.LimitUtility,
            this.LimitAuthentication,
            this.LimitService,
            this.UsageMarketing,
            this.UsageUtility,
            this.UsageAuthentication,
            this.UsageService,
            this.CurrentCost,
            this.MetaCost,
            this.MaxEstimatedCost});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView.Location = new System.Drawing.Point(0, 40);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(1000, 474);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.DataGridView_CellBeginEdit);
            this.dataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellEndEdit);
            this.dataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.DataGridView_UserDeletingRow);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(8, 10);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 20);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(214, 8);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(829, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(910, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // panelToolbar
            // 
            this.panelToolbar.Controls.Add(this.btnSearch);
            this.panelToolbar.Controls.Add(this.txtSearch);
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelToolbar.Location = new System.Drawing.Point(0, 0);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Padding = new System.Windows.Forms.Padding(8);
            this.panelToolbar.Size = new System.Drawing.Size(1000, 40);
            this.panelToolbar.TabIndex = 1;
            // 
            // panelFooter
            // 
            this.panelFooter.Controls.Add(this.btnSave);
            this.panelFooter.Controls.Add(this.btnRefresh);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 485);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(1000, 29);
            this.panelFooter.TabIndex = 2;
            // 
            // IdServer
            // 
            this.IdServer.DataPropertyName = "IdServer";
            this.IdServer.HeaderText = "ID";
            this.IdServer.Name = "IdServer";
            this.IdServer.ReadOnly = true;
            // 
            // dgName
            // 
            this.dgName.DataPropertyName = "Name";
            this.dgName.HeaderText = "Name";
            this.dgName.Name = "dgName";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Created At";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // LimitMarketing
            // 
            this.LimitMarketing.DataPropertyName = "LimitMarketing";
            this.LimitMarketing.HeaderText = "Limit Marketing";
            this.LimitMarketing.Name = "LimitMarketing";
            // 
            // LimitUtility
            // 
            this.LimitUtility.DataPropertyName = "LimitUtility";
            this.LimitUtility.HeaderText = "Limit Utilitas";
            this.LimitUtility.Name = "LimitUtility";
            // 
            // LimitAuthentication
            // 
            this.LimitAuthentication.DataPropertyName = "LimitAuthentication";
            this.LimitAuthentication.HeaderText = "Limit Autentikasi";
            this.LimitAuthentication.Name = "LimitAuthentication";
            // 
            // LimitService
            // 
            this.LimitService.DataPropertyName = "LimitService";
            this.LimitService.HeaderText = "Limit Service";
            this.LimitService.Name = "LimitService";
            // 
            // UsageMarketing
            // 
            this.UsageMarketing.HeaderText = "Usage Marketing";
            this.UsageMarketing.Name = "UsageMarketing";
            this.UsageMarketing.ReadOnly = true;
            // 
            // UsageUtility
            // 
            this.UsageUtility.HeaderText = "Usage Utilitas";
            this.UsageUtility.Name = "UsageUtility";
            this.UsageUtility.ReadOnly = true;
            // 
            // UsageAuthentication
            // 
            this.UsageAuthentication.HeaderText = "Usage Autentikasi";
            this.UsageAuthentication.Name = "UsageAuthentication";
            this.UsageAuthentication.ReadOnly = true;
            // 
            // UsageService
            // 
            this.UsageService.HeaderText = "Usage Service";
            this.UsageService.Name = "UsageService";
            this.UsageService.ReadOnly = true;
            // 
            // CurrentCost
            // 
            this.CurrentCost.HeaderText = "Est Tagihan Saat Ini";
            this.CurrentCost.Name = "CurrentCost";
            this.CurrentCost.ReadOnly = true;
            // 
            // MetaCost
            // 
            this.MetaCost.HeaderText = "Tagihan Meta";
            this.MetaCost.Name = "MetaCost";
            this.MetaCost.ReadOnly = true;
            // 
            // MaxEstimatedCost
            // 
            this.MaxEstimatedCost.HeaderText = "Maks. Est. Tagihan";
            this.MaxEstimatedCost.Name = "MaxEstimatedCost";
            this.MaxEstimatedCost.ReadOnly = true;
            // 
            // CompanyView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panelToolbar);
            this.Name = "CompanyView";
            this.Size = new System.Drawing.Size(1000, 514);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.panelToolbar.ResumeLayout(false);
            this.panelToolbar.PerformLayout();
            this.panelFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdServer;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn LimitMarketing;
        private System.Windows.Forms.DataGridViewTextBoxColumn LimitUtility;
        private System.Windows.Forms.DataGridViewTextBoxColumn LimitAuthentication;
        private System.Windows.Forms.DataGridViewTextBoxColumn LimitService;
        private System.Windows.Forms.DataGridViewTextBoxColumn UsageMarketing;
        private System.Windows.Forms.DataGridViewTextBoxColumn UsageUtility;
        private System.Windows.Forms.DataGridViewTextBoxColumn UsageAuthentication;
        private System.Windows.Forms.DataGridViewTextBoxColumn UsageService;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrentCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn MetaCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxEstimatedCost;
    }
}
