using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaDesktop.Domain.Entities;
using WaDesktop.Infrastructure.Constants;

namespace WaDesktop.Infrastructure.Services
{
    public partial class ApiClient
    {
        public async Task<AppSetting> GetAppSettingsAsync()
        {
            var data = await GetListAsync<SettingItem>(ApiRoutes.Settings.Base);
            var setting = new AppSetting();
            foreach (var item in data)
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
            return setting;
        }

        public Task<List<string>> SaveAppSettingsAsync(AppSetting settings)
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

            return PutAsync<object, List<string>>(ApiRoutes.Settings.Base, payload);
        }

        public Task SetupWebhookAsync(string callbackUrl)
            => PostAsync(ApiRoutes.Webhook.Setup, new { callback_url = callbackUrl });

        public async Task<WebhookStatus> GetWebhookStatusAsync()
        {
            try
            {
                var json = await GetStringAsync(ApiRoutes.Webhook.Health);
                var wrapper = JObject.Parse(json);
                var data = wrapper["data"];
                
                var status = new WebhookStatus { IsRunning = false, Message = "Unknown error" };
                if (data != null && data["webhook"] != null)
                {
                    var webhookData = data["webhook"];
                    if (webhookData["is_running"] != null) status.IsRunning = webhookData["is_running"].Value<bool>();
                    if (webhookData["message"] != null) status.Message = webhookData["message"].Value<string>();
                }
                return status;
            }
            catch
            {
                return new WebhookStatus { IsRunning = false, Message = "Failed to connect" };
            }
        }

        private static int ParseRetentionDays(string value)
        {
            if (int.TryParse(value, out var days) && days >= 1) return days;
            return 90;
        }

        private class SettingItem
        {
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("value")] public string Value { get; set; }
        }
    }
}