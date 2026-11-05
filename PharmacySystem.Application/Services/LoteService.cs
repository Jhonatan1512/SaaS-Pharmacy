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

        public async Task ActualizarLoteAsync(Guid id, actualizarLoteDto dto)
        {
            var loteExiste = await _loteRepository.ObtenerLotePorIAsync(id);
            if (loteExiste == null)
                throw new KeyNotFoundException("Registro de lote no encontrado.");

            // 2. Determinar los valores objetivo para evaluar duplicidad
            var productoIdObjetivo = dto.ProductoId ?? loteExiste.ProductoId;

            if (dto.CodigoLote != null)
            {
                dto.CodigoLote = dto.CodigoLote.Trim().ToUpper();
            }
            var codigoLoteObjetivo = dto.CodigoLote ?? loteExiste.CodigoLote;

            // 3. Validar duplicidad excluyendo el propio ID que se está actualizando
            if (await _loteRepository.ExisteCodigoLoteEnOtroRegistroAsync(productoIdObjetivo, codigoLoteObjetivo, id))
            {
                throw new InvalidOperationException($"El producto ya cuenta con otro lote registrado bajo el código '{codigoLoteObjetivo}'.");
            }

            // 4. Copiar únicamente los valores enviadas en el DTO a la entidad
            dto.Adapt(loteExiste);

            // 5. Persistir cambios en la base de datos via ChangeTracker
            await _loteRepository.ActualizarloteAsync(loteExiste);
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

        public async Task<IEnumerable<GetLoteDto>> GetLoteAsync()
        {
            var datosBd = await _loteRepository.ObtenerLotesAsync();

            if (datosBd == null) throw new InvalidOperationException("No existe datos registrado");

            var result = datosBd.Adapt<List<GetLoteDto>>();
            return result;
        }
    }
}
