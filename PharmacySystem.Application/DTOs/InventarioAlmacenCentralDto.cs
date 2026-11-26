using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class InventarioAlmacenCentralDto
    {
        public Guid Id { get; set; }
        public Guid LoteId { get; set; }
        public string CodigoLote { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public Guid ProductoId { get; set; }
        public string NombreComercial { get; set; } = string.Empty;
        public string CodigoBarras {  get; set; } = string.Empty;
        public int StockFisicoUnidades { get; set; }
    }

    public class InventarioDetalleAlmacenCentralDto
    {
        public Guid Id { get; set; }
        public int StockFisicoUnidades { get; set; }
        public int StockMinimoUnidades { get; set; }
        public DateTime FechaUltimaActualizacion { get; set; }

        public LoteDetalleDto DetalleLote { get; set; } = new();
        public ProductoDetalleDto DetalleProducto { get; set; } = new();
    }

    public class LoteDetalleDto
    {
        public Guid Id { get; set; }
        public string CodigoLote { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public decimal PrecioCostoUnitario { get; set; }
        public int CantidadInicialUnidades { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class ProductoDetalleDto
    {
        public Guid Id { get; set; }
        public string CodigoBarras { get; set; } = string.Empty;
        public string CodigoProductoSunat { get; set; } = string.Empty;
        public string NombreComercial { get; set; } = string.Empty;
        public string PrincipioActivo { get; set; } = string.Empty;
        public string Presentacion { get; set; } = string.Empty;
        public string RegistroSanitario { get; set; } = string.Empty;
        public string EsPsicotropico { get; set; } = string.Empty;
        public string TipoAfectacionIgvDefault { get; set; } = string.Empty;
        public int UnidadesPorCaja { get; set; }
        public int UnidadesPorBlister { get; set; }
        public decimal PrecioCajaRecomendado { get; set; }
        public decimal PrecioBlisterRecomendado { get; set; }
        public decimal PrecioUnidadRecomendado { get; set; }
        public string Activo { get; set; } = string.Empty;
    }
}
