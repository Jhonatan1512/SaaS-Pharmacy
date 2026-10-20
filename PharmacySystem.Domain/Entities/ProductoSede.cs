using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class ProductoSede
    {
        public Guid Id { get; set; }
        public Guid SedeId { get; set; }
        public Guid ProductoId { get; set; }
        public int StockMinimoUnidades { get; set; }
        public int StockDeseadoUnidades { get; set; }

        public Sede Sede { get; set; } = null!;
        public Producto Producto { get; set; } = null!;
    }
}
