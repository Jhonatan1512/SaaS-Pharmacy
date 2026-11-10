using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;

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
    }
}
