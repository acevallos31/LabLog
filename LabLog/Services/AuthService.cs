using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using LabLog.DTOs;
using LabLog.Models;

namespace LabLog.Services;

public class AuthService
{
    private readonly FirebaseService _firebaseService;
    private readonly IConfiguration _configuration;

    public AuthService(
        FirebaseService firebaseService,
        IConfiguration configuration)
    {
        _firebaseService = firebaseService;
        _configuration = configuration;
    }

    public async Task<User> Register(RegisterDto dto)
    {
        var collection = _firebaseService.GetCollection("users");

        var existing = await collection
            .WhereEqualTo("Email", dto.Email)
            .GetSnapshotAsync();

        if (existing.Count > 0)
            throw new Exception("Ya existe un usuario con ese correo");

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            FullName = dto.DisplayName,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            Role = "user",
            CreatedAt = DateTime.UtcNow
        };

        await collection.Document(user.Id).SetAsync(
            new Dictionary<string, object>
            {
                { "Id", user.Id },
                { "FullName", user.FullName },
                { "Email", user.Email },
                { "PasswordHash", user.PasswordHash },
                { "Role", user.Role },
                { "CreatedAt", user.CreatedAt }
            });

        return user;
    }

    public async Task<string> Login(LoginDto dto)
    {
        var collection = _firebaseService.GetCollection("users");

        var snapshot = await collection
            .WhereEqualTo("Email", dto.Email)
            .GetSnapshotAsync();

        if (snapshot.Count == 0)
            throw new Exception("No existe ningún usuario con ese correo");

        var doc = snapshot.Documents[0];

        var data = doc.ToDictionary();

        var user = new User
        {
            Id = data["Id"].ToString()!,
            FullName = data["FullName"].ToString()!,
            Email = data["Email"].ToString()!,
            PasswordHash = data["PasswordHash"].ToString()!,
            Role = data["Role"].ToString()!,
            CreatedAt =
                ((Timestamp)data["CreatedAt"]).ToDateTime()
        };

        if (!VerifyPassword(dto.Password, user.PasswordHash))
            throw new Exception("Password incorrecto");

        return GenerateToken(user);
    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id),

            new Claim(
                ClaimTypes.Email,
                user.Email),

            new Claim(
                ClaimTypes.Role,
                user.Role)
        };

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

        var creds =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    private bool VerifyPassword(
        string password,
        string passwordHash)
    {
        return HashPassword(password) == passwordHash;
    }

    private string HashPassword(string password)
    {
        var bytes =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(password));

        return Convert.ToBase64String(bytes);
    }
}