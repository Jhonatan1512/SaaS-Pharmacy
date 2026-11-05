using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class InventarioSede
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SedeId { get; set; }
        public Guid LoteId { get; set; }

        public int StockFisicoUnidades { get; private set; } 
        public decimal PrecioVentaUnidad { get; private set; }
        public decimal PrecioVentaBlister { get; private set; }
        public decimal PrecioVentaCaja { get; private set; }

        public EstadoLoteSedeEnum Estado { get; private set; } = EstadoLoteSedeEnum.EnTransito;
        public DateTime? FechaRecepcion { get; private set; }
        public Guid? UsuarioRecibeId { get; private set; }

        public Sede Sede { get; set; } = null!;
        public Lote Lote { get; set; } = null!;
        public Usuario? UsuarioRecibe { get; set; }

        /// <summary>
        /// Reabastecimiento y actualización condicional de precios y recepción.
        /// </summary>
        public void ReabastecerStock(
        int cantidad,
        decimal? precioUnidad = null,
        decimal? precioBlister = null,
        decimal? precioCaja = null)
        {
            StockFisicoUnidades += cantidad;

            if (precioUnidad is > 0) PrecioVentaUnidad = precioUnidad.Value;
            if (precioBlister is >= 0) PrecioVentaBlister = precioBlister.Value;
            if (precioCaja is >= 0) PrecioVentaCaja = precioCaja.Value;
        }

        /// <summary>
        /// Exclusivo para el flujo del farmacéutico cuando verifica el paquete físico en la sucursal.
        /// </summary>
        public void ConfirmarRecepcion(Guid usuarioRecibeId, DateTime fechaRecepcion)
        {
            if (Estado != EstadoLoteSedeEnum.EnTransito)
                throw new InvalidOperationException("Solo se pueden confirmar lotes que se encuentren en tránsito.");

            UsuarioRecibeId = usuarioRecibeId;
            FechaRecepcion = fechaRecepcion;
            Estado = EstadoLoteSedeEnum.Activo;
        }
    }
}
