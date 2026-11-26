using FluentValidation;
using Mapster;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;
using PharmacySystem.Domain.Enums;

namespace PharmacySystem.Application.Services
{
    public class AlmacenCentralService : IAlmacenCentralService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ILoteRepository _loteRepository;
        private readonly IInventarioAlmacenCentralRepository _inventarioCentralRepository;
        private readonly ITransaccionKardexRepository _kardexRepository;
        private readonly IValidator<RegistrarProductoConLoteDto> _productoConLoteValidator;
        private readonly IValidator<RegistrarIngresoLoteDto> _ingresoLoteValidator;
        public AlmacenCentralService(IProductoRepository productoRepository, ILoteRepository loteRepository, IInventarioAlmacenCentralRepository inventarioCentralRepository,
        ITransaccionKardexRepository kardexRepository, IValidator<RegistrarProductoConLoteDto> productoConLoteValidator,
        IValidator<RegistrarIngresoLoteDto> ingresoLoteValidator)
        {
            _productoRepository = productoRepository;
            _loteRepository = loteRepository;
            _inventarioCentralRepository = inventarioCentralRepository;
            _kardexRepository = kardexRepository;
            _productoConLoteValidator = productoConLoteValidator;
            _ingresoLoteValidator = ingresoLoteValidator;
        }

        public async Task<InventarioDetalleAlmacenCentralDto?> ObtenerDetalleRegistroAsync(Guid id)
        {
            var existeRegistro = await _inventarioCentralRepository.ObtenerDetalleRegistroPorIdAsync(id);
            if (existeRegistro == null) throw new KeyNotFoundException("Registro no encontrado");

            var datos = existeRegistro.Adapt<InventarioDetalleAlmacenCentralDto>();
            return datos;
        }

        public async Task<List<InventarioAlmacenCentralDto>> ObtenerTodosLosActivosAsync()
        {
            var datosBd = await _inventarioCentralRepository.ObtenerTodosLosActivosAsync();
            return datosBd.Adapt<List<InventarioAlmacenCentralDto>>();             
        }

        public async Task RegistrarIngresoLoteExistenteAsync(Guid usuarioId, RegistrarIngresoLoteDto dto)
        {
            await _ingresoLoteValidator.ValidateAndThrowAsync(dto);

            var producto = await _productoRepository.ObtenerPorIdAsync(dto.ProductoId)
                ?? throw new KeyNotFoundException("El producto especificado no existe.");

            string codigoLoteLimpio = dto.CodigoLote.Trim().ToUpper();

            if (await _loteRepository.ExisteLoteEnProductoAsync(dto.ProductoId, codigoLoteLimpio))
                throw new InvalidOperationException($"El producto '{producto.NombreComercial}' ya tiene registrado el lote '{codigoLoteLimpio}'.");

            DateTime ahora = DateTime.UtcNow;

            var nuevoLote = dto.Adapt<Lote>();
            nuevoLote.Id = Guid.NewGuid();
            nuevoLote.ProductoId = producto.Id;
            nuevoLote.CodigoLote = codigoLoteLimpio;
            nuevoLote.CantidadInicialUnidades = dto.CantidadUnidades; 
            nuevoLote.FechaCreacion = ahora;

            var inventarioCentral = dto.Adapt<InventarioAlmacenCentral>();
            inventarioCentral.Id = Guid.NewGuid();
            inventarioCentral.LoteId = nuevoLote.Id;
            inventarioCentral.StockFisicoUnidades = dto.CantidadUnidades; 
            inventarioCentral.StockMinimoUnidades = dto.StockMinimoAlmacenCentral; 
            inventarioCentral.FechaUltimaActualizacion = ahora;

            var movimientoKardex = dto.Adapt<TransaccionKardex>();
            movimientoKardex.Id = Guid.NewGuid();
            movimientoKardex.SedeId = null;
            movimientoKardex.EsAlmacenCentral = true;
            movimientoKardex.LoteId = nuevoLote.Id; 
            movimientoKardex.TipoMovimiento = TipoMovimientoKardexEnum.IngresoLote;
            movimientoKardex.CantidadUnidades = dto.CantidadUnidades;
            movimientoKardex.StockAnteriorUnidades = 0;
            movimientoKardex.StockNuevoUnidades = dto.CantidadUnidades; 
            movimientoKardex.Fecha = ahora;
            movimientoKardex.UsuarioId = usuarioId;
            movimientoKardex.ReferenciaDocumento = string.IsNullOrWhiteSpace(dto.ReferenciaDocumento)
                ? $"Ingreso lote {codigoLoteLimpio}"
                : dto.ReferenciaDocumento.Trim();

            await _loteRepository.CrearLoteAsync(nuevoLote);
            await _inventarioCentralRepository.AgregarAsync(inventarioCentral);
            await _kardexRepository.AgregarAsync(movimientoKardex);

            await _kardexRepository.GuardarCambiosAsync();
        }

        public async Task RegistrarProductoConLoteInicialAsync(Guid usuarioId, RegistrarProductoConLoteDto dto)
        {
            await _productoConLoteValidator.ValidateAndThrowAsync(dto);

            string codigoBarrasLimpio = dto.CodigoBarras.Trim();
            var productoExistente = await _productoRepository.ObtenerPorCodigoBarrasAsync(codigoBarrasLimpio);
            if (productoExistente != null)
                throw new InvalidOperationException($"Ya existe un producto registrado con el código de barras '{codigoBarrasLimpio}'.");

            string codigoLoteLimpio = dto.CodigoLote.Trim().ToUpper();
            DateTime ahora = DateTime.UtcNow;

            var nuevoProducto = dto.Adapt<Producto>();
            nuevoProducto.Id = Guid.NewGuid();
            nuevoProducto.CodigoBarras = codigoBarrasLimpio;
            nuevoProducto.RegistroSanitario = dto.RegistroSanitario.Trim().ToUpper();
            nuevoProducto.Activo = true;
            nuevoProducto.FechaCreacion = ahora;

            var nuevoLote = dto.Adapt<Lote>();
            nuevoLote.Id = Guid.NewGuid();
            nuevoLote.ProductoId = nuevoProducto.Id;
            nuevoLote.CodigoLote = codigoLoteLimpio;
            nuevoLote.CantidadInicialUnidades = dto.CantidadUnidades; 
            nuevoLote.FechaCreacion = ahora;

            var inventarioCentral = dto.Adapt<InventarioAlmacenCentral>();
            inventarioCentral.Id = Guid.NewGuid();
            inventarioCentral.LoteId = nuevoLote.Id;
            inventarioCentral.StockFisicoUnidades = dto.CantidadUnidades; 
            inventarioCentral.StockMinimoUnidades = dto.StockMinimoAlmacenCentral; 
            inventarioCentral.FechaUltimaActualizacion = ahora;

            var movimientoKardex = dto.Adapt<TransaccionKardex>();
            movimientoKardex.Id = Guid.NewGuid();
            movimientoKardex.SedeId = null;
            movimientoKardex.EsAlmacenCentral = true;
            movimientoKardex.LoteId = nuevoLote.Id;
            movimientoKardex.TipoMovimiento = TipoMovimientoKardexEnum.IngresoLote;
            movimientoKardex.CantidadUnidades = dto.CantidadUnidades;
            movimientoKardex.StockAnteriorUnidades = 0;
            movimientoKardex.StockNuevoUnidades = dto.CantidadUnidades; 
            movimientoKardex.Fecha = ahora;
            movimientoKardex.UsuarioId = usuarioId;
            movimientoKardex.ReferenciaDocumento = string.IsNullOrWhiteSpace(dto.ReferenciaDocumento)
                ? $"Ingreso inicial lote {codigoLoteLimpio}"
                : dto.ReferenciaDocumento.Trim();

            await _productoRepository.CrearProductoAsync(nuevoProducto);
            await _loteRepository.CrearLoteAsync(nuevoLote);
            await _inventarioCentralRepository.AgregarAsync(inventarioCentral);
            await _kardexRepository.AgregarAsync(movimientoKardex);

            await _kardexRepository.GuardarCambiosAsync();
        }
    }
}
