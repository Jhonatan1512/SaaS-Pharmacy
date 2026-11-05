using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventarioSedeController : ControllerBase
    {
        private readonly IInventarioSedeService _inventarioSedeService;
        public InventarioSedeController(IInventarioSedeService inventarioSedeService)
        {
            _inventarioSedeService = inventarioSedeService;
        }

        [HttpPost("crear-invetario")]
        public async Task<IActionResult> CrearInventario(InventarioSedeDto dto)
        {
            await _inventarioSedeService.CrearInvetarioSedeAsync(dto);
            return Ok(new { message = "Registro de invetario creado" });
        }
    }
}
