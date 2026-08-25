using Newtonsoft.Json;

namespace WaDesktop.Domain.Entities
{
    public class WebhookConfig
    {
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }
        [JsonProperty("whatsapp_business_account")]
        public string WhatsAppBusinessAccount { get; set; }
        [JsonProperty("application")]
        public string Application { get; set; }
    }
}
