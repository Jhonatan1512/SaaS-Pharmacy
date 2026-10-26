using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public interface IProductoRepository
    {
        Task<Producto> CrearProductoAsync(Producto producto);
        Task<IEnumerable<Producto>> GetAllProductsAsync();
        Task<Producto?> ObtenerPorIdAsync(Guid Id);
        Task<Producto?> ObtenerPorCodigoBarrasAsync(string codigoBarras);
    }
}
