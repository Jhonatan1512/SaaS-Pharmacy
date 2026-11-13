using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface ITurnoCajaService
    {
        Task AbrirTurnoAsync(Guid userId, TurnoCajaDto dto);
        Task CerrarTurnoAsync(Guid userId, Guid turnoId, CerrarCajaDto dto);
        Task CorregirConteoCierreAsync(Guid userId, Guid turnoId, CorregirConteoTurnoDto dto);
        Task<TurnoActivoResponseDto?> ObtenerMiTurnoActivoAsync(Guid userId);
        Task<IEnumerable<TurnoCajaResponseDto>> ObtenerTurnosMiSedeAsync();
        Task<TurnoCajaResponseDto> ObtenerPorIdAsync(Guid id);
    }
}
