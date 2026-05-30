using System.Net.Http.Json;
using System.Text.Json.Serialization;
using LabLog.DTOs;

namespace LabLog.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _firebaseApiKey;

        public AuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Corregido para .NET 10: Se pasa el mensaje de manera limpia
            _firebaseApiKey = configuration["Firebase:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "Firebase API Key no configurada.");
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto)
        {
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_firebaseApiKey}";

            var payload = new
            {
                email = loginDto.Email,
                password = loginDto.Password,
                returnSecureToken = true
            };

            var response = await _httpClient.PostAsJsonAsync(url, payload);

            if (!response.IsSuccessStatusCode)
            {
                return null; 
            }

            var firebaseResult = await response.Content.ReadFromJsonAsync<FirebaseLoginPayload>();

            if (firebaseResult == null) return null;

            return new LoginResponseDto
            {
                IdToken = firebaseResult.IdToken,
                LocalId = firebaseResult.LocalId,
                Email = firebaseResult.Email
            };
        }

        // Esta clase es necesaria al final para mapear lo que responde Firebase
        private class FirebaseLoginPayload
        {
            [JsonPropertyName("idToken")]
            public string IdToken { get; set; } = string.Empty;

            [JsonPropertyName("localId")]
            public string LocalId { get; set; } = string.Empty;

            [JsonPropertyName("email")]
            public string Email { get; set; } = string.Empty;
        }
    }

    public interface IAuthService
    {
    }
}