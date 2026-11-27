using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface IVentaService
    {
        Task RealizarVentaAsync(Guid userId, VentasDto dto);
        Task AnularVentaAsync(Guid userId, Guid ventaId, AnularVentaDto dto);
    }
}
