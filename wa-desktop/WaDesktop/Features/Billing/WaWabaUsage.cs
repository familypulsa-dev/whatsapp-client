using System;
using Newtonsoft.Json;

namespace WaDesktop.Domain.Entities
{
    public class WaWabaUsage
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("waba_id")]
        public string WabaId { get; set; }

        [JsonProperty("start_time")]
        public long StartTime { get; set; }

        [JsonProperty("end_time")]
        public long EndTime { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("pricing_category")]
        public string PricingCategory { get; set; }

        [JsonProperty("volume")]
        public int Volume { get; set; }

        [JsonProperty("cost")]
        public double Cost { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
