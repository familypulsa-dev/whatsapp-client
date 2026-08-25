using Newtonsoft.Json;

namespace WaDesktop.Infrastructure.Payloads.Settings
{
    /// <summary>Item setting generik backend: pasangan name/value.</summary>
    public class SettingItemPayload
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("value")] public string Value { get; set; }
    }

    /// <summary>Amplop respons health webhook: data.webhook.</summary>
    public class WebhookHealthEnvelope
    {
        [JsonProperty("webhook")] public WebhookHealthData Webhook { get; set; }
    }

    public class WebhookHealthData
    {
        [JsonProperty("is_running")] public bool IsRunning { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
    }
}
