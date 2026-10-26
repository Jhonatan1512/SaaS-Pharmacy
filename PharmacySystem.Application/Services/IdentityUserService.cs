using Microsoft.AspNetCore.Identity;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class IdentityUserService : IIdentityUserService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public IdentityUserService(UserManager<Usuario> userManager, RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task AsignarRoleAsync(Guid identityRole, string roleName)
        {
            var user = await _userManager.FindByIdAsync(identityRole.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException("Usuario de seguridad no encontrado");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var resultCrear = await _roleManager.CreateAsync(new Role
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                });

                if (!resultCrear.Succeeded)
                {
                    throw new Exception($"No se pudo crear el rol: {string.Join(", ", resultCrear.Errors.Select(e => e.Description))}");
                }
            }

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                var resultAsignar = await _userManager.AddToRoleAsync(user, roleName);
                if (!resultAsignar.Succeeded)
                {
                    throw new Exception($"No se pudo asignar el rol al usuario: {string.Join(", ", resultAsignar.Errors.Select(e => e.Description))}");
                }
            }
        }

        public async Task<Guid> CrearCuentaAsync(UsuarioDto dto, string password)
        {
            var newUser = new Usuario
            {
                Email = dto.Email,
                UserName = dto.Email,
                EmailConfirmed = true,

                Documento = dto.TipoDocumento,
                NumeroDoc = dto.NumeroDocumento,
                NombreCompletoRazonSocial = dto.NombreCompletoRazonSocial,
                SedeId = dto.SedeId,
            };

            var result = await _userManager.CreateAsync(newUser, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            return newUser.Id;
        }
    }
}
