using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public interface ISerieComprobanteRepository
    {
        Task<IEnumerable<SerieComprobante>> ObtenerTodasAsync();
        Task<IEnumerable<SerieComprobante>> ObtenerPorSedeIdAsync(Guid sedeId);
        Task<SerieComprobante?> ObtenerPorIdAsync(Guid id);
        Task<bool> ExisteSerieEnSedeAsync(Guid sedeId, TipoComprobanteEnum tipoComprobante, string serie);
        Task CrearAsync(SerieComprobante serieComprobante);
        Task<IEnumerable<string>> ObtenerCodigosSeriePorSedeYTipoAsync(Guid sedeId, TipoComprobanteEnum tipoComprobante);
        Task ActualizarEstadoSerieAsync(SerieComprobante serieComprobante);
    }
}
