using LabLog.DTOs;
using Newtonsoft.Json;
using ProyectoClaseQ2.DTOs;
using System.Text;
using System.Text.Json.Serialization;

namespace ProyectoClaseQ2.Services;

public class AuthService
{
    // Maneja lo relacionado a registro e inicio de sesion
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public AuthService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<AuthResponseDto> Register(RegisterDto dto)
    {
        // Firebase Authentication crea el usuario
        var apiKey = _configuration["Firebase:ApiKey"];

        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}";

        var requestBody = new
        {
            email = dto.Email,
            password = dto.Password,
            displayName = dto.DisplayName,
            returnSecureToken = true
        };

        var json = JsonConvert.SerializeObject(requestBody);

        var response = await _httpClient.PostAsync(
            url,
            new StringContent(json, Encoding.UTF8, "application/json")
        );

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(responseContent);

        var firebaseResponse =
            JsonConvert.DeserializeObject<FirebaseAuthResponse>(responseContent);

        return new AuthResponseDto
        {
            IdToken = firebaseResponse!.IdToken,
            LocalId = firebaseResponse.LocalId,
            Email = firebaseResponse.Email
        };
    }

    public async Task<AuthResponseDto> Login(LoginDto dto)
    {
        // Firebase Authentication valida el correo y password
        var apiKey = _configuration["Firebase:ApiKey"];

        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

        var requestBody = new
        {
            email = dto.Email,
            password = dto.Password,
            returnSecureToken = true
        };

        var json = JsonConvert.SerializeObject(requestBody);

        var response = await _httpClient.PostAsync(
            url,
            new StringContent(json, Encoding.UTF8, "application/json")
        );

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(responseContent);

        var firebaseResponse =
            JsonConvert.DeserializeObject<FirebaseAuthResponse>(responseContent);

        return new AuthResponseDto
        {
            IdToken = firebaseResponse!.IdToken,
            LocalId = firebaseResponse.LocalId,
            Email = firebaseResponse.Email
        };
    }

    private class FirebaseAuthResponse
    {
        [JsonProperty("idToken")]
        public string IdToken { get; set; } = string.Empty;

        [JsonProperty("localId")]
        public string LocalId { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;
    }
}