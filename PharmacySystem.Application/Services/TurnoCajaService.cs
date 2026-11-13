using Mapster;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services
{
    public class TurnoCajaService : ITurnoCajaService
    {
        private readonly ITurnoCajaRepository _turnoCajaRepository;
        private readonly ICurrentUserService _currentUserService;
        public TurnoCajaService(ITurnoCajaRepository turnoCajaRepository, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _turnoCajaRepository = turnoCajaRepository;
        }
        public async Task AbrirTurnoAsync(Guid userId, TurnoCajaDto dto)
        {
            Guid? userSedeId = _currentUserService.GetSedeId();
            if (!userSedeId.HasValue)
                throw new UnauthorizedAccessException("El usuario autenticado no tiene una sede asignada.");

            var turnoActivoExistente = await _turnoCajaRepository.ObtenerTurnoAbiertoPorUsuarioAsync(userId);
            if (turnoActivoExistente != null)
                throw new InvalidOperationException("Ya tienes un turno de caja abierto. Debes cerrarlo antes de abrir uno nuevo.");

            var nuevoTurno = dto.Adapt<TurnoCaja>();
            nuevoTurno.SedeId = userSedeId.Value;
            nuevoTurno.UsuarioId = userId;

            await _turnoCajaRepository.CrearAsync(nuevoTurno);
        }

        public async Task CerrarTurnoAsync(Guid userId, Guid turnoId, CerrarCajaDto dto)
        {
            var existeTunoAbierto = await _turnoCajaRepository.ObtenerPorIdAsync(turnoId);
            if (existeTunoAbierto == null)
                throw new KeyNotFoundException("El turno de caja especificado no existe");

            Guid? userSedeId = _currentUserService.GetSedeId();
            if (!userSedeId.HasValue || existeTunoAbierto.SedeId != userSedeId.Value)
                throw new UnauthorizedAccessException("No tiene permisos para cerrar turnos de otra sede.");

            if(existeTunoAbierto.UsuarioId != userId)
                throw new UnauthorizedAccessException("Solo el usuario que aperturó este turno puede realizar su cierre.");

            var totalesSistema = await _turnoCajaRepository.ObtenerTotalesVentasPorTurnoAsync(existeTunoAbierto.Id);

            existeTunoAbierto.CerrarTurno(
                conteoEfectivo: dto.ConteoFisicoEfectivo,
                conteoYape: dto.ConteoFisicoYape,
                conteoPlin: dto.ConteoFisicoPlin,
                conteoTarjeta: dto.ConteoFisicoTarjeta,
                sistemaEfectivo: totalesSistema.Efectivo,
                sistemaYape: totalesSistema.Yape,
                sistemaPlin: totalesSistema.Plin,
                sistemaTarjeta: totalesSistema.Tarjeta,
                observacionesCierre: dto.Observaciones
                );

            await _turnoCajaRepository.GuardarCambiosAsync();
        }

        public async Task CorregirConteoCierreAsync(Guid userId, Guid turnoId, CorregirConteoTurnoDto dto)
        {
            var turno = await _turnoCajaRepository.ObtenerPorIdAsync(turnoId);
            if (turno == null)
                throw new KeyNotFoundException("El turno de caja especificado no existe.");

            Guid? userSedeId = _currentUserService.GetSedeId();
            if (!userSedeId.HasValue || turno.SedeId != userSedeId.Value)
                throw new UnauthorizedAccessException("No tiene permisos para modificar turnos de otra sede.");

            if (turno.UsuarioId != userId)
                throw new UnauthorizedAccessException("Solo el usuario responsable del turno puede corregir su conteo de cierre.");

            turno.CorregirConteoCierre(
                nuevoEfectivo: dto.ConteoFisicoEfectivo,
                nuevoYape: dto.ConteoFisicoYape,
                nuevoPlin: dto.ConteoFisicoPlin,
                nuevoTarjeta: dto.ConteoFisicoTarjeta,
                motivoCorreccion: dto.MotivoCorreccion
            );

            await _turnoCajaRepository.GuardarCambiosAsync();
        }

        public async Task<TurnoActivoResponseDto?> ObtenerMiTurnoActivoAsync(Guid userId)
        {
            var turnoActivo = await _turnoCajaRepository.ObtenerTurnoAbiertoPorUsuarioAsync(userId);
            if (turnoActivo == null)
                throw new InvalidOperationException("No tiene turnos de caja activos.");
            return turnoActivo?.Adapt<TurnoActivoResponseDto>();
        }

        public async Task<TurnoCajaResponseDto> ObtenerPorIdAsync(Guid id)
        {
            var turno = await _turnoCajaRepository.ObtenerPorIdAsync(id);
            if (turno == null)
                throw new KeyNotFoundException("El turno de caja no fue encontrado.");

            Guid? userSedeId = _currentUserService.GetSedeId();
            if (userSedeId.HasValue && turno.SedeId != userSedeId.Value)
                throw new UnauthorizedAccessException("No tiene permisos para consultar turnos de otra sede.");

            if (turno.EstadoAbierto)
                throw new InvalidOperationException("No se puede visualizar el arqueo detallado mientras el turno siga abierto.");

            return turno.Adapt<TurnoCajaResponseDto>();
        }

        public async Task<IEnumerable<TurnoCajaResponseDto>> ObtenerTurnosMiSedeAsync()
        {
            Guid? userSedeId = _currentUserService.GetSedeId();
            if (!userSedeId.HasValue)
                throw new UnauthorizedAccessException("El usuario autenticado no tiene una sede asignada.");

            var turnos = await _turnoCajaRepository.ObtenerPorSedeIdAsync(userSedeId.Value);
            return turnos.Adapt<IEnumerable<TurnoCajaResponseDto>>();
        }
    }
}
