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

        public async Task<List<VentaDelDiaDto>> ObtenerVentasDelDiaAsync(Guid userId)
        {
            Guid userSedeId = _currentUserService.GetSedeId();
            if (userSedeId == Guid.Empty)
                throw new UnauthorizedAccessException("El farmacéutico no tiene una sede asignada.");

            var turnoActivo = await _turnoCajaRepository.ObtenerTurnoAbiertoPorUsuarioAsync(userId);
            if (turnoActivo == null || turnoActivo.SedeId != userSedeId)
                return new List<VentaDelDiaDto>();

            var ventas = await _ventaRepository.ObtenerTodasLasVentasTurnoActivoAsync(turnoActivo.Id, userSedeId);

            return ventas.Adapt<List<VentaDelDiaDto>>();
        }
    }
}
