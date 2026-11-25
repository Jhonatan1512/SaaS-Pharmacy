using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class SolicitudReposicionDocumentoDto
    { 
        public Guid Id { get; set; }
        public string SedeSolicitante { get; set; } = string.Empty;
        public string UsuarioSolicitante { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string MotivoSolicitud { get; set; } = string.Empty;
        public DateTime? FechaLimiteRequerida { get; set; }
        public string DocumentoFormal { get; set; } = string.Empty;
        public string? RespuestaAdministrador { get; set; } = string.Empty;
        public DateTime? FechaRespuesta { get; set; } 
        public List<DetalleSolicitudResponseDto> Productos { get; set; } = [];
    }

    public class DetalleSolicitudResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int CantidadSolicitadaUnidades { get; set; }
        public int? CantidadAprobadaUnidades { get; set; }
    }
}
