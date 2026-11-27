using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Domain.Interfaces;
using PharmacySystem.Infrastructure.Data;

namespace PharmacySystem.Infrastructure.Repositories
{
    public class InventarioSedeRepository : IInventarioSedeRepository
    {
        private readonly ApplicationDbContext _context;

        public InventarioSedeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteInventarioEnSedeAsync(Guid sedeId, Guid loteId)
        {
            return await _context.InventarioSedes
                .AnyAsync(i => i.SedeId == sedeId && i.LoteId == loteId);
        }

        public async Task<InventarioSede?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.InventarioSedes
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
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

        public async Task DespacharLoteASedeSpAsync(
            Guid sedeId,
            Guid loteId,
            int cantidadUnidades,
            decimal? precioVentaUnidad,
            decimal? precioVentaBlister,
            decimal? precioVentaCaja,
            Guid usuarioId)
        {
            try
            {
                int estadoEnTransito = (int)EstadoLoteSedeEnum.EnTransito;
                int tipoMovimientoSalida = (int)TipoMovimientoKardexEnum.SalidaLote;
                DateTime fechaActual = DateTime.UtcNow;

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC sp_DespacharLoteASede 
                        @SedeId = {sedeId},
                        @LoteId = {loteId},
                        @CantidadUnidades = {cantidadUnidades},
                        @PrecioVentaUnidad = {precioVentaUnidad},
                        @PrecioVentaBlister = {precioVentaBlister},
                        @PrecioVentaCaja = {precioVentaCaja},
                        @UsuarioId = {usuarioId},
                        @EstadoEnTransito = {estadoEnTransito},
                        @TipoMovimientoSalida = {tipoMovimientoSalida},
                        @FechaActual = {fechaActual}");
            }
            catch (SqlException ex) when (ex.Number is >= 51001 and <= 51007)
            {
                if (ex.Number is 51002 or 51003)
                    throw new KeyNotFoundException(ex.Message);

                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task ConfirmarRecepcionSpAsync(
            Guid inventarioSedeId,
            Guid sedeUsuarioId,
            Guid usuarioRecibeId,
            int cantidadDaniada,
            string? observacion)
        {
            try
            {
                int estadoEnTransito = (int)EstadoLoteSedeEnum.EnTransito;
                int estadoActivo = (int)EstadoLoteSedeEnum.Activo;
                int estadoAgotado = (int)EstadoLoteSedeEnum.Agotado;
                int tipoMovimientoIngreso = (int)TipoMovimientoKardexEnum.IngresoLote;
                int tipoMovimientoMerma = (int)TipoMovimientoKardexEnum.Merma;
                DateTime fechaRecepcion = DateTime.UtcNow;

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC sp_ConfirmarRecepcionSede 
                        @InventarioSedeId = {inventarioSedeId},
                        @SedeUsuarioId = {sedeUsuarioId},
                        @UsuarioRecibeId = {usuarioRecibeId},
                        @CantidadDaniada = {cantidadDaniada},
                        @Observacion = {observacion},
                        @EstadoEnTransito = {estadoEnTransito},
                        @EstadoActivo = {estadoActivo},
                        @EstadoAgotado = {estadoAgotado},
                        @TipoMovimientoIngreso = {tipoMovimientoIngreso},
                        @TipoMovimientoMerma = {tipoMovimientoMerma},
                        @FechaRecepcion = {fechaRecepcion}");
            }
            catch (SqlException ex) when (ex.Number is >= 51010 and <= 51014)
            {
                if (ex.Number == 51011) throw new KeyNotFoundException(ex.Message);
                if (ex.Number == 51012) throw new UnauthorizedAccessException(ex.Message);

                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task ActualizarPrecioSpAsync(
            Guid inventarioSedeId,
            decimal? precioVentaUnidad,
            decimal? precioVentaBlister,
            decimal? precioVentaCaja)
        {
            try
            {
                int estadoEnTransito = (int)EstadoLoteSedeEnum.EnTransito;
                int estadoAgotado = (int)EstadoLoteSedeEnum.Agotado;
                int estadoActivo = (int)EstadoLoteSedeEnum.Activo;

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC sp_ActualizarPrecioInventarioSede 
                        @InventarioSedeId = {inventarioSedeId},
                        @PrecioVentaUnidad = {precioVentaUnidad},
                        @PrecioVentaBlister = {precioVentaBlister},
                        @PrecioVentaCaja = {precioVentaCaja},
                        @EstadoEnTransito = {estadoEnTransito},
                        @EstadoAgotado = {estadoAgotado},
                        @EstadoActivo = {estadoActivo}");
            }
            catch (SqlException ex) when (ex.Number is >= 51020 and <= 51023)
            {
                if (ex.Number == 51020)
                    throw new KeyNotFoundException(ex.Message);

                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task CorregirMermaRecepcionSpAsync(
            Guid inventarioSedeId,
            Guid sedeUsuarioId,
            Guid usuarioId,
            int nuevaCantidadDanada,
            string? motivoCorreccion)
        {
            try
            {
                int estadoActivo = (int)EstadoLoteSedeEnum.Activo;
                int estadoAgotado = (int)EstadoLoteSedeEnum.Agotado;
                int tipoMovimientoAjuste = (int)TipoMovimientoKardexEnum.Ajuste;
                DateTime fechaActual = DateTime.UtcNow;

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC sp_CorregirMermaRecepcionSede 
                        @InventarioSedeId = {inventarioSedeId},
                        @SedeUsuarioId = {sedeUsuarioId},
                        @UsuarioId = {usuarioId},
                        @NuevaCantidadDanada = {nuevaCantidadDanada},
                        @MotivoCorreccion = {motivoCorreccion},
                        @EstadoActivo = {estadoActivo},
                        @EstadoAgotado = {estadoAgotado},
                        @TipoMovimientoAjuste = {tipoMovimientoAjuste},
                        @FechaActual = {fechaActual}");
            }
            catch (SqlException ex) when (ex.Number is >= 51030 and <= 51035)
            {
                if (ex.Number == 51031) throw new KeyNotFoundException(ex.Message);
                if (ex.Number == 51032) throw new UnauthorizedAccessException(ex.Message);

                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task DescontarStockSpAsync(
            Guid inventarioSedeId,
            Guid sedeId,
            Guid usuarioId,
            int cantidadUnidadesMinimas,
            string referenciaDocumento,
            DateTime fechaEmision)
        {
            try
            {
                int estadoActivo = (int)EstadoLoteSedeEnum.Activo;
                int estadoAgotado = (int)EstadoLoteSedeEnum.Agotado;
                int tipoMovimientoVenta = (int)TipoMovimientoKardexEnum.Venta;

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
            EXEC sp_DescontarStockVenta 
                @InventarioSedeId = {inventarioSedeId},
                @SedeId = {sedeId},
                @UsuarioId = {usuarioId},
                @CantidadUnidadesMinimas = {cantidadUnidadesMinimas},
                @ReferenciaDocumento = {referenciaDocumento},
                @EstadoActivo = {estadoActivo},
                @EstadoAgotado = {estadoAgotado},
                @TipoMovimientoVenta = {tipoMovimientoVenta},
                @FechaEmision = {fechaEmision}");
            }
            catch (SqlException ex) when (ex.Number is >= 50001 and <= 50008)
            {
                if (ex.Number == 50002)
                    throw new KeyNotFoundException(ex.Message);

                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}