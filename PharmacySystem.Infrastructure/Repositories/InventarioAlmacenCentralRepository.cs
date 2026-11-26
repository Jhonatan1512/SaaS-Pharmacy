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
    public class InventarioAlmacenCentralRepository : IInventarioAlmacenCentralRepository
    {
        private readonly ApplicationDbContext _context;

        public InventarioAlmacenCentralRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AgregarAsync(InventarioAlmacenCentral inventario)
        {
            await _context.InventariosAlmacenCentral.AddAsync(inventario);
        }       

        public async Task<List<InventarioAlmacenCentral>> ObtenerActivosParaAlertasAsync()
        {
            return await _context.InventariosAlmacenCentral
                .AsNoTracking()
                .Include(i => i.Lote)
                    .ThenInclude(l => l.Producto)
                .Where(i => i.Lote.Producto.Activo)
                .ToListAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<InventarioAlmacenCentral>> ObtenerTodosLosActivosAsync()
        {
            return await _context.InventariosAlmacenCentral
                .AsNoTracking()
                .Include(i => i.Lote)
                    .ThenInclude(l => l.Producto)
                .OrderBy(i => i.Lote.FechaVencimiento)
                .ThenBy(i => i.Lote.CodigoLote)
                .ToListAsync();
        }

        public async Task<InventarioAlmacenCentral?> ObtenerDetalleRegistroPorIdAsync(Guid id)
        {
            return await _context.InventariosAlmacenCentral
               .AsNoTracking()
               .Include(i => i.Lote)
                   .ThenInclude(l => l.Producto)
               .FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}
