using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface IAlmacenCentralService
    {
        Task RegistrarProductoConLoteInicialAsync(Guid usuarioId, RegistrarProductoConLoteDto dto);
        Task RegistrarIngresoLoteExistenteAsync(Guid usuarioId, RegistrarIngresoLoteDto dto);
        Task<List<InventarioAlmacenCentralDto>> ObtenerTodosLosActivosAsync();
        Task<InventarioDetalleAlmacenCentralDto?> ObtenerDetalleRegistroAsync(Guid id);
    }
}
