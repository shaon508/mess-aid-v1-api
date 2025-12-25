using System.Text.Json.Serialization;

namespace MessAidVOne.Application.DTOs
{
    public class LoginDto
    {
        public string Token { get; set; }
        [JsonIgnore]
        public string? DeviceId { get; set; }
        [JsonIgnore]
        public string? DeviceToken { get; set; }
        [JsonIgnore]
        public UserInformation User { get; set; }
    }
}
