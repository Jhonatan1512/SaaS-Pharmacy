using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class CalculoTributarioVentaService : ICalculoTributarioVentaService
    {
        private const decimal PorcentajeIgvDefault = 18.00m;

        public DetalleVenta ConstruirDetalle(
        Guid ventaId,
        Guid inventarioSedeId,
        TipoUnidadVentaEnum tipoUnidadVenta,
        int cantidadVenta,
        int cantidadUnidadesMinimas,
        decimal precioUnitarioConIgv,
        TipoAfectacionIgvEnum tipoAfectacionIgv)
        
        {
            decimal totalConIgv = Math.Round(precioUnitarioConIgv * cantidadVenta, 2, MidpointRounding.AwayFromZero);
            decimal valorUnitarioSinIgv;
            decimal subTotalSinIgv;
            decimal montoIgv;
            decimal tasaAplicada;

            if (tipoAfectacionIgv == TipoAfectacionIgvEnum.Gravado)
            {
                tasaAplicada = PorcentajeIgvDefault;
                decimal factorIgv = 1 + (PorcentajeIgvDefault / 100m);
                valorUnitarioSinIgv = Math.Round(precioUnitarioConIgv / factorIgv, 6, MidpointRounding.AwayFromZero);
                subTotalSinIgv = Math.Round(totalConIgv / factorIgv, 2, MidpointRounding.AwayFromZero);
                montoIgv = totalConIgv - subTotalSinIgv;
            }
            else
            {
                tasaAplicada = 0m;
                valorUnitarioSinIgv = precioUnitarioConIgv;
                subTotalSinIgv = totalConIgv;
                montoIgv = 0m;
            }

            return new DetalleVenta
            {
                Id = Guid.NewGuid(),
                VentaId = ventaId,
                InventarioSedeId = inventarioSedeId,
                TipoUnidadVenta = tipoUnidadVenta,
                CantidadVenta = cantidadVenta,
                CantidadUnidadesMinimas = cantidadUnidadesMinimas,
                TipoAfectacionIgv = tipoAfectacionIgv,
                PorcentajeIgv = tasaAplicada,
                ValorUnitarioSinIgv = valorUnitarioSinIgv,
                PrecioUnitarioConIgv = precioUnitarioConIgv,
                SubTotalSinIgv = subTotalSinIgv,
                MontoIgv = montoIgv,
                TotalConIgv = totalConIgv
            };
        }

        public void ConsolidarTotalesYValidarPagos(Venta venta)
        {
            venta.MontoOperacionesGravadas = venta.Detalles
                .Where(d => d.TipoAfectacionIgv == TipoAfectacionIgvEnum.Gravado)
                .Sum(d => d.SubTotalSinIgv);

            venta.MontoOperacionesExoneradas = venta.Detalles
                .Where(d => d.TipoAfectacionIgv == TipoAfectacionIgvEnum.Exonerado)
                .Sum(d => d.SubTotalSinIgv);

            venta.MontoOperacionesInafectas = venta.Detalles
                .Where(d => d.TipoAfectacionIgv == TipoAfectacionIgvEnum.Inafecto)
                .Sum(d => d.SubTotalSinIgv);

            venta.MontoIgv = venta.Detalles.Sum(d => d.MontoIgv);
            venta.MontoTotalImpuestos = venta.MontoIgv;
            venta.MontoTotalVenta = venta.Detalles.Sum(d => d.TotalConIgv);

        }
    }
}
