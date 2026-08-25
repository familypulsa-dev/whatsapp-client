using Newtonsoft.Json;

namespace WaDesktop.Domain.Entities
{
    /// <summary>Bentuk respons save profil phone number.</summary>
    public class SavePhoneResult
    {
        [JsonProperty("data")]
        public PhoneNumberDetail Detail { get; set; }
        [JsonProperty("warnings")]
        public System.Collections.Generic.List<string> Warnings { get; set; }
    }
}
