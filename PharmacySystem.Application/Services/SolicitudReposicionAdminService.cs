using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services
{
    public class SolicitudReposicionAdminService : ISolicitudReposicionAdminService
    {
        private readonly ISolicitudReposicionRepository _repository;
        private readonly IPlantillaSolicitudService _plantillaSolicitudService;
        public SolicitudReposicionAdminService(IPlantillaSolicitudService plantillaSolicitudService, ISolicitudReposicionRepository solicitudReposicionRepository)
        {
            _plantillaSolicitudService = plantillaSolicitudService;
            _repository = solicitudReposicionRepository;
        }
        public async Task ResponderSolicitudAsync(Guid solicitudId, Guid adminId, ResponderSolicitudReposicionDto dto)
        {
            var solicitud = await _repository.ObtenerPorIdParaActualizarAsync(solicitudId)
            ?? throw new KeyNotFoundException("La solicitud de reposición no existe.");

            if (solicitud.Estado != EstadoSolicitudEnum.Pendiente)
                throw new InvalidOperationException($"La solicitud ya fue atendida con estado '{solicitud.Estado}'.");

            solicitud.UsuarioAprobadorId = adminId;
            solicitud.FechaRespuesta = DateTime.UtcNow;
            solicitud.RespuestaAdministrador = dto.RespuestaAdministrador.Trim();

            if (dto.Aprobada)
                ProcesarAprobacion(solicitud, dto.ProductosAprobados);
            else
                ProcesarRechazo(solicitud);

            await _repository.GuardarCambiosAsync();
        }

        private static void ProcesarAprobacion(SolicitudReposicion solicitud, List<DetalleAprobacionItemDto> productosAprobados)
        {
            var aprobadosPorProducto = productosAprobados
                .GroupBy(p => p.ProductoId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.CantidadAprobadaUnidades));

            foreach (var detalle in solicitud.Detalles)
            {
                if (!aprobadosPorProducto.TryGetValue(detalle.ProductoId, out int cantidadAprobada))
                    throw new InvalidOperationException($"Falta indicar la cantidad aprobada para el producto '{detalle.ProductoId}'.");

                if (cantidadAprobada > detalle.CantidadSolicitadaUnidades)
                    throw new InvalidOperationException("La cantidad aprobada no puede superar la cantidad solicitada.");

                detalle.CantidadAprobadaUnidades = cantidadAprobada;
            }

            bool tieneUnidadesAprobadas = solicitud.Detalles.Any(d => d.CantidadAprobadaUnidades > 0);
            solicitud.Estado = tieneUnidadesAprobadas
                ? EstadoSolicitudEnum.Aprobado
                : EstadoSolicitudEnum.Rechazado;
        }

        private static void ProcesarRechazo(SolicitudReposicion solicitud)
        {
            solicitud.Estado = EstadoSolicitudEnum.Rechazado;
            foreach (var detalle in solicitud.Detalles)
            {
                detalle.CantidadAprobadaUnidades = 0;
            }
        }

        public async Task ModificarRespuestaSolicitudAsync(Guid solicitudId, Guid adminId, ResponderSolicitudReposicionDto dto)
        {
            var solicitud = await _repository.ObtenerPorIdParaActualizarAsync(solicitudId)
                ?? throw new KeyNotFoundException("La solicitud de reposición no existe.");

            if (solicitud.Estado == EstadoSolicitudEnum.Pendiente)
                throw new InvalidOperationException("La solicitud aún está pendiente; utilice el endpoint de responder.");

            if (solicitud.Estado == EstadoSolicitudEnum.Despachado)
                throw new InvalidOperationException("No se puede modificar una solicitud que ya fue despachada.");

            solicitud.UsuarioAprobadorId = adminId;
            solicitud.FechaRespuesta = DateTime.UtcNow;
            solicitud.RespuestaAdministrador = dto.RespuestaAdministrador.Trim();

            if (dto.Aprobada)
                ProcesarAprobacion(solicitud, dto.ProductosAprobados);
            else
                ProcesarRechazo(solicitud);

            await _repository.GuardarCambiosAsync();
        }

        public async Task FinalizarSolictudAsync(Guid solicitudId)
        {
            var solicitud = await _repository.ObtenerPorIdParaActualizarAsync(solicitudId)
                ?? throw new KeyNotFoundException("La solicitud de reposición no existe.");

            if (solicitud.Estado != EstadoSolicitudEnum.Aprobado) throw new InvalidOperationException("No se puede finalizar solicitudes que no esten aprobadas");

            solicitud.Estado = EstadoSolicitudEnum.Despachado;
            await _repository.GuardarCambiosAsync();
        }

        public async Task<List<SolicitudReposicionDocumentoDto>> ListarTodasAsync(Guid? sedeId = null, EstadoSolicitudEnum? estado = null)
        {
            var solicitudes = await _repository.ObtenerTodasConDetalleAsync(sedeId, estado);
            return solicitudes.Select(MapearADocumentoDto).ToList();
        }

        public async Task<SolicitudReposicionDocumentoDto> ObtenerPorIdAsync(Guid id)
        {
            var solicitud = await _repository.ObtenerPorIdConDetallesAsync(id)
                ?? throw new KeyNotFoundException("La solicitud de reposición no existe.");

            return MapearADocumentoDto(solicitud);
        }

        private SolicitudReposicionDocumentoDto MapearADocumentoDto(SolicitudReposicion solicitud)
        {
            return new SolicitudReposicionDocumentoDto
            {
                Id = solicitud.Id,
                SedeId = solicitud.SedeSolicitante.Id,
                SedeSolicitante = solicitud.SedeSolicitante.Nombre,
                UsuarioSolicitante = solicitud.UsuarioSolicitante.NombreCompletoRazonSocial,
                FechaSolicitud = solicitud.FechaSolicitud,
                Estado = solicitud.Estado.ToString(),
                MotivoSolicitud = solicitud.MotivoSolicitud,
                FechaLimiteRequerida = solicitud.FechaLimiteRequerida,
                DocumentoFormal = _plantillaSolicitudService.GenerarCartaFormal(solicitud),
                RespuestaAdministrador = solicitud.RespuestaAdministrador,
                FechaRespuesta = solicitud.FechaRespuesta,
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
