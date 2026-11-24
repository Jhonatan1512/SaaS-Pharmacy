using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface IUserService
    {
        Task CrearUsuarioAsync(UsuarioDto usuario);
        Task ActualizarUsuarioAsync(Guid usuarioId, ActualizarUsuarioDto dto);
        Task<IEnumerable<GetUsuarioDto>> GetAllUsuariosAsync();
        Task ReestablecerPasswordAsync(Guid usuarioId, RestablecerPasswordDto dto);
        Task ActualizarEstadoUsuarioAsync(Guid usuarioId);
    }
}
