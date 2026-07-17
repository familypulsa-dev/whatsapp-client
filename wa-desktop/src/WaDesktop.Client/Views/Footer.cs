using System;
using System.Windows.Forms;

namespace Shared.Views.Footer
{
    public interface IFooterView
    {
        event Action OnClickLogApp;
        event Action OnClickVersionUpdate;
        event Action<bool> OnLogAppVisible;

        void SetVersion(string version);
        void SetServerName(string name);
        void SetTimeServer(string time);
        void ShowIconVersionUpdate(bool show);
        void SetInfo(string info);

    }

    public partial class FooterView : UserControl, IFooterView
    {
        public event Action OnClickLogApp;
        public event Action OnClickVersionUpdate;
        public event Action<bool> OnLogAppVisible;

        public FooterView()
        {
            InitializeComponent();

            this.logAppHide.Click += OnLogAppHideClick;
            this.logAppShow.Click += OnLogAppShowClick;
        }


        private void OnLogAppHideClick(object sender, EventArgs e)
        {
            OnLogAppVisible?.Invoke(false);
        }

        private void OnLogAppShowClick(object sender, EventArgs e)
        {
            OnLogAppVisible?.Invoke(true);
        }


        private void ToolStripLogApp_Click(object sender, System.EventArgs e)
        {
            OnClickLogApp?.Invoke();
        }

        public void ShowIconVersionUpdate(bool show)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowIconVersionUpdate(show)));
                return;
            }
            ToolStripUpdate.Visible = show;
        }

        public void SetInfo(string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetInfo(msg)));
                return;
            }
            ToolStripStatus.Text = msg;
        }

        public void SetVersion(string version)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => SetVersion(version)));
                return;
            }

            ToolStripVersion.Text = version;
        }

        public void SetServerName(string name)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => SetServerName(name)));
                return;
            }

            toolStripNamaServer.Text = name;
        }

        public void SetTimeServer(string time)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => SetTimeServer(time)));
                return;
            }

            ToolStripWaktu.Text = time;
        }

        private void ToolStripUpdate_Click(object sender, EventArgs e)
        {
            OnClickVersionUpdate?.Invoke();
        }

        private void ToolStripUpdate_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(StatusStrip1, "Versi terbaru tersedia!");
        }

        private void ToolStripUpdate_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(StatusStrip1, "");
        }
    }
}
