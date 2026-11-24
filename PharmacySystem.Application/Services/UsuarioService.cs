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
        private readonly ISedeRepository _sedeRepository;
        public UsuarioService(IIdentityUserService identityUserService, ICredencialesFermaceutico credencialesFermaceutico, 
            IUsuarioRepository usuarioRepository, UserManager<Usuario> userManager, ISedeRepository sedeRepository)
        {
            _identityUserService = identityUserService;
            _credencialesFermaceutico = credencialesFermaceutico;
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
            _sedeRepository = sedeRepository;
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

        public async Task ActualizarUsuarioAsync(Guid usuarioId, ActualizarUsuarioDto dto)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId)
                ?? throw new KeyNotFoundException("El usuario no existe.");

            var tipoDocFinal = dto.TipoDocumento ?? usuario.Documento;
            var numDocFinal = !string.IsNullOrWhiteSpace(dto.NumeroDocumento)
                ? dto.NumeroDocumento.Trim()
                : usuario.NumeroDoc;

            bool cambioDocumento = tipoDocFinal != usuario.Documento || numDocFinal != usuario.NumeroDoc;

            string? dniValidar = cambioDocumento && tipoDocFinal == TipoDocumento.DNI ? numDocFinal : null;
            string? rucValidar = cambioDocumento && tipoDocFinal == TipoDocumento.RUC ? numDocFinal : null;

            await _credencialesFermaceutico.ValidarRegistrosDuplicados(
                email: dto.Email,
                dni: dniValidar,
                ruc: rucValidar,
                usuarioIdExcluir: usuario.Id
            );

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                string emailNormalizado = dto.Email.Trim();
                usuario.Email = emailNormalizado;
                usuario.UserName = emailNormalizado;
            }

            if (cambioDocumento)
            {
                usuario.Documento = tipoDocFinal;
                usuario.NumeroDoc = numDocFinal;
            }

            if (!string.IsNullOrWhiteSpace(dto.NombreCompletoRazonSocial))
            {
                usuario.NombreCompletoRazonSocial = dto.NombreCompletoRazonSocial.Trim();
            }

            if (dto.SedeId.HasValue)
            {
                var sedeId = await _sedeRepository.ObtenerSedeIdAsync(dto.SedeId.Value);
                if (dto.SedeId.Value == Guid.Empty || sedeId == null)
                    throw new KeyNotFoundException("La sede especificada no existe.");

                usuario.SedeId = dto.SedeId.Value;
            }

            var resultado = await _userManager.UpdateAsync(usuario);
            if (!resultado.Succeeded)
            {
                var errores = string.Join(", ", resultado.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"No se pudo actualizar el usuario: {errores}");
            }
        }

        public async Task ReestablecerPasswordAsync(Guid usuarioId, RestablecerPasswordDto dto)
        {
            var usuario = await _userManager.FindByIdAsync(usuarioId.ToString())
            ?? throw new KeyNotFoundException("El usuario no existe.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var resultado = await _userManager.ResetPasswordAsync(usuario, token, dto.NuevaPassword);

            if (!resultado.Succeeded)
            {
                var errores = string.Join(" ", resultado.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"No se pudo restablecer la contraseña: {errores}");
            }
        }

        public async Task ActualizarEstadoUsuarioAsync(Guid usuarioId)
        {
            var usuarioExiste = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);
            if (usuarioExiste == null) throw new KeyNotFoundException("Sede no encontrada");

            usuarioExiste.Activo = !usuarioExiste.Activo;
            await _userManager.UpdateAsync(usuarioExiste);
        }
    }
}
