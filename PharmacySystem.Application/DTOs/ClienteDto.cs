using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class ClienteDto
    {
        public TipoDoc TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = string.Empty;
        public string NombresRazonSocial { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
    }

    public class ClienteResponseDto
    {
        public Guid Id { get; set; }
        public TipoDoc TipoDocumento { get; set; }
        public string TipoDocumentoNombre => TipoDocumento.ToString();
        public string NumeroDocumento { get; set; } = string.Empty;
        public string NombresRazonSocial { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
    }

    public class ClienteUpdateDto
    {
        public TipoDoc? TipoDocumento { get; set; }
        public string? NumeroDocumento { get; set; } 
        public string? NombresRazonSocial { get; set; } 
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
    }
}
