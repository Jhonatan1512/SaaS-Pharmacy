using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public interface ILoteRepository
    {
        Task CrearLoteAsync(Lote lote);
        Task<IEnumerable<Lote>> ObtenerLotesAsync();
        Task<bool> ExisteLoteEnProductoAsync(Guid productoId, string codigoLote);
    }
}
