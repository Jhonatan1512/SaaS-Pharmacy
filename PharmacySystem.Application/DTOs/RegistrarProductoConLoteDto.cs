using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class RegistrarProductoConLoteDto
    {
        // -- Datos del Producto --
        public string CodigoBarras { get; set; } = string.Empty;
        public string CodigoProductoSunat { get; set; } = string.Empty;
        public string NombreComercial { get; set; } = string.Empty;
        public string PrincipioActivo { get; set; } = string.Empty;
        public string Presentacion { get; set; } = string.Empty;
        public string RegistroSanitario { get; set; } = string.Empty;
        public bool EsPsicotropico { get; set; }
        public string TipoAfectacionIgvDefault { get; set; } = "10";

        public int UnidadesPorCaja { get; set; } = 1;
        public int UnidadesPorBlister { get; set; } = 1;

        public decimal PrecioCaja { get; set; }
        public decimal PrecioBlister { get; set; }
        public decimal PrecioUnidad { get; set; }

        // --- Datos del Lote Inicial ---
        public string CodigoLote { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public decimal PrecioCostoUnitario { get; set; }
        public int CantidadUnidades { get; set; }

        // --- Configuración Almacén Central y Kardex ---
        public int StockMinimoAlmacenCentral { get; set; } = 100;
        public string? ReferenciaDocumento { get; set; }
    }
}
