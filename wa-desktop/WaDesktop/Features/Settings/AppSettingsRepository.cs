using System.Collections.Generic;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Infrastructure.Data.Remote.DataSources;

namespace WaDesktop.Infrastructure.Data.Repositories
{
    /// <summary>Agregasi item setting backend ↔ entity AppSetting (logika pindah dari ApiClient).</summary>
    public class AppSettingsRepository : IAppSettingsRepository
    {
        private readonly AppSettingsDataSource _dataSource;

        public AppSettingsRepository(AppSettingsDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<Result<AppSetting>> GetAsync()
        {
            var result = await _dataSource.FetchItems();
            if (result.IsFailure)
                return Result<AppSetting>.Failure(result.Error);

            var setting = new AppSetting();
            foreach (var item in result.Value)
            {
                switch (item.Name)
                {
                    case "wa_waba_token":   setting.WabaToken = item.Value; break;
                    case "wa_app_id":       setting.AppId = item.Value; break;
                    case "wa_app_secret":   setting.AppSecret = item.Value; break;
                    case "wa_bussiness_id": setting.BusinessId = item.Value; break;
                    case "wa_verify_token": setting.VerifyToken = item.Value; break;
                    case "wa_webhook_url":  setting.WebhookUrl = item.Value; break;
                    case "wa_message_cleanup_enabled": setting.MessageCleanupEnabled = item.Value == "true"; break;
                    case "wa_message_retention_days": setting.MessageRetentionDays = ParseRetentionDays(item.Value); break;
                }
            }
            return Result<AppSetting>.Success(setting);
        }

        public async Task<Result<List<string>>> SaveAsync(AppSetting settings)
        {
            var payload = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(settings.WabaToken))   payload["wa_waba_token"] = settings.WabaToken;
            if (!string.IsNullOrEmpty(settings.AppId))       payload["wa_app_id"] = settings.AppId;
            if (!string.IsNullOrEmpty(settings.AppSecret))   payload["wa_app_secret"] = settings.AppSecret;
            if (!string.IsNullOrEmpty(settings.BusinessId))  payload["wa_business_id"] = settings.BusinessId;
            if (!string.IsNullOrEmpty(settings.VerifyToken)) payload["wa_verify_token"] = settings.VerifyToken;
            if (settings.WebhookUrl != null) payload["wa_webhook_url"] = settings.WebhookUrl;
            payload["wa_message_cleanup_enabled"] = settings.MessageCleanupEnabled ? "true" : "false";
            payload["wa_message_retention_days"] = settings.MessageRetentionDays.ToString();

            return await _dataSource.SaveItems(payload);
        }

        public async Task<Result<bool>> SetupWebhookAsync(string callbackUrl)
        {
            return await _dataSource.SetupWebhook(callbackUrl);
        }

        public async Task<Result<WebhookStatus>> GetWebhookStatusAsync()
        {
            var result = await _dataSource.FetchHealth();
            if (result.IsFailure)
                return Result<WebhookStatus>.Failure(result.Error);

            var envelope = result.Value;
            if (envelope == null || envelope.Webhook == null)
                return Result<WebhookStatus>.Success(
                    new WebhookStatus { IsRunning = false, Message = "Unknown error" });

            return Result<WebhookStatus>.Success(new WebhookStatus
            {
                IsRunning = envelope.Webhook.IsRunning,
                Message = envelope.Webhook.Message
            });
        }

        private static int ParseRetentionDays(string value)
        {
            if (int.TryParse(value, out var days) && days >= 1) return days;
            return 90;
        }
    }
}
