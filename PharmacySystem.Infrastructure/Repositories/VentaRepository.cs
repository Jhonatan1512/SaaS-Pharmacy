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

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AnularVentaAsync(Guid ventaId, Guid sedeId, Guid usuarioId, string motivoAnulacion)
        {
            try
            {
                int estadoActivo = (int)EstadoLoteSedeEnum.Activo;
                int estadoAgotado = (int)EstadoLoteSedeEnum.Agotado;
                int tipoMovimientoAjuste = (int)TipoMovimientoKardexEnum.Ajuste;
                DateTime fechaAnulacion = DateTime.UtcNow;

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
            EXEC sp_AnularVenta 
                @VentaId = {ventaId},
                @SedeId = {sedeId},
                @UsuarioId = {usuarioId},
                @MotivoAnulacion = {motivoAnulacion},
                @EstadoActivo = {estadoActivo},
                @EstadoAgotado = {estadoAgotado},
                @TipoMovimientoAjuste = {tipoMovimientoAjuste},
                @FechaAnulacion = {fechaAnulacion}");
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 50012)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 50013)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is >= 50011 and <= 50019)
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

        public async Task<(string Serie, string Numero)> PrepararCabeceraVentaSpAsync(
            Guid sedeId,
            Guid usuarioId,
            Guid turnoCajaId,
            TipoComprobanteEnum tipoComprobante)
        {
            try
            {
                var serieParam = new Microsoft.Data.SqlClient.SqlParameter("@SerieOut", System.Data.SqlDbType.NVarChar, 20)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                var numeroParam = new Microsoft.Data.SqlClient.SqlParameter("@NumeroOut", System.Data.SqlDbType.NVarChar, 20)
                {
                    Direction = System.Data.ParameterDirection.Output
                };

                int tipoCompInt = (int)tipoComprobante;

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_PrepararCabeceraVenta @SedeId={0}, @UsuarioId={1}, @TurnoCajaId={2}, @TipoComprobante={3}, @SerieOut=@SerieOut OUTPUT, @NumeroOut=@NumeroOut OUTPUT",
                    sedeId, usuarioId, turnoCajaId, tipoCompInt, serieParam, numeroParam);

                return ((string)serieParam.Value, (string)numeroParam.Value);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is >= 50030 and <= 50035)
            {
                if (ex.Number is 50030 or 50032 or 50035)
                    throw new KeyNotFoundException(ex.Message);

                if (ex.Number == 50033)
                    throw new UnauthorizedAccessException(ex.Message);

                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
