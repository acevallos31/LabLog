namespace LabLog.DTOs
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
namespace LabLog.DTOs
{
    public class LoginResponseDto
    {
        public string IdToken { get; set; } = string.Empty;
        public string LocalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

