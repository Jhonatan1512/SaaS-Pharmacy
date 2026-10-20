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
        public async Task ValidarRegistrosDuplicados(string email, string dni, string ruc)
        {
            var dniExiste = await _userRepository.ObtenerPorDniAsync(dni);
            if (dniExiste != null)
            {
                throw new InvalidOperationException("El DNI ya esta registrado");
            }

            var rucExiste = await _userRepository.ObtenerPorRucAsync(ruc);
            if (rucExiste != null)
            {
                throw new InvalidOperationException("El RUC ya esta registrado");
            }

            var emailExiste = await _userManager.FindByEmailAsync(email);
            if (emailExiste != null)
            {
                throw new InvalidOperationException("El Email ya esta registrado");
            }

        }
    }
}
