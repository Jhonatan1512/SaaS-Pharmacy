using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class VentasDto
    {
        public Guid? ClienteId { get; set; }
        public TipoComprobanteEnum TipoComprobante { get; set; }
        public List<DetalleVentaCreateDto> Detalles { get; set; } = new();
        public List<PagoVentaCreateDto> Pagos { get; set; } = new();
    }

    public class DetalleVentaCreateDto
    {
        public Guid InventarioSedeId { get; set; }       
        public TipoUnidadVentaEnum TipoUnidadVenta { get; set; }
        public int CantidadVenta { get; set; }
    }

    public class PagoVentaCreateDto
    {
        public MedioPagoEnum MedioPago { get; set; }
        public decimal? Monto { get; set; }
    }
}
