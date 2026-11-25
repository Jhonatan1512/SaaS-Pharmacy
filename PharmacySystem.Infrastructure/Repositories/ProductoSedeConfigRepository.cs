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
    public class ProductoSedeConfigRepository : IProductoSedeConfigRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductoSedeConfigRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AgregarAsync(ProductoSedeConfig config)
        {
            await _context.ProductosSedeConfig.AddAsync(config);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductoSedeConfig>> ObtenerConInventarioPorSedeAsync(Guid sedeId)
        {
            return await _context.ProductosSedeConfig
                .AsNoTracking()
                .AsSplitQuery()
                .Where(c => c.SedeId == sedeId)
                .Include(c => c.Producto)
                    .ThenInclude(p => p.Lotes)
                        .ThenInclude(l => l.InventariosSede.Where(i => i.SedeId == sedeId && i.StockFisicoUnidades > 0))
                .ToListAsync();
        }

        public async Task<ProductoSedeConfig?> ObtenerPorSedeYProductoAsync(Guid sedeId, Guid productoId)
        {
            return await _context.ProductosSedeConfig
            .FirstOrDefaultAsync(c => c.SedeId == sedeId && c.ProductoId == productoId);
        }
    }
}
