using PharmacySystem.Application.DTOs;
using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface ICrearDetalleVentaService
    {
        Task<List<DetalleVenta>> CrearDetalleVentaAsync(Guid ventaId, Guid sedeId, IEnumerable<DetalleVentaCreateDto> dto);
    }
}
