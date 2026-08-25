using Newtonsoft.Json;

namespace WaDesktop.Domain.Entities
{
    /// <summary>Respons endpoint login.</summary>
    public class AuthResult
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }
    }
}
