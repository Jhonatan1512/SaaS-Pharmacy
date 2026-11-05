using Microsoft.EntityFrameworkCore;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;
using PharmacySystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Infrastructure.Repositories
{
    public class InventarioSedeRepository : IInventarioSedeRepository
    {
        private readonly ApplicationDbContext _context;
        public InventarioSedeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task CrearInventarioSedeAsync(InventarioSede sede)
        {
            _context.InventarioSedes.Add(sede);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteInventarioEnSedeAsync(Guid sedeId, Guid loteId)
        {
            return await _context.InventarioSedes
                .AnyAsync(i => i.SedeId == sedeId && i.LoteId == loteId);
        }

        public async Task<InventarioSede?> ObtenerPorSedeYLoteActualizarAsync(Guid sedeId, Guid loteId)
        {
            return await _context.InventarioSedes
                .FirstOrDefaultAsync(i => i.SedeId == sedeId && i.LoteId == loteId);
        }
    }
}
