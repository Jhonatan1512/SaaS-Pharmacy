using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class PagoVenta
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid VentaId { get; set; }
        public MedioPagoEnum MedioPago { get; set; }
        public decimal Monto { get; set; }
        public string? NumOperacion { get; set; }

        public Venta Venta { get; set; } = null!;
    }
}
