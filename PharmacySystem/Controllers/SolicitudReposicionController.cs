using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Application.Services;
using PharmacySystem.Domain.Enums;
using System.Security.Claims;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SolicitudReposicionController : ControllerBase
    {
        private readonly ISolicitudReposicionService _solicitudReposicionService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISolicitudReposicionAdminService _serviceAdmin;
        public SolicitudReposicionController(ISolicitudReposicionService solicitudReposicionService, ICurrentUserService currentUserService,
            ISolicitudReposicionAdminService adminService)
        {
            _currentUserService = currentUserService;
            _solicitudReposicionService = solicitudReposicionService;
            _serviceAdmin = adminService;
        }

        [HttpPost]
        [Authorize(Roles = "Farmaceutico")]
        public async Task<IActionResult> CrearSolicitud(CrearSolicitudReposicionDto dto)
        {
            await _solicitudReposicionService.CrearsolicitudAsync(UsuarioId, dto);
            return Ok(new { message = "Solicitud de reposición creada" });
        }

        [HttpGet("ver-detalle-solicitud/{id}")]
        public async Task<IActionResult> ObtenerDetalleSolicutd(Guid id)
        {
            var result = await _solicitudReposicionService.ObtenerSolicitudPorId(id);
            return Ok(result);
        }

        [HttpGet("lista-solicitudes")]
        public async Task<IActionResult> ObtenerTodasConDetalleSolicutd([FromQuery] EstadoSolicitudEnum? estado = null, [FromQuery] Guid? sedeId = null)
        {
            var result = await _solicitudReposicionService.ListarsolicitudesAsync();
            return Ok(result);
        }

        //Acciones por parte de gerencia

        [HttpPatch("responder-solictud/{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ResponderSolicitud(Guid id, ResponderSolicitudReposicionDto dto)
        {
            await _serviceAdmin.ResponderSolicitudAsync(id, UsuarioId, dto);
            return Ok(new { message = "Se dio respuesta a la solicitud"});
        }

        [HttpGet("lista-admin")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult<List<SolicitudReposicionDocumentoDto>>> ListarTodas([FromQuery] Guid? sedeId = null, [FromQuery] EstadoSolicitudEnum? estado = null)
        {
            var solicitudes = await _serviceAdmin.ListarTodasAsync(sedeId, estado);
            return Ok(solicitudes);
        }

        [HttpGet("detalle-admin/{id:guid}")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult<SolicitudReposicionDocumentoDto>> ObtenerPorId(Guid id)
        {
            var solicitud = await _serviceAdmin.ObtenerPorIdAsync(id);
            return Ok(solicitud);
        }

        [HttpPut("modificar-respuesta/{id:guid}")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult<SolicitudReposicionDocumentoDto>> ModificarRespuestaSolicitud(Guid id, ResponderSolicitudReposicionDto dto)
        {
            await _serviceAdmin.ModificarRespuestaSolicitudAsync(id, UsuarioId, dto);
            return Ok(new { message = "Se actualizo la respuesta de la solicitud"});
        }

        [HttpPatch("finalizar-solicitud/{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> FinalizarSolicitud(Guid id)
        {
            await _serviceAdmin.FinalizarSolictudAsync(id);
            return Ok(new { message = "Proceso de la solitud finalizado"});
        }

        private Guid UsuarioId => _currentUserService.GetUserId();
    }
}
