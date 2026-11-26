using Mapster;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services;

public class InventarioSedeService : IInventarioSedeService
{
    private readonly IInventarioSedeRepository _inventarioSedeRepository;
    private readonly ISedeRepository _sedeRepository;
    private readonly ILoteRepository _loteRepository;
    private readonly ICurrentUserService _currentUserService;

    public InventarioSedeService( IInventarioSedeRepository inventarioSedeRepository, ISedeRepository sedeRepository, ILoteRepository loteRepository,
        ICurrentUserService currentUserService)
    {
        _inventarioSedeRepository = inventarioSedeRepository;
        _sedeRepository = sedeRepository;
        _loteRepository = loteRepository;
        _currentUserService = currentUserService;
    }

    public async Task CrearInvetarioSedeAsync(InventarioSedeDto dto)
    {
        Guid usuarioId = _currentUserService.GetUserId();

        if (usuarioId == Guid.Empty)
            throw new UnauthorizedAccessException("No se pudo identificar al usuario que realiza el despacho.");

        await _inventarioSedeRepository.DespacharLoteASedeSpAsync(
            dto.SedeId,
            dto.LoteId,
            dto.StockFisicoUnidades,
            dto.PrecioVentaUnidad,
            dto.PrecioVentaBlister,
            dto.PrecioVentaCaja,
            usuarioId);
    }

    public async Task ConfirmarRecepcionAsync(Guid userId, Guid inventarioSedeId, ConfirmarRecepcionInventarioDto dto)
    {
        Guid userSedeId = _currentUserService.GetSedeId();
        if(userSedeId == Guid.Empty) 
            throw new UnauthorizedAccessException("El usuario autenticado no tiene una sede asignada.");

        await _inventarioSedeRepository.ConfirmarRecepcionSpAsync(
            inventarioSedeId,
            userSedeId,
            userId,
            dto.CantidadDaniada,
            dto.Observacion);
    }

    public async Task ActualizarInvetarioSedeAsync(Guid inventarioSedeId, CorregirPrecioInventarioSedeDto dto)
    {
        await _inventarioSedeRepository.ActualizarPrecioSpAsync(
                inventarioSedeId,
                dto.PrecioVentaUnidad,
                dto.PrecioVentaBlister,
                dto.PrecioVentaCaja);
    }
    public async Task CorregirMermaRecepcionAsync(Guid userId, Guid inventarioSedeId, CorregirRecepcionDto dto)
    {
        Guid userSedeId = _currentUserService.GetSedeId();
        if (userSedeId == Guid.Empty)
            throw new UnauthorizedAccessException("El usuario autenticado no tiene una sede asignada.");

        await _inventarioSedeRepository.CorregirMermaRecepcionSpAsync(
            inventarioSedeId,
            userSedeId,
            userId,
            dto.NuevaCantidadDaniada,
            dto.MotivoCorreccion);
    }

    public async Task<IEnumerable<InvetarioSedeResponseDto>> GetInvetarioSedeAsync(Guid sedeId)
    {
        var datosBd = await _inventarioSedeRepository.ObtenerPorSedeIdAsync(sedeId);
        if (datosBd == null || !datosBd.Any())
        {
            return Enumerable.Empty<InvetarioSedeResponseDto>();
        }
        return datosBd.Adapt<IEnumerable<InvetarioSedeResponseDto>>();
    }

    public async Task<IEnumerable<InvetarioSedeResponseDto>> GetInvetarioMiSedeAsync()
    {
        Guid? userSedeId = _currentUserService.GetSedeId();
        if (!userSedeId.HasValue)
        {
            throw new UnauthorizedAccessException("El usuario autenticado no tiene sede asignada");
        }

        var datosDb = await _inventarioSedeRepository.ObtenerPorSedeIdAsync(userSedeId.Value);
        if( datosDb == null)
        {
            return Enumerable.Empty<InvetarioSedeResponseDto>();
        }
        return datosDb.Adapt<List<InvetarioSedeResponseDto>>();
    }
}