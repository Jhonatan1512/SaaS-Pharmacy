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
        private readonly IDetallesVentasService _detallesVentasService;

        public VentaController(IVentaService ventaService, ICurrentUserService currentUserService, IDetallesVentasService detallesVentasService)
        {
            _currentUserService = currentUserService;
            _ventaService = ventaService;
            _detallesVentasService = detallesVentasService;
        }

        [HttpPost("registrar-venta")]
        [Authorize(Roles = "Farmaceutico")]
        public async Task<IActionResult> RegistrarVenta(VentasDto dto)
        {
            await _ventaService.RealizarVentaAsync(UserId, dto);
            return Ok(new { message = "Venta realizada" });
        }

        [HttpPatch("anular-venta/{id}")]
        [Authorize(Roles = "Farmaceutico")]
        public async Task<IActionResult> AnularVenta(Guid id, AnularVentaDto dto)
        {
            await _ventaService.AnularVentaAsync(id, dto);
            return Ok(new { message = "Venta anulada" });
        }

        [HttpGet("ver-total-ventas")]
        [Authorize(Roles = "Farmaceutico")]
        public async Task<IActionResult> VerTotalVentas()
        {
            var detalle = await _detallesVentasService.EjecutarAsync(UserId);
            return Ok(detalle);
        }

        [HttpGet("ventas")]
        [Authorize(Roles = "Farmaceutico")]
        public async Task<IActionResult> ObtenerVentas()
        {
            var response = await _detallesVentasService.ObtenerVentasDelDiaAsync(UserId);
            return Ok(response);
        }

        [Authorize(Roles = "Farmaceutico")]
        [HttpGet("{ventaId:guid}/detalles")]
        public async Task<ActionResult<List<DetalleVentasDto>>> ObtenerDetallesPorVenta(Guid ventaId)
        {
            var detalles = await _detallesVentasService.ObtenerDetalleAsync(ventaId);
            return Ok(detalles);
        }

        [Authorize(Roles = "Farmaceutico")]
        [HttpGet("historial-turnos-cerrados")]
        public async Task<IActionResult> ObteneHistorialVentas([FromQuery] DateTime fechaCierre)
        {
            var ventas = await _detallesVentasService.HistorialVentasVentasAsync(fechaCierre, UserId);
            return Ok(ventas);
        }

        private Guid UserId => _currentUserService.GetUserId();
    }
}
