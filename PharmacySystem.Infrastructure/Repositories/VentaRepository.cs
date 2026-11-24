using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

        public async Task AnularVentaAsync(Guid ventaId, Guid sedeId, string motivoAnulacion)
        {
            try
            {
                int estadoActivo = (int)EstadoLoteSedeEnum.Activo;
                int estadoAgotado = (int)EstadoLoteSedeEnum.Agotado;

                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"EXEC sp_AnularVenta @VentaId={ventaId}, @SedeId={sedeId}, @MotivoAnulacion={motivoAnulacion}, @EstadoActivo={estadoActivo}, @EstadoAgotado={estadoAgotado}");
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 50012)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 50013)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number >= 50011 && ex.Number <= 50020)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<List<Venta>> ObtenerTodasLasVentasTurnoActivoAsync(Guid turnoCajaId, Guid sedeId, Guid usuarioId)
        {
            return await _context.Ventas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.Pagos)
                .Where(v => v.TurnoCajaId == turnoCajaId && v.SedeId == sedeId && v.UsuarioId == usuarioId)
                .OrderByDescending(v => v.FechaEmision)
                .ToListAsync();
        }

        public async Task<List<DetalleVenta>> ObtenerDetalleVentaPorIdAsync(Guid ventaId, Guid sedeId)
        {
            return await _context.DetallesVenta
                .AsNoTracking()
                .Include(d => d.InventarioSede)
                    .ThenInclude(i => i.Lote)
                        .ThenInclude(l => l.Producto)
                .Where(d => d.VentaId == ventaId && d.Venta.SedeId == sedeId)
                .ToListAsync();
        }

        public async Task<List<Venta>> ObtenerPorTurnosCerradosPorFechaCierreAsync(Guid sedeId, Guid usuarioId, DateTime fechaInicioUtc, DateTime fechaFinUtc)
        {
            var turnosCerradosIds = _context.TurnosCaja
                .Where(t => t.SedeId == sedeId
                         && !t.EstadoAbierto
                         && t.FechaCierre != null
                         && t.FechaCierre >= fechaInicioUtc
                         && t.FechaCierre < fechaFinUtc)
                .Select(t => t.Id);

            return await _context.Ventas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.Pagos)
                .Where(v => v.SedeId == sedeId
                         && v.UsuarioId == usuarioId
                         && v.TurnoCajaId != null
                         && turnosCerradosIds.Contains(v.TurnoCajaId.Value))
                .OrderByDescending(v => v.FechaEmision)
                .ToListAsync();
        }
    }
}
