using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public interface IProductoSedeConfigRepository
    {
        Task<List<ProductoSedeConfig>> ObtenerConInventarioPorSedeAsync(Guid sedeId);
        Task<ProductoSedeConfig?> ObtenerPorSedeYProductoAsync(Guid sedeId, Guid productoId);
        Task<List<ProductoSedeConfig>> ObtenerConfiguracionProductosAsync();
        Task AgregarAsync(ProductoSedeConfig config);
        Task GuardarCambiosAsync();
    }
}
