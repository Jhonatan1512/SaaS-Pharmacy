using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class SerieComprobanteDto
    {
        public Guid SedeId { get; set; }
        public TipoComprobanteEnum TipoComprobante { get; set; }
    }
    public class SerieComprobanteResponseDto
    {
        public Guid Id { get; set; }
        public Guid SedeId { get; set; }
        public string NombreSede { get; set; } = string.Empty;
        public TipoComprobanteEnum TipoComprobante { get; set; }
        public string TipoComprobanteNombre => TipoComprobante.ToString();
        public string Serie { get; set; } = string.Empty;
        public int UltimoCorrelativo { get; set; }
        public bool Activo { get; set; }
    }
}
