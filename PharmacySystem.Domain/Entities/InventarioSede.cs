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

}