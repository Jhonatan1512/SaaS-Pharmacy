using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class ProcesadorPagosVentaService : IProcesadorPagosVentaService
    {
        public List<PagoVenta> GenerarPagos(Guid ventaId, string serie, string numeroComprobante, decimal montoTotalVenta, IReadOnlyList<PagoVentaCreateDto> pagosDto)
        {
            if (pagosDto == null || pagosDto.Count == 0)
                throw new InvalidOperationException("Debe registrar al menos un medio de pago.");

            int pagosSinMonto = pagosDto.Count(p => !p.Monto.HasValue);
            if (pagosSinMonto > 1)
                throw new InvalidOperationException("Solo el último medio de pago puede omitir el monto para calcular el saldo restante.");

            if (pagosSinMonto == 1 && pagosDto[^1].Monto.HasValue)
                throw new InvalidOperationException("Si desea que el sistema calcule el saldo restante, omita el monto únicamente en el último pago.");

            var pagosGenerados = new List<PagoVenta>(pagosDto.Count);
            decimal acumuladoParcial = 0m;

            for (int i = 0; i < pagosDto.Count; i++)
            {
                var item = pagosDto[i];
                decimal montoCalculado;

                if (item.Monto.HasValue)
                {
                    montoCalculado = Math.Round(item.Monto.Value, 2, MidpointRounding.AwayFromZero);
                    acumuladoParcial += montoCalculado;
                }
                else
                {
                    montoCalculado = Math.Round(montoTotalVenta - acumuladoParcial, 2, MidpointRounding.AwayFromZero);

                    if (montoCalculado <= 0)
                        throw new InvalidOperationException("Los montos parciales ingresados igualan o superan el total de la venta.");

                    acumuladoParcial += montoCalculado;
                }

                pagosGenerados.Add(new PagoVenta
                {
                    Id = Guid.NewGuid(),
                    VentaId = ventaId,
                    MedioPago = item.MedioPago,
                    Monto = montoCalculado,
                    NumOperacion = $"OP-{serie}{numeroComprobante}-{i + 1:D2}"
                });
            }

            if (acumuladoParcial != montoTotalVenta)
                throw new InvalidOperationException(
                    $"La suma de los pagos ({acumuladoParcial:F2}) no coincide con el total de la venta ({montoTotalVenta:F2}).");

            return pagosGenerados;
        }
    }
}
