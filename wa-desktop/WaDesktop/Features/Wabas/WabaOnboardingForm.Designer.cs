namespace WaDesktop.Client.Views.ManagementViews
{
    partial class WabaOnboardingForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.toolbar = new System.Windows.Forms.FlowLayoutPanel();
            this.nameLabel = new System.Windows.Forms.Label();
            this._name = new System.Windows.Forms.TextBox();
            this._fillName = new System.Windows.Forms.Button();
            this._toggleBrowser = new System.Windows.Forms.Button();
            this._external = new System.Windows.Forms.Button();
            this._sync = new System.Windows.Forms.Button();
            this._close = new System.Windows.Forms.Button();
            this._content = new System.Windows.Forms.SplitContainer();
            this.browserHost = new System.Windows.Forms.Panel();
            this._logs = new System.Windows.Forms.DataGridView();
            this.timeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            System.Windows.Forms.DataGridViewCellStyle messageStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._content)).BeginInit();
            this._content.Panel1.SuspendLayout();
            this._content.Panel2.SuspendLayout();
            this._content.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._logs)).BeginInit();
            this.SuspendLayout();
            //
            // toolbar
            //
            this.toolbar.Controls.Add(this.nameLabel);
            this.toolbar.Controls.Add(this._name);
            this.toolbar.Controls.Add(this._fillName);
            this.toolbar.Controls.Add(this._toggleBrowser);
            this.toolbar.Controls.Add(this._external);
            this.toolbar.Controls.Add(this._sync);
            this.toolbar.Controls.Add(this._close);
            this.toolbar.AutoSize = true;
            this.toolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolbar.Padding = new System.Windows.Forms.Padding(8);
            this.toolbar.Name = "toolbar";
            this.toolbar.Size = new System.Drawing.Size(1084, 76);
            this.toolbar.TabIndex = 0;
            //
            // nameLabel
            //
            this.nameLabel.AutoSize = true;
            this.nameLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Text = "Nama WABA";
            //
            // _name
            //
            this._name.Name = "_name";
            this._name.Size = new System.Drawing.Size(220, 20);
            this._name.TabIndex = 0;
            //
            // _fillName
            //
            this._fillName.AutoSize = true;
            this._fillName.Name = "_fillName";
            this._fillName.Text = "Isi nama dan cari Lanjutkan";
            this._fillName.TabIndex = 1;
            this._fillName.UseVisualStyleBackColor = true;
            //
            // _toggleBrowser
            //
            this._toggleBrowser.AutoSize = true;
            this._toggleBrowser.Name = "_toggleBrowser";
            this._toggleBrowser.Text = "Tampilkan WebView";
            this._toggleBrowser.TabIndex = 2;
            this._toggleBrowser.UseVisualStyleBackColor = true;
            //
            // _external
            //
            this._external.AutoSize = true;
            this._external.Name = "_external";
            this._external.Text = "Buka di browser";
            this._external.TabIndex = 3;
            this._external.UseVisualStyleBackColor = true;
            //
            // _sync
            //
            this._sync.AutoSize = true;
            this._sync.Name = "_sync";
            this._sync.Text = "Sudah selesai — Sinkronkan";
            this._sync.TabIndex = 4;
            this._sync.UseVisualStyleBackColor = true;
            //
            // _close
            //
            this._close.AutoSize = true;
            this._close.Name = "_close";
            this._close.Text = "Tutup";
            this._close.TabIndex = 5;
            this._close.UseVisualStyleBackColor = true;
            //
            // _content
            //
            this._content.Dock = System.Windows.Forms.DockStyle.Fill;
            this._content.Name = "_content";
            this._content.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this._content.Panel1.Controls.Add(this.browserHost);
            this._content.Panel2.Controls.Add(this._logs);
            this._content.Size = new System.Drawing.Size(1084, 635);
            this._content.SplitterDistance = 400;
            this._content.TabIndex = 1;
            //
            // browserHost
            //
            this.browserHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserHost.Name = "browserHost";
            this.browserHost.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.browserHost.TabIndex = 0;
            //
            // _logs
            //
            this._logs.AllowUserToAddRows = false;
            this._logs.AllowUserToDeleteRows = false;
            this._logs.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this._logs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.timeColumn, this.messageColumn });
            this._logs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logs.Name = "_logs";
            this._logs.ReadOnly = true;
            this._logs.RowHeadersVisible = false;
            this._logs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._logs.TabIndex = 0;
            //
            // timeColumn
            //
            this.timeColumn.HeaderText = "Waktu";
            this.timeColumn.Name = "timeColumn";
            this.timeColumn.ReadOnly = true;
            this.timeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.timeColumn.Width = 150;
            //
            // messageColumn
            //
            this.messageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            messageStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.messageColumn.DefaultCellStyle = messageStyle;
            this.messageColumn.HeaderText = "Pesan log";
            this.messageColumn.Name = "messageColumn";
            this.messageColumn.ReadOnly = true;
            this.messageColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // WabaOnboardingForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 711);
            this.Controls.Add(this._content);
            this.Controls.Add(this.toolbar);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "WabaOnboardingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tambah WABA — WhatsApp Manager";
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            this._content.Panel1.ResumeLayout(false);
            this._content.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._content)).EndInit();
            this._content.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._logs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.FlowLayoutPanel toolbar;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox _name;
        private System.Windows.Forms.Button _fillName;
        private System.Windows.Forms.Button _toggleBrowser;
        private System.Windows.Forms.Button _external;
        private System.Windows.Forms.Button _sync;
        private System.Windows.Forms.Button _close;
        private System.Windows.Forms.SplitContainer _content;
        private System.Windows.Forms.Panel browserHost;
        private System.Windows.Forms.DataGridView _logs;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn messageColumn;
    }
}
