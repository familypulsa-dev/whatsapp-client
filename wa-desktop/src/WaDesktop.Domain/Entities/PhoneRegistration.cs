using Newtonsoft.Json;

namespace WaDesktop.Domain.Entities
{
    public class CreatePhoneNumberRequest
    {
        [JsonProperty("cc")]
        public string Cc { get; set; }
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }
        [JsonProperty("verified_name")]
        public string VerifiedName { get; set; }
    }

    public class CreatePhoneNumberResponse
    {
        [JsonProperty("phone_number_id")]
        public string PhoneNumberId { get; set; }
    }

    public class RequestCodeRequest
    {
        [JsonProperty("code_method")]
        public string CodeMethod { get; set; } = "SMS";
        [JsonProperty("language")]
        public string Language { get; set; } = "en_US";
    }

    public class VerifyCodeRequest
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public class RegisterPhoneRequest
    {
        [JsonProperty("pin")]
        public string Pin { get; set; }
    }
}
