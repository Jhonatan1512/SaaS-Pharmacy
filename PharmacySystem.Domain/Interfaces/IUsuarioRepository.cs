using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorDniAsync(string dni); 
        Task<Usuario?> ObtenerPorRucAsync(string ruc);
        Task<Usuario?> ObtenerPorIdAsync(Guid id);
        Task<IEnumerable<Usuario>> ObtenerTodosUsuariosAsync();
    }
}
