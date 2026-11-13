using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public record TotalesVentasTurno(decimal Efectivo, decimal Yape, decimal Plin, decimal Tarjeta);
    public interface ITurnoCajaRepository
    {
        Task<TurnoCaja?> ObtenerPorIdAsync(Guid id);
        Task<TurnoCaja?> ObtenerTurnoAbiertoPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<TurnoCaja>> ObtenerPorSedeIdAsync(Guid sedeId);
        Task<TotalesVentasTurno> ObtenerTotalesVentasPorTurnoAsync(Guid turnoId);
        Task CrearAsync(TurnoCaja turno);
        Task GuardarCambiosAsync();
    }
}
