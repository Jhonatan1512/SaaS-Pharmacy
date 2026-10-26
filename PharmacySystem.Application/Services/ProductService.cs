using Mapster;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class ProductService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }
        public async Task CrearProductoAsync(ProductoDto dto)
        {
            var existeCodigoBarras = await _productoRepository.ObtenerPorCodigoBarrasAsync(dto.CodigoBarras!);
            if (existeCodigoBarras != null) throw new InvalidOperationException("El código de barras ya esta registrado");

            if (dto.UnidadesPorCaja <= 0 || dto.UnidadesPorBlister <= 0)
            {
                throw new ArgumentException("Los factores de conversión (unidades por caja/blister) deben ser mayores a 0.");
            }

            var nuevoProducto = dto.Adapt<Producto>();
            nuevoProducto.Id = Guid.NewGuid();
            nuevoProducto.Activo = true;
            nuevoProducto.FechaCreacion = DateTime.UtcNow;

            await _productoRepository.CrearProductoAsync(nuevoProducto); 
        }

        public async Task<IEnumerable<GetProductosDto>> ObtenerProductosAsync()
        {
            var datosDto = await _productoRepository.GetAllProductsAsync();
            var result = datosDto.Adapt<List<GetProductosDto>>();
            return result;
        }
    }
}
