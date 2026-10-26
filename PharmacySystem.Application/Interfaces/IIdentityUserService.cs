using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface IIdentityUserService
    {
        Task<Guid> CrearCuentaAsync(UsuarioDto dto, string password);
        Task AsignarRoleAsync(Guid identityRole, string roleName);
    }
}
