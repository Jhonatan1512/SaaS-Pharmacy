using Mapster;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class InvetarioSedeService : IInventarioSedeService
    {
        private readonly IInventarioSedeRepository _inventarioSedeRepository;
        private readonly ISedeRepository _sedeRepository;
        private readonly ILoteRepository _loteRepository;
        public InvetarioSedeService(IInventarioSedeRepository inventarioSedeRepository, ISedeRepository sedeRepository, ILoteRepository loteRepository)
        {
            _inventarioSedeRepository = inventarioSedeRepository;
            _sedeRepository = sedeRepository;
            _loteRepository = loteRepository;
        }
        public async Task CrearInvetarioSedeAsync(InventarioSedeDto dto)
        {
            var existeSede = await _sedeRepository.ObtenerSedeIdAsync(dto.SedeId);
            if (existeSede == null) throw new KeyNotFoundException("La sede especificada no existe.");

            var existeLote = await _loteRepository.ObtenerLotePorIAsync(dto.LoteId);
            if (existeLote == null) throw new KeyNotFoundException("El lote especificado no existe.");

            var inventarioExistente = await _inventarioSedeRepository.ObtenerPorSedeYLoteActualizarAsync(dto.SedeId, dto.LoteId);
            if (inventarioExistente != null)
            {
                //Actualizamos si es que ya existe registro en la BD
                inventarioExistente.ReabastecerStock(
                    dto.StockFisicoUnidades,
                    dto.PrecioVentaUnidad,
                    dto.PrecioVentaBlister,
                    dto.PrecioVentaCaja
                );

                await _inventarioSedeRepository.ActualizarAsync();
            } else
            {
                // Crear nuevo registro si es el primer envío de este lote a la sede
                var nuevoInventario = dto.Adapt<InventarioSede>();
                nuevoInventario.Id = Guid.NewGuid();

                await _inventarioSedeRepository.CrearInventarioSedeAsync(nuevoInventario);
            }            
        }

        public Task<IEnumerable<InvetarioSedeResponseDto>> GetInvetarioSedeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
