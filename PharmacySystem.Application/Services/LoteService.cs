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
    public class LoteService : ILoteService
    {
        private readonly ILoteRepository _loteRepository;
        private readonly IProductoRepository _productoRepository;
        public LoteService(ILoteRepository loteRepository, IProductoRepository productoRepository)
        {
            _loteRepository = loteRepository;
            _productoRepository = productoRepository;
        }
        public async Task CrearLoteAsync(LoteDto dto)
        {
            var productoExiste = await _productoRepository.ObtenerPorIdAsync(dto.ProductoId);
            if (productoExiste == null) 
            {
                throw new KeyNotFoundException("El producto no existe");
            }

            var codigoLoteLimpio = dto.CodigoLote.Trim().ToUpper();

            if(await _loteRepository.ExisteLoteEnProductoAsync(dto.ProductoId, dto.CodigoLote))
            {
                throw new InvalidOperationException("El producto ya tiene un lote registrado");
            }

            var nuevoLote = dto.Adapt<Lote>();
            nuevoLote.Id = Guid.NewGuid(); 
            nuevoLote.CodigoLote = codigoLoteLimpio;
            nuevoLote.FechaCreacion = DateTime.UtcNow;

            await _loteRepository.CrearLoteAsync(nuevoLote);
        }

        public Task<IEnumerable<GetLoteDto>> GetLoteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
