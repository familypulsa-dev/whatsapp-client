namespace WaDesktop.Client.Views.ManagementViews
{
    partial class TemplatesView
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.ComboBox cmbWabaSync;
        private System.Windows.Forms.Panel panelToolbar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPreview = new System.Windows.Forms.DataGridViewButtonColumn();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSync = new System.Windows.Forms.Button();
            this.cmbWabaSync = new System.Windows.Forms.ComboBox();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.panelToolbar.SuspendLayout();
            this.panelFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.colPreview});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(687, 405);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            this.dataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGridView_UserDeletingRow);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "ID";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Name";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Language";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Status";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Category";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // colPreview
            // 
            this.colPreview.HeaderText = "";
            this.colPreview.Name = "colPreview";
            this.colPreview.ReadOnly = true;
            this.colPreview.Text = "Preview";
            this.colPreview.UseColumnTextForButtonValue = true;
            this.colPreview.Width = 70;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(8, 10);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 20);
            this.txtSearch.TabIndex = 5;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(420, 8);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(841, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Tambah";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(922, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 0;
            this.btnDelete.Text = "Simpan";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSync
            // 
            this.btnSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSync.Location = new System.Drawing.Point(760, 3);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(75, 23);
            this.btnSync.TabIndex = 4;
            this.btnSync.Text = "Sinkron";
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // cmbWabaSync
            // 
            this.cmbWabaSync.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWabaSync.FormattingEnabled = true;
            this.cmbWabaSync.Location = new System.Drawing.Point(214, 10);
            this.cmbWabaSync.Name = "cmbWabaSync";
            this.cmbWabaSync.Size = new System.Drawing.Size(200, 21);
            this.cmbWabaSync.TabIndex = 6;
            this.cmbWabaSync.SelectedIndexChanged += new System.EventHandler(this.CmbWabaSync_SelectedIndexChanged);
            // 
            // panelToolbar
            // 
            this.panelToolbar.Controls.Add(this.cmbWabaSync);
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
            this.panelFooter.Controls.Add(this.btnSync);
            this.panelFooter.Controls.Add(this.btnDelete);
            this.panelFooter.Controls.Add(this.btnAdd);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 445);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(1000, 32);
            this.panelFooter.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 40);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView);
            this.splitContainer1.Size = new System.Drawing.Size(1000, 405);
            this.splitContainer1.SplitterDistance = 687;
            this.splitContainer1.TabIndex = 3;
            // 
            // TemplatesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelToolbar);
            this.Name = "TemplatesView";
            this.Size = new System.Drawing.Size(1000, 477);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.panelToolbar.ResumeLayout(false);
            this.panelToolbar.PerformLayout();
            this.panelFooter.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridViewButtonColumn colPreview;
    }
}
