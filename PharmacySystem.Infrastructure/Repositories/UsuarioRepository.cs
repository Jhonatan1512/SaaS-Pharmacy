using Microsoft.EntityFrameworkCore;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;
        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public Task<Usuario?> ObtenerPorDniAsync(string dni)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.NumeroDoc == dni && u.Documento == TipoDocumento.DNI);
        }

        public async Task<Usuario?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<Usuario?> ObtenerPorRucAsync(string ruc)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.NumeroDoc == ruc && u.Documento == TipoDocumento.RUC);
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodosUsuariosAsync()
        {
            return await _context.Users
                .Include(u => u.Sede)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
