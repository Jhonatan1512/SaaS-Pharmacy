using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SerieComprobanteController : ControllerBase
    {
        private readonly ISerieComprobanteService _serieComprobanteService;
        public SerieComprobanteController(ISerieComprobanteService serieComprobanteService)
        {
            _serieComprobanteService = serieComprobanteService;
        }

        [HttpPost("crear-serie")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> CrearSerie(SerieComprobanteDto dto)
        {
            await _serieComprobanteService.CrearSerieAsync(dto);
            return Ok(new { message = "Serie creada" });
        }

        [HttpPatch("actualizar-estado-serie/{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ActualizarEstadoSerie(Guid id)
        {
            await _serieComprobanteService.ActualizarEstadoSerieAsync(id);
            return Ok(new { message = "Estado Actualizado" });
        }

        [HttpGet("series")]
        public async Task<ActionResult<IEnumerable<SerieComprobanteResponseDto>>> ObtenerTodas()
        {
            var series = await _serieComprobanteService.ObtenerTodasAsync();
            return Ok(series);
        }

        [HttpGet("serie-sede/{id}")]
        public async Task<ActionResult<SerieComprobanteResponseDto>> ObtenerPorSedeId(Guid id)
        {
            var serie = await _serieComprobanteService.ObtenerPorSedeIdAsync(id);
            return Ok(serie);
        }
    }
}
