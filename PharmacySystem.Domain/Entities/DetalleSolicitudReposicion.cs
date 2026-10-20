using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class DetalleSolicitudReposicion
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SolicitudId { get; set; }
        public Guid ProductoId { get; set; }
        public int CantidadSolicitadaUnidades { get; set; }
        public int? CantidadAprobadaUnidades { get; set; }

        public SolicitudReposicion Solicitud { get; set; } = null!;
        public Producto Producto { get; set; } = null!;
    }
}
