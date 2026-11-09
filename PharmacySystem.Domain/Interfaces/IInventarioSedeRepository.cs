using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public interface IInventarioSedeRepository
    {
        Task<bool> ExisteInventarioEnSedeAsync(Guid sedeId, Guid loteId);
        Task<InventarioSede?> ObtenerPorSedeYLoteActualizarAsync(Guid sedeId, Guid loteId);
        Task CrearInventarioSedeAsync(InventarioSede sede);
        Task ActualizarAsync();
        Task<InventarioSede?> ObtenerCapaActivaPorSedeLoteYPrecioAsync(Guid sedeId, Guid loteId, decimal? precioVenta);
        Task EliminarRegistroAsync(InventarioSede inventario);
        Task<InventarioSede?> ObtenerPorIdAsync(Guid id);
        Task<InventarioSede?> ObtenerPorSedeLotePrecioYEstadoAsync(Guid sedeId, Guid loteId, decimal precioVentaUnidad, EstadoLoteSedeEnum estado);
    }
}
