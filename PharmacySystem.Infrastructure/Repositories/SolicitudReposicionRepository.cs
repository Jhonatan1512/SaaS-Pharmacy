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
    public class SolicitudReposicionRepository : ISolicitudReposicionRepository
    {
        private readonly ApplicationDbContext _context;
        public SolicitudReposicionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(SolicitudReposicion solicitud)
        {
            await _context.SolicitudesReposicion.AddAsync(solicitud);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<SolicitudReposicion?> ObtenerPorIdConDetallesAsync(Guid id)
        {
            return await _context.SolicitudesReposicion
            .AsNoTracking()
            .Include(s => s.SedeSolicitante)
            .Include(s => s.UsuarioSolicitante)
            .Include(s => s.Detalles)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<SolicitudReposicion>> ObtenerTodasConDetalleAsync(Guid? sedeId = null, EstadoSolicitudEnum? estado = null)
        {
            var query = _context.SolicitudesReposicion
                .AsNoTracking()
                .AsSplitQuery()
                .Include(s => s.SedeSolicitante)
                .Include(s => s.UsuarioSolicitante)
                .Include(s => s.Detalles)
                    .ThenInclude(d => d.Producto)
                .AsQueryable();

            if (sedeId.HasValue)
                query = query.Where(s => s.SedeSolicitanteId == sedeId.Value);

            if (estado.HasValue)
                query = query.Where(s => s.Estado == estado.Value);

            return await query
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<SolicitudReposicion?> ObtenerPorIdParaActualizarAsync(Guid id)
        {
            return await _context.SolicitudesReposicion
                .Include(s => s.Detalles)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
