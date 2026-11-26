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
    public class ProductoSedeConfigService : IProductoSedeConfigService
    {
        private readonly IProductoSedeConfigRepository _productoSedeRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly ISedeRepository _sedeRepository;
        public ProductoSedeConfigService(IProductoSedeConfigRepository productoSedeConfigRepository, ISedeRepository sedeRepository, IProductoRepository productoRepository)
        {
            _productoSedeRepository = productoSedeConfigRepository;
            _productoRepository = productoRepository;
            _sedeRepository = sedeRepository;
        }

        public async Task<List<ProductoSedeConfigResponseDto>> ObtenerConfiguracionesProductoAsync()
        {
            var datosBd = await _productoSedeRepository.ObtenerConfiguracionProductosAsync();
            var response = datosBd.Adapt<List<ProductoSedeConfigResponseDto>>();
            return response;
        }

        public async Task RegistrarConfiguracionAsync(ProductoSedeConfigDto dto)
        {
            var sedeId = await _sedeRepository.ObtenerSedePorIdAsync(dto.SedeId);
            if(sedeId == null) throw new KeyNotFoundException("La sede especificada no existe");

            var productoId = await _productoRepository.ObtenerPorIdAsync(dto.ProductoId);
            if (productoId == null) throw new KeyNotFoundException("El producto especificado no existe");

            var configExistente = await _productoSedeRepository.ObtenerPorSedeYProductoAsync(dto.SedeId, dto.ProductoId);
            if (configExistente != null)
            {
                dto.Adapt(configExistente); 
            }
            else
            {
                var newConfig = dto.Adapt<ProductoSedeConfig>();
                await _productoSedeRepository.AgregarAsync(newConfig);
            }
            await _productoSedeRepository.GuardarCambiosAsync();
        }
    }
}
