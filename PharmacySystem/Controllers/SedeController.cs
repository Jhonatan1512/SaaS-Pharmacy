using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SedeController : ControllerBase
    { 
        private readonly ISedeService _sedeService;

        public SedeController(ISedeService sedeService)
        {
            _sedeService = sedeService;
        }

        [HttpPost("crear")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult> CrearSede(SedeDto dto)
        {
            var result = await _sedeService.CrearSedeAsync(dto);
            return Ok(result);
        }

        [HttpGet("lista-sedes")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult> Obtenersedes()
        {
            var result = await _sedeService.ObtenerSedeAsync();
            return Ok(result);
        }

        [HttpGet("lista-personal/{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ObtenerPersonalSede(Guid id)
        {
            var result = await _sedeService.ObtenerPersonalOficinaAsync(id);
            return Ok(result);
        }

        [HttpPut("update-sede/{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ActualizarSede(ActualizarSedeDto dto, Guid id)
        {
            await _sedeService.ActualizarSedeAsync(dto, id);
            return Ok(new { mesagge = "Datos Actualizados"});
        }

        [HttpPatch("update-estado/{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ActualizarEstadoSede(Guid id)
        {
            var result = await _sedeService.ActualizarEstadoAsync(id);
            return Ok(new { message = "Estado modificado", nuevoEstado =  result});
        }
    }
}
