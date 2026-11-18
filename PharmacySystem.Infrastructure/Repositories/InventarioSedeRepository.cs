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
    public class InventarioSedeRepository : IInventarioSedeRepository
    {
        private readonly ApplicationDbContext _context;
        public InventarioSedeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task CrearInventarioSedeAsync(InventarioSede sede)
        {
            _context.InventarioSedes.Add(sede);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarRegistroAsync(InventarioSede inventario)
        {
            _context.InventarioSedes.Remove(inventario);
            await Task.CompletedTask;
        }

        public async Task<bool> ExisteInventarioEnSedeAsync(Guid sedeId, Guid loteId)
        {
            return await _context.InventarioSedes
                .AnyAsync(i => i.SedeId == sedeId && i.LoteId == loteId);
        }

        public async Task<InventarioSede?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.InventarioSedes
                .FirstOrDefaultAsync(i => i.Id == id);
        } 

        public async Task<InventarioSede?> ObtenerCapaActivaPorSedeLoteYPrecioAsync(Guid sedeId, Guid loteId, decimal? precioVenta)
        {
            return await _context.InventarioSedes
                .FirstOrDefaultAsync(i =>
                    i.SedeId == sedeId &&
                    i.LoteId == loteId &&
                    i.PrecioVentaUnidad == precioVenta &&
                    i.Estado == EstadoLoteSedeEnum.Activo);
        }

        public async Task<InventarioSede?> ObtenerPorSedeYLoteActualizarAsync(Guid sedeId, Guid loteId)
        {
            return await _context.InventarioSedes
                .FirstOrDefaultAsync(i => i.SedeId == sedeId && i.LoteId == loteId);
        }

        public async Task<InventarioSede?> ObtenerPorSedeLotePrecioYEstadoAsync( Guid sedeId, Guid loteId, decimal precioVentaUnidad, EstadoLoteSedeEnum estado)
        {
            return await _context.InventarioSedes
                .FirstOrDefaultAsync(i =>
                    i.SedeId == sedeId &&
                    i.LoteId == loteId &&
                    i.PrecioVentaUnidad == precioVentaUnidad &&
                    i.Estado == estado);
        }

        public async Task<IEnumerable<InventarioSede>> ObtenerPorSedeIdAsync(Guid sedeId)
        {
            return await _context.InventarioSedes
                .AsNoTracking()
                .Include(i => i.Sede)                  
                .Include(i => i.UsuarioRecibe)          
                .Include(i => i.Lote)
                    .ThenInclude(l => l.Producto)      
                .Where(i => i.SedeId == sedeId)
                .OrderBy(i => i.Estado)
                .ThenBy(i => i.Lote.FechaVencimiento)
                .ToListAsync();
        }

        public async Task<List<InventarioSede>> ObtenerPorIdsParaVentaAsync(IEnumerable<Guid> ids)
        {
            return await _context.InventarioSedes
                .AsNoTracking()
                .Include(i => i.Lote)
                    .ThenInclude(l => l.Producto)
                .Where(i => ids.Contains(i.Id))
                .ToListAsync();
        }

        public async Task DescontarStockSpAsync(Guid inventarioSedeId, Guid sedeId, int cantidadUnidadesMinimas)
        {
            try
            {
                int estadoActivo = (int)EstadoLoteSedeEnum.Activo;
                int estadoAgotado = (int)EstadoLoteSedeEnum.Agotado;
                DateTime fechaActual = DateTime.UtcNow.Date;

                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"EXEC sp_DescontarStockVenta @InventarioSedeId={inventarioSedeId}, @SedeId={sedeId}, @CantidadUnidadesMinimas={cantidadUnidadesMinimas}, @EstadoActivo={estadoActivo}, @EstadoAgotado={estadoAgotado}, @FechaActual={fechaActual}");
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number >= 50001 && ex.Number <= 50010)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
