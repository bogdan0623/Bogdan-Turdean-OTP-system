using System.Text.Json.Serialization;

namespace OTPBackend.DTO
{
    public class UserDto
    {
        public int Id { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
