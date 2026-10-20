using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class Sede
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CodigoEstablecimientoAnexo { get; set; } = "0000"; // Ej: 0000 (Principal), 0001 (Sucursal 1)
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Departamento { get; set; } = "Cajamarca";
        public string Provincia { get; set; } = "Cajamarca";
        public string Distrito { get; set; } = string.Empty;
        public string Ubigeo { get; set; } = string.Empty; // Código de 6 dígitos SUNAT
        public string? Telefono { get; set; }
        public bool Activa { get; set; } = true;

        public virtual ICollection<Usuario> Usuarios { get; set; } = [];
        public virtual ICollection<InventarioSede> Inventarios { get; set; } = [];
        public ICollection<ProductoSedeConfig> ProductosConfig { get; set; } = [];
        public ICollection<SerieComprobante> SeriesComprobantes { get; set; } = [];
    }
}
