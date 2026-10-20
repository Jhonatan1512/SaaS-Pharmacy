using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class Venta
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SedeId { get; set; }
        public Guid UsuarioId { get; set; }
        public Guid? ClienteId { get; set; }
        public Guid? TurnoCajaId { get; set; }

        public TipoComprobanteEnum TipoComprobante { get; set; } 
        public string Serie { get; set; } = string.Empty;   // "F001", "B001", "NV01"
        public string Numero { get; set; } = string.Empty;  // "00000001"
        public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
        public string Moneda { get; set; } = "PEN";

        public decimal MontoOperacionesGravadas { get; set; } = 0;
        public decimal MontoOperacionesExoneradas { get; set; } = 0;
        public decimal MontoOperacionesInafectas { get; set; } = 0;
        public decimal MontoIgv { get; set; } = 0;
        public decimal MontoTotalImpuestos { get; set; } = 0;
        public decimal MontoTotalVenta { get; set; } = 0;

        // Reservado para Integración Futura con SUNAT 
        public string EstadoSunat { get; set; } = "NO_EMITIDO"; // "NO_EMITIDO", "PENDIENTE", "ACEPTADO", "RECHAZADO"
        public string? CodigoRespuestaSunat { get; set; }
        public string? MensajeRespuestaSunat { get; set; }
        public string? HashXml { get; set; }
        public string? RutaXml { get; set; }
        public string? RutaCdr { get; set; }

        public bool Anulado { get; set; } = false;
        public string? MotivoAnulacion { get; set; }

        public Sede Sede { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
        public Cliente? Cliente { get; set; }
        public TurnoCaja? TurnoCaja { get; set; }
        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
        public ICollection<PagoVenta> Pagos { get; set; } = new List<PagoVenta>();
    }    
}
