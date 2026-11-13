using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TurnoCajaController : ControllerBase
    {
        private readonly ITurnoCajaService _turnoCajaService;
        private readonly ICurrentUserService _currentUserService;
        public TurnoCajaController(ITurnoCajaService turnoCajaService, ICurrentUserService currentUserService)
        {
            _turnoCajaService = turnoCajaService;
            _currentUserService = currentUserService;
        }

        [HttpPost("aperturar-caja")]
        [Authorize(Roles = "Farmaceutico")]
        public async Task<IActionResult> CrearTurno(TurnoCajaDto dto)
        {
            await _turnoCajaService.AbrirTurnoAsync(UsuarioId, dto);
            return Ok(new { message = "Nuevo turno de caja aperturado" });
        }

        [HttpPost("cerrar-turno-caja/{id}")]
        [Authorize(Roles = "Farmaceutico")]
        public async Task<ActionResult<TurnoCajaResponseDto>> CerrarTurno(Guid id, [FromBody] CerrarCajaDto dto)
        {
            await _turnoCajaService.CerrarTurnoAsync(UsuarioId, id, dto);
            return Ok(new { message = "Ha cerrado su turno en la caja"});
        }

        [HttpPatch("corregir-conteo/{id}")]
        public async Task<ActionResult<TurnoCajaResponseDto>> CorregirConteoCierre(Guid id, [FromBody] CorregirConteoTurnoDto dto)
        {
            await _turnoCajaService.CorregirConteoCierreAsync(UsuarioId, id, dto);
            return Ok(new { message = "Se actualizaron los montos de fisicos del cierre de caja"});
        }

        [HttpGet("mi-turno")]
        [Authorize(Roles = "Farmaceutico")]
        public async Task<ActionResult<TurnoActivoResponseDto>> ObtenerMiTurnoActivo()
        {
            var turno = await _turnoCajaService.ObtenerMiTurnoActivoAsync(UsuarioId);
            if (turno == null) return NoContent();
            return Ok(turno);
        }

        [HttpGet("mi-sede")]
        [Authorize(Roles = "Farmaceutico")]
        public async Task<ActionResult<IEnumerable<TurnoCajaResponseDto>>> ObtenerTurnosMiSede()
        {
            var turnos = await _turnoCajaService.ObtenerTurnosMiSedeAsync();
            return Ok(turnos);
        }

        [HttpGet("turno-id/{id}")]
        public async Task<ActionResult<TurnoCajaResponseDto>> ObtenerPorId(Guid id)
        {
            var turno = await _turnoCajaService.ObtenerPorIdAsync(id);
            return Ok(turno);
        }

        private Guid UsuarioId => _currentUserService.GetUserId();
    }
}
