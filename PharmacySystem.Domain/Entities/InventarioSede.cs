using PharmacySystem.Domain.Enums;
using System;

namespace PharmacySystem.Domain.Entities;

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
    public string? ObservacionRecepcion { get; private set; }

    public Sede Sede { get; set; } = null!;
    public Lote Lote { get; set; } = null!;
    public Usuario? UsuarioRecibe { get; set; }

    /// Reabastecimiento y actualización condicional de precios realizada por Gerencia/Almacén.
    /// Mantiene el estado en EnTransito hasta la recepción en la sede.
    
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

    /// Exclusivo para el flujo del farmacéutico al recibir la mercancía física.
    /// Ajusta mermas, registra observaciones y cambia el estado a Activo.

    public void ConfirmarRecepcion(
        Guid usuarioRecibeId,
        DateTime fechaRecepcion,
        int cantidadDanada = 0,
        string? observacion = null)
    {
        if (Estado != EstadoLoteSedeEnum.EnTransito)
            throw new InvalidOperationException("Solo se pueden confirmar lotes que se encuentren en tránsito.");

        if (cantidadDanada > 0)
        {
            if (cantidadDanada > StockFisicoUnidades)
                throw new InvalidOperationException("La cantidad dañada no puede superar el stock enviado.");

            StockFisicoUnidades -= cantidadDanada;
        }

        UsuarioRecibeId = usuarioRecibeId;
        FechaRecepcion = fechaRecepcion;
        ObservacionRecepcion = observacion;

        Estado = StockFisicoUnidades > 0
            ? EstadoLoteSedeEnum.Activo
            : EstadoLoteSedeEnum.Agotado;
    }
}