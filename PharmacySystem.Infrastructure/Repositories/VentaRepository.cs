using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
    public class VentaRepository : IVentaRepository
    {
        private readonly ApplicationDbContext _context;
        public VentaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IDbContextTransaction> IniciarTransaccionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CrearAsync(Venta venta)
        {
            await _context.Ventas.AddAsync(venta);
        }

        public async Task<Venta?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Ventas
                .AsNoTracking()
                .Include(v => v.Sede)
                .Include(v => v.Usuario)
                .Include(v => v.Cliente)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.InventarioSede)
                        .ThenInclude(i => i.Lote)
                            .ThenInclude(l => l.Producto)
                .Include(v => v.Pagos)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
