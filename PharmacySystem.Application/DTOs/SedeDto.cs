using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class SedeDto
    {        
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string Distrito { get; set; } = string.Empty;
        public string Ubigeo { get; set; } = string.Empty; 
        public string? Telefono { get; set; }
    }

    public class  ActualizarSedeDto 
    {
        public string? Nombre { get; set; } 
        public string? Direccion { get; set; } 
        public string? Departamento { get; set; }
        public string? Provincia { get; set; } 
        public string? Distrito { get; set; } 
        public string? Ubigeo { get; set; } 
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

    public class SedeDataDto
    {
        public Guid Id { get; set; } 
        public string CodigoEstablecimientoAnexo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string Distrito { get; set; } = string.Empty;
        public string Ubigeo { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public bool Activa { get; set; } = true;
        public IEnumerable<SedePersonalDto> Personal { get; set; } = [];
    }

    public class SedePersonalDto
    {
        public Guid Id { get; set; } 
        public string UsuarioNombreCompletoRazonSocial {  get; set; } = string.Empty;
        public string UsuarioDocumento {  get; set; } = string.Empty;
        public string UsuarioNumeroDoc { get; set;} = string.Empty;
        public string UsuarioEmail { get; set;} = string.Empty;
    }
}
