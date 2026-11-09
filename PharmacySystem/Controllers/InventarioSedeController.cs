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
        private readonly ICurrentUserService _currentUser;
        public InventarioSedeController(IInventarioSedeService inventarioSedeService , ICurrentUserService currentUserService)
        {
            _inventarioSedeService = inventarioSedeService;
            _currentUser = currentUserService;
        }

        [HttpPost("crear-invetario")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> CrearInventario(InventarioSedeDto dto)
        {
            await _inventarioSedeService.CrearInvetarioSedeAsync(dto);
            return Ok(new { message = "Registro de invetario creado" });
        }

        [HttpPatch("corregir-precios/{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> CorregirPrecios(Guid id, [FromBody] CorregirPrecioInventarioSedeDto dto)
        {
            await _inventarioSedeService.ActualizarInvetarioSedeAsync(id, dto);
            return NoContent(); 
        }

        [HttpPost("confirmar-recepcion/{id}")]
        [Authorize(Roles = "Farmaceutico")]
        public async Task<IActionResult> ConfirmarRecepcion(Guid id, [FromBody] ConfirmarRecepcionInventarioDto dto)
        {
            await _inventarioSedeService.ConfirmarRecepcionAsync(UsuarioId, id, dto);
            return NoContent(); 
        }

        private Guid UsuarioId => _currentUser.GetUserId();
    }
}
