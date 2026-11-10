using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
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
}
