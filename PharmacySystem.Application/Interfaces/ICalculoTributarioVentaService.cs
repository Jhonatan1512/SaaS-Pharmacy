using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface ICalculoTributarioVentaService
    {
        DetalleVenta ConstruirDetalle(Guid ventaId, Guid inventarioSedeId, TipoUnidadVentaEnum tipoUnidadVenta, int cantidadVenta, 
                                      int cantidadUnidadesMinimas, decimal precioUnitarioConIgv, TipoAfectacionIgvEnum tipoAfectacionIgv);

        void ConsolidarTotalesYValidarPagos(Venta venta);
    }
}
