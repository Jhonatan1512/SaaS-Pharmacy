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
    public class LoteRepository : ILoteRepository
    {
        private readonly ApplicationDbContext _context;
        public LoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CrearLoteAsync(Lote lote)
        {
            _context.Lotes.Add(lote);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteLoteEnProductoAsync(Guid productoId, string codigoLote)
        {
            return await _context.Lotes.AnyAsync(l => l.ProductoId == productoId && l.CodigoLote.ToLower() == codigoLote.ToLower());
        }

        public async Task<IEnumerable<Lote>> ObtenerLotesAsync()
        {
            return await _context.Lotes.ToListAsync();
        }
    }
}
