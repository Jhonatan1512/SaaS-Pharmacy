using Microsoft.AspNetCore.Identity;
using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerarToken(IdentityUser<Guid> user, IList<string> role, string displayName, string localId, string sedeNombre);
    } 
}
