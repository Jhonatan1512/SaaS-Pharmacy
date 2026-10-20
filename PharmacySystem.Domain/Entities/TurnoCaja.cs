using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class TurnoCaja
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SedeId { get; set; }
        public Guid UsuarioId { get; set; }
        public DateTime FechaApertura { get; set; } = DateTime.UtcNow;
        public DateTime? FechaCierre { get; set; }
        public decimal SaldoInicialEfectivo { get; set; }

        // Cierre Ciego
        public decimal? ConteoFisicoEfectivo { get; set; }
        public decimal? ConteoFisicoYape { get; set; }
        public decimal? ConteoFisicoPlin { get; set; }
        public decimal? ConteoFisicoTarjeta { get; set; }

        // Valores teóricos calculados por el Sistema
        public decimal? SistemaTotalEfectivo { get; set; }
        public decimal? SistemaTotalYape { get; set; }
        public decimal? SistemaTotalPlin { get; set; }
        public decimal? SistemaTotalTarjeta { get; set; }

        public decimal? DiferenciaSobranteFaltante { get; set; }
        public bool EstadoAbierto { get; set; } = true;
        public string? Observaciones { get; set; }

        public Sede Sede { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
