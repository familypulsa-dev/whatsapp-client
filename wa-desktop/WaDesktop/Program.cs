using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Messages;
using WaDesktop.Domain.State;
using WaDesktop.Infrastructure;
using WaDesktop.Infrastructure.EventAggregator;
using WaDesktop.Infrastructure.Services;
using WaDesktop.Client.Views;
using WaDesktop.Client.Presenters;
using WaDesktop.Client.Extensions;
using WaDesktop.Shell;
using Velopack;

namespace WaDesktop.Client
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                VelopackApp.Build().Run();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Velopack init error: {ex.Message}");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var splashView = new SplashForm();
            var updateService = new VelopackUpdateService();
            using (var splashPresenter = new SplashPresenter(splashView, updateService))
            {
                if (splashView.ShowDialog() != DialogResult.OK)
                {   
                    return;
                }
            }

            var apiBaseUrl = "https://test.waba.mbi-software.com";
            var wwwRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
            Directory.CreateDirectory(wwwRoot);

            using (var embeddedServer = new EmbeddedServer(wwwRoot, apiBaseUrl))
            {
                embeddedServer.StartAsync().GetAwaiter().GetResult();
                var messagesUrl = embeddedServer.BaseUrl;

                // --- SETUP DEPENDENCY INJECTION ---
                var services = new ServiceCollection();
                services.AddAppServices(apiBaseUrl, messagesUrl, updateService);
                
                using (var provider = services.BuildServiceProvider())
                {
                    var eventAggregator = provider.GetRequiredService<IEventAggregator>();
                    var sessionStore = provider.GetRequiredService<IAuthSessionStore>();

                    // Bridge event sesi dari pipeline HTTP ke EventAggregator.
                    sessionStore.SessionExpired += (s, e) => eventAggregator.Publish(new SessionExpiredMessage());
                    sessionStore.TokenRefreshed += (s, e) => eventAggregator.Publish(new TokenRefreshedMessage());

                    var authService = provider.GetRequiredService<IAuthService>();
                    var hasRestoredSession = authService.RestoreSessionAsync().GetAwaiter().GetResult();
                    RunApplication(provider, hasRestoredSession);
                }
            }
        }

        private static void RunApplication(IServiceProvider provider, bool hasRestoredSession)
        {
            var showSessionExpiredMessage = false;
            var isAuthenticated = hasRestoredSession;

            while (true)
            {
                if (!isAuthenticated)
                {
                    using (var loginView = provider.GetRequiredService<LoginView>())
                    using (var loginPresenter = ActivatorUtilities.CreateInstance<LoginPresenter>(provider, loginView))
                    {
                        if (showSessionExpiredMessage)
                        {
                            loginView.ShowSessionExpiredMessage();
                        }

                        if (loginView.ShowDialog() != DialogResult.OK)
                        {
                            return;
                        }
                    }

                    isAuthenticated = true;
                }

                var shellView = provider.GetRequiredService<IShellView>();
                var shellForm = shellView as Form;
                if (shellForm == null)
                {
                    throw new InvalidOperationException("IShellView must be a WinForms Form.");
                }

                ShellExitReason exitReason;
                using (shellForm)
                using (var shellPresenter = ActivatorUtilities.CreateInstance<ShellPresenter>(provider, shellView))
                using (var sidebarView = provider.GetRequiredService<SidebarView>())
                using (var sidebarPresenter = ActivatorUtilities.CreateInstance<SidebarPresenter>(provider, sidebarView))
                {
                    shellView.RenderSidebar(sidebarView);
                    _ = sidebarPresenter.LoadDataAsync();

                    Application.Run(shellForm);
                    exitReason = shellView.ExitReason;
                }

                if (exitReason == ShellExitReason.ApplicationExit)
                {
                    return;
                }

                showSessionExpiredMessage = exitReason == ShellExitReason.SessionExpired;
                isAuthenticated = false;
            }
        }
    }
}
