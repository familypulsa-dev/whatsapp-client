using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Infrastructure.Constants;
using WaDesktop.Infrastructure.Payloads.Settings;

namespace WaDesktop.Infrastructure.Data.Remote.DataSources
{
    /// <summary>Akses HTTP mentah untuk settings + webhook.</summary>
    public class AppSettingsDataSource : BaseDataSource
    {
        public AppSettingsDataSource(HttpClient http, string baseUrl) : base(http, baseUrl)
        {
        }

        public async Task<Result<List<SettingItemPayload>>> FetchItems()
        {
            var result = await SendAsync(HttpMethod.Get, $"{BaseUrl}{ApiRoutes.Settings.Base}", null);
            return result.IsSuccess
                ? ParseList<SettingItemPayload>(result.Value)
                : Result<List<SettingItemPayload>>.Failure(result.Error);
        }

        public async Task<Result<List<string>>> SaveItems(Dictionary<string, string> payload)
        {
            var result = await SendAsync(HttpMethod.Put, $"{BaseUrl}{ApiRoutes.Settings.Base}", payload);
            return result.IsSuccess
                ? ParseData<List<string>>(result.Value)
                : Result<List<string>>.Failure(result.Error);
        }

        public async Task<Result<bool>> SetupWebhook(string callbackUrl)
        {
            var result = await SendAsync(HttpMethod.Post, $"{BaseUrl}{ApiRoutes.Webhook.Setup}",
                new { callback_url = callbackUrl });
            return result.IsSuccess
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(result.Error);
        }

        public async Task<Result<WebhookHealthEnvelope>> FetchHealth()
        {
            var result = await SendAsync(HttpMethod.Get, $"{BaseUrl}{ApiRoutes.Webhook.Health}", null);
            return result.IsSuccess
                ? ParseData<WebhookHealthEnvelope>(result.Value)
                : Result<WebhookHealthEnvelope>.Failure(result.Error);
        }
    }
}
