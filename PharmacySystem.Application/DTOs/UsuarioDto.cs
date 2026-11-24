using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class UsuarioDto
    {
        public string Email { get; set; } = string.Empty;
        public TipoDocumento TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = string.Empty;
        public string NombreCompletoRazonSocial { get; set; } = string.Empty;
        public Guid? SedeId { get; set; }
        public string Rol { get; set; } = string.Empty;
    }

    public class ActualizarUsuarioDto
    {
        public string? Email { get; set; } 
        public TipoDocumento? TipoDocumento { get; set; }
        public string? NumeroDocumento { get; set; } 
        public string? NombreCompletoRazonSocial { get; set; } 
        public Guid? SedeId { get; set; }
    }

    public class RestablecerPasswordDto
    {
        public string NuevaPassword { get; set; } = string.Empty;
        public string ConfirmarPassword { get; set; } = string.Empty;
    }

    public class GetUsuarioDto
    {
        public Guid Id { get; set; } 
        public string Email { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string NumeroDoc { get; set; } = string.Empty; 
        public string NombreCompletoRazonSocial { get; set; } = string.Empty;
        public string? SedeNombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty; 
    }
}
