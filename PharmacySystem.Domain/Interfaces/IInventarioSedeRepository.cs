using PharmacySystem.Domain.Entities;
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
    }
}
