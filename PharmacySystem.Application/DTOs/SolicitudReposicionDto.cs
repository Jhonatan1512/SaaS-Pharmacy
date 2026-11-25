using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{

    public class CrearSolicitudReposicionDto
    {
        public string MotivoSolicitud { get; set; } = string.Empty;
        public DateTime? FechaLimiteRequerida { get; set; }
        public List<ItemProductoSolicitudDto> Productos { get; set; } = [];
    }

    public class ItemProductoSolicitudDto
    {
        public Guid ProductoId { get; set; }
        public int CantidadSolicitadaUnidades { get; set; }
    }

    public class ResponderSolicitudReposicionDto
    {
        public bool Aprobada { get; set; }
        public string RespuestaAdministrador { get; set; } = string.Empty;
        public List<DetalleAprobacionItemDto> ProductosAprobados { get; set; } = [];
    }

    public class DetalleAprobacionItemDto
    {
        public Guid ProductoId { get; set; }
        public int CantidadAprobadaUnidades { get; set; }
    }
}
