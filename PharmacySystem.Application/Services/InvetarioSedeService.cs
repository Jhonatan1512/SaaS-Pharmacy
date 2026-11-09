using Mapster;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        ILoteRepository loteRepository, IUsuarioRepository usuarioRepository, ICurrentUserService currentUserService)
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

        inventarioEnTransito.ConfirmarRecepcion(
            userId,
            DateTime.UtcNow,
            dto.CantidadDaniada,
            dto.Observacion
        );

        var capaActivaExistente = await _inventarioSedeRepository.ObtenerCapaActivaPorSedeLoteYPrecioAsync(
            inventarioEnTransito.SedeId,
            inventarioEnTransito.LoteId,
            inventarioEnTransito.PrecioVentaUnidad
        );

        // Si existe otra capa activa previa (diferente a la que acabamos de confirmar):
        if (capaActivaExistente != null && capaActivaExistente.Id != inventarioEnTransito.Id)
        {
            // Sumar el stock conforme recién recibido a la capa activa previa
            capaActivaExistente.ReabastecerStock(
                cantidad: inventarioEnTransito.StockFisicoUnidades
            );

            // Eliminar el registro de tránsito que ya fue consolidado
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

    public Task<IEnumerable<InvetarioSedeResponseDto>> GetInvetarioSedeAsync()
    {
        throw new NotImplementedException();
    }
}