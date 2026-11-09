using PharmacySystem.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public Guid GetSedeId()
        {
            var user = _contextAccessor.HttpContext.User;
            if(user == null)
            {
                throw new UnauthorizedAccessException("No hay contexto de usuario");
            }

            var claimUser = user.Claims.FirstOrDefault(c => c.Type.Equals("sedeId", StringComparison.OrdinalIgnoreCase))?.Value;
            if(string.IsNullOrWhiteSpace(claimUser) || !Guid.TryParse(claimUser, out var parseGuid))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado o token mal estructurado");
            }

            if (parseGuid == Guid.Empty)
            {
                throw new UnauthorizedAccessException("El usuario con sesión activa no pertenece a ninguna oficina.");
            }

            return parseGuid;
        }

        public Guid GetUserId()
        {
            var user  = _contextAccessor.HttpContext?.User;
            if(user == null)
            {
                throw new UnauthorizedAccessException("No hay contexto de usuario");
            }

            var usuarioClaim = user.FindFirst("id")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(usuarioClaim) || !Guid.TryParse(usuarioClaim, out var parsedGuid))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            return parsedGuid;
        }
    }
}
