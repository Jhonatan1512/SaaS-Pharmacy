using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IInventarioSedeRepository _inventarioSedeRepository;
        private readonly ISerieComprobanteRepository _serieComprobanteRepository;
        private readonly ITurnoCajaRepository _turnoCajaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IComprobanteValidacionService _comprobanteValidacionService;
        private readonly IPresentacionInventarioService _presentacionService;
        private readonly ICalculoTributarioVentaService _calculoTributarioService;
        private readonly IProcesadorPagosVentaService _procesadorPagosService;
        public VentaService(
        IVentaRepository ventaRepository, IInventarioSedeRepository inventarioRepository, ISerieComprobanteRepository serieRepository,
        ITurnoCajaRepository turnoRepository, IClienteRepository clienteRepository, ICurrentUserService currentUserService,
        IComprobanteValidacionService comprobanteValidacionService, IPresentacionInventarioService presentacionService, ICalculoTributarioVentaService calculoTributarioService, 
        IProcesadorPagosVentaService procesadorPagosService)
        {
            _ventaRepository = ventaRepository;
            _inventarioSedeRepository = inventarioRepository;
            _serieComprobanteRepository = serieRepository;
            _turnoCajaRepository = turnoRepository;
            _clienteRepository = clienteRepository;
            _currentUserService = currentUserService;
            _comprobanteValidacionService = comprobanteValidacionService;
            _presentacionService = presentacionService;
            _calculoTributarioService = calculoTributarioService;
            _procesadorPagosService = procesadorPagosService;
        }

        public async Task RealizarVentaAsync(Guid userId, VentasDto dto)
        {
            Guid userSedeId = _currentUserService.GetSedeId();
            if (userSedeId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("El farmacéutico no tiene una sede asignada.");
            }

            var turnoActivo = await _turnoCajaRepository.ObtenerTurnoAbiertoPorUsuarioAsync(userId);
            if (turnoActivo == null || turnoActivo.SedeId != userSedeId)
                throw new InvalidOperationException("Debe tener un turno de caja abierto en su sede para realizar ventas.");

            await _comprobanteValidacionService.ValidarClienteParaComprobanteAsync(dto.TipoComprobante, dto.ClienteId);

            await using var transaction = await _ventaRepository.IniciarTransaccionAsync();

            try
            {
                var serieComprobante = await _serieComprobanteRepository.ObtenerSerieActivaPorSedeYTipoAsync(userSedeId, dto.TipoComprobante)
                ?? throw new InvalidOperationException($"No existe una serie activa configurada para {dto.TipoComprobante} en esta sede.");

                serieComprobante.UltimoCorrelativo++;
                string numeroFormateado = serieComprobante.UltimoCorrelativo.ToString("D8");

                var idsInventario = dto.Detalles.Select(d => d.InventarioSedeId).Distinct().ToList();
                var inventarios = await _inventarioSedeRepository.ObtenerPorIdsParaVentaAsync(idsInventario);
                var inventarioDict = inventarios.ToDictionary(i => i.Id);

                var venta = new Venta
                {
                    Id = Guid.NewGuid(),
                    SedeId = userSedeId,
                    UsuarioId = userId,
                    ClienteId = dto.ClienteId,
                    TurnoCajaId = turnoActivo.Id,
                    TipoComprobante = dto.TipoComprobante,
                    Serie = serieComprobante.Serie,
                    Numero = numeroFormateado,
                    FechaEmision = DateTime.UtcNow,
                    Moneda = "PEN",
                    EstadoSunat = "NO_EMITIDO"
                };

                var detallesOrdenadosFefo = dto.Detalles
                    .Select(item =>
                    {
                        if (!inventarioDict.TryGetValue(item.InventarioSedeId, out var inventario))
                            throw new KeyNotFoundException($"El registro de inventario '{item.InventarioSedeId}' no existe.");

                        return new { Item = item, Inventario = inventario };
                    })
                    .OrderBy(x => x.Inventario.Lote.FechaVencimiento)
                    .ToList();

                foreach (var entry in detallesOrdenadosFefo)
                {
                    var item = entry.Item;
                    var inventario = entry.Inventario;

                    int unidadesMinimas = _presentacionService.CalcularUnidadesMinimas(inventario, item.TipoUnidadVenta, item.CantidadVenta);
                    decimal precioUnitarioConIgv = _presentacionService.ObtenerPrecioPorPresentacion(inventario, item.TipoUnidadVenta);

                    await _inventarioSedeRepository.DescontarStockSpAsync(inventario.Id, userSedeId, unidadesMinimas);

                    var valorTexto = inventario.Lote.Producto.TipoAfectacionIgvDefault;

                    if (!Enum.TryParse<TipoAfectacionIgvEnum>(valorTexto, ignoreCase: true, out var tipoAfectacion)
                        || !Enum.IsDefined(typeof(TipoAfectacionIgvEnum), tipoAfectacion))
                    {
                        throw new InvalidOperationException($"El tipo de afectación IGV '{valorTexto}' no es válido.");
                    }

                    var detalle = _calculoTributarioService.ConstruirDetalle(
                        ventaId: venta.Id,
                        inventarioSedeId: inventario.Id,
                        tipoUnidadVenta: item.TipoUnidadVenta,
                        cantidadVenta: item.CantidadVenta,
                        cantidadUnidadesMinimas: unidadesMinimas,
                        precioUnitarioConIgv: precioUnitarioConIgv,
                        tipoAfectacionIgv: tipoAfectacion
                    );

                    venta.Detalles.Add(detalle);
                }               

                _calculoTributarioService.ConsolidarTotalesYValidarPagos(venta);
                _comprobanteValidacionService.ValidarReglaMontoIdentificacion(venta.TipoComprobante, venta.MontoTotalVenta, venta.ClienteId);

                venta.Pagos = _procesadorPagosService.GenerarPagos(
                    venta.Id,
                    venta.Serie,
                    venta.Numero,
                    venta.MontoTotalVenta,
                    dto.Pagos
                );

                await _ventaRepository.CrearAsync(venta);
                await _ventaRepository.GuardarCambiosAsync();
                await transaction.CommitAsync();

                await _ventaRepository.ObtenerPorIdAsync(venta.Id);

            } catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
