using PharmacySystem.Application.DTOs;
using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface ISolicitudReposicionAdminService
    {
        Task ResponderSolicitudAsync(Guid solicitudId, Guid adminId, ResponderSolicitudReposicionDto dto);
        Task<List<SolicitudReposicionDocumentoDto>> ListarTodasAsync(Guid? sedeId = null, EstadoSolicitudEnum? estado = null);
        Task<SolicitudReposicionDocumentoDto> ObtenerPorIdAsync(Guid id);
    }
}
