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
    public class SerieComprobanteRepository : ISerieComprobanteRepository
    {
        private readonly ApplicationDbContext _context;
        public SerieComprobanteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarEstadoSerieAsync(SerieComprobante serieComprobante)
        {
            _context.SeriesComprobantes.Update(serieComprobante);
            await _context.SaveChangesAsync();
        }

        public async Task CrearAsync(SerieComprobante serieComprobante)
        {
            _context.SeriesComprobantes.Add(serieComprobante);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExisteSerieEnSedeAsync(Guid sedeId, TipoComprobanteEnum tipoComprobante, string serie)
        {
            return _context.SeriesComprobantes
                .AnyAsync(s => s.SedeId == sedeId &&
                               s.TipoComprobante == tipoComprobante &&
                               s.Serie == serie);

        }

        public async Task<IEnumerable<string>> ObtenerCodigosSeriePorSedeYTipoAsync(Guid sedeId, TipoComprobanteEnum tipoComprobante)
        {
            return await _context.SeriesComprobantes.AsNoTracking()
                .Where(s => s.SedeId == sedeId && s.TipoComprobante == tipoComprobante)
                .Select(s => s.Serie)
                .ToListAsync();
        }

        public Task<SerieComprobante?> ObtenerPorIdAsync(Guid id)
        {
            return _context.SeriesComprobantes.AsNoTracking().Include(s => s.Sede).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<SerieComprobante>> ObtenerPorSedeIdAsync(Guid sedeId)
        {
            return await _context.SeriesComprobantes.AsNoTracking()
                .Include(s => s.Sede)
                .Where(s => s.SedeId == sedeId)
                .OrderBy(s => s.TipoComprobante)
                .ThenBy(s => s.Serie)
                .ToListAsync();
        }

        public async Task<IEnumerable<SerieComprobante>> ObtenerTodasAsync()
        {
            return await _context.SeriesComprobantes
                .AsNoTracking()
                .Include(s => s.Sede)
                .OrderBy(s => s.Sede.Nombre)
                .ThenBy(s => s.TipoComprobante)
                .ThenBy(s => s.Serie)
                .ToListAsync();
        }
    }
}
