namespace WaDesktop.Client.Views.ManagementViews
{
    partial class WebhookInputDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblPhoneId = new System.Windows.Forms.Label();
            this.sep = new System.Windows.Forms.Label();
            this.lblCurrentUrl = new System.Windows.Forms.Label();
            this.txtCurrentUrl = new System.Windows.Forms.TextBox();
            this.lblNewUrl = new System.Windows.Forms.Label();
            this.txtWebhookUrl = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTitle.Size = new System.Drawing.Size(200, 19);
            this.lblTitle.Text = "Webhook Configuration";
            // 
            // lblPhoneId
            // 
            this.lblPhoneId.AutoSize = true;
            this.lblPhoneId.ForeColor = System.Drawing.Color.Gray;
            this.lblPhoneId.Location = new System.Drawing.Point(12, 40);
            this.lblPhoneId.Size = new System.Drawing.Size(200, 13);
            this.lblPhoneId.Text = "Phone Number ID:";
            // 
            // sep
            // 
            this.sep.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.sep.Location = new System.Drawing.Point(12, 62);
            this.sep.Size = new System.Drawing.Size(460, 2);
            // 
            // lblCurrentUrl
            // 
            this.lblCurrentUrl.Location = new System.Drawing.Point(12, 74);
            this.lblCurrentUrl.Size = new System.Drawing.Size(120, 20);
            this.lblCurrentUrl.Text = "Current URL:";
            this.lblCurrentUrl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCurrentUrl
            // 
            this.txtCurrentUrl.BackColor = System.Drawing.SystemColors.Control;
            this.txtCurrentUrl.Location = new System.Drawing.Point(135, 72);
            this.txtCurrentUrl.ReadOnly = true;
            this.txtCurrentUrl.Size = new System.Drawing.Size(337, 22);
            this.txtCurrentUrl.TabIndex = 1;
            // 
            // lblNewUrl
            // 
            this.lblNewUrl.Location = new System.Drawing.Point(12, 106);
            this.lblNewUrl.Size = new System.Drawing.Size(120, 20);
            this.lblNewUrl.Text = "New URL:";
            this.lblNewUrl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtWebhookUrl
            // 
            this.txtWebhookUrl.Location = new System.Drawing.Point(135, 104);
            this.txtWebhookUrl.Size = new System.Drawing.Size(337, 22);
            this.txtWebhookUrl.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(290, 150);
            this.btnSave.Size = new System.Drawing.Size(80, 30);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(380, 150);
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // WebhookInputDialog
            // 
            this.AcceptButton = this.btnSave;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(484, 196);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblPhoneId);
            this.Controls.Add(this.sep);
            this.Controls.Add(this.lblCurrentUrl);
            this.Controls.Add(this.txtCurrentUrl);
            this.Controls.Add(this.lblNewUrl);
            this.Controls.Add(this.txtWebhookUrl);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WebhookInputDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Webhook";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblPhoneId;
        private System.Windows.Forms.Label sep;
        private System.Windows.Forms.Label lblCurrentUrl;
        private System.Windows.Forms.TextBox txtCurrentUrl;
        private System.Windows.Forms.Label lblNewUrl;
        private System.Windows.Forms.TextBox txtWebhookUrl;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
