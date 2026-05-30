<<<<<<< HEAD
﻿using System.Net.Http.Json;
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
=======
﻿using LabLog.DTOs;
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
>>>>>>> 369047ebd21c0da9bd41071e7c8ec21d01030d88
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }
<<<<<<< HEAD
=======

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
>>>>>>> 369047ebd21c0da9bd41071e7c8ec21d01030d88
}