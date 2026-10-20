using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class Producto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CodigoBarras { get; set; } = string.Empty;
        public string CodigoProductoSunat { get; set; } = string.Empty; 
        public string NombreComercial { get; set; } = string.Empty;
        public string PrincipioActivo { get; set; } = string.Empty;
        public string Presentacion { get; set; } = string.Empty; // Ej: Caja x 100 tabletas
        public string RegistroSanitario { get; set; } = string.Empty;
        public bool EsPsicotropico { get; set; }

        // Tipo Afectación IGV por defecto según SUNAT ("10"=Gravado, "20"=Exonerado)
        public string TipoAfectacionIgvDefault { get; set; } = "10";

        // Factores de conversión a la unidad mínima (pastilla)
        public int UnidadesPorCaja { get; set; } = 1;
        public int UnidadesPorBlister { get; set; } = 1;

        // Precios Base Referenciales (Soles)
        public decimal PrecioCaja { get; set; }
        public decimal PrecioBlister { get; set; }
        public decimal PrecioUnidad { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public ICollection<Lote> Lotes { get; set; } = [];
    }
}
