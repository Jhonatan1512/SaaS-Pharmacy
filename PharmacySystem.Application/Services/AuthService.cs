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
    public class AuthService : IAuthService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenBlackListService _tokenBlackListService;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<Usuario> userManager, IUsuarioRepository usuarioRepository, 
            ITokenBlackListService tokenBlackListService, ITokenService tokenService)
        {
            _userManager = userManager;
            _usuarioRepository = usuarioRepository;
            _tokenBlackListService = tokenBlackListService;
            _tokenService = tokenService;
        }
        public async Task<string> AuthenticateAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new Exception("Credenciales incorrectas");
            }

            var passwordCorrect = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordCorrect)
            {
                throw new Exception("Credenciales incorrectas");
            }

            var estaActivo = await _usuarioRepository.ObtenerPorIdAsync(user.Id);
            if (estaActivo != null && estaActivo.Activo != true)
            {
                throw new Exception("El usuario esta inactivo.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            string nameToken = "User";
            string sedeId = "";
            string sedeNombre = "";

            if (roles.Contains("Admin"))
            {
                nameToken = "Administrador del Sistema";
                sedeId = Guid.Empty.ToString();
                sedeNombre = "Administrador";
            }
            else
            {
                if (estaActivo == null)
                {
                    throw new Exception("Error");
                }
                nameToken = $"{estaActivo.NombreCompletoRazonSocial}";
                sedeId = estaActivo.SedeId?.ToString() ?? Guid.Empty.ToString();
                sedeNombre = estaActivo.Sede?.Nombre ?? "";
            }

            return _tokenService.GenerarToken(user, roles, nameToken, sedeId!, sedeNombre);
        }
    }
}
