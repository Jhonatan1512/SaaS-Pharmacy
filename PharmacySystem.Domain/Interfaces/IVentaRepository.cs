using Microsoft.EntityFrameworkCore.Storage;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public interface IVentaRepository
    {
        Task<IDbContextTransaction> IniciarTransaccionAsync();
        Task CrearAsync(Venta venta);
        Task GuardarCambiosAsync();
        Task AnularVentaAsync(Guid ventaId, Guid sedeId, Guid usuarioId, string motivoAnulacion);
        Task<List<Venta>> ObtenerTodasLasVentasTurnoActivoAsync(Guid turnoCajaId, Guid sedeId, Guid usuarioId);
        Task<List<DetalleVenta>> ObtenerDetalleVentaPorIdAsync(Guid ventaId, Guid sedeId);
        Task<List<Venta>> ObtenerPorTurnosCerradosPorFechaCierreAsync(Guid sedeId, Guid usuarioId, DateTime fechaInicioUtc, DateTime fechaFinUtc);
        public Task<(string Serie, string Numero)> PrepararCabeceraVentaSpAsync(
            Guid sedeId,
            Guid usuarioId,
            Guid turnoCajaId,
            TipoComprobanteEnum tipoComprobante);
        
    }
}
