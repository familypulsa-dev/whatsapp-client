using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Messages;
using WaDesktop.Domain.State;
using WaDesktop.Infrastructure;
using WaDesktop.Client.Views;
using WaDesktop.Client.Views.ManagementViews;

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
        
        private IDisposable _tabSub;
        private IDisposable _sessionSub;
        private IDisposable _notifSub;
        private IDisposable _badgeSub;
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
            _sessionSub = bus.Subscribe<SessionExpiredMessage>(OnSessionExpired);
            _notifSub = bus.Subscribe<ShowNotificationMessage>(m => _view.ShowNotification(m.Title, m.Body));
            _badgeSub = bus.Subscribe<SetBadgeMessage>(m => _view.SetBadge(m.Count));

            view.MessagesClicked += (s, e) => OpenMessages();
            view.CompanyClicked += (s, e) => OpenCompany();
            view.UsersClicked += (s, e) => OpenUsers();
            view.PhoneNumbersClicked += (s, e) => OpenPhoneNumbers();
            view.WabaClicked += (s, e) => OpenWaba();
            view.TemplatesClicked += (s, e) => OpenTemplates();
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
            view.StatusText = $"Logged in as {_auth.DisplayName}";

            if (isAgent)
            {
                OpenMessages();
            }
            else
            {
                OpenPhoneNumbers();
            }
        }

        private void OnTabClosed(object sender, string moduleKey)
        {
            if (_activeTabScopes.TryGetValue(moduleKey, out var scope))
            {
                scope.Dispose(); // Otomatis dispose View & Presenter di tab ini
                _activeTabScopes.Remove(moduleKey);
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
                    coPresenter.LoadData();
                    return coView;

                case "users":
                    var usrView = provider.GetRequiredService<UsersView>();
                    var usrPresenter = ActivatorUtilities.CreateInstance<UsersPresenter>(provider, usrView);
                    usrPresenter.LoadData();
                    return usrView;

                case "phonenumbers":
                    var pnView = provider.GetRequiredService<PhoneNumberView>();
                    var pnPresenter = ActivatorUtilities.CreateInstance<PhoneNumbersPresenter>(provider, pnView);
                    pnPresenter.LoadData();
                    return pnView;

                case "waba":
                    var wbView = provider.GetRequiredService<WabaView>();
                    var wbPresenter = ActivatorUtilities.CreateInstance<WabasPresenter>(provider, wbView);
                    wbPresenter.LoadData();
                    return wbView;

                case "templates":
                    var tplView = provider.GetRequiredService<TemplatesView>();
                    var tplPresenter = ActivatorUtilities.CreateInstance<TemplatesPresenter>(provider, tplView);
                    tplPresenter.LoadData();
                    return tplView;

                case "appsettings":
                    var setView = provider.GetRequiredService<AppSettingsView>();
                    var setPresenter = ActivatorUtilities.CreateInstance<AppSettingsPresenter>(provider, setView);
                    setPresenter.LoadData();
                    return setView;

                default:
                    if (moduleKey.StartsWith("phonedetail_"))
                    {
                        var phoneId = moduleKey.Substring("phonedetail_".Length);
                        var detailView = provider.GetRequiredService<PhoneNumberDetailView>();
                        
                        var detailPresenter = ActivatorUtilities.CreateInstance<PhoneNumberDetailPresenter>(
                            provider, detailView, phoneId);
                            
                        detailPresenter.LoadData();
                        return detailView;
                    } else if(moduleKey.StartsWith("templatedetail_"))
                    {
                        var templateId = moduleKey.Substring("templatedetail_".Length);
                        var detailView = provider.GetRequiredService<MessagesView>();

                        var detailPresenter = ActivatorUtilities.CreateInstance<MessagesPresenter>(
                            provider, detailView, _messagesUrl + "templates/new", _apiBaseUrl);

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
