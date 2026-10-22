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
            return Ok(new {message = "Usuario creado correctatemente"});
        }

        [HttpGet("lista-personal")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var result = await _userService.GetAllUsuariosAsync();
            return Ok(result);
        }
    }
}
