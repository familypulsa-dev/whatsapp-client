using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WaDesktop.Client.Presenters;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Client.Views.ManagementViews
{
    public sealed partial class WabaOnboardingForm : Form, IWabaOnboardingView
    {
        private readonly MetaWabaBrowser _browser;
        private readonly WabaOnboardingPresenter _presenter;
        private bool _busy;

        public List<Waba> LatestWabas => _presenter.LatestWabas;
        public List<Waba> NewWabas => _presenter.NewWabas;

        public WabaOnboardingForm()
        {
            InitializeComponent();
        }

        public WabaOnboardingForm(IWabaRepository repository, IEnumerable<Waba> baseline) : this()
        {
            _presenter = new WabaOnboardingPresenter(repository, this, baseline);
            // Keep browser initialization out of the Visual Studio designer.
            _browser = new MetaWabaBrowser { Dock = DockStyle.Fill };
            browserHost.Controls.Add(_browser);
            _content.Panel1Collapsed = true;
            _toggleBrowser.Click += (s, e) =>
            {
                _content.Panel1Collapsed = !_content.Panel1Collapsed;
                _toggleBrowser.Text = _content.Panel1Collapsed ? "Tampilkan WebView" : "Sembunyikan WebView";
            };
            _close.Click += (s, e) => Close();
            _fillName.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(_name.Text))
                {
                    ShowStatus("Nama WABA harus diisi.");
                    _name.Focus();
                    return;
                }
                _browser.SetNameAndContinue(_name.Text);
            };
            _sync.Click += async (s, e) => await _presenter.SyncAsync();
            _external.Click += (s, e) =>
            {
                try { MetaWabaBrowser.OpenExternal(); }
                catch (Exception ex) { ShowStatus("Gagal membuka browser: " + ex.Message); }
            };
            _browser.StatusChanged += ShowStatus;
            Shown += async (s, e) => await _browser.InitializeAsync();
            FormClosing += (s, e) => { if (_busy) e.Cancel = true; };
        }

        public bool IsBusy
        {
            set
            {
                _busy = value;
                _sync.Enabled = _close.Enabled = _external.Enabled = !value;
                _fillName.Enabled = _name.Enabled = !value;
                UseWaitCursor = value;
            }
        }

        public void ShowStatus(string message)
        {
            if (IsDisposed || Disposing) return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(ShowStatus), message);
                return;
            }
            var index = _logs.Rows.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message);
            _logs.FirstDisplayedScrollingRowIndex = index;
        }
    }
}
