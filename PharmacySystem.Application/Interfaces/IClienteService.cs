using PharmacySystem.Domain.Entities;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteResponseDto>> ObtenerTodosAsync(string? busqueda = null);
        Task<ClienteResponseDto> ObtenerPorIdAsync(Guid id);
        Task<ClienteResponseDto> ObtenerPorDocumentoAsync(string numeroDocumento);
        Task CrearAsync(ClienteDto dto);
        Task ActualizarAsync(Guid id, ClienteUpdateDto dto);
        Task EliminarAsync(Guid id);
    }
}
