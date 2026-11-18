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
    public class PresentacionInventarioService : IPresentacionInventarioService
    {
        public int CalcularUnidadesMinimas(InventarioSede inventario, TipoUnidadVentaEnum tipoUnidad, int cantidadVenta)
        {
            var producto = inventario.Lote.Producto;

            return tipoUnidad switch
            {
                TipoUnidadVentaEnum.Unidad => cantidadVenta,

                TipoUnidadVentaEnum.Blister => producto.UnidadesPorBlister > 0
                    ? cantidadVenta * producto.UnidadesPorBlister
                    : throw new InvalidOperationException($"El producto '{producto.NombreComercial}' no tiene configuradas unidades por blíster."),

                TipoUnidadVentaEnum.Caja => producto.UnidadesPorCaja > 0
                    ? cantidadVenta * producto.UnidadesPorCaja
                    : throw new InvalidOperationException($"El producto '{producto.NombreComercial}' no tiene configuradas unidades por caja."),

                _ => throw new ArgumentOutOfRangeException(nameof(tipoUnidad), "Tipo de unidad de venta no válido.")
            };
        }

        public decimal ObtenerPrecioPorPresentacion(InventarioSede inventario, TipoUnidadVentaEnum tipoUnidad)
        {
            return tipoUnidad switch
            {
                TipoUnidadVentaEnum.Unidad => inventario.PrecioVentaUnidad > 0
                    ? inventario.PrecioVentaUnidad
                    : throw new InvalidOperationException("Este lote no tiene configurado un precio por unidad."),

                TipoUnidadVentaEnum.Blister => inventario.PrecioVentaBlister > 0
                    ? inventario.PrecioVentaBlister
                    : throw new InvalidOperationException("Este lote no tiene configurado un precio por blíster."),

                TipoUnidadVentaEnum.Caja => inventario.PrecioVentaCaja > 0
                    ? inventario.PrecioVentaCaja
                    : throw new InvalidOperationException("Este lote no tiene configurado un precio por caja."),

                _ => throw new ArgumentOutOfRangeException(nameof(tipoUnidad), "Tipo de unidad de venta no válido.")
            };
        }
    }
}
