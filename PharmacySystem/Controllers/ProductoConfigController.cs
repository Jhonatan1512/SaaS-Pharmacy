

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductoConfigController : ControllerBase
    {
        private readonly IProductoSedeConfigService _configService;
        public ProductoConfigController(IProductoSedeConfigService productoSedeConfigService)
        {
            _configService = productoSedeConfigService;
        }

        [HttpPost]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> RegistrarConfiguracion(ProductoSedeConfigDto dto)
        {
            await _configService.RegistrarConfiguracionAsync(dto);
           return Ok(new { message = "Configuración para el producto registrada"});
        }
    }
}
