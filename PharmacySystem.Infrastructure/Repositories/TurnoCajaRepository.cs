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
    public class TurnoCajaRepository : ITurnoCajaRepository
    {
        private readonly ApplicationDbContext _context;
        public TurnoCajaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CrearAsync(TurnoCaja turno)
        {
            _context.TurnosCaja.Add(turno);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<TurnoCaja?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.TurnosCaja
                .Include(t => t.Sede)
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TurnoCaja>> ObtenerPorSedeIdAsync(Guid sedeId)
        {
            return await _context.TurnosCaja
            .AsNoTracking()
            .Include(t => t.Sede)
            .Include(t => t.Usuario)
            .Where(t => t.SedeId == sedeId)
            .OrderByDescending(t => t.FechaApertura)
            .ToListAsync();
        }

        public async Task<TotalesVentasTurno> ObtenerTotalesVentasPorTurnoAsync(Guid turnoId)
        {
            await Task.CompletedTask;
            return new TotalesVentasTurno(0m, 0m, 0m, 0m);
        }

        public async Task<TurnoCaja?> ObtenerTurnoAbiertoPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.TurnosCaja
            .Include(t => t.Sede)
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.UsuarioId == usuarioId && t.EstadoAbierto);
        }
    }
}
