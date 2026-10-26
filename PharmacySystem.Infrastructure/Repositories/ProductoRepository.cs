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
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Producto> CrearProductoAsync(Producto producto)
        {
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<IEnumerable<Producto>> GetAllProductsAsync()
        {
            return await _context.Productos.ToListAsync();
        }

        public async Task<Producto?> ObtenerPorCodigoBarrasAsync(string codigoBarras)
        {
            return await _context.Productos.FirstOrDefaultAsync(cb => cb.CodigoBarras == codigoBarras);
        }

        public async Task<Producto?> ObtenerPorIdAsync(Guid Id)
        {
            return await _context.Productos.FirstOrDefaultAsync(x => x.Id == Id);
        }
    }
}
