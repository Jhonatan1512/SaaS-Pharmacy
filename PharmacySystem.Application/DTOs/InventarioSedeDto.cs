using PharmacySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class InventarioSedeDto
    {
        public Guid SedeId { get; set; }
        public Guid LoteId { get; set; }
        public int StockFisicoUnidades { get; set; }
        public decimal? PrecioVentaUnidad { get; set; }
        public decimal? PrecioVentaBlister { get; set; }
        public decimal? PrecioVentaCaja { get; set; }
        public EstadoLoteSedeEnum Estado { get; set; } = EstadoLoteSedeEnum.EnTransito;
        
    }

    public class InvetarioSedeResponseDto
    {
        public Guid Id { get; set; }
        public Guid SedeId { get; set; }
        public string NombreSede { get; set; } = string.Empty;
        public Guid LoteId { get; set; }
        public string CodigoLote { get; set; } = string.Empty;
        public string NombreProducto { get; set; } = string.Empty;
        public int StockFisicoUnidades { get; set; }
        public decimal PrecioVentaUnidad { get; set; }
        public decimal PrecioVentaBlister { get; set; }
        public decimal PrecioVentaCaja { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaRecepcion { get; set; }
        public Guid? UsuarioRecibeId { get; set; }
    }

    public class CorregirPrecioInventarioSedeDto
    {
        public decimal? PrecioVentaUnidad { get; set; }
        public decimal? PrecioVentaBlister { get; set; }
        public decimal? PrecioVentaCaja { get; set; }
    }

    public class ConfirmarRecepcionInventarioDto
    {
        public int CantidadDaniada { get; set; }
        public string Observacion { get; set; } = string.Empty;
    }
}
