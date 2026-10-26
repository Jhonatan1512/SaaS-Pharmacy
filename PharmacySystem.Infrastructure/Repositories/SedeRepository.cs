using Microsoft.EntityFrameworkCore;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;
using PharmacySystem.Infrastructure.Data;

namespace PharmacySystem.Infrastructure.Repositories
{
    public class SedeRepository : ISedeRepository
    {
        private readonly ApplicationDbContext _context;
        
        public SedeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarSedeAsync(Sede sede)
        {
            _context.Sedes.Update(sede);
            await _context.SaveChangesAsync();
        }

        public async Task<Sede> CrearSedeAsync(Sede sede)
        {
            await _context.Sedes.AddAsync(sede);
            await _context.SaveChangesAsync();
            return sede;
        }

        public async Task<IEnumerable<Sede>> GetSedeAsync()
        {
            return await _context.Sedes.ToListAsync();
        }

        public async Task<Sede?> ObtenerSedeIdAsync(Guid Id)
        {
            return await _context.Sedes
                .Include(s => s.Usuarios.Where(u => u.Activo))
                .AsNoTracking()
                .FirstOrDefaultAsync(se => se.Id == Id);
        }

        public async Task<Sede?> ObtenerSedePorIdAsync(Guid Id)
        {
            return await _context.Sedes.FirstOrDefaultAsync(se => se.Id == Id);
        }
    }
}
