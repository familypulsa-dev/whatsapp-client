using Newtonsoft.Json;

namespace WaDesktop.Infrastructure.Payloads.Companies
{
    /// <summary>Kontrak JSON backend untuk company (snake_case).</summary>
    public class CompanyPayload
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("limit_marketing")] public int? LimitMarketing { get; set; }
        [JsonProperty("limit_utility")] public int? LimitUtility { get; set; }
        [JsonProperty("limit_authentication")] public int? LimitAuthentication { get; set; }
        [JsonProperty("limit_service")] public int? LimitService { get; set; }

        [JsonProperty("usage_marketing")] public int UsageMarketing { get; set; }
        [JsonProperty("usage_utility")] public int UsageUtility { get; set; }
        [JsonProperty("usage_authentication")] public int UsageAuthentication { get; set; }
        [JsonProperty("usage_service")] public int UsageService { get; set; }

        [JsonProperty("meta_cost")] public decimal MetaCost { get; set; }
        [JsonProperty("created_at")] public string CreatedAt { get; set; }
    }
}
