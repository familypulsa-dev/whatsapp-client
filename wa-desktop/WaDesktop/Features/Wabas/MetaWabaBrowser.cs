using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;

namespace WaDesktop.Client.Views.ManagementViews
{
    public sealed class MetaWabaBrowser : UserControl
    {
        public const string ManagerUrl = "https://business.facebook.com/latest/settings/whatsapp_account?business_id=547976478398955&selected_asset_type=whatsapp-business-account";
        private readonly WebView2 _browser = new WebView2 { Dock = DockStyle.Fill };
        private readonly List<Form> _popups = new List<Form>();
        private CoreWebView2Environment _environment;
        private readonly Timer _addTimer = new Timer { Interval = 500 };
        private bool _checkingAdd;
        private bool _addClicked;
        private bool _createClicked;
        private bool _nameFilled;
        private bool _continueFound;
        private string _wabaName;
        private DateTime _addDeadline;
        private static string ClickButtonScript(bool createStep)
        {
            var matcher = createStep
                ? @"/^(create a new whatsapp(?: business)?(?: account)?|buat akun whatsapp(?: business| bisnis)?(?: baru)?|buat akun whatsapp baru(?: business| bisnis)?)$/.test(normalize(value))"
                : @"['tambahkan', 'add'].includes(normalize(value))";
            return @"(() => {
            const normalize = value => (value || '').replace(/\s+/g, ' ').trim().toLowerCase();
            const matches = value => " + matcher + @";
            const labelledText = element => (element.getAttribute('aria-labelledby') || '')
                .split(/\s+/).filter(Boolean)
                .map(id => element.ownerDocument.getElementById(id))
                .filter(Boolean).map(label => label.textContent).join(' ');
            const candidates = Array.from(document.querySelectorAll('button, [role=button], [role=menuitem], a, input[type=button]'))
                .filter(element => {
                    const style = getComputedStyle(element);
                    return element.getClientRects().length > 0 && style.visibility !== 'hidden'
                        && style.display !== 'none' && !element.disabled
                        && element.getAttribute('aria-disabled') !== 'true'
                        && (matches(labelledText(element)) || matches(element.innerText || element.value) || matches(element.getAttribute('aria-label'))
                            || matches(element.getAttribute('title')));
                });
            if (candidates.length > 1) return 'ambiguous';
            if (candidates.length === 0) return 'missing';
            candidates[0].click();
            return 'clicked';
        })()";
        }
        public event Action<string> StatusChanged;

        public void SetNameAndContinue(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nama WABA harus diisi.", nameof(name));
            _continueFound = false;
            _wabaName = name.Trim();
            _nameFilled = false;
            _addDeadline = DateTime.UtcNow.AddSeconds(30);
            StatusChanged?.Invoke("Nama WABA disiapkan. Menunggu form nama di Meta…");
            if (IsWabaSettingsPage()) _addTimer.Start();
        }

        private static string FillNameScript(string name, bool continueStep)
        {
            return @"(() => {
                const expected = " + JsonConvert.SerializeObject(name) + @";
                const normalize = value => (value || '').replace(/\s+/g, ' ').trim().toLowerCase();
                const labelledText = element => (element.getAttribute('aria-labelledby') || '').split(/\s+/)
                    .filter(Boolean).map(id => element.ownerDocument.getElementById(id))
                    .filter(Boolean).map(label => label.textContent).join(' ');
                const visible = element => element.getClientRects().length > 0
                    && getComputedStyle(element).visibility !== 'hidden'
                    && !element.disabled && element.getAttribute('aria-disabled') !== 'true';
                const dialogs = Array.from(document.querySelectorAll('[role=dialog]')).filter(visible);
                if (dialogs.length > 1) return 'ambiguous';
                const scope = dialogs[0] || document;
                const inputs = Array.from(scope.querySelectorAll('input[type=text][role=combobox]'))
                    .filter(element => visible(element) && !element.readOnly && element.getAttribute('aria-busy') !== 'true');
                const named = inputs.filter(element => /nama|name/.test(normalize(labelledText(element)
                    + ' ' + (element.getAttribute('aria-label') || ''))));
                const candidates = named.length > 0 ? named : inputs;
                if (candidates.length > 1) return 'ambiguous';
                if (candidates.length === 0) return 'missing';
                const input = candidates[0];
                if (!" + (continueStep ? "true" : "false") + @") {
                    if (input.value === expected) return 'filled';
                    input.focus();
                    const setter = Object.getOwnPropertyDescriptor(HTMLInputElement.prototype, 'value').set;
                    setter.call(input, expected);
                    input.dispatchEvent(new Event('input', { bubbles: true }));
                    input.dispatchEvent(new Event('change', { bubbles: true }));
                    input.blur();
                    return 'waiting';
                }
                if (input.value !== expected) return 'name_changed';
                const matches = value => ['lanjutkan', 'continue', 'next'].includes(normalize(value));
                const buttons = Array.from(scope.querySelectorAll('button, [role=button], input[type=button]'))
                    .filter(element => visible(element) && element.getAttribute('aria-busy') !== 'true'
                        && (matches(labelledText(element)) || matches(element.innerText || element.value)
                            || matches(element.getAttribute('aria-label'))));
                if (buttons.length > 1) return 'ambiguous';
                if (buttons.length === 0) return 'missing';
                return 'found';
            })()";
        }

        public MetaWabaBrowser()
        {
            Controls.Add(_browser);
            _addTimer.Tick += CheckAddButton;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var profile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WaDesktop", "MetaWabaWebView");
                _environment = await CoreWebView2Environment.CreateAsync(null, profile);
                if (IsDisposed) return;
                await _browser.EnsureCoreWebView2Async(_environment);
                if (IsDisposed) return;
                _browser.CoreWebView2.NewWindowRequested += OnNewWindow;
                _browser.CoreWebView2.NavigationCompleted += OnNavigationCompleted;
                StatusChanged?.Invoke("Memuat WhatsApp Manager…");
                _browser.CoreWebView2.Navigate(ManagerUrl);
            }
            catch (Exception ex)
            {
                if (!IsDisposed) StatusChanged?.Invoke("Browser Meta tidak dapat dibuka: " + ex.Message + ". Gunakan Buka di browser.");
            }
        }

        private void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            _addTimer.Stop();
            StatusChanged?.Invoke(e.IsSuccess
                ? "Buat WABA di halaman Meta, lalu klik Sudah selesai — Sinkronkan."
                : "Halaman Meta gagal dimuat: " + e.WebErrorStatus + ". Gunakan Buka di browser jika login bermasalah.");
            if (e.IsSuccess && !_continueFound && IsWabaSettingsPage())
            {
                _addDeadline = DateTime.UtcNow.AddSeconds(30);
                StatusChanged?.Invoke(_addClicked ? "Mencari opsi Buat akun WhatsApp / Create a new WhatsApp…" : "Mencari tombol Tambahkan / Add…");
                _addTimer.Start();
            }
        }

        private bool IsWabaSettingsPage()
        {
            Uri uri;
            return Uri.TryCreate(_browser.CoreWebView2?.Source, UriKind.Absolute, out uri)
                && uri.Scheme == "https" && uri.Host == "business.facebook.com"
                && uri.AbsolutePath.TrimEnd('/') == "/latest/settings/whatsapp_account";
        }

        private async void CheckAddButton(object sender, EventArgs e)
        {
            if (_checkingAdd || IsDisposed) return;
            if (_continueFound || !IsWabaSettingsPage()) { _addTimer.Stop(); return; }
            if (_createClicked && string.IsNullOrWhiteSpace(_wabaName))
            {
                _addTimer.Stop();
                StatusChanged?.Invoke("Form Meta terbuka. Isi nama WABA pada textbox aplikasi, lalu klik Isi nama dan cari Lanjutkan.");
                return;
            }
            _checkingAdd = true;
            var createStep = _addClicked;
            var nameStep = _createClicked;
            var label = nameStep ? (_nameFilled ? "Lanjutkan" : "Nama WABA")
                : createStep ? "Buat akun WhatsApp / Create a new WhatsApp" : "Tambahkan / Add";
            try
            {
                var script = nameStep ? FillNameScript(_wabaName, _nameFilled) : ClickButtonScript(createStep);
                var result = JsonConvert.DeserializeObject<string>(await _browser.CoreWebView2.ExecuteScriptAsync(script));
                if (IsDisposed) return;
                if (result == "filled")
                {
                    _nameFilled = true;
                    _addDeadline = DateTime.UtcNow.AddSeconds(30);
                    StatusChanged?.Invoke("Nama WABA terisi. Mencari tombol Lanjutkan…");
                }
                else if (result == "name_changed")
                {
                    _addTimer.Stop();
                    StatusChanged?.Invoke("Nama di form Meta berbeda. Periksa nama lalu klik Isi nama dan cari Lanjutkan lagi.");
                }
                else if (result == "found" && nameStep)
                {
                    _continueFound = true;
                    _addTimer.Stop();
                    StatusChanged?.Invoke("Nama WABA terisi. Tombol Lanjutkan ditemukan dan aktif; belum diklik. Anda dapat klik secara manual.");
                }
                else if (result == "clicked")
                {
                    if (createStep)
                    {
                        _createClicked = true;
                        _addDeadline = DateTime.UtcNow.AddSeconds(30);
                        StatusChanged?.Invoke("Opsi Buat akun WhatsApp diklik. Menunggu form nama…");
                    }
                    else
                    {
                        _addClicked = true;
                        _addDeadline = DateTime.UtcNow.AddSeconds(30);
                        StatusChanged?.Invoke("Tambahkan / Add diklik. Mencari opsi Buat akun WhatsApp / Create a new WhatsApp…");
                    }
                }
                else if (result == "ambiguous" || DateTime.UtcNow >= _addDeadline)
                {
                    _addTimer.Stop();
                    StatusChanged?.Invoke(result == "ambiguous"
                        ? "Ada beberapa opsi " + label + ". Silakan pilih yang sesuai secara manual."
                        : "Opsi " + label + " belum ditemukan atau belum aktif. Anda dapat melanjutkan secara manual.");
                }
            }
            catch (Exception)
            {
                if (!IsDisposed && DateTime.UtcNow >= _addDeadline)
                {
                    _addTimer.Stop();
                    StatusChanged?.Invoke("Opsi " + label + " tidak dapat diperiksa. Silakan lanjutkan secara manual.");
                }
            }
            finally { _checkingAdd = false; }
        }

        private async void OnNewWindow(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            var deferral = e.GetDeferral();
            e.Handled = true;
            Form popup = null;
            try
            {
                if (IsDisposed) return;
                popup = new Form { Text = "Meta — Login", Width = 900, Height = 700, StartPosition = FormStartPosition.CenterParent };
                var browser = new WebView2 { Dock = DockStyle.Fill };
                popup.Controls.Add(browser);
                _popups.Add(popup);
                popup.FormClosed += (s, args) => { _popups.Remove(popup); popup.Dispose(); };
                popup.Show(FindForm());
                await browser.EnsureCoreWebView2Async(_environment);
                if (IsDisposed || popup.IsDisposed) return;
                browser.CoreWebView2.NewWindowRequested += OnNewWindow;
                browser.CoreWebView2.WindowCloseRequested += (s, args) => popup.Close();
                e.NewWindow = browser.CoreWebView2;
            }
            catch (Exception ex)
            {
                if (popup != null && !popup.IsDisposed) popup.Close();
                if (!IsDisposed) StatusChanged?.Invoke("Popup Meta gagal dibuka: " + ex.Message + ". Gunakan Buka di browser.");
            }
            finally { deferral.Complete(); }
        }

        public static void OpenExternal()
        {
            Process.Start(new ProcessStartInfo(ManagerUrl) { UseShellExecute = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _addTimer.Stop();
                _addTimer.Dispose();
                foreach (var popup in _popups.ToArray()) popup.Close();
                _popups.Clear();
            }
            base.Dispose(disposing);
        }
    }
}
