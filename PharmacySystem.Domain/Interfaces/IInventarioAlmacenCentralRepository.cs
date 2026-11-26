using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public interface IInventarioAlmacenCentralRepository
    {
        Task AgregarAsync(InventarioAlmacenCentral inventario);
        Task<List<InventarioAlmacenCentral>> ObtenerActivosParaAlertasAsync();
        Task<List<InventarioAlmacenCentral>> ObtenerTodosLosActivosAsync();
        Task<InventarioAlmacenCentral?> ObtenerDetalleRegistroPorIdAsync(Guid id);
        Task GuardarCambiosAsync();
    }
}
