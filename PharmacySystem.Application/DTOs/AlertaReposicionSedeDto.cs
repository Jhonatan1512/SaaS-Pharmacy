using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class AlertaReposicionSedeDto
    {
        public Guid ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;

        public int StockActualUnidades { get; set; }
        public int StockMinimoUnidades { get; set; }
        public int StockDeseadoUnidades { get; set; }
        public int CantidadSugeridaReponer { get; set; }

        public bool TieneAlertaStockBajo { get; set; }
        public bool TieneAlertaVencimiento { get; set; }
        public string NivelPrioridad { get; set; } = string.Empty; 

        public List<LotePorVencerDto> LotesEnRiesgo { get; set; } = [];
    }

    public class LotePorVencerDto 
    {
        public Guid InventarioSedeId { get; set; }
        public string CodigoLote { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public int DiasParaVencer { get; set; }
        public int UnidadesEnLote { get; set; }
        public bool YaVencido { get; set; }
    }
}
