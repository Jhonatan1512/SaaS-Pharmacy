using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class AlertasAlmacenCentralResponseDto
    {
        public int TotalProductosStockBajo { get; set; }
        public int TotalLotesPorVencer { get; set; }
        public List<AlertaStockBajoCentralDto> ProductosStockBajo { get; set; } = [];
        public List<AlertaVencimientoCentralDto> LotesPorVencer { get; set; } = [];
    }

    public class AlertaStockBajoCentralDto
    {
        public Guid ProductoId { get; set; }
        public string CodigoBarras { get; set; } = string.Empty;
        public string NombreComercial { get; set; } = string.Empty;
        public string Presentacion { get; set; } = string.Empty;
        public int StockTotalUnidades { get; set; }
        public int StockMinimoReferencial { get; set; }
        public int UnidadesFaltantesParaMinimo { get; set; }
        public string NivelCriticidad { get; set; } = string.Empty; 
        public int CantidadLotesActivos { get; set; }
    }

    public class AlertaVencimientoCentralDto
    {
        public Guid LoteId { get; set; }
        public Guid ProductoId { get; set; }
        public string CodigoBarras { get; set; } = string.Empty;
        public string NombreComercial { get; set; } = string.Empty;
        public string CodigoLote { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public int DiasParaVencer { get; set; }
        public int StockFisicoUnidades { get; set; }
        public string EstadoVencimiento { get; set; } = string.Empty; 
    }
}
