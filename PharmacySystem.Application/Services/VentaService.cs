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

        public async Task AnularVentaAsync(Guid userId, Guid ventaId, AnularVentaDto dto)
        {
            Guid userSedeId = _currentUserService.GetSedeId();
            if (userSedeId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("El farmacéutico no tiene una sede asignada.");
            }

            await _ventaRepository.AnularVentaAsync(ventaId, userSedeId, userId, dto.MotivoAnulacion);
        }

        public async Task RealizarVentaAsync(Guid userId, VentasDto dto)
        {
            Guid userSedeId = _currentUserService.GetSedeId();
            if (userSedeId == Guid.Empty)
                throw new UnauthorizedAccessException("El farmacéutico no tiene una sede asignada.");

            if (dto.Detalles == null || dto.Detalles.Count == 0)
                throw new InvalidOperationException("Debe agregar al menos un producto al detalle de la venta.");

            var turnoActivo = await _turnoCajaRepository.ObtenerTurnoAbiertoPorUsuarioAsync(userId);
            if (turnoActivo == null || turnoActivo.SedeId != userSedeId)
                throw new InvalidOperationException("Debe tener un turno de caja abierto en su sede para realizar ventas.");

            await _comprobanteValidacionService.ValidarClienteParaComprobanteAsync(dto.TipoComprobante, dto.ClienteId);

            await using var transaction = await _ventaRepository.IniciarTransaccionAsync();

            try
            {
                var fechaEmision = DateTime.UtcNow;

                // Valida sede activa, turno abierto en BD y reserva correlativo con bloqueo de fila
                var (serie, numeroFormateado) = await _ventaRepository.PrepararCabeceraVentaSpAsync(
                    userSedeId, userId, turnoActivo.Id, dto.TipoComprobante);

                string referenciaComprobante = $"Venta {serie}-{numeroFormateado}";

                var venta = new Venta
                {
                    Id = Guid.NewGuid(),
                    SedeId = userSedeId,
                    UsuarioId = userId,
                    ClienteId = dto.ClienteId,
                    TurnoCajaId = turnoActivo.Id,
                    TipoComprobante = dto.TipoComprobante,
                    Serie = serie,
                    Numero = numeroFormateado,
                    FechaEmision = fechaEmision,
                    Moneda = "PEN",
                    EstadoSunat = "NO_EMITIDO"
                };

                venta.Detalles = await _crearDetalleVentaService.CrearDetalleVentaAsync(
                    venta.Id,
                    userSedeId,
                    userId,
                    referenciaComprobante,
                    fechaEmision,
                    dto.Detalles);

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
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
