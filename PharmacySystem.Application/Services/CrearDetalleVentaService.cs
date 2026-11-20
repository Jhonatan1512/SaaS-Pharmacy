using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services.Ventas;

public class CrearDetalleVentaService : ICrearDetalleVentaService
{
    private readonly IInventarioSedeRepository _inventarioSedeRepository;
    private readonly IPresentacionInventarioService _presentacionService;
    private readonly ICalculoTributarioVentaService _calculoTributarioService;

    public CrearDetalleVentaService(
        IInventarioSedeRepository inventarioSedeRepository,
        IPresentacionInventarioService presentacionService,
        ICalculoTributarioVentaService calculoTributarioService)
    {
        _inventarioSedeRepository = inventarioSedeRepository;
        _presentacionService = presentacionService;
        _calculoTributarioService = calculoTributarioService;
    }

    public async Task<List<DetalleVenta>> CrearDetalleVentaAsync(Guid ventaId, Guid sedeId, IEnumerable<DetalleVentaCreateDto> dto)
    {
        var idsInventario = dto.Select(d => d.InventarioSedeId).Distinct().ToList();
        var inventarios = await _inventarioSedeRepository.ObtenerPorIdsParaVentaAsync(idsInventario);
        var inventarioDict = inventarios.ToDictionary(i => i.Id);

        var detallesOrdenadosFefo = dto
            .Select(item =>
            {
                if (!inventarioDict.TryGetValue(item.InventarioSedeId, out var inventario))
                    throw new KeyNotFoundException($"El registro de inventario '{item.InventarioSedeId}' no existe.");

                return new { Item = item, Inventario = inventario };
            })
            .OrderBy(x => x.Inventario.Lote.FechaVencimiento)
            .ToList();

        var detallesGenerados = new List<DetalleVenta>(detallesOrdenadosFefo.Count);

        foreach (var entry in detallesOrdenadosFefo)
        {
            var item = entry.Item;
            var inventario = entry.Inventario;

            int unidadesMinimas = _presentacionService.CalcularUnidadesMinimas(
                inventario, item.TipoUnidadVenta, item.CantidadVenta);

            decimal precioUnitarioConIgv = _presentacionService.ObtenerPrecioPorPresentacion(
                inventario, item.TipoUnidadVenta);

            await _inventarioSedeRepository.DescontarStockSpAsync(inventario.Id, sedeId, unidadesMinimas);

            var valorTexto = inventario.Lote.Producto.TipoAfectacionIgvDefault;

            if (!Enum.TryParse<TipoAfectacionIgvEnum>(valorTexto, ignoreCase: true, out var tipoAfectacion)
                || !Enum.IsDefined(typeof(TipoAfectacionIgvEnum), tipoAfectacion))
            {
                throw new InvalidOperationException($"El tipo de afectación IGV '{valorTexto}' no es válido.");
            }

            var detalle = _calculoTributarioService.ConstruirDetalle(
                ventaId: ventaId,
                inventarioSedeId: inventario.Id,
                tipoUnidadVenta: item.TipoUnidadVenta,
                cantidadVenta: item.CantidadVenta,
                cantidadUnidadesMinimas: unidadesMinimas,
                precioUnitarioConIgv: precioUnitarioConIgv,
                tipoAfectacionIgv: tipoAfectacion
            );

            detallesGenerados.Add(detalle);
        }

        return detallesGenerados;
    }
}