namespace LabLog.Models
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
    public class RegisterResponseDto
    {
        public string IdToken { get; set; } = string.Empty;
        public string LocalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}