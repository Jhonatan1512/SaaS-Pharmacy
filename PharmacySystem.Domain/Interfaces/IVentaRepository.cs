using Microsoft.EntityFrameworkCore.Storage;
using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public interface IVentaRepository
    {
        Task<IDbContextTransaction> IniciarTransaccionAsync();
        Task CrearAsync(Venta venta);
        Task GuardarCambiosAsync();
        Task<Venta?> ObtenerPorIdAsync(Guid id);
    }
}
