using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface IComprobanteValidacionService
    {
        Task ValidarClienteParaComprobanteAsync(TipoComprobanteEnum tipoComprobante, Guid? clienteId);
        void ValidarReglaMontoIdentificacion(TipoComprobanteEnum tipoComprobante, decimal montoTotalVenta, Guid? clienteId);
    }
}
