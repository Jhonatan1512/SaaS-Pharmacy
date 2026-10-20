using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SedeController : ControllerBase
    {
        private readonly ISedeService _sedeService;

        public SedeController(ISedeService sedeService)
        {
            _sedeService = sedeService;
        }

        [HttpPost("crear")]
        public async Task<ActionResult> CrearSede(SedeDto dto)
        {
            var result = await _sedeService.CrearSedeAsync(dto);
            return Ok(result);
        }

        [HttpGet("lista-sedes")]
        public async Task<ActionResult> Obtenersedes()
        {
            var result = await _sedeService.ObtenerSedeAsync();
            return Ok(result);
        }
    }
}
