namespace WaDesktop.Client.Views.ManagementViews
{
    partial class TagihanView
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Panel panelFilter;

        private System.Windows.Forms.ComboBox cmbWaba;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBulan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMarketing;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUtility;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAuth;
        private System.Windows.Forms.DataGridViewTextBoxColumn colService;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalVolume;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalBiaya;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.colBulan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMarketing = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUtility = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAuth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colService = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalBiaya = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmbWaba = new System.Windows.Forms.ComboBox();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.btnFilter = new System.Windows.Forms.Button();
            this.panelFilter = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.panelFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBulan,
            this.colMarketing,
            this.colUtility,
            this.colAuth,
            this.colService,
            this.colTotalVolume,
            this.colTotalBiaya});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 40);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(1000, 474);
            this.dataGridView.TabIndex = 0;
            // 
            // colBulan
            // 
            this.colBulan.HeaderText = "Bulan";
            this.colBulan.Name = "colBulan";
            this.colBulan.ReadOnly = true;
            // 
            // colMarketing
            // 
            dataGridViewCellStyle6.Format = "C2";
            dataGridViewCellStyle6.FormatProvider = new System.Globalization.CultureInfo("id-ID");
            this.colMarketing.DefaultCellStyle = dataGridViewCellStyle6;
            this.colMarketing.HeaderText = "Marketing";
            this.colMarketing.Name = "colMarketing";
            this.colMarketing.ReadOnly = true;
            this.colMarketing.Width = 120;
            // 
            // colUtility
            // 
            dataGridViewCellStyle7.Format = "C2";
            dataGridViewCellStyle7.FormatProvider = new System.Globalization.CultureInfo("id-ID");
            this.colUtility.DefaultCellStyle = dataGridViewCellStyle7;
            this.colUtility.HeaderText = "Utility";
            this.colUtility.Name = "colUtility";
            this.colUtility.ReadOnly = true;
            this.colUtility.Width = 120;
            // 
            // colAuth
            // 
            dataGridViewCellStyle8.Format = "C2";
            dataGridViewCellStyle8.FormatProvider = new System.Globalization.CultureInfo("id-ID");
            this.colAuth.DefaultCellStyle = dataGridViewCellStyle8;
            this.colAuth.HeaderText = "Auth";
            this.colAuth.Name = "colAuth";
            this.colAuth.ReadOnly = true;
            this.colAuth.Width = 120;
            // 
            // colService
            // 
            dataGridViewCellStyle9.Format = "C2";
            dataGridViewCellStyle9.FormatProvider = new System.Globalization.CultureInfo("id-ID");
            this.colService.DefaultCellStyle = dataGridViewCellStyle9;
            this.colService.HeaderText = "Service";
            this.colService.Name = "colService";
            this.colService.ReadOnly = true;
            this.colService.Width = 120;
            // 
            // colTotalVolume
            // 
            this.colTotalVolume.HeaderText = "Total Volume";
            this.colTotalVolume.Name = "colTotalVolume";
            this.colTotalVolume.ReadOnly = true;
            // 
            // colTotalBiaya
            // 
            dataGridViewCellStyle10.Format = "C2";
            dataGridViewCellStyle10.FormatProvider = new System.Globalization.CultureInfo("id-ID");
            this.colTotalBiaya.DefaultCellStyle = dataGridViewCellStyle10;
            this.colTotalBiaya.HeaderText = "Total Biaya";
            this.colTotalBiaya.Name = "colTotalBiaya";
            this.colTotalBiaya.ReadOnly = true;
            this.colTotalBiaya.Width = 150;
            // 
            // cmbWaba
            // 
            this.cmbWaba.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWaba.FormattingEnabled = true;
            this.cmbWaba.Location = new System.Drawing.Point(322, 9);
            this.cmbWaba.Name = "cmbWaba";
            this.cmbWaba.Size = new System.Drawing.Size(200, 21);
            this.cmbWaba.TabIndex = 3;
            this.cmbWaba.SelectedIndexChanged += new System.EventHandler(this.cmbWaba_SelectedIndexChanged);
            // 
            // dtpStart
            // 
            this.dtpStart.CustomFormat = "MMMM yyyy";
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStart.Location = new System.Drawing.Point(8, 10);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.ShowUpDown = true;
            this.dtpStart.Size = new System.Drawing.Size(150, 20);
            this.dtpStart.TabIndex = 0;
            // 
            // dtpEnd
            // 
            this.dtpEnd.CustomFormat = "MMMM yyyy";
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEnd.Location = new System.Drawing.Point(166, 10);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.ShowUpDown = true;
            this.dtpEnd.Size = new System.Drawing.Size(150, 20);
            this.dtpEnd.TabIndex = 1;
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(528, 9);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(75, 23);
            this.btnFilter.TabIndex = 2;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // panelFilter
            // 
            this.panelFilter.Controls.Add(this.cmbWaba);
            this.panelFilter.Controls.Add(this.dtpStart);
            this.panelFilter.Controls.Add(this.dtpEnd);
            this.panelFilter.Controls.Add(this.btnFilter);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 0);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Padding = new System.Windows.Forms.Padding(8);
            this.panelFilter.Size = new System.Drawing.Size(1000, 40);
            this.panelFilter.TabIndex = 1;
            // 
            // TagihanView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panelFilter);
            this.Name = "TagihanView";
            this.Size = new System.Drawing.Size(1000, 514);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.panelFilter.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
