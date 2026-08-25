using System.Collections.Generic;
using Newtonsoft.Json;

namespace WaDesktop.Infrastructure.Payloads.PhoneNumbers
{
    /// <summary>Item ringkas untuk tree/sidebar.</summary>
    public class PhoneNumberNodePayload
    {
        [JsonProperty("phone_number_id")] public string PhoneNumberId { get; set; }
        [JsonProperty("display_phone_number")] public string DisplayPhoneNumber { get; set; }
        [JsonProperty("verified_name")] public string DisplayName { get; set; }
    }

    /// <summary>Kontrak JSON backend untuk detail phone number.</summary>
    public class PhoneNumberDetailPayload
    {
        [JsonProperty("phone_number_id")] public string PhoneNumberId { get; set; }
        [JsonProperty("waba_id")] public string WabaId { get; set; }
        [JsonProperty("display_name")] public string DisplayName { get; set; }
        [JsonProperty("display_phone_number")] public string DisplayPhone { get; set; }
        [JsonProperty("quality_rating")] public string QualityRating { get; set; }
        [JsonProperty("name_status")] public string NameStatus { get; set; }
        [JsonProperty("code_verification_status")] public string CodeVerificationStatus { get; set; }
        [JsonProperty("meta_status")] public string MetaStatus { get; set; }
        [JsonProperty("pin_enabled")] public bool PinEnabled { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("about")] public string About { get; set; }
        [JsonProperty("address")] public string Address { get; set; }
        [JsonProperty("vertical")] public string Vertical { get; set; }
        [JsonProperty("websites")] public List<string> Websites { get; set; }
        [JsonProperty("profile_picture")] public string ProfilePictureUrl { get; set; }
        [JsonProperty("created_at")] public string CreatedAt { get; set; }
        [JsonProperty("updated_at")] public string UpdatedAt { get; set; }
    }

    /// <summary>Bentuk respons save profil (paritas dengan perilaku unwrap lama).</summary>
    public class SavePhonePayload
    {
        [JsonProperty("data")] public PhoneNumberDetailPayload Detail { get; set; }
        [JsonProperty("warnings")] public List<string> Warnings { get; set; }
    }
}
