using System;
using System.Windows.Forms;

namespace WaDesktop.Client.Views.ManagementViews
{
    public class WebhookInputDialog : Form
    {
        private Label lblTitle;
        private Label lblPhoneId;
        private Label lblCurrentUrl;
        private TextBox txtCurrentUrl;
        private Label lblNewUrl;
        private TextBox txtWebhookUrl;
        private Button btnSave;
        private Button btnCancel;

        public string DialogTitle { get => lblTitle.Text; set => lblTitle.Text = value; }
        public string PhoneNumberId { get => lblPhoneId.Text; set => lblPhoneId.Text = $"Phone Number ID: {value}"; }
        public string CurrentWebhookUrl { get => txtCurrentUrl.Text; set => txtCurrentUrl.Text = value; }
        public string WebhookUrl { get => txtWebhookUrl.Text; set => txtWebhookUrl.Text = value; }

        public WebhookInputDialog()
        {
            BuildLayout();
        }

        private void BuildLayout()
        {
            this.Text = "Webhook";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new System.Drawing.Size(500, 220);

            lblTitle = new Label { Top = 12, Left = 12, Width = 470, Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold) };
            lblPhoneId = new Label { Top = 40, Left = 12, Width = 470, ForeColor = System.Drawing.Color.Gray };

            var sep = new Label { Top = 60, Left = 12, Width = 470, Height = 1, BorderStyle = BorderStyle.Fixed3D };

            lblCurrentUrl = new Label { Top = 72, Left = 12, Width = 120, Text = "Current URL:" };
            txtCurrentUrl = new TextBox { Top = 70, Left = 135, Width = 347, ReadOnly = true, BackColor = System.Drawing.SystemColors.Control };

            lblNewUrl = new Label { Top = 105, Left = 12, Width = 120, Text = "New URL:" };
            txtWebhookUrl = new TextBox { Top = 103, Left = 135, Width = 347 };

            btnSave = new Button { Text = "Save", Width = 80, Top = 150, Left = 300, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancel", Width = 80, Top = 150, Left = 390, DialogResult = DialogResult.Cancel };

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            this.Controls.AddRange(new Control[] { lblTitle, lblPhoneId, sep, lblCurrentUrl, txtCurrentUrl, lblNewUrl, txtWebhookUrl, btnSave, btnCancel });
        }
    }
}
