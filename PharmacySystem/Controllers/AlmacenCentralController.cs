using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Application.Services;
using PharmacySystem.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlmacenCentralController : ControllerBase
    {
        private readonly IAlmacenCentralService _centralService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAlertasAlmacenCentralSerivce _alertasAlmacenCentralSerivce;
        public AlmacenCentralController(IAlmacenCentralService almacenCentralService, ICurrentUserService currentUserService, IAlertasAlmacenCentralSerivce alertasAlmacenCentralSerivce)
        {
            _centralService = almacenCentralService;
            _currentUserService = currentUserService;
            _alertasAlmacenCentralSerivce = alertasAlmacenCentralSerivce;
        }

        [HttpPost("producto-con-lote")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> RegistrarProductoConLote(RegistrarProductoConLoteDto dto)
        {
            await _centralService.RegistrarProductoConLoteInicialAsync(UsuarioId, dto);
            return Ok(new { message = "Registros creados correctamente"});
        }

        [HttpPost("registrar-lote")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> RegistrarIngresoLote(RegistrarIngresoLoteDto dto)
        {
            await _centralService.RegistrarIngresoLoteExistenteAsync(UsuarioId, dto);
            return Ok(new { message = "Se registro el lote para el producto"});
        }

        [HttpGet("alertas")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult<AlertasAlmacenCentralResponseDto>> ObtenerAlertas([FromQuery] int diasAnticipacionVencimiento = 90)
        {
            var alertas = await _alertasAlmacenCentralSerivce.ObtenerAlertasAlmacenCentralAsync(diasAnticipacionVencimiento);
            return Ok(alertas);
        }

        [HttpGet("lista-productos")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ObtenerTodosProductos()
        {
            var result = await _centralService.ObtenerTodosLosActivosAsync();
            return Ok(result);
        }

        [HttpGet("lista-detalle-registro/{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ObtenerDetalleRegistro(Guid id)
        {
            var result = await _centralService.ObtenerDetalleRegistroAsync(id);
            return Ok(result);
        }

        private Guid UsuarioId => _currentUserService.GetUserId();
    }
}
