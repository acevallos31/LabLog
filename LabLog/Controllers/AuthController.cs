using Microsoft.AspNetCore.Mvc;
using LabLog.DTOs;
using LabLog.Services;

namespace LabLog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var user = await _authService.Register(dto);

                return Ok(new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.Role,
                    user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _authService.Login(dto);

                return Ok(new
                {
                    token = token
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}