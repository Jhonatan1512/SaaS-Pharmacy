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

    public int CantidadDaniadaAcumulada { get; private set; } = 0;
    public int UltimaCantidadDaniada { get; private set; } = 0;

    public EstadoLoteSedeEnum Estado { get; private set; } = EstadoLoteSedeEnum.EnTransito;
    public DateTime? FechaRecepcion { get; private set; }
    public Guid? UsuarioRecibeId { get; private set; }
    public string? ObservacionRecepcion { get; private set; }

    public Sede Sede { get; set; } = null!;
    public Lote Lote { get; set; } = null!;
    public Usuario? UsuarioRecibe { get; set; }

    /// Reabastece el stock y absorbe las mermas y datos de recepción al fusionar capas.
    public void ReabastecerStock(
        int cantidad,
        int cantidadDanada = 0,
        decimal? precioUnidad = null,
        decimal? precioBlister = null,
        decimal? precioCaja = null,
        Guid? usuarioRecibeId = null,
        DateTime? fechaRecepcion = null,
        string? observacion = null)
    {
        StockFisicoUnidades += cantidad;

        // Acumular merma y registrar la última cantidad dañada
        if (cantidadDanada > 0)
        {
            CantidadDaniadaAcumulada += cantidadDanada;
            UltimaCantidadDaniada = cantidadDanada;
        }

        // Transferir auditoría de recepción a la capa activa existente
        if (usuarioRecibeId.HasValue) UsuarioRecibeId = usuarioRecibeId;
        if (fechaRecepcion.HasValue) FechaRecepcion = fechaRecepcion;
        if (!string.IsNullOrWhiteSpace(observacion)) ObservacionRecepcion = observacion;

        if (precioUnidad is > 0) PrecioVentaUnidad = precioUnidad.Value;
        if (precioBlister is >= 0) PrecioVentaBlister = precioBlister.Value;
        if (precioCaja is >= 0) PrecioVentaCaja = precioCaja.Value;

        if (Estado == EstadoLoteSedeEnum.Agotado && StockFisicoUnidades > 0)
        {
            Estado = EstadoLoteSedeEnum.Activo;
        }
    }

    public void ConfirmarRecepcion(
        Guid usuarioRecibeId,
        DateTime fechaRecepcion,
        int cantidadDanada,
        string? observacion = null)
    {
        if (Estado != EstadoLoteSedeEnum.EnTransito)
            throw new InvalidOperationException("Solo se pueden confirmar lotes que se encuentren en tránsito.");

        if (cantidadDanada < 0)
            throw new ArgumentException("La cantidad dañada no puede ser un número negativo.");

        if (cantidadDanada > StockFisicoUnidades)
            throw new InvalidOperationException("La cantidad dañada no puede superar el stock enviado.");

        UltimaCantidadDaniada = cantidadDanada;
        CantidadDaniadaAcumulada = cantidadDanada;

        StockFisicoUnidades -= cantidadDanada;

        UsuarioRecibeId = usuarioRecibeId;
        FechaRecepcion = fechaRecepcion;
        ObservacionRecepcion = observacion;

        Estado = StockFisicoUnidades > 0
            ? EstadoLoteSedeEnum.Activo
            : EstadoLoteSedeEnum.Agotado;
    }

    public void CorregirMermaRecepcion(int nuevaCantidadDanada, string? nuevaObservacion)
    {
        if (Estado != EstadoLoteSedeEnum.Activo && Estado != EstadoLoteSedeEnum.Agotado)
            throw new InvalidOperationException("Solo se pueden corregir mermas de registros recepcionados.");

        if (nuevaCantidadDanada < 0)
            throw new ArgumentException("La cantidad dañada no puede ser un número negativo.");

        int diferenciaStock = UltimaCantidadDaniada - nuevaCantidadDanada;

        if (StockFisicoUnidades + diferenciaStock < 0)
        {
            throw new InvalidOperationException(
                $"No se puede ajustar la merma. El stock vendible actual ({StockFisicoUnidades}) es menor a la reducción solicitada.");
        }

        if (CantidadDaniadaAcumulada - diferenciaStock < 0)
        {
            throw new InvalidOperationException("El ajuste dejaría el acumulado total de mermas en un número negativo.");
        }

        StockFisicoUnidades += diferenciaStock;
        CantidadDaniadaAcumulada -= diferenciaStock;
        UltimaCantidadDaniada = nuevaCantidadDanada;

        if (!string.IsNullOrWhiteSpace(nuevaObservacion))
        {
            ObservacionRecepcion = $"[CORREGIDO]: {nuevaObservacion}";
        }

        Estado = StockFisicoUnidades > 0
            ? EstadoLoteSedeEnum.Activo
            : EstadoLoteSedeEnum.Agotado;
    }
}