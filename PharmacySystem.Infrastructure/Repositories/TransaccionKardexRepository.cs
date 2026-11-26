using Microsoft.EntityFrameworkCore;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Domain.Interfaces;
using PharmacySystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Infrastructure.Repositories
{
    public class TransaccionKardexRepository : ITransaccionKardexRepository
    {
        private readonly ApplicationDbContext _context;

        public TransaccionKardexRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(TransaccionKardex transaccion)
        {
            await _context.TransaccionesKardex.AddAsync(transaccion);
        }       

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<TransaccionKardex>> ObtenerRegistrosKardexAsync()
        {
            return await _context.TransaccionesKardex
                .AsNoTracking()
                .Include(k => k.Sede)
                .Include(k => k.Lote)
                .OrderBy(k => k.Fecha)
                .ToListAsync();
        }
    }
}
