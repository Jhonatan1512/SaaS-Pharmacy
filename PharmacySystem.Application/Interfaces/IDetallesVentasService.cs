using PharmacySystem.Application.DTOs;
using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface IDetallesVentasService
    {
        Task<ResumenTotalVentasTurnoActivoDto> EjecutarAsync(Guid userId);
        Task<List<VentaDelDiaDto>> ObtenerVentasDelDiaAsync(Guid userId);
        Task<List<DetalleVentasDto>> ObtenerDetalleAsync(Guid ventaId);
        Task<List<VentaDelDiaDto>> HistorialVentasVentasAsync(DateTime fechaCierre, Guid usuarioId);
    }
}
