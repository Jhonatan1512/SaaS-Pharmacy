using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class InventarioAlmacenCentral
    { 
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LoteId { get; set; }
        public int StockFisicoUnidades { get; set; }
        public int StockMinimoUnidades { get; set; } = 100;
        public DateTime FechaUltimaActualizacion { get; set; } = DateTime.Now;

        public Lote Lote { get; set; } = null!;
    }
}
