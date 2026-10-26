using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;
        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpPost("crear-producto")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> CrearProducto(ProductoDto dto)
        {
            await _productoService.CrearProductoAsync(dto);
            return Ok(new { menssage = "Producto creado correctamente" });
        }

        [HttpGet("lista-producto")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ObtenerProductos()
        {
            var result = await _productoService.ObtenerProductosAsync();
            return Ok(result);
        }
    }
}
