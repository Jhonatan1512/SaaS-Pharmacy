using PharmacySystem.Application.DTOs;
using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface ISolicitudReposicionService
    {
        Task CrearsolicitudAsync(Guid usuarioId, CrearSolicitudReposicionDto dto);
        Task<SolicitudReposicionDocumentoDto> ObtenerSolicitudPorId(Guid id);
        Task<List<SolicitudReposicionDocumentoDto>> ListarsolicitudesAsync(Guid? sedeFiltroAdmin = null, EstadoSolicitudEnum ? estado = null);
    }
}
