using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface IInventarioSedeService
    {
        Task CrearInvetarioSedeAsync(InventarioSedeDto dto);
        Task<IEnumerable<InvetarioSedeResponseDto>> GetInvetarioSedeAsync();
        Task ActualizarInvetarioSedeAsync(Guid inventarioSedeId, CorregirPrecioInventarioSedeDto dto);
        Task ConfirmarRecepcionAsync(Guid userId, Guid inventarioSedeId, ConfirmarRecepcionInventarioDto dto);
    }
}
