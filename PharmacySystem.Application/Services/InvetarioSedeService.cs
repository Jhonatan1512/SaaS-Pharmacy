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
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;

    public InventarioSedeService(
        IInventarioSedeRepository inventarioSedeRepository,
        ISedeRepository sedeRepository,
        ILoteRepository loteRepository,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService)
    {
        _inventarioSedeRepository = inventarioSedeRepository;
        _sedeRepository = sedeRepository;
        _loteRepository = loteRepository;
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
    }

    public async Task CrearInvetarioSedeAsync(InventarioSedeDto dto)
    {
        var existeSede = await _sedeRepository.ObtenerSedeIdAsync(dto.SedeId);
        if (existeSede == null)
            throw new KeyNotFoundException("La sede especificada no existe.");

        var existeLote = await _loteRepository.ObtenerLotePorIAsync(dto.LoteId);
        if (existeLote == null)
            throw new KeyNotFoundException("El lote especificado no existe.");

        var nuevoInventarioEnTransito = dto.Adapt<InventarioSede>();
        nuevoInventarioEnTransito.Id = Guid.NewGuid();

        await _inventarioSedeRepository.CrearInventarioSedeAsync(nuevoInventarioEnTransito);
    }

    public async Task ConfirmarRecepcionAsync(Guid userId, Guid inventarioSedeId, ConfirmarRecepcionInventarioDto dto)
    {
        var inventarioEnTransito = await _inventarioSedeRepository.ObtenerPorIdAsync(inventarioSedeId);
        if (inventarioEnTransito == null)
            throw new KeyNotFoundException("El registro de inventario en tránsito no fue encontrado.");

        Guid? userSedeId = _currentUserService.GetSedeId();
        if (!userSedeId.HasValue)
        {
            throw new UnauthorizedAccessException("El usuario autenticado no tiene una sede asignada.");
        }

        if (inventarioEnTransito.SedeId != userSedeId.Value)
        {
            throw new UnauthorizedAccessException("No está autorizado para confirmar la recepción de un lote destinado a otra sede.");
        }

        DateTime fechaRecepcionUtc = DateTime.UtcNow;

        // Aplicar confirmación física en el objeto en tránsito
        inventarioEnTransito.ConfirmarRecepcion(
            userId,
            fechaRecepcionUtc,
            dto.CantidadDaniada,
            dto.Observacion
        );

        // Buscar si ya existe una capa activa previa con el mismo precio en la sede
        var capaActivaExistente = await _inventarioSedeRepository.ObtenerCapaActivaPorSedeLoteYPrecioAsync(
            inventarioEnTransito.SedeId,
            inventarioEnTransito.LoteId,
            inventarioEnTransito.PrecioVentaUnidad
        );

        // Si existe otra capa activa previa:
        if (capaActivaExistente != null && capaActivaExistente.Id != inventarioEnTransito.Id)
        {
            capaActivaExistente.ReabastecerStock(
                cantidad: inventarioEnTransito.StockFisicoUnidades,
                cantidadDanada: inventarioEnTransito.UltimaCantidadDaniada,
                usuarioRecibeId: userId,
                fechaRecepcion: fechaRecepcionUtc,
                observacion: dto.Observacion
            );

            await _inventarioSedeRepository.EliminarRegistroAsync(inventarioEnTransito);
        }

        await _inventarioSedeRepository.ActualizarAsync();
    }

    public async Task ActualizarInvetarioSedeAsync(Guid inventarioSedeId, CorregirPrecioInventarioSedeDto dto)
    {
        var inventarioActual = await _inventarioSedeRepository.ObtenerPorIdAsync(inventarioSedeId);
        if (inventarioActual == null)
            throw new KeyNotFoundException("El registro de inventario no existe.");

        decimal nuevoPrecioUnidadTarget = dto.PrecioVentaUnidad ?? inventarioActual.PrecioVentaUnidad;

        var capaExistenteMismoPrecio = await _inventarioSedeRepository
            .ObtenerPorSedeLotePrecioYEstadoAsync(inventarioActual.SedeId, inventarioActual.LoteId, nuevoPrecioUnidadTarget, inventarioActual.Estado);

        if (capaExistenteMismoPrecio != null && capaExistenteMismoPrecio.Id != inventarioActual.Id)
        {
            capaExistenteMismoPrecio.ReabastecerStock(
                cantidad: inventarioActual.StockFisicoUnidades,
                cantidadDanada: inventarioActual.UltimaCantidadDaniada,
                precioUnidad: dto.PrecioVentaUnidad,
                precioBlister: dto.PrecioVentaBlister,
                precioCaja: dto.PrecioVentaCaja
            );

            await _inventarioSedeRepository.EliminarRegistroAsync(inventarioActual);
        }
        else
        {
            inventarioActual.ReabastecerStock(
                cantidad: 0,
                precioUnidad: dto.PrecioVentaUnidad,
                precioBlister: dto.PrecioVentaBlister,
                precioCaja: dto.PrecioVentaCaja
            );
        }

        await _inventarioSedeRepository.ActualizarAsync();
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

    public async Task CorregirMermaRecepcionAsync(Guid userId, Guid inventarioSedeId, CorregirRecepcionDto dto)
    {
        var inventario = await _inventarioSedeRepository.ObtenerPorIdAsync(inventarioSedeId);
        if (inventario == null)
            throw new KeyNotFoundException("El registro de inventario no fue encontrado.");

        Guid? userSedeId = _currentUserService.GetSedeId();
        if (!userSedeId.HasValue || inventario.SedeId != userSedeId.Value)
        {
            throw new UnauthorizedAccessException("No tiene permisos para modificar inventarios de otra sede.");
        }

        inventario.CorregirMermaRecepcion(
            dto.NuevaCantidadDanada,
            dto.MotivoCorreccion
        );

        await _inventarioSedeRepository.ActualizarAsync();
    }
}