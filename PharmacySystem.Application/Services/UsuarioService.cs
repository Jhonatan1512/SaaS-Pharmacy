using Mapster;
using Microsoft.AspNetCore.Identity;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class UsuarioService : IUserService
    {
        private readonly IIdentityUserService _identityUserService;
        private readonly ICredencialesFermaceutico _credencialesFermaceutico;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UserManager<Usuario> _userManager;
        public UsuarioService(IIdentityUserService identityUserService, ICredencialesFermaceutico credencialesFermaceutico, 
            IUsuarioRepository usuarioRepository, UserManager<Usuario> userManager)
        {
            _identityUserService = identityUserService;
            _credencialesFermaceutico = credencialesFermaceutico;
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
        }

        private string GenerarPassword(string dni)
        {
            return $"Pl{dni}*"; 
        }

        public async Task CrearUsuarioAsync(UsuarioDto usuario)
        {
            await _credencialesFermaceutico.ValidarRegistrosDuplicados(usuario.Email, usuario.NumeroDocumento, usuario.NumeroDocumento);
            var passwordGenerada = GenerarPassword(usuario.NumeroDocumento);
            var identityuserId = await _identityUserService.CrearCuentaAsync(usuario, passwordGenerada);
            var rolAsignar = string.IsNullOrEmpty(usuario.Rol) ? "Farmaceutico" : usuario.Rol;
            await _identityUserService.AsignarRoleAsync(identityuserId, rolAsignar);            
        }

        public async Task<IEnumerable<GetUsuarioDto>> GetAllUsuariosAsync()
        {
            var usuarioData = await _usuarioRepository.ObtenerTodosUsuariosAsync();
            var result = usuarioData.Adapt<List<GetUsuarioDto>>();

            foreach (var dto in result)
            {
                var user = usuarioData.First(u => u.Id == dto.Id);
                var roles = await _userManager.GetRolesAsync(user);
                dto.Rol = roles.FirstOrDefault() ?? string.Empty; 
            }

            return result;
        }
    }
}
