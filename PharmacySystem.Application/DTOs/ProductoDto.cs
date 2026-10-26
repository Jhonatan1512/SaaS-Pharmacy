using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class ProductoDto
    {
        public string? CodigoBarras { get; set; }
        public string? CodigoProductoSunat { get; set; }
        public string NombreComercial { get; set; } = string.Empty;
        public string PrincipioActivo { get; set; } = string.Empty;
        public string Presentacion { get; set; } = string.Empty;
        public string RegistroSanitario { get; set; } = string.Empty;
        public bool EsPsicotropico { get; set; }

        // "10" = Gravado, "20" = Exonerado
        public string TipoAfectacionIgvDefault { get; set; } = "10";

        public int UnidadesPorCaja { get; set; } = 1;
        public int UnidadesPorBlister { get; set; } = 1;
        public decimal PrecioCaja { get; set; }
        public decimal PrecioBlister { get; set; }
        public decimal PrecioUnidad { get; set; }
    }

    public class GetProductosDto
    {
        public Guid Id { get; set; }
        public string? CodigoBarras { get; set; }
        public string? CodigoProductoSunat { get; set; }
        public string NombreComercial { get; set; } = string.Empty;
        public string PrincipioActivo { get; set; } = string.Empty;
        public string Presentacion { get; set; } = string.Empty;
        public string RegistroSanitario { get; set; } = string.Empty;
        public bool EsPsicotropico { get; set; }
        public string TipoAfectacionIgvDefault { get; set; } = "10";
        public int UnidadesPorCaja { get; set; }
        public int UnidadesPorBlister { get; set; }
        public decimal PrecioCaja { get; set; }
        public decimal PrecioBlister { get; set; }
        public decimal PrecioUnidad { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
