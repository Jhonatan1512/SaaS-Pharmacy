using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class RegistrarIngresoLoteDto
    {
        public Guid ProductoId { get; set; }
        public string CodigoLote { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public decimal PrecioCostoUnitario { get; set; }
        public int CantidadUnidades { get; set; }
        public int StockMinimoAlmacenCentral { get; set; } = 100;
        public string? ReferenciaDocumento { get; set; }
    }
}
