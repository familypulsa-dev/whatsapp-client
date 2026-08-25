namespace WaDesktop.Client.Views.ManagementViews
{
    partial class AppSettingsView
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel tableLayout;
        // Row 0: Webhook Base URL
        private System.Windows.Forms.Label labelWebhookUrl;
        private System.Windows.Forms.FlowLayoutPanel panelWebhook;
        private System.Windows.Forms.TextBox txtWebhookBaseUrl;
        private System.Windows.Forms.Label lblWebhookSuffix;
        private System.Windows.Forms.Button btnSetupWebhook;
        // Row 1: Cleanup
        private System.Windows.Forms.Label labelCleanupEnabled;
        private System.Windows.Forms.CheckBox chkCleanupEnabled;
        // Row 2: Retention
        private System.Windows.Forms.Label labelRetentionDays;
        private System.Windows.Forms.NumericUpDown numRetentionDays;
        // Row 3: Buttons
        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRefresh;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.labelWebhookUrl = new System.Windows.Forms.Label();
            this.panelWebhook = new System.Windows.Forms.FlowLayoutPanel();
            this.txtWebhookBaseUrl = new System.Windows.Forms.TextBox();
            this.lblWebhookSuffix = new System.Windows.Forms.Label();
            this.btnSetupWebhook = new System.Windows.Forms.Button();
            this.labelCleanupEnabled = new System.Windows.Forms.Label();
            this.chkCleanupEnabled = new System.Windows.Forms.CheckBox();
            this.labelRetentionDays = new System.Windows.Forms.Label();
            this.numRetentionDays = new System.Windows.Forms.NumericUpDown();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.panelWebhook.SuspendLayout();
            this.tableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRetentionDays)).BeginInit();
            this.flowPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // tableLayout
            //
            this.tableLayout.ColumnCount = 2;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayout.Controls.Add(this.labelWebhookUrl, 0, 0);
            this.tableLayout.Controls.Add(this.panelWebhook, 1, 0);
            this.tableLayout.Controls.Add(this.labelCleanupEnabled, 0, 1);
            this.tableLayout.Controls.Add(this.chkCleanupEnabled, 1, 1);
            this.tableLayout.Controls.Add(this.labelRetentionDays, 0, 2);
            this.tableLayout.Controls.Add(this.numRetentionDays, 1, 2);
            this.tableLayout.Controls.Add(this.flowPanel, 1, 3);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.Location = new System.Drawing.Point(0, 0);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.Padding = new System.Windows.Forms.Padding(20);
            this.tableLayout.RowCount = 4;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayout.Size = new System.Drawing.Size(913, 190);
            this.tableLayout.TabIndex = 0;
            //
            // labelWebhookUrl
            //
            this.labelWebhookUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelWebhookUrl.Location = new System.Drawing.Point(23, 20);
            this.labelWebhookUrl.Name = "labelWebhookUrl";
            this.labelWebhookUrl.Size = new System.Drawing.Size(255, 36);
            this.labelWebhookUrl.TabIndex = 0;
            this.labelWebhookUrl.Text = "Base Webhook URL:";
            this.labelWebhookUrl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panelWebhook
            //
            this.panelWebhook.Controls.Add(this.txtWebhookBaseUrl);
            this.panelWebhook.Controls.Add(this.lblWebhookSuffix);
            this.panelWebhook.Controls.Add(this.btnSetupWebhook);
            this.panelWebhook.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelWebhook.Location = new System.Drawing.Point(281, 23);
            this.panelWebhook.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.panelWebhook.Name = "panelWebhook";
            this.panelWebhook.Size = new System.Drawing.Size(612, 30);
            this.panelWebhook.TabIndex = 1;
            this.panelWebhook.WrapContents = false;
            //
            // txtWebhookBaseUrl
            //
            this.txtWebhookBaseUrl.Location = new System.Drawing.Point(3, 5);
            this.txtWebhookBaseUrl.Margin = new System.Windows.Forms.Padding(3, 5, 0, 0);
            this.txtWebhookBaseUrl.Name = "txtWebhookBaseUrl";
            this.txtWebhookBaseUrl.Size = new System.Drawing.Size(382, 20);
            this.txtWebhookBaseUrl.TabIndex = 0;
            //
            // lblWebhookSuffix
            //
            this.lblWebhookSuffix.AutoSize = true;
            this.lblWebhookSuffix.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblWebhookSuffix.Location = new System.Drawing.Point(385, 8);
            this.lblWebhookSuffix.Margin = new System.Windows.Forms.Padding(0, 8, 5, 0);
            this.lblWebhookSuffix.Name = "lblWebhookSuffix";
            this.lblWebhookSuffix.Size = new System.Drawing.Size(92, 13);
            this.lblWebhookSuffix.TabIndex = 1;
            this.lblWebhookSuffix.Text = "/api/v1/webhook";
            //
            // btnSetupWebhook
            //
            this.btnSetupWebhook.Location = new System.Drawing.Point(487, 2);
            this.btnSetupWebhook.Margin = new System.Windows.Forms.Padding(5, 2, 0, 0);
            this.btnSetupWebhook.Name = "btnSetupWebhook";
            this.btnSetupWebhook.Size = new System.Drawing.Size(100, 26);
            this.btnSetupWebhook.TabIndex = 2;
            this.btnSetupWebhook.Text = "Setup ke Meta";
            this.btnSetupWebhook.UseVisualStyleBackColor = true;
            this.btnSetupWebhook.Click += new System.EventHandler(this.btnSetupWebhook_Click);
            //
            // labelCleanupEnabled
            //
            this.labelCleanupEnabled.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCleanupEnabled.Location = new System.Drawing.Point(23, 56);
            this.labelCleanupEnabled.Name = "labelCleanupEnabled";
            this.labelCleanupEnabled.Size = new System.Drawing.Size(255, 30);
            this.labelCleanupEnabled.TabIndex = 2;
            this.labelCleanupEnabled.Text = "Hapus Otomatis:";
            this.labelCleanupEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // chkCleanupEnabled
            //
            this.chkCleanupEnabled.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkCleanupEnabled.Location = new System.Drawing.Point(284, 59);
            this.chkCleanupEnabled.Name = "chkCleanupEnabled";
            this.chkCleanupEnabled.Size = new System.Drawing.Size(606, 24);
            this.chkCleanupEnabled.TabIndex = 3;
            this.chkCleanupEnabled.Text = "Aktifkan penghapusan otomatis pesan lama";
            this.chkCleanupEnabled.UseVisualStyleBackColor = true;
            //
            // labelRetentionDays
            //
            this.labelRetentionDays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelRetentionDays.Location = new System.Drawing.Point(23, 86);
            this.labelRetentionDays.Name = "labelRetentionDays";
            this.labelRetentionDays.Size = new System.Drawing.Size(255, 30);
            this.labelRetentionDays.TabIndex = 4;
            this.labelRetentionDays.Text = "Hapus Pesan Lebih Dari (hari):";
            this.labelRetentionDays.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // numRetentionDays
            //
            this.numRetentionDays.Location = new System.Drawing.Point(284, 89);
            this.numRetentionDays.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.numRetentionDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRetentionDays.Name = "numRetentionDays";
            this.numRetentionDays.Size = new System.Drawing.Size(80, 20);
            this.numRetentionDays.TabIndex = 5;
            this.numRetentionDays.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            //
            // flowPanel
            //
            this.flowPanel.Controls.Add(this.btnSave);
            this.flowPanel.Controls.Add(this.btnRefresh);
            this.flowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowPanel.Location = new System.Drawing.Point(284, 119);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(606, 36);
            this.flowPanel.TabIndex = 6;
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(109, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // AppSettingsView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayout);
            this.Name = "AppSettingsView";
            this.Size = new System.Drawing.Size(913, 190);
            this.panelWebhook.ResumeLayout(false);
            this.panelWebhook.PerformLayout();
            this.tableLayout.ResumeLayout(false);
            this.tableLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRetentionDays)).EndInit();
            this.flowPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
