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
using WaDesktop.Client.Services;
using WaDesktop.Client.Views;
using WaDesktop.Client.Presenters;
using WaDesktop.Client.Extensions;
using WaMeta.Client.Views.Splash;
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

            var apiBaseUrl = "https://waba.mbi-software.com";
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
                    var apiClient = (ApiClient)provider.GetRequiredService<IApiClient>();

                    apiClient.SessionExpired += (s, e) => eventAggregator.Publish(new SessionExpiredMessage());
                    apiClient.TokenRefreshed += (s, e) => eventAggregator.Publish(new TokenRefreshedMessage());

                    // --- LOGIN ---
                    var loginView = provider.GetRequiredService<LoginView>();
                    var loginPresenter = ActivatorUtilities.CreateInstance<LoginPresenter>(provider, loginView);
                    if (loginView.ShowDialog() != DialogResult.OK)
                    {
                        return; // Exit
                    }

                    // --- MAIN SHELL ---
                    var shellView = provider.GetRequiredService<IShellView>();
                    var shellPresenter = ActivatorUtilities.CreateInstance<ShellPresenter>(provider, shellView, messagesUrl, apiBaseUrl, provider);

                    var sidebarView = provider.GetRequiredService<SidebarView>();
                    var sidebarPresenter = ActivatorUtilities.CreateInstance<SidebarPresenter>(provider, sidebarView);
                    
                    shellView.RenderSidebar(sidebarView);
                    _ = sidebarPresenter.LoadDataAsync();

                    Application.Run(shellView as Form);
                }
            }
        }
    }
}
