using System;
using System.ComponentModel;
using System.Windows.Forms;

using WaDesktop.Domain.Interfaces;

namespace WaMeta.Client.Views.Splash
{
    public partial class SplashForm : Form, ISplashView
    {
        public event Action Initialized;
        public event Action OpenSettingsRequested;
        public event Action ConnectManualRequested;

        public SplashForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";
            Label1.Text = $"Ver. {version}";

            ProgressBar1.MarqueeAnimationSpeed = 30;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Initialized?.Invoke();
        }

        public void ShowStatus(string message, int percent)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowStatus(message, percent)));
                return;
            }

            if (this.IsDisposed || !this.IsHandleCreated) return;

            lblStatus.Text = message;
            if (percent >= 0 && percent <= 100)
            {
                ProgressBar1.Style = ProgressBarStyle.Blocks;
                ProgressBar1.Value = percent;
            }
            else
            {
                ProgressBar1.Style = ProgressBarStyle.Marquee;
            }
        }

        public void ShowError(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowError(message)));
                return;
            }

            if (this.IsDisposed) return;
            
            // If handle not created yet, just log it. Trying to show MB without handle on a modal can cause SEHException
            if (!this.IsHandleCreated)
            {
                System.Diagnostics.Debug.WriteLine($"[Splash] Error before handle: {message}");
                return;
            }

            MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void FormClose()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => FormClose()));
                return;
            }
            Close();
        }

        public void CloseView()
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => CloseView()));
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Application.Exit();
        }
    }
}
