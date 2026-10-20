using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class Cliente
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public TipoDoc TipoDocumento { get; set; } 
        public string NumeroDocumento { get; set; } = string.Empty;
        public string NombresRazonSocial { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
    }

    public enum TipoDoc
    {
        DNI = 1,
        RUC = 2,
    }
}
