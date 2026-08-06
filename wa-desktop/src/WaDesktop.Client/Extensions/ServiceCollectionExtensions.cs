using Microsoft.Extensions.DependencyInjection;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.State;
using WaDesktop.Domain.UseCases;
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
            services.AddSingleton<IApiClient>(new ApiClient(apiBaseUrl));
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IUpdateService>(updateService);
            services.AddSingleton<AppState>();

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
