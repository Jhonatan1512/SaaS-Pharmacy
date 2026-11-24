using Mapster;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class DetallesVentasService : IDetallesVentasService
    {
        private readonly ITurnoCajaRepository _turnoCajaRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IVentaRepository _ventaRepository;
        public DetallesVentasService(ITurnoCajaRepository turnoCajaRepository, ICurrentUserService currentUserService, IVentaRepository ventaRepository)
        {
            _currentUserService = currentUserService;
            _turnoCajaRepository = turnoCajaRepository;
            _ventaRepository = ventaRepository;
        }
        public async Task<ResumenTotalVentasTurnoActivoDto> EjecutarAsync(Guid userId)
        {
            Guid userSedeId = _currentUserService.GetSedeId();
            if (userSedeId == Guid.Empty)
                throw new UnauthorizedAccessException("El farmacéutico no tiene una sede asignada.");

            var turnoActivo = await _turnoCajaRepository.ObtenerTurnoAbiertoPorUsuarioAsync(userId);
            if (turnoActivo == null || turnoActivo.SedeId != userSedeId)
                throw new InvalidOperationException("No tiene un turno de caja abierto en su sede actual.");

            var totales = await _turnoCajaRepository.ObtenerTotalesVentasPorTurnoAsync(turnoActivo.Id);
            var detalle = totales.Adapt<ResumenTotalVentasTurnoActivoDto>();

            detalle.TurnoCajaId = turnoActivo.Id;
            detalle.FechaApertura = turnoActivo.FechaApertura;

            return detalle;
        }

        public async Task<List<VentaDelDiaDto>> HistorialVentasVentasAsync(DateTime fechaCierre, Guid usuarioId)
        {
            Guid userSedeId = _currentUserService.GetSedeId();
            if (userSedeId == Guid.Empty)
                throw new UnauthorizedAccessException("El usuario no tiene una sede asignada.");

            var (inicioUtc, finUtc) = ObtenerRangoUtcPeru(fechaCierre);

            var ventas = await _ventaRepository.ObtenerPorTurnosCerradosPorFechaCierreAsync(userSedeId, usuarioId, inicioUtc, finUtc);

            return ventas.Adapt<List<VentaDelDiaDto>>();
        }

        public async Task<List<DetalleVentasDto>> ObtenerDetalleAsync(Guid ventaId)
        {
            Guid userSedeId = _currentUserService.GetSedeId();
            if (userSedeId == Guid.Empty)
                throw new UnauthorizedAccessException("El usuario no tiene una sede asignada.");

            var detalles = await _ventaRepository.ObtenerDetalleVentaPorIdAsync(ventaId, userSedeId);

            if (detalles.Count == 0)
                throw new KeyNotFoundException("No se encontraron detalles para la venta especificada en su sede.");

            return detalles.Adapt<List<DetalleVentasDto>>();
        }

        public async Task<List<VentaDelDiaDto>> ObtenerVentasDelDiaAsync(Guid userId)
        {
            Guid sedeId = _currentUserService.GetSedeId();
            if (sedeId == Guid.Empty)
                throw new UnauthorizedAccessException("El farmacéutico no tiene una sede asignada.");


            var turnoActivo = await _turnoCajaRepository.ObtenerTurnoAbiertoPorUsuarioAsync(userId);
            if (turnoActivo == null || turnoActivo.SedeId != sedeId)
                return new List<VentaDelDiaDto>();

            var ventas = await _ventaRepository.ObtenerTodasLasVentasTurnoActivoAsync(turnoActivo.Id, sedeId, userId);

            return ventas.Adapt<List<VentaDelDiaDto>>();
        }

        private static (DateTime InicioUtc, DateTime FinUtc) ObtenerRangoUtcPeru(DateTime fechaLocal)
        {
            var zonaPeru = TimeZoneInfo.FindSystemTimeZoneById("America/Lima");
            var inicioLocal = DateTime.SpecifyKind(fechaLocal.Date, DateTimeKind.Unspecified);
            var finLocal = inicioLocal.AddDays(1);

            return (
                TimeZoneInfo.ConvertTimeToUtc(inicioLocal, zonaPeru),
                TimeZoneInfo.ConvertTimeToUtc(finLocal, zonaPeru)
            );
        }
    }
}
