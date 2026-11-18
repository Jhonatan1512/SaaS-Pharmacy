using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class ComprobanteValidacionService : IComprobanteValidacionService
    {
        private const decimal MontoLimiteBoletaSinCliente = 700.00m;
        private readonly IClienteRepository _clienteRepository;

        public ComprobanteValidacionService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task ValidarClienteParaComprobanteAsync(TipoComprobanteEnum tipoComprobante, Guid? clienteId)
        {
            if (tipoComprobante == TipoComprobanteEnum.Factura)
            {
                if (!clienteId.HasValue)
                    throw new InvalidOperationException("Para emitir una Factura es obligatorio seleccionar un cliente con RUC.");

                var cliente = await _clienteRepository.ObtenerPorIdAsync(clienteId.Value)
                    ?? throw new KeyNotFoundException("El cliente especificado no existe.");

                if (cliente.TipoDocumento != TipoDoc.RUC)
                    throw new InvalidOperationException("Solo se puede emitir Factura a clientes registrados con RUC.");

                return;
            }

            if (clienteId.HasValue)
            {
                var cliente = await _clienteRepository.ObtenerPorIdAsync(clienteId.Value);
                if (cliente == null)
                    throw new KeyNotFoundException("El cliente especificado no existe.");
            }
        }

        public void ValidarReglaMontoIdentificacion(TipoComprobanteEnum tipoComprobante, decimal montoTotalVenta, Guid? clienteId)
        {
            if (tipoComprobante == TipoComprobanteEnum.Boleta &&
                montoTotalVenta >= MontoLimiteBoletaSinCliente &&
                !clienteId.HasValue)
            {
                throw new InvalidOperationException(
                    $"Para boletas de venta iguales o mayores a S/ {MontoLimiteBoletaSinCliente:F2} es obligatorio identificar al cliente.");
            }
        }
    }
}
