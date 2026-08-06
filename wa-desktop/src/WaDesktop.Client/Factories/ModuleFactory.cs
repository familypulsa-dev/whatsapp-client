using System;
using Microsoft.Extensions.DependencyInjection;
using WaDesktop.Client.Presenters;
using WaDesktop.Client.Views;
using WaDesktop.Client.Views.ManagementViews;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Client.Factories
{
    /// <summary>
    /// Pabrik perakit modul tab. Menyembunyikan detail ActivatorUtilities,
    /// DI Scope, dan pemanggilan LoadData awal dari ShellPresenter.
    /// </summary>
    public class ModuleFactory : IModuleFactory
    {
        private readonly IServiceProvider _rootProvider;
        private readonly string _messagesUrl;
        private readonly string _apiBaseUrl;

        public ModuleFactory(IServiceProvider rootProvider, string messagesUrl, string apiBaseUrl)
        {
            _rootProvider = rootProvider;
            _messagesUrl = messagesUrl;
            _apiBaseUrl = apiBaseUrl;
        }

        public ModuleInstance Create(string moduleKey)
        {
            var scope = _rootProvider.CreateScope();
            try
            {
                var provider = scope.ServiceProvider;

                switch (moduleKey)
                {
                    case "messages":
                        return CreateMessages(provider, scope, _messagesUrl);

                    case "company":
                        return CreateAndLoad<CompanyView, CompanyPresenter>(provider, scope);

                    case "users":
                        return CreateAndLoad<UsersView, UsersPresenter>(provider, scope);

                    case "phonenumbers":
                        return CreateAndLoad<PhoneNumberView, PhoneNumbersPresenter>(provider, scope);

                    case "waba":
                        return CreateAndLoad<WabaView, WabasPresenter>(provider, scope);

                    case "templates":
                        return CreateTemplates(provider, scope);

                    case "appsettings":
                        return CreateAndLoad<AppSettingsView, AppSettingsPresenter>(provider, scope);

                    case "billing":
                        return CreateAndLoad<TagihanView, TagihanPresenter>(provider, scope);

                    default:
                        return CreateDynamic(provider, scope, moduleKey);
                }
            }
            catch
            {
                scope.Dispose();
                throw;
            }
        }

        // ── Modul dengan presenter tambahan (WebView2 / parameter URL) ──

        private ModuleInstance CreateMessages(IServiceProvider provider, IServiceScope scope, string url)
        {
            var view = provider.GetRequiredService<MessagesView>();
            var presenter = ActivatorUtilities.CreateInstance<MessagesPresenter>(provider, view, url, _apiBaseUrl);
            return new ModuleInstance(view, presenter, scope);
        }

        private ModuleInstance CreateTemplates(IServiceProvider provider, IServiceScope scope)
        {
            var view = provider.GetRequiredService<TemplatesView>();
            var presenter = ActivatorUtilities.CreateInstance<TemplatesPresenter>(
                provider, view, _messagesUrl, _apiBaseUrl);
            presenter.LoadData();
            return new ModuleInstance(view, presenter, scope);
        }

        /// <summary>
        /// Pola umum: resolve view → rakit presenter (1 view + dependensi DI) →
        /// muat data awal jika presenter mendukung refresh.
        /// </summary>
        private ModuleInstance CreateAndLoad<TView, TPresenter>(IServiceProvider provider, IServiceScope scope)
            where TView : class, IViewBase
            where TPresenter : class
        {
            var view = provider.GetRequiredService<TView>();
            var presenter = ActivatorUtilities.CreateInstance<TPresenter>(provider, view);
            (presenter as IPresenterBase)?.LoadData();
            return new ModuleInstance(view, presenter, scope);
        }

        // ── Modul dinamis berbasis prefiks moduleKey (detail / edit / create) ──

        private ModuleInstance CreateDynamic(IServiceProvider provider, IServiceScope scope, string moduleKey)
        {
            if (moduleKey.StartsWith("phonedetail_"))
            {
                var phoneId = moduleKey.Substring("phonedetail_".Length);
                var view = provider.GetRequiredService<PhoneNumberDetailView>();
                var presenter = ActivatorUtilities.CreateInstance<PhoneNumberDetailPresenter>(
                    provider, view, phoneId);
                presenter.LoadData();
                return new ModuleInstance(view, presenter, scope);
            }

            if (moduleKey.StartsWith("template_detail_"))
            {
                var templateId = moduleKey.Substring("template_detail_".Length);
                var view = provider.GetRequiredService<MessagesView>();
                var presenter = ActivatorUtilities.CreateInstance<MessagesPresenter>(
                    provider, view, _messagesUrl + $"templates/edit/{templateId}", _apiBaseUrl);
                return new ModuleInstance(view, presenter, scope);
            }

            if (moduleKey.StartsWith("template_create_"))
            {
                var wabaId = moduleKey.Substring("template_create_".Length);
                var view = provider.GetRequiredService<MessagesView>();
                var presenter = ActivatorUtilities.CreateInstance<MessagesPresenter>(
                    provider, view, _messagesUrl + $"templates/create?{wabaId}", _apiBaseUrl);
                return new ModuleInstance(view, presenter, scope);
            }

            throw new ArgumentException($"Unknown module key: {moduleKey}");
        }
    }
}
