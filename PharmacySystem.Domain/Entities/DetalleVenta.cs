using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class DetalleVenta
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid VentaId { get; set; }
        public Guid InventarioSedeId { get; set; }

        public TipoUnidadVentaEnum TipoUnidadVenta { get; set; }
        public int CantidadVenta { get; set; }
        public int CantidadUnidadesMinimas { get; set; }

        // Precios e Impuestos SUNAT por línea
        public string TipoAfectacionIgv { get; set; } = "10"; // "10"=Gravado, "20"=Exonerado
        public decimal PorcentajeIgv { get; set; } = 18.00m;
        public decimal ValorUnitarioSinIgv { get; set; }
        public decimal PrecioUnitarioConIgv { get; set; }
        public decimal SubTotalSinIgv { get; set; }
        public decimal MontoIgv { get; set; }
        public decimal TotalConIgv { get; set; }

        public Venta Venta { get; set; } = null!;
        public InventarioSede InventarioSede { get; set; } = null!;
    }
}
