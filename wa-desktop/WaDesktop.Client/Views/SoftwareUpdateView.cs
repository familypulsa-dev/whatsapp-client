using System;
using System.Diagnostics;
using System.Windows.Forms;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Client.Views
{
    public partial class SoftwareUpdateView : Form, ISoftwareUpdateView
    {
        public event Action OnClickDownloadUpdate;
        public event Action OnClickClose;
        public event Action OnLoadView;

        public SoftwareUpdateView()
        {
            InitializeComponent();
            currentVersion.Text = Application.ProductVersion;
            newVersion.Text = Application.ProductVersion;

            btnClose.Click += (s, e) => OnClickClose?.Invoke();
            btnUpgrade.Click += (s, e) => 
            {
                btnUpgrade.Enabled = false;
                btnUpgrade.Text = "Downloading...";
                OnClickDownloadUpdate?.Invoke();
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetCanUpdate(false);
            btnUpgrade.Text = "Upgrade";

            OnLoadView?.Invoke();
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (lbLoader.Text.StartsWith("Fetching"))
            {
                switch (lbLoader.Text.ToLower())
                {
                    case "fetching data": lbLoader.Text = "Fetching data."; break;
                    case "fetching data.": lbLoader.Text = "Fetching data.."; break;
                    case "fetching data..": lbLoader.Text = "Fetching data..."; break;
                    case "fetching data...": lbLoader.Text = "Fetching data"; break;
                }
            }
        }

        public void Loader(bool status)
        {
            if(status)
            {
                lbLoader.Visible = true;
                timer1.Start();
            }
            else
            {
                   timer1.Stop();
            }
        }

        public void SetParameters(string newVersion, string notes)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => SetParameters(newVersion, notes)));
                return;
            }

            this.newVersion.Text = newVersion;
            textBox1.Text = notes;
        }

        public void SetCanUpdate(bool canUpdate)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => SetCanUpdate(canUpdate)));
                return;
            }

            lbLoader.Visible = false;

            if (canUpdate)
            {
                subtitleCanUpdate.Text = "New version of Application is available.";

                lblNotes.Visible = true;
                textBox1.Visible = true;
                btnUpgrade.Visible = true;

                this.Width = 456;
                this.Height = 327;
                groupBox1.Width = 436;
                groupBox1.Height = 257;
            }
            else
            {
                subtitleCanUpdate.Text = "Application is up to date.";

                lblNotes.Visible = false;
                textBox1.Visible = false;
                btnUpgrade.Visible = false;

                groupBox1.Width = 254;
                groupBox1.Height = 75;
                this.Width = 278;
                this.Height = 146;
            }
        }

        public void UpdateProgress(string status, int percent)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateProgress(status, percent)));
                return;
            }

            lbLoader.Visible = true;
            lbLoader.Text = $"{status} ({percent}%)";
        }

        public void CloseView()
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => CloseView()));
                return;
            }
            this.Close();
        }

        public void ShowError(string message)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowError(message)));
                return;
            }
            MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowMessage(string message)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowMessage(message)));
                return;
            }
            MessageBox.Show(this, message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public bool IsLoading { set { } }
    }
}
