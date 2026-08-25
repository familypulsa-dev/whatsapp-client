using Newtonsoft.Json;

namespace WaDesktop.Infrastructure.Payloads.Users
{
    /// <summary>Kontrak JSON backend untuk user.</summary>
    public class UserPayload
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("username")] public string Username { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("role")] public string Role { get; set; }
        [JsonProperty("company_id")] public string CompanyId { get; set; }
        [JsonProperty("is_active")] public bool IsActive { get; set; } = true;
    }
}
