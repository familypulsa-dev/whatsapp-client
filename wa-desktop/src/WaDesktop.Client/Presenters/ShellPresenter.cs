using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaDesktop.Client.Views;
using WaDesktop.Client.Views.ManagementViews;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Messages;
using WaDesktop.Domain.State;
using WaDesktop.Infrastructure;

namespace WaDesktop.Client.Presenters
{
    public class ShellPresenter : IDisposable
    {
        private readonly IShellView _view;
        private readonly IAuthService _auth;
        private readonly IEventAggregator _bus;
        private readonly AppState _state;
        private readonly string _messagesUrl;
        private readonly string _apiBaseUrl;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, IServiceScope> _activeTabScopes = new Dictionary<string, IServiceScope>();
        private readonly Dictionary<string, IPresenterBase> _activePresenterScopes = new Dictionary<string, IPresenterBase>();

        private IDisposable _tabSub;
        private IDisposable _sessionSub;
        private IDisposable _notifSub;
        private IDisposable _badgeSub;
        private IDisposable _closeTabSub;
        private IDisposable _refreshTabSub;
        private bool _disposed;

        public ShellPresenter(IShellView view, IAuthService auth, IEventAggregator bus, AppState state,
            string messagesUrl, string apiBaseUrl, IServiceProvider serviceProvider)
        {
            _view = view;
            _auth = auth;
            _bus = bus;
            _state = state;
            _messagesUrl = messagesUrl;
            _apiBaseUrl = apiBaseUrl;
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
            view.WabaVisible = auth.IsSuperAdmin;
            view.TemplatesVisible = !isAgent;
            view.BillingVisible = !isAgent;
            view.AppSettingsVisible = auth.IsSuperAdmin;
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

        private void OnRefreshTabMessage(RequestRefreshTabMessage message)
        {
            //each tab presenter should implement IRefreshable interface to handle refresh logic
            foreach (var kvp in _activePresenterScopes)
            {
                if (kvp.Key == message.ModuleKey && kvp.Value is IPresenterBase refreshable)
                {
                    refreshable.LoadData();
                    break;
                }
            }
        }

        private void OnTabClosed(object sender, string moduleKey)
        {
            if (_activeTabScopes.TryGetValue(moduleKey, out var scope))
            {
                scope.Dispose(); // Otomatis dispose View & Presenter di tab ini
                _activeTabScopes.Remove(moduleKey);
            }

            if(_activePresenterScopes.TryGetValue(moduleKey, out var presenter))
            {
                _activePresenterScopes.Remove(moduleKey);
            }
        }

        private void OnRequestOpenTab(RequestOpenTabMessage msg)
        {
            if (_activeTabScopes.ContainsKey(msg.ModuleKey))
            {
                // Tab sudah terbuka, hanya perlu difokuskan
                _view.AddOrSelectTab(msg.ModuleKey, msg.Title, null);
                return;
            }

            _view.AddOrSelectTab(msg.ModuleKey, msg.Title, CreateModuleView(msg.ModuleKey));
        }

        private void OnRequestCloseTab(RequestCloseTabMessage msg)
        {
            _view.CloseTab(msg.ModuleKey);
        }


        private IViewBase CreateModuleView(string moduleKey)
        {
            var scope = _serviceProvider.CreateScope();
            _activeTabScopes.Add(moduleKey, scope);
            var provider = scope.ServiceProvider;

            switch (moduleKey)
            {
                case "messages":
                    var msgView = provider.GetRequiredService<MessagesView>();
                    var msgPresenter = ActivatorUtilities.CreateInstance<MessagesPresenter>(provider, msgView, _messagesUrl, _apiBaseUrl);
                    return msgView;

                case "company":
                    var coView = provider.GetRequiredService<CompanyView>();
                    var coPresenter = ActivatorUtilities.CreateInstance<CompanyPresenter>(provider, coView);
                    _activePresenterScopes.Add(moduleKey, coPresenter);
                    coPresenter.LoadData();
                    return coView;

                case "users":
                    var usrView = provider.GetRequiredService<UsersView>();
                    var usrPresenter = ActivatorUtilities.CreateInstance<UsersPresenter>(provider, usrView);
                    _activePresenterScopes.Add(moduleKey, usrPresenter);
                    usrPresenter.LoadData();
                    return usrView;

                case "phonenumbers":
                    var pnView = provider.GetRequiredService<PhoneNumberView>();
                    var pnPresenter = ActivatorUtilities.CreateInstance<PhoneNumbersPresenter>(provider, pnView);
                    _activePresenterScopes.Add(moduleKey, pnPresenter);
                    pnPresenter.LoadData();
                    return pnView;

                case "waba":
                    var wbView = provider.GetRequiredService<WabaView>();
                    var wbPresenter = ActivatorUtilities.CreateInstance<WabasPresenter>(provider, wbView);
                    _activePresenterScopes.Add(moduleKey, wbPresenter);
                    wbPresenter.LoadData();
                    return wbView;

                case "templates":
                    var tplView = provider.GetRequiredService<TemplatesView>();
                    var tplPresenter = ActivatorUtilities.CreateInstance<TemplatesPresenter>(provider, tplView, _messagesUrl, _apiBaseUrl);
                    _activePresenterScopes.Add(moduleKey, tplPresenter);
                    tplPresenter.LoadData();
                    return tplView;

                case "appsettings":
                    var setView = provider.GetRequiredService<AppSettingsView>();
                    var setPresenter = ActivatorUtilities.CreateInstance<AppSettingsPresenter>(provider, setView);
                    _activePresenterScopes.Add(moduleKey, setPresenter);
                    setPresenter.LoadData();
                    return setView;

                case "billing":
                    var billView = provider.GetRequiredService<TagihanView>();
                    var billPresenter = ActivatorUtilities.CreateInstance<TagihanPresenter>(provider, billView);
                    _activePresenterScopes.Add(moduleKey, billPresenter);
                    billPresenter.LoadData();
                    return billView;

                default:
                    if (moduleKey.StartsWith("phonedetail_"))
                    {
                        var phoneId = moduleKey.Substring("phonedetail_".Length);
                        var detailView = provider.GetRequiredService<PhoneNumberDetailView>();
                        
                        var detailPresenter = ActivatorUtilities.CreateInstance<PhoneNumberDetailPresenter>(
                            provider, detailView, phoneId);
                            
                        detailPresenter.LoadData();
                        return detailView;
                    } else if(moduleKey.StartsWith("template_detail_"))
                    {
                        var templateId = moduleKey.Substring("template_detail_".Length);
                        var detailView = provider.GetRequiredService<MessagesView>();

                        var detailPresenter = ActivatorUtilities.CreateInstance<MessagesPresenter>(
                            provider, detailView, _messagesUrl + $"templates/edit/{templateId}", _apiBaseUrl);

                        return detailView;
                    }else if (moduleKey.StartsWith("template_create_"))
                    {
                        var waba_id = moduleKey.Substring("template_create_".Length);
                        var detailView = provider.GetRequiredService<MessagesView>();

                        var detailPresenter = ActivatorUtilities.CreateInstance<MessagesPresenter>(
                            provider, detailView, _messagesUrl + $"templates/create?{waba_id}", _apiBaseUrl);

                        return detailView;
                    }
                    throw new ArgumentException($"Unknown module key: {moduleKey}");
            }
        }

        private void OpenMessages() => OnRequestOpenTab(new RequestOpenTabMessage("messages", "Messages"));
        private void OpenCompany() => OnRequestOpenTab(new RequestOpenTabMessage("company", "Server"));
        private void OpenUsers() => OnRequestOpenTab(new RequestOpenTabMessage("users", "Users"));
        private void OpenPhoneNumbers() => OnRequestOpenTab(new RequestOpenTabMessage("phonenumbers", "Nomor HP"));
        private void OpenWaba() => OnRequestOpenTab(new RequestOpenTabMessage("waba", "WABA"));
        private void OpenTemplates() => OnRequestOpenTab(new RequestOpenTabMessage("templates", "Templates"));
        private void OpenBilling() => OnRequestOpenTab(new RequestOpenTabMessage("billing", "Tagihan"));
        private void OpenAppSettings() => OnRequestOpenTab(new RequestOpenTabMessage("appsettings", "App Settings"));

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
            _view.ClearTabs();
            _view.StatusText = "Session expired — login ulang";

            var loginView = _serviceProvider.GetRequiredService<LoginView>();
            var loginPresenter = ActivatorUtilities.CreateInstance<LoginPresenter>(_serviceProvider, loginView);
            if (loginView.ShowDialog() == DialogResult.OK)
            {
                _view.StatusText = $"Logged in as {_auth.DisplayName}";
                _view.SetFooterServerName(_state.DisplayName + " - " + _state.CompanyName ?? "Unknown");
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
                _sessionSub?.Dispose();
                _notifSub?.Dispose();
                _badgeSub?.Dispose();
                _closeTabSub?.Dispose();
                
                foreach (var scope in _activeTabScopes.Values)
                {
                    scope.Dispose();
                }
                _activeTabScopes.Clear();
                _disposed = true;
            }
        }
    }
}
