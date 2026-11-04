using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface IProductoService
    {
        Task CrearProductoAsync(ProductoDto dto);
        Task<IEnumerable<GetProductosDto>> ObtenerProductosAsync();
        Task ActualizarProductoAsync(Guid id, ActualizarProductoDto dto);
        Task ActualizarEstadoProductoAsync(Guid id);
    }
}
