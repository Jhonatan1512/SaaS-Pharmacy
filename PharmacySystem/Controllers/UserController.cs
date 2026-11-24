using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create-user")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> CrearUsuario(UsuarioDto dto)
        {
            await _userService.CrearUsuarioAsync(dto);
            return Ok(new { message = "Usuario creado correctatemente" });
        }

        [HttpGet("lista-personal")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var result = await _userService.GetAllUsuariosAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Gerente")]
        [HttpPatch("actualizar-datos/{id}")]
        public async Task<IActionResult> ActualizarUsuario(Guid id, ActualizarUsuarioDto dto)
        {
            await _userService.ActualizarUsuarioAsync(id, dto);
            return Ok(new { message = "Datos del usuario actualizado" });
        }

        [Authorize(Roles = "Gerente")]
        [HttpPatch("restablecer-password/{id}")]
        public async Task<IActionResult> RestablecerPassword(Guid id, RestablecerPasswordDto dto)
        {
            await _userService.ReestablecerPasswordAsync(id, dto);
            return Ok(new { message = "Se ha modificado la password"});
        }

        [Authorize(Roles = "Gerente")]
        [HttpPatch("actualiza-estado-user/{id}")]
        public async Task<IActionResult> ActualizarEstadoUsuario(Guid id)
        {
            await _userService.ActualizarEstadoUsuarioAsync(id);
            return Ok(new { message = "Se ha actualizado el estado del usuario" });
        }
    }
}
