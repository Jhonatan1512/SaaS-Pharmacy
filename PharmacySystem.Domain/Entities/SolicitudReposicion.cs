using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class SolicitudReposicion
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SedeSolicitanteId { get; set; }
        public Guid UsuarioSolicitanteId { get; set; }
        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
        public EstadoSolicitudEnum Estado { get; set; } = EstadoSolicitudEnum.Pendiente;

        public string MotivoSolicitud { get; set; } = string.Empty;
        public DateTime? FechaLimiteRequerida { get; set; }

        public string? RespuestaAdministrador { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        public Guid? UsuarioAprobadorId { get; set; }

        public Sede SedeSolicitante { get; set; } = null!;
        public Usuario UsuarioSolicitante { get; set; } = null!;
        public Usuario? UsuarioAprobador { get; set; }
        public virtual ICollection<DetalleSolicitudReposicion> Detalles { get; set; } = [];
    }
}
