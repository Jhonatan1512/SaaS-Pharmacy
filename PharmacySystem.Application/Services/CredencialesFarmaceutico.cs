using Microsoft.AspNetCore.Identity;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class CredencialesFarmaceutico : ICredencialesFermaceutico
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IUsuarioRepository _userRepository;

        public CredencialesFarmaceutico(UserManager<Usuario> userManager, IUsuarioRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }
        public async Task ValidarRegistrosDuplicados(string? email, string? dni, string? ruc, Guid? usuarioIdExcluir = null)
        {
            if (!string.IsNullOrWhiteSpace(dni))
            {
                var dniExiste = await _userRepository.ObtenerPorDniAsync(dni.Trim());
                if (dniExiste != null && dniExiste.Id != usuarioIdExcluir)
                {
                    throw new InvalidOperationException("El DNI ya está registrado.");
                }
            }

            if (!string.IsNullOrWhiteSpace(ruc))
            {
                var rucExiste = await _userRepository.ObtenerPorRucAsync(ruc.Trim());
                if (rucExiste != null && rucExiste.Id != usuarioIdExcluir)
                {
                    throw new InvalidOperationException("El RUC ya está registrado.");
                }
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailExiste = await _userManager.FindByEmailAsync(email.Trim());
                if (emailExiste != null && emailExiste.Id != usuarioIdExcluir)
                {
                    throw new InvalidOperationException("El Email ya está registrado.");
                }
            }
        }
    }
}
