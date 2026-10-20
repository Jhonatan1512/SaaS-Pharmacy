using Microsoft.AspNetCore.Identity;
using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class Usuario : IdentityUser<Guid>
    {
        public TipoDocumento Documento { get; set; }
        public string NumeroDoc { get; set; } = string.Empty;
        public string NombreCompletoRazonSocial { get; set; } = string.Empty;
        public Guid? SedeId { get; set; }
        public bool Activo { get; set; } = true;
        public Sede? Sede { get; set; }
    }

    public class Role : IdentityRole<Guid> 
    {
        public string? Descripcion { get; set; }
        public Role() : base() { }
        public Role(string roleName) : base(roleName) { } 
    }

    public enum TipoDocumento
    {
        DNI = 1,
        RUC = 2,
    }
}
