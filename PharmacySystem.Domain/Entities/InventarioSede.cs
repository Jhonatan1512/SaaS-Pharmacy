using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class InventarioSede
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SedeId { get; set; }
        public Guid LoteId { get; set; }

        public int StockFisicoUnidades { get; set; } 
        public decimal PrecioVentaUnidad { get; set; }
        public decimal PrecioVentaBlister { get; set; }
        public decimal PrecioVentaCaja { get; set; }

        public EstadoLoteSedeEnum Estado { get; set; } = EstadoLoteSedeEnum.EnTransito;
        public DateTime? FechaRecepcion { get; set; }
        public Guid? UsuarioRecibeId { get; set; }

        public Sede Sede { get; set; } = null!;
        public Lote Lote { get; set; } = null!;
        public Usuario? UsuarioRecibe { get; set; } 
    }
}
