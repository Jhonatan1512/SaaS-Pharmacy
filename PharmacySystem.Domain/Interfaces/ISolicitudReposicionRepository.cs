using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public interface ISolicitudReposicionRepository
    {
        Task AgregarAsync(SolicitudReposicion solicitud);
        Task<SolicitudReposicion?> ObtenerPorIdConDetallesAsync(Guid id);
        Task<List<SolicitudReposicion>> ObtenerTodasConDetalleAsync(Guid? sedeId = null, EstadoSolicitudEnum? estado = null);
        Task<SolicitudReposicion?> ObtenerPorIdParaActualizarAsync(Guid id);
        Task GuardarCambiosAsync();
    }
}
