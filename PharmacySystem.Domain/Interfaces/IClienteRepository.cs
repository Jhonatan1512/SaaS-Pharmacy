using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> ObtenerTodosAsync(string? busqueda = null);
        Task<Cliente?> ObtenerPorIdAsync(Guid id);
        Task<Cliente?> ObtenerPorDocumentoAsync(TipoDoc tipoDoc, string numeroDocumento);
        Task<bool> ExisteDocumentoAsync(TipoDoc tipoDoc, string numeroDocumento, Guid? ignorarId = null);
        Task CrearAsync(Cliente cliente);
        Task Actualizar(Cliente cliente);
        Task Eliminar(Cliente cliente);
    }
}
