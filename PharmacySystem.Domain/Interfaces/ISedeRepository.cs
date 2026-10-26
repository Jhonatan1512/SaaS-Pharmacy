using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public interface ISedeRepository
    {
        Task<Sede> CrearSedeAsync(Sede sede);
        Task<IEnumerable<Sede>> GetSedeAsync();
        Task<Sede?> ObtenerSedeIdAsync(Guid Id);
        Task ActualizarSedeAsync(Sede sede);
        Task<Sede?> ObtenerSedePorIdAsync(Guid Id);
    }
}
