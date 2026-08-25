using System.Collections.Generic;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;

namespace WaDesktop.Domain.Interfaces
{
    /// <summary>Pengaturan aplikasi (Super Admin) dan setup/status webhook.</summary>
    public interface IAppSettingsRepository
    {
        Task<Result<AppSetting>> GetAsync();
        /// <summary>Mengembalikan daftar peringatan dari backend (boleh kosong).</summary>
        Task<Result<List<string>>> SaveAsync(AppSetting settings);
        Task<Result<bool>> SetupWebhookAsync(string callbackUrl);
        Task<Result<WebhookStatus>> GetWebhookStatusAsync();
    }
}
