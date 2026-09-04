using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using WaDesktop.Client.Factories;
using WaDesktop.Client.Views;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Messages;
using WaDesktop.Domain.State;

namespace WaDesktop.Client.Presenters
{
    /// <summary>
    /// Cangkang aplikasi: router antar modul (via EventAggregator), lifecycle tab,
    /// konfigurasi menu berbasis role, dan footer. Perakitan modul didelegasikan
    /// ke IModuleFactory — ShellPresenter tidak tahu cara membuat View/Presenter.
    /// </summary>
    public class ShellPresenter : IDisposable
    {
        private readonly IShellView _view;
        private readonly IAuthService _auth;
        private readonly IEventAggregator _bus;
        private readonly AppState _state;
        private readonly IModuleFactory _moduleFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, ModuleInstance> _activeModules = new Dictionary<string, ModuleInstance>();

        private IDisposable _tabSub;
        private IDisposable _closeTabSub;
        private IDisposable _sessionSub;
        private IDisposable _notifSub;
        private IDisposable _badgeSub;
        private IDisposable _refreshTabSub;
        private bool _disposed;
        private bool _isLoggingIn = false;

        public ShellPresenter(IShellView view, IAuthService auth, IEventAggregator bus, AppState state,
            IModuleFactory moduleFactory, IServiceProvider serviceProvider)
        {
            _view = view;
            _auth = auth;
            _bus = bus;
            _state = state;
            _moduleFactory = moduleFactory;
            _serviceProvider = serviceProvider;

            _tabSub = bus.Subscribe<RequestOpenTabMessage>(OnRequestOpenTab);
            _closeTabSub = bus.Subscribe<RequestCloseTabMessage>(OnRequestCloseTab);
            _sessionSub = bus.Subscribe<SessionExpiredMessage>(OnSessionExpired);
            _notifSub = bus.Subscribe<ShowNotificationMessage>(m => _view.ShowNotification(m.Title, m.Body));
            _badgeSub = bus.Subscribe<SetBadgeMessage>(m => _view.SetBadge(m.Count));
            _refreshTabSub = bus.Subscribe<RequestRefreshTabMessage>(OnRefreshTabMessage);

            view.MessagesClicked += (s, e) => OpenMessages();
            view.CompanyClicked += (s, e) => OpenCompany();
            view.UsersClicked += (s, e) => OpenUsers();
            view.PhoneNumbersClicked += (s, e) => OpenPhoneNumbers();
            view.WabaClicked += (s, e) => OpenWaba();
            view.TemplatesClicked += (s, e) => OpenTemplates();
            view.BillingClicked += (s, e) => OpenBilling();
            view.AppSettingsClicked += (s, e) => OpenAppSettings();
            view.SoftwareUpdateClicked += (s, e) => OnSoftwareUpdate();
            view.LogoutClicked += OnLogout;
            view.TabClosed += OnTabClosed;

            bool isAgent = _state.Role == "cs";
            view.SidebarCollapsed = isAgent;
            view.CompanyVisible = _auth.IsSuperAdmin;
            view.UsersVisible = !isAgent;
            view.PhoneNumbersVisible = !isAgent;
            view.WabaVisible = _auth.IsSuperAdmin;
            view.TemplatesVisible = !isAgent;
            view.BillingVisible = !isAgent;
            view.AppSettingsVisible = _auth.IsSuperAdmin;
            view.StatusText = $"Logged in as {_auth.DisplayName}";

            if (isAgent)
            {
                OpenMessages();
            }
            else
            {
                OpenPhoneNumbers();
            }

            // ── Footer: Version ──
            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            _view.SetFooterVersion($"v {ver?.ToString(3) ?? "1.0.0"}");

            // ── Footer: Server Name ──
            _view.SetFooterServerName(_state.DisplayName + " - " + _state.CompanyName ?? "Unknown");

            // ── Footer: DateTime ──
            var timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) => _view.SetFooterTime(DateTime.Now.ToString("dd/MM/yy HH:mm:ss"));
            timer.Start();
        }

        // ── Message Handlers ──

        private void OnRefreshTabMessage(RequestRefreshTabMessage message)
        {
            if (_activeModules.TryGetValue(message.ModuleKey, out var instance))
                instance.Refreshable?.LoadData();
        }

        private void OnRequestOpenTab(RequestOpenTabMessage msg)
        {
            if (_activeModules.ContainsKey(msg.ModuleKey))
            {
                // Tab sudah terbuka, hanya perlu difokuskan
                _view.AddOrSelectTab(msg.ModuleKey, msg.Title, null);
                return;
            }

            var instance = _moduleFactory.Create(msg.ModuleKey);
            _activeModules.Add(msg.ModuleKey, instance);
            _view.AddOrSelectTab(msg.ModuleKey, msg.Title, instance.View);
        }

        private void OnRequestCloseTab(RequestCloseTabMessage msg) => _view.CloseTab(msg.ModuleKey);

        private void OnTabClosed(object sender, string moduleKey)
        {
            if (_activeModules.TryGetValue(moduleKey, out var instance))
            {
                _activeModules.Remove(moduleKey);
                instance.Dispose(); // Dispose View (via scope) + Presenter
            }
        }

        // ── Navigasi ──

        private void OpenMessages() => OnRequestOpenTab(new RequestOpenTabMessage("messages", "Messages"));
        private void OpenCompany() => OnRequestOpenTab(new RequestOpenTabMessage("company", "Server"));
        private void OpenUsers() => OnRequestOpenTab(new RequestOpenTabMessage("users", "Users"));
        private void OpenPhoneNumbers() => OnRequestOpenTab(new RequestOpenTabMessage("phonenumbers", "Nomor HP"));
        private void OpenWaba() => OnRequestOpenTab(new RequestOpenTabMessage("waba", "WABA"));
        private void OpenTemplates() => OnRequestOpenTab(new RequestOpenTabMessage("templates", "Templates"));
        private void OpenBilling() => OnRequestOpenTab(new RequestOpenTabMessage("billing", "Tagihan"));
        private void OpenAppSettings() => OnRequestOpenTab(new RequestOpenTabMessage("appsettings", "App Settings"));

        // ── Aksi Shell Lainnya ──

        private void OnSoftwareUpdate()
        {
            var updateService = _serviceProvider.GetRequiredService<IUpdateService>();
            var updateView = _view.CreateSoftwareUpdateView();
            using (var presenter = new SoftwareUpdatePresenter(updateView, updateService))
            {
                _view.ShowDialog(updateView);
            }
        }

        private void OnSessionExpired(SessionExpiredMessage msg)
        {
            if (_isLoggingIn) return;
            _isLoggingIn = true;

            try
            {
                _view.ClearTabs();
                _view.StatusText = "Session expired — login ulang";

                var loginView = _serviceProvider.GetRequiredService<LoginView>();
                var loginPresenter = ActivatorUtilities.CreateInstance<LoginPresenter>(_serviceProvider, loginView);
                
                if (loginView.ShowDialog() == DialogResult.OK)
                {
                    _view.StatusText = $"Logged in as {_auth.DisplayName}";
                    _view.SetFooterServerName(_state.DisplayName + " - " + (_state.CompanyName ?? "Unknown"));
                    OpenMessages();
                }
                else
                {
                    _auth.Logout();
                    _bus.Publish(new LogoutMessage());
                    _view.StatusText = "Logged out";
                }
                loginPresenter.Dispose();
            }
            finally
            {
                _isLoggingIn = false;
            }
        }

        private void OnLogout(object sender, EventArgs e)
        {
            _auth.Logout();
            _bus.Publish(new LogoutMessage());
            _view.ClearTabs();
            _view.StatusText = "Logged out";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _tabSub?.Dispose();
                _closeTabSub?.Dispose();
                _sessionSub?.Dispose();
                _notifSub?.Dispose();
                _badgeSub?.Dispose();
                _refreshTabSub?.Dispose();

                foreach (var instance in _activeModules.Values)
                {
                    instance.Dispose();
                }
                _activeModules.Clear();
                _disposed = true;
            }
        }
    }
}
