using System.Security.Claims;
using LabLog.DTOs;
using LabLog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabLog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LabNoteController : ControllerBase
    {
        private readonly LabNoteService _labNoteService;

        public LabNoteController(LabNoteService labNoteService)
        {
            _labNoteService = labNoteService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(LabNoteDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Token invalido");

                var note = await _labNoteService.Create(dto, userId);

                return Ok(note);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Token invalido");

                var notes = await _labNoteService.GetByUser(userId);

                return Ok(notes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Token invalido");

                var deleted = await _labNoteService.Delete(id, userId);

                if (!deleted)
                    return StatusCode(403, "No tiene permiso para eliminar esta nota");

                return Ok("Nota eliminada correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}