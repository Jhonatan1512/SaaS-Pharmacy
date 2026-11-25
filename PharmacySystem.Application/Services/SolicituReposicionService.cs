using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services
{
    public class SolicituReposicionService : ISolicitudReposicionService
    {
        private readonly ISolicitudReposicionRepository _solicitudReposicion;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPlantillaSolicitudService _plantillaSolicitudService;
        private readonly IProductoRepository _productoRepository;

        public SolicituReposicionService(ISolicitudReposicionRepository solicitudReposicionRepository, ICurrentUserService currentUserService, 
            IPlantillaSolicitudService plantillaSolicitudService, IProductoRepository productoRepository)
        {
            _currentUserService = currentUserService;
            _plantillaSolicitudService = plantillaSolicitudService;
            _solicitudReposicion = solicitudReposicionRepository;
            _productoRepository = productoRepository;
        }
        public async Task CrearsolicitudAsync(Guid usuarioId, CrearSolicitudReposicionDto dto)
        {
            Guid sedeId = _currentUserService.GetSedeId();
            if (sedeId == Guid.Empty)
                throw new UnauthorizedAccessException("El usuario no tiene una sede asignada.");

            ValidarFechaLimite(dto.FechaLimiteRequerida);

            var idsSolicitados = dto.Productos.Select(p => p.ProductoId).Distinct();

            foreach (var productoId in idsSolicitados)
            {
                var producto = await _productoRepository.ObtenerPorIdAsync(productoId);
                if (producto == null)
                    throw new KeyNotFoundException($"El producto con ID '{productoId}' no existe.");
            }

            var solicitud = ConstruirSolicitud(usuarioId, sedeId, dto);

            await _solicitudReposicion.AgregarAsync(solicitud);
            await _solicitudReposicion.GuardarCambiosAsync();

        }

        private static void ValidarFechaLimite(DateTime? fechaLimite)
        {
            if (fechaLimite.HasValue && fechaLimite.Value.Date < DateTime.UtcNow.AddHours(-5).Date)
                throw new InvalidOperationException("La fecha límite requerida no puede ser anterior al día de hoy.");
        }

        private static SolicitudReposicion ConstruirSolicitud(
        Guid usuarioId,
        Guid sedeId,
        CrearSolicitudReposicionDto dto)
        {
            var detallesAgrupados = dto.Productos
                .GroupBy(p => p.ProductoId)
                .Select(grupo => new DetalleSolicitudReposicion
                {
                    ProductoId = grupo.Key,
                    CantidadSolicitadaUnidades = grupo.Sum(item => item.CantidadSolicitadaUnidades)
                })
                .ToList();

            return new SolicitudReposicion
            {
                SedeSolicitanteId = sedeId,
                UsuarioSolicitanteId = usuarioId,
                MotivoSolicitud = dto.MotivoSolicitud.Trim(),
                FechaLimiteRequerida = dto.FechaLimiteRequerida,
                Detalles = detallesAgrupados
            };
        }

        public async Task<SolicitudReposicionDocumentoDto> ObtenerSolicitudPorId(Guid id)
        {
            var solicitud = await _solicitudReposicion.ObtenerPorIdConDetallesAsync(id)
              ?? throw new KeyNotFoundException("La solicitud de reposición no existe.");

            if (!_currentUserService.EsAdministrador())
            {
                Guid sedeUsuario = _currentUserService.GetSedeId();
                if (sedeUsuario == Guid.Empty || solicitud.SedeSolicitanteId != sedeUsuario)
                    throw new UnauthorizedAccessException("No tiene permisos para ver solicitudes de otra sede.");
            }

            return MapearADocumentoDto(solicitud);
        }

        public async Task<List<SolicitudReposicionDocumentoDto>> ListarsolicitudesAsync(Guid? sedeFiltroAdmin = null, EstadoSolicitudEnum? estado = null)
        {
            Guid? sedeIdAplicada;
            if (_currentUserService.EsAdministrador())
            {
                sedeIdAplicada = sedeFiltroAdmin;
            }
            else
            {
                Guid sedeUsuario = _currentUserService.GetSedeId();
                if (sedeUsuario == Guid.Empty)
                    throw new UnauthorizedAccessException("El usuario no tiene una sede asignada.");

                sedeIdAplicada = sedeUsuario;
            }
            var solicitudes = await _solicitudReposicion.ObtenerTodasConDetalleAsync(sedeIdAplicada, estado);
            return solicitudes.Select(MapearADocumentoDto).ToList();
        }

        private SolicitudReposicionDocumentoDto MapearADocumentoDto(SolicitudReposicion solicitud)
        {
            return new SolicitudReposicionDocumentoDto
            {
                Id = solicitud.Id,
                SedeSolicitante = solicitud.SedeSolicitante.Nombre,
                UsuarioSolicitante = solicitud.UsuarioSolicitante.NombreCompletoRazonSocial,
                FechaSolicitud = solicitud.FechaSolicitud,
                Estado = solicitud.Estado.ToString(),
                MotivoSolicitud = solicitud.MotivoSolicitud,
                FechaLimiteRequerida = solicitud.FechaLimiteRequerida,
                DocumentoFormal = _plantillaSolicitudService.GenerarCartaFormal(solicitud),
                Productos = solicitud.Detalles.Select(d => new DetalleSolicitudResponseDto
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.NombreComercial,
                    CantidadSolicitadaUnidades = d.CantidadSolicitadaUnidades,
                    CantidadAprobadaUnidades = d.CantidadAprobadaUnidades
                }).ToList()
            };
        }
    }
}
