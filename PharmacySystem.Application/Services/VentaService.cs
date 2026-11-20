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
        private readonly ICrearDetalleVentaService _crearDetalleVentaService;
        public VentaService(
        IVentaRepository ventaRepository, IInventarioSedeRepository inventarioRepository, ISerieComprobanteRepository serieRepository,
        ITurnoCajaRepository turnoRepository, IClienteRepository clienteRepository, ICurrentUserService currentUserService,
        IComprobanteValidacionService comprobanteValidacionService, IPresentacionInventarioService presentacionService, ICalculoTributarioVentaService calculoTributarioService, 
        IProcesadorPagosVentaService procesadorPagosService, ICrearDetalleVentaService crearDetalleVentaService)
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
            _crearDetalleVentaService = crearDetalleVentaService;
        }

        public async Task AnularVentaAsync(Guid ventaId, AnularVentaDto dto)
        {
            Guid userSedeId = _currentUserService.GetSedeId();
            if (userSedeId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("El farmacéutico no tiene una sede asignada.");
            }

            await _ventaRepository.AnularVentaAsync(ventaId, userSedeId, dto.MotivoAnulacion);
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

                venta.Detalles = await _crearDetalleVentaService.CrearDetalleVentaAsync(venta.Id, userSedeId, dto.Detalles);            

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

            } catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
