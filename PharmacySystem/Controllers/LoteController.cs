using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoteController : ControllerBase
    {
        private readonly ILoteService _loteService;
        public LoteController(ILoteService loteService)
        {
            _loteService = loteService;
        }

        [HttpPost("crear-lote")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> CrearLote(LoteDto dto)
        {
            await _loteService.CrearLoteAsync(dto);
            return Ok(new { message = "Lote creado correctamente" });
        }

        [HttpGet("lista-lotes")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ObtenerListaLotes()
        {
            var result = await _loteService.GetLoteAsync();
            return Ok(result);
        }

        [HttpPatch("actualizar-lote/{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ActualizarLote(Guid id, actualizarLoteDto dto)
        {
            await _loteService.ActualizarLoteAsync(id, dto);
            return Ok(new { messagd = "Registro actualizado" });
        }
    }
}
