using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public interface ITransaccionKardexRepository
    {
        Task AgregarAsync(TransaccionKardex transaccion);
        Task<List<TransaccionKardex>> ObtenerRegistrosKardexAsync();
        Task GuardarCambiosAsync();
    }
}
