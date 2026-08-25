using System;
using System.Linq;
using System.Threading.Tasks;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Entities;

namespace WaDesktop.Client.Presenters
{
    public class AppSettingsPresenter : IDisposable, IPresenterBase
    {
        private readonly IAppSettingsView _view;
        private readonly IAppSettingsRepository _settings;
        private bool _disposed;

        public AppSettingsPresenter(IAppSettingsView view, IAppSettingsRepository settings)
        {
            _view = view;
            _settings = settings;

            _view.SaveClicked += async (s, e) => await SaveAsync();
            _view.RefreshClicked += async (s, e) => await LoadDataAsync();
            _view.SetupWebhookClicked += async (s, e) => await SetupWebhookAsync();
        }

        public async void LoadData(string search = null) => await LoadDataAsync();

        private async Task LoadDataAsync()
        {
            _view.IsSaving = true;
            try
            {
                var result = await Task.Run(() => _settings.GetAsync());
                if (result.IsFailure)
                    throw new Exception(result.Error.Message);

                var settings = result.Value;
                _view.WabaToken = settings.WabaToken;
                _view.AppId = settings.AppId;
                _view.AppSecret = settings.AppSecret;
                _view.BusinessId = settings.BusinessId;
                _view.VerifyToken = settings.VerifyToken;
                _view.WebhookBaseUrl = settings.WebhookUrl;
                _view.MessageCleanupEnabled = settings.MessageCleanupEnabled;
                _view.MessageRetentionDays = settings.MessageRetentionDays;
            }
            catch (Exception ex)
            {
                _view.ShowError($"Gagal load settings: {ex.Message}");
            }
            finally
            {
                _view.IsSaving = false;
            }
        }

        private async Task<bool> SaveAsync(bool silent = false)
        {
            _view.IsSaving = true;
            try
            {
                var settings = new AppSetting
                {
                    WabaToken = _view.WabaToken,
                    AppId = _view.AppId,
                    AppSecret = _view.AppSecret,
                    BusinessId = _view.BusinessId,
                    VerifyToken = _view.VerifyToken,
                    WebhookUrl = _view.WebhookBaseUrl,
                    MessageCleanupEnabled = _view.MessageCleanupEnabled,
                    MessageRetentionDays = _view.MessageRetentionDays
                };
                var warnings = await Task.Run(() => _settings.SaveAsync(settings));
                if (warnings.IsFailure)
                    throw new Exception(warnings.Error.Message);

                if (warnings.Value != null && warnings.Value.Any())
                {
                    if (!silent) _view.ShowWarning(string.Join("\n", warnings.Value));
                }
                else
                {
                    if (!silent) _view.ShowSuccess("Settings saved.");
                }
                return true;
            }
            catch (Exception ex)
            {
                if (!silent) _view.ShowError($"Gagal save: {ex.Message}");
                return false;
            }
            finally
            {
                _view.IsSaving = false;
            }
        }

        private async Task SetupWebhookAsync()
        {
            var baseUrl = _view.WebhookBaseUrl?.TrimEnd('/');
            if (string.IsNullOrEmpty(baseUrl))
            {
                _view.ShowWarning("Base Webhook URL tidak boleh kosong.");
                return;
            }

            // Simpan konfigurasi secara silent terlebih dahulu
            var saved = await SaveAsync(silent: true);
            if (!saved)
            {
                _view.ShowError("Gagal menyimpan konfigurasi. Proses setup webhook dibatalkan.");
                return;
            }

            _view.IsSaving = true;
            try
            {
                var fullUrl = baseUrl + "/api/v1/webhook";
                var setup = await Task.Run(() => _settings.SetupWebhookAsync(fullUrl));
                if (setup.IsFailure)
                    throw new Exception(setup.Error.Message);
                _view.ShowSuccess("Pengaturan tersimpan permanen dan Webhook berhasil di-setup ke Meta!");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Gagal setup webhook ke Meta: {ex.Message}");
            }
            finally
            {
                _view.IsSaving = false;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _view.SaveClicked -= null;
                _view.RefreshClicked -= null;
                _view.SetupWebhookClicked -= null;
                _disposed = true;
            }
        }
    }
}
