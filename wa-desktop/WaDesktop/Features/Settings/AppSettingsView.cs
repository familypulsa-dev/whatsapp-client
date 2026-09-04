using System;
using System.Windows.Forms;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Client.Extensions;

namespace WaDesktop.Client.Views.ManagementViews
{
    public partial class AppSettingsView : UserControl, IAppSettingsView
    {
        public AppSettingsView()
        {
            InitializeComponent();
        }

        public string WebhookBaseUrl
        {
            get => txtWebhookBaseUrl.Text;
            set => this.InvokeIfRequired(() => txtWebhookBaseUrl.Text = value);
        }

        public bool MessageCleanupEnabled
        {
            get => chkCleanupEnabled.Checked;
            set => this.InvokeIfRequired(() => chkCleanupEnabled.Checked = value);
        }

        public int MessageRetentionDays
        {
            get => (int)numRetentionDays.Value;
            set => this.InvokeIfRequired(() =>
            {
                if (value < (int)numRetentionDays.Minimum)
                    value = (int)numRetentionDays.Minimum;
                if (value > (int)numRetentionDays.Maximum)
                    value = (int)numRetentionDays.Maximum;
                numRetentionDays.Value = value;
            });
        }

        public bool IsSaving
        {
            set
            {
                this.InvokeIfRequired(() =>
                {
                    btnSave.Enabled = !value;
                    btnSetupWebhook.Enabled = !value;
                    Cursor = value ? Cursors.WaitCursor : Cursors.Default;
                });
            }
        }

        public event EventHandler SaveClicked;
        public event EventHandler RefreshClicked;
        public event EventHandler SetupWebhookClicked;

        public void ShowSuccess(string message) => MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        public void ShowWarning(string message) => MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        public void ShowError(string message) => MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void btnSave_Click(object sender, EventArgs e) => SaveClicked?.Invoke(this, EventArgs.Empty);
        private void btnRefresh_Click(object sender, EventArgs e) => RefreshClicked?.Invoke(this, EventArgs.Empty);
        private void btnSetupWebhook_Click(object sender, EventArgs e) => SetupWebhookClicked?.Invoke(this, EventArgs.Empty);
    }
}
