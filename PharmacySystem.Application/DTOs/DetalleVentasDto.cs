using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class DetalleVentasDto
    {

    }

    public class VentaDelDiaDto
    {
        public Guid Id { get; set; }
        public TipoComprobanteEnum TipoComprobante { get; set; }
        public string Comprobante { get; set; } = string.Empty; // Ej: "B001-00000012"
        public DateTime FechaEmision { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public List<string> MetodosPago { get; set; } = new(); // Ej: ["Efectivo"] o ["Efectivo", "Yape"]
        public decimal MontoIgv { get; set; }
        public decimal MontoTotalVenta { get; set; }
        public bool Anulado { get; set; }
    }

    public class ResumenTotalVentasTurnoActivoDto
    {
        public Guid TurnoCajaId { get; set; }
        public DateTime FechaApertura { get; set; }

        public decimal TotalEfectivo { get; set; }
        public decimal TotalYape { get; set; }
        public decimal TotalPlin { get; set; }
        public decimal TotalTarjeta { get; set; }

        public decimal TotalGeneralVentas => TotalEfectivo + TotalYape + TotalPlin + TotalTarjeta;
    }

}
