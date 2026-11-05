using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class LoteDto
    {
        public Guid ProductoId { get; set; }
        public string CodigoLote { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public decimal PrecioCostoUnitario { get; set; }
    }

    public class GetLoteDto
    {
        public Guid Id { get; set; }
        public Guid ProductoId { get; set; }
        public string CodigoLote { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public decimal PrecioCostoUnitario { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class actualizarLoteDto
    {
        public Guid? ProductoId { get; set; }
        public string? CodigoLote { get; set; } 
        public DateTime? FechaVencimiento { get; set; }
        public decimal? PrecioCostoUnitario { get; set; }
    }
}
