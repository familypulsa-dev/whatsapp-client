using Newtonsoft.Json;

namespace WaDesktop.Domain.Entities
{
    public class Waba
    {
        [JsonProperty("waba_id")]
        public string WabaId { get; set; }
        [JsonProperty("company_id")]
        public string CompanyId { get; set; }
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("messaging_limit")]
        public string MessagingLimit { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }
        [JsonProperty("last_sync_pricing")]
        public string LastSyncPricing { get; set; }
    }
}
