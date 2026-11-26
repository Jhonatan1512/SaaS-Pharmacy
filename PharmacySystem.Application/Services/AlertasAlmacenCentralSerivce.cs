using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class AlertasAlmacenCentralSerivce : IAlertasAlmacenCentralSerivce
    {
        private readonly IInventarioAlmacenCentralRepository _inventarioCentralRepository;
        public AlertasAlmacenCentralSerivce(IInventarioAlmacenCentralRepository inventarioAlmacenCentralRepository)
        {
            _inventarioCentralRepository = inventarioAlmacenCentralRepository;
        }
        public async Task<AlertasAlmacenCentralResponseDto> ObtenerAlertasAlmacenCentralAsync(int diasAnticipacionVencimiento = 90)
        {
            if (diasAnticipacionVencimiento < 0)
                throw new ArgumentException("Los días de anticipación no pueden ser negativos.");

            var inventarios = await _inventarioCentralRepository.ObtenerActivosParaAlertasAsync();
            DateTime hoy = DateTime.UtcNow.Date;
            DateTime fechaLimiteVencimiento = hoy.AddDays(diasAnticipacionVencimiento);

            // Alertas de Stock Bajo (agrupado por Producto para sumar todos sus lotes)
            var productosStockBajo = inventarios
                .GroupBy(i => new
                {
                    i.Lote.ProductoId,
                    i.Lote.Producto.CodigoBarras,
                    i.Lote.Producto.NombreComercial,
                    i.Lote.Producto.Presentacion
                })
                .Select(g =>
                {
                    int stockTotal = g.Sum(x => x.StockFisicoUnidades);
                    // Toma el stock mínimo configurado (por defecto 100)
                    int stockMinimo = g.Max(x => x.StockMinimoUnidades);

                    return new
                    {
                        g.Key.ProductoId,
                        g.Key.CodigoBarras,
                        g.Key.NombreComercial,
                        g.Key.Presentacion,
                        StockTotal = stockTotal,
                        StockMinimo = stockMinimo,
                        LotesActivos = g.Count(x => x.StockFisicoUnidades > 0)
                    };
                })
                .Where(x => x.StockTotal <= x.StockMinimo)
                .OrderBy(x => x.StockTotal)
                .Select(x => new AlertaStockBajoCentralDto
                {
                    ProductoId = x.ProductoId,
                    CodigoBarras = x.CodigoBarras,
                    NombreComercial = x.NombreComercial,
                    Presentacion = x.Presentacion,
                    StockTotalUnidades = x.StockTotal,
                    StockMinimoReferencial = x.StockMinimo,
                    UnidadesFaltantesParaMinimo = Math.Max(0, x.StockMinimo - x.StockTotal),
                    NivelCriticidad = x.StockTotal == 0
                        ? "Agotado"
                        : x.StockTotal <= (x.StockMinimo / 2)
                            ? "Critico"
                            : "Bajo",
                    CantidadLotesActivos = x.LotesActivos
                })
                .ToList();

            // Alertas de Vencimiento (por Lote con stock físico > 0)
            var lotesPorVencer = inventarios
                .Where(i => i.StockFisicoUnidades > 0 && i.Lote.FechaVencimiento.Date <= fechaLimiteVencimiento)
                .OrderBy(i => i.Lote.FechaVencimiento)
                .Select(i =>
                {
                    int diasRestantes = (i.Lote.FechaVencimiento.Date - hoy).Days;
                    string estado = diasRestantes < 0
                        ? "Vencido"
                        : diasRestantes <= 30
                            ? "Critico"
                            : "Proximo";

                    return new AlertaVencimientoCentralDto
                    {
                        LoteId = i.LoteId,
                        ProductoId = i.Lote.ProductoId,
                        CodigoBarras = i.Lote.Producto.CodigoBarras,
                        NombreComercial = i.Lote.Producto.NombreComercial,
                        CodigoLote = i.Lote.CodigoLote,
                        FechaVencimiento = i.Lote.FechaVencimiento,
                        DiasParaVencer = diasRestantes,
                        StockFisicoUnidades = i.StockFisicoUnidades,
                        EstadoVencimiento = estado
                    };
                })
                .ToList();

            return new AlertasAlmacenCentralResponseDto
            {
                TotalProductosStockBajo = productosStockBajo.Count,
                TotalLotesPorVencer = lotesPorVencer.Count,
                ProductosStockBajo = productosStockBajo,
                LotesPorVencer = lotesPorVencer
            };
        }
    }
}
