using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface IPresentacionInventarioService
    {
        int CalcularUnidadesMinimas(InventarioSede inventario, TipoUnidadVentaEnum tipoUnidad, int cantidadVenta);
        decimal ObtenerPrecioPorPresentacion(InventarioSede inventario, TipoUnidadVentaEnum tipoUnidad);
    }
}
