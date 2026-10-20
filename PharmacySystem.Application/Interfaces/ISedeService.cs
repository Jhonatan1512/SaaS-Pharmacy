using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface ISedeService
    {
        Task<SedeDto> CrearSedeAsync(SedeDto sede);
        Task<IEnumerable<ObtenerSedeDto>> ObtenerSedeAsync();
    }
}
