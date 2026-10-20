using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class SedeDto
    {
        public string CodigoEstablecimientoAnexo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string Distrito { get; set; } = string.Empty;
        public string Ubigeo { get; set; } = string.Empty; 
        public string? Telefono { get; set; }
    }

    public class ObtenerSedeDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CodigoEstablecimientoAnexo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string Distrito { get; set; } = string.Empty;
        public string Ubigeo { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public bool Activa { get; set; } = true;
    }
}
