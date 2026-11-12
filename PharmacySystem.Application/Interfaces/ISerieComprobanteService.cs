using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface ISerieComprobanteService
    {
        Task CrearSerieAsync(SerieComprobanteDto dto);
        Task ActualizarEstadoSerieAsync(Guid id);
        Task<IEnumerable<SerieComprobanteResponseDto>> ObtenerTodasAsync();
        Task<IEnumerable<SerieComprobanteResponseDto>> ObtenerPorSedeIdAsync(Guid sedeId);
    }
}
