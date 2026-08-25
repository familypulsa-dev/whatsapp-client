using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.State;
using WaDesktop.Domain.UseCases;
using WaDesktop.Infrastructure.Data.Repositories;
using WaDesktop.Infrastructure.EventAggregator;
using WaDesktop.Infrastructure.Services;
using WaDesktop.Client.Factories;
using WaDesktop.Client.Presenters;
using WaDesktop.Client.Views;
using WaDesktop.Client.Views.ManagementViews;

namespace WaDesktop.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, string apiBaseUrl, string messagesUrl, IUpdateService updateService)
        {
            // 1. Core Services (Singleton = Satu instance untuk seluruh aplikasi)
            services.AddSingleton<IEventAggregator, EventAggregator>();
            services.AddSingleton<IAuthSessionStore, AuthSessionStore>();
            services.AddSingleton(sp => WaDesktop.Infrastructure.Data.Remote.ApiHttpPipeline.Create(
                sp.GetRequiredService<IAuthSessionStore>(), apiBaseUrl));
            services.AddSingleton<IApiClient>(sp => new ApiClient(apiBaseUrl,
                sp.GetRequiredService<IAuthSessionStore>(),
                sp.GetRequiredService<HttpClient>()));
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IUpdateService>(updateService);
            services.AddSingleton<AppState>();

            // 1.2 Data layer ala onpay (Repository + DataSource per fitur)
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<IBillingRepository, BillingRepository>();
            services.AddTransient<IAppSettingsRepository, AppSettingsRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITemplateRepository, TemplateRepository>();
            services.AddTransient(sp => new WaDesktop.Infrastructure.Data.Remote.DataSources.CompanyDataSource(
                sp.GetRequiredService<HttpClient>(), apiBaseUrl));
            services.AddTransient(sp => new WaDesktop.Infrastructure.Data.Remote.DataSources.BillingDataSource(
                sp.GetRequiredService<HttpClient>(), apiBaseUrl));
            services.AddTransient(sp => new WaDesktop.Infrastructure.Data.Remote.DataSources.AppSettingsDataSource(
                sp.GetRequiredService<HttpClient>(), apiBaseUrl));
            services.AddTransient(sp => new WaDesktop.Infrastructure.Data.Remote.DataSources.UserDataSource(
                sp.GetRequiredService<HttpClient>(), apiBaseUrl));
            services.AddTransient(sp => new WaDesktop.Infrastructure.Data.Remote.DataSources.TemplateDataSource(
                sp.GetRequiredService<HttpClient>(), apiBaseUrl));

            // 1.5 Module Factory (Singleton = root provider; tiap Create bikin child scope)
            services.AddSingleton<IModuleFactory>(sp => new ModuleFactory(sp, messagesUrl, apiBaseUrl));
            services.AddTransient<IPhoneRegistrationUseCase, PhoneRegistrationUseCase>();

            // 2. Views (Transient = Bikin baru tiap kali dipanggil)
            services.AddTransient<IShellView, ShellView>();
            services.AddTransient<MessagesView>();
            services.AddTransient<CompanyView>();
            services.AddTransient<UsersView>();
            services.AddTransient<PhoneNumberView>();
            services.AddTransient<WabaView>();
            services.AddTransient<TemplatesView>();
            services.AddTransient<AppSettingsView>();
            services.AddTransient<PhoneNumberDetailView>();
            services.AddTransient<TagihanView>();
            
            services.AddTransient<LoginView>();
            services.AddTransient<SidebarView>();

            // 3. Presenters TIDAK DIDAFTARKAN di sini.
            // Kita akan selalu merakitnya secara dinamis menggunakan ActivatorUtilities.CreateInstance<T>(...)
            // agar bisa menginjeksi (menyatukan) instance View yang tepat ke Presenter, tanpa terjadi double-instance.
            
            return services;
        }
    }
}
