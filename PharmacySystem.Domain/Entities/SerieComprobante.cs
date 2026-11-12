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
        public string Serie { get; set; } = string.Empty;            
        public int UltimoCorrelativo { get; set; } = 0;
        public bool Activo { get; set; } = true;

        public Sede Sede { get; set; } = null!;

        /// Genera el código de serie tomando la inicial del tipo de comprobante y 4 dígitos (Ej: B0001, F0001).
        public static string GenerarCodigoSerie(TipoComprobanteEnum tipoComprobante, int numeroSerie)
        {
            if (numeroSerie is < 1 or > 9999)
                throw new InvalidOperationException("El número de serie debe estar entre 1 y 9999.");

            char prefijo = char.ToUpperInvariant(tipoComprobante.ToString()[0]);
            return $"{prefijo}{numeroSerie:D3}";
        }
    }
}
