using Newtonsoft.Json;

namespace WaDesktop.Infrastructure.Payloads.Billing
{
    /// <summary>Kontrak JSON backend untuk ringkasan pemakaian per bulan.</summary>
    public class WaWabaUsageSummaryPayload
    {
        [JsonProperty("month_period")] public string MonthPeriod { get; set; }
        [JsonProperty("total_volume")] public int TotalVolume { get; set; }
        [JsonProperty("marketing_cost")] public double MarketingCost { get; set; }
        [JsonProperty("utility_cost")] public double UtilityCost { get; set; }
        [JsonProperty("auth_cost")] public double AuthCost { get; set; }
        [JsonProperty("service_cost")] public double ServiceCost { get; set; }
        [JsonProperty("total_cost")] public double TotalCost { get; set; }
    }
}
