using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class KardexResponseDto
    {
        public Guid Id { get; set; }
        public Guid SedeId { get; set; }
        public string NombreSede { get; set; } = string.Empty;
        public Guid LoteId { get; set; }
        public string CodigoLote { get; set; } = string.Empty;
        public string TipoMovimiento {  get; set; } = string.Empty;
        public int CantidadUnidades { get; set; }
        public int StockAnteriorUnidades { get; set; }
        public int StockNuevoUnidades { get; set; }
        public DateTime Fecha { get; set; }
        public decimal PrecioCostoUnitario { get; set; }
    }
}
