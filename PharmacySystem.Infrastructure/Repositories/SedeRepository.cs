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
    }
}
