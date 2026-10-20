using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class SerieComprobante
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SedeId { get; set; }
        public TipoComprobanteEnum TipoComprobante { get; set; } 
        public string Serie { get; set; } = string.Empty;            // Ej: F001, B001, NV01
        public int UltimoCorrelativo { get; set; } = 0;
        public bool Activo { get; set; } = true;

        public Sede Sede { get; set; } = null!;
    }
}
