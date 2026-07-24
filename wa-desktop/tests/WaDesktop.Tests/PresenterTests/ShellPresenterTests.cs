using NUnit.Framework;
using System;
using Microsoft.Extensions.DependencyInjection;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.State;
using WaDesktop.Domain.Messages;
using WaDesktop.Infrastructure.EventAggregator;

namespace WaDesktop.Tests.PresenterTests
{
    [TestFixture]
    public class ShellPresenterTests
    {
        private class FakeAuthService : IAuthService
        {
            public string AccessToken => "token";
            public string RefreshToken => "rtoken";
            public string Role => "admin";
            public string DisplayName => "Test User";
            public bool IsLoggedIn => true;
            public bool IsSuperAdmin => false;

            public System.Threading.Tasks.Task<bool> LoginAsync(string username, string password)
                => System.Threading.Tasks.Task.FromResult(true);
            public System.Threading.Tasks.Task<bool> RefreshTokenAsync()
                => System.Threading.Tasks.Task.FromResult(true);
            public void Logout() { }
        }

        private class FakeShellView : IShellView
        {
            public string StatusText { get; set; }
            public bool AppSettingsVisible { get; set; }
            public bool SidebarCollapsed { get; set; }
            public bool CompanyVisible { get; set; }
            public bool UsersVisible { get; set; }
            public bool TemplatesVisible { get; set; }
            public bool PhoneNumbersVisible { get; set; }
            public bool WabaVisible { get; set; }
            public bool BillingVisible { get; set; }
            public bool InvokeRequired => false;
            public event EventHandler MessagesClicked;
            public event EventHandler CompanyClicked;
            public event EventHandler UsersClicked;
            public event EventHandler PhoneNumbersClicked;
            public event EventHandler WabaClicked;
            public event EventHandler TemplatesClicked;
            public event EventHandler BillingClicked;
            public event EventHandler AppSettingsClicked;
            public event EventHandler LogoutClicked;
            public event EventHandler SoftwareUpdateClicked;
            public event EventHandler<string> TabClosed;

            public void AddOrSelectTab(string key, string title, IViewBase content) { }
            public void CloseTab(string key) { }
            public void ClearTabs() { }
            public void ShowNotification(string title, string body) { }
            public void SetBadge(int count) { }
            public void SetFooterVersion(string version) { }
            public void SetFooterServerName(string name) { }
            public void SetFooterTime(string time) { }
            public ISoftwareUpdateView CreateSoftwareUpdateView() => null;
            public bool ShowDialog(ISoftwareUpdateView view) => false;
            public void RenderSidebar(IViewBase sidebarContent) { }

            public void TriggerDashboard() => MessagesClicked?.Invoke(this, EventArgs.Empty);
            public void TriggerLogout() => LogoutClicked?.Invoke(this, EventArgs.Empty);
        }

        [Test]
        public void Constructor_SetsStatusText()
        {
            var view = new FakeShellView();
            var auth = new FakeAuthService();
            var bus = new EventAggregator();
            var state = new AppState();
            state.SetSession("t", "rt", "admin", "Test");

            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var provider = services.BuildServiceProvider();

            var presenter = new global::WaDesktop.Client.Presenters.ShellPresenter(view, auth, bus, state, "http://localhost:5000", "http://localhost:8080", provider);

            Assert.That(view.StatusText, Does.Contain("Test"));
            presenter.Dispose();
        }

        [Test]
        public void Logout_ClearsSession()
        {
            var view = new FakeShellView();
            var auth = new FakeAuthService();
            var bus = new EventAggregator();
            var state = new AppState();
            state.SetSession("t", "rt", "admin", "Test");

            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var provider = services.BuildServiceProvider();

            var presenter = new global::WaDesktop.Client.Presenters.ShellPresenter(view, auth, bus, state, "http://localhost:5000", "http://localhost:8080", provider);

            view.TriggerLogout();

            Assert.That(state.IsLoggedIn, Is.False);
            presenter.Dispose();
        }
    }
}
