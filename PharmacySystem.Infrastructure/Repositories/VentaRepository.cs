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

        public async Task<List<Venta>> ObtenerTodasLasVentasTurnoActivoAsync(Guid turnoCajaId, Guid sedeId)
        {
            return await _context.Ventas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.Pagos)
                .Where(v => v.TurnoCajaId == turnoCajaId && v.SedeId == sedeId)
                .OrderByDescending(v => v.FechaEmision)
                .ToListAsync();
        }
    }
}
