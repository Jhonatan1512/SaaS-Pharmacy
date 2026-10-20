using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class TransaccionKardex
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SedeId { get; set; }
        public Guid LoteId { get; set; }
        public TipoMovimientoKardexEnum TipoMovimiento { get; set; }
        public int CantidadUnidades { get; set; }
        public int StockAnteriorUnidades { get; set; }
        public int StockNuevoUnidades { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public Guid UsuarioId { get; set; }
        public string? ReferenciaDocumento { get; set; }

        public Sede Sede { get; set; } = null!;
        public Lote Lote { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}
