using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;
        private readonly ICurrentUserService _currentUserService;

        public VentaController(IVentaService ventaService, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _ventaService = ventaService; 
        }

        [HttpPost("registrar-venta")]
        [Authorize(Roles = "Farmaceutico")]
        public async Task<IActionResult> RegistrarVenta(VentasDto dto)
        {
            await _ventaService.RealizarVentaAsync(UserId, dto);
            return Ok(new { message = "Venta realizada" });
        }

        private Guid UserId => _currentUserService.GetUserId();
    }
}
