using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class Lote
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductoId { get; set; }
        public string CodigoLote { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public decimal PrecioCostoUnitario { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public Producto Producto { get; set; } = null!;
        public virtual ICollection<InventarioSede> InventariosSede { get; set; } = [];
    }
}
