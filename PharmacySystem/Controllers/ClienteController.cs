using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpPost("crear-cliente")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> CrearCliente(ClienteDto dto)
        {
            await _clienteService.CrearAsync(dto);
            return Ok(new { message = "Nuevo cliente creado"});
        }

        [HttpGet("lista-clientes")]
        public async Task<IActionResult> ObtenerCliente(ClienteDto dto)
        {
            var result = await _clienteService.ObtenerTodosAsync();
            return Ok(result);
        }

        [HttpPatch("actualizar-cliente/{id}")]
        public async Task<IActionResult> ActualizarCliente(Guid id, ClienteUpdateDto dto)
        {
            await _clienteService.ActualizarAsync(id, dto);
            return Ok(new { message = "Datos del cliente actualizado" });
        }

        [HttpGet("cliente-documento/{numeroDocumento}")]
        public async Task<ActionResult<ClienteResponseDto>> ObtenerPorDocumento(string numeroDocumento)
        {
            var cliente = await _clienteService.ObtenerPorDocumentoAsync(numeroDocumento);
            return Ok(cliente);
        }

        [HttpGet("cliente-id/{id}")]
        public async Task<ActionResult<ClienteResponseDto>> ObtenerPorId(Guid id)
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            return Ok(cliente);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            await _clienteService.EliminarAsync(id);
            return NoContent();
        }
    }
}
