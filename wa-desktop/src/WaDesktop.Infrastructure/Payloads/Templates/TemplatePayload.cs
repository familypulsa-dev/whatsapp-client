using Newtonsoft.Json;

namespace WaDesktop.Infrastructure.Payloads.Templates
{
    /// <summary>Kontrak JSON backend untuk template (snake_case).</summary>
    public class TemplatePayload
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("waba_id")] public string WabaId { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("language")] public string Language { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("category")] public string Category { get; set; }
        [JsonProperty("message_send_ttl_seconds")] public int? MessageSendTtlSeconds { get; set; }
        [JsonProperty("parameter_format")] public string ParameterFormat { get; set; }
    }
}
