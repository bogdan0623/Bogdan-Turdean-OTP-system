﻿using System.Text.Json.Serialization;

namespace OTPBackend.DTO
{
    public class RegisterDto
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = default!;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = default!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = default!;

        [JsonPropertyName("password_confirm")]
        public string PasswordConfirms { get; set; } = default!;
    }
}
