using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface ILoteService
    {
        Task CrearLoteAsync(LoteDto dto);
        Task<IEnumerable<GetLoteDto>> GetLoteAsync();
        Task ActualizarLoteAsync(Guid id, actualizarLoteDto dto);
    }
}
