using PharmacySystem.Application.DTOs;
using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface IProcesadorPagosVentaService
    {
        List<PagoVenta> GenerarPagos(Guid ventaId, string serie, string numeroComprobante, decimal montoTotalVenta, IReadOnlyList<PagoVentaCreateDto> pagosDto);
    }
}
