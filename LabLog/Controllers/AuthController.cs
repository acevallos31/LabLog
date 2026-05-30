using Microsoft.AspNetCore.Mvc;
using LabLog.DTOs;
using LabLog.Services;

namespace LabLog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Inyectamos el servicio que creamos en los pasos anteriores
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Endpoint requerido: POST /api/Auth/login (sin [Authorize])
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("El email y la contraseña son requeridos.");
            }

            // Toda la lógica de negocio va en el Service, cumpliendo la restricción
            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized("Credenciales inválidas o error en la autenticación con Firebase.");
            }

            // Retorna idToken, localId y email tal como lo pide el documento
            return Ok(result); 
        }
    }
}