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
    public class AlertaReposicionService : IAlertaReposicionService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IProductoSedeConfigRepository _configRepository;
        public AlertaReposicionService(ICurrentUserService currentUserService, IProductoSedeConfigRepository productoSedeConfigRepository)
        {
            _currentUserService = currentUserService;
            _configRepository = productoSedeConfigRepository;
        }
        public async Task<List<AlertaReposicionSedeDto>> GenerarAlertaAsync()
        {
            Guid sedeId = _currentUserService.GetSedeId();
            if (sedeId == Guid.Empty)
                throw new UnauthorizedAccessException("El usuario no tiene una sede asignada.");

            DateTime hoyPeru = DateTime.UtcNow.AddHours(-5).Date;
            var configuraciones = await _configRepository.ObtenerConInventarioPorSedeAsync(sedeId);

            var alertas = new List<AlertaReposicionSedeDto>();

            foreach (var config in configuraciones)
            {
                var inventariosConStock = config.Producto.Lotes
                    .SelectMany(lote => lote.InventariosSede
                        .Where(inv => inv.SedeId == sedeId && inv.StockFisicoUnidades > 0)
                        .Select(inv => new { Lote = lote, Inventario = inv }))
                    .ToList();

                // Stock útil (excluye lotes ya vencidos)
                int stockUtil = inventariosConStock
                    .Where(x => x.Lote.FechaVencimiento.Date >= hoyPeru)
                    .Sum(x => x.Inventario.StockFisicoUnidades);

                bool alertaStockBajo = stockUtil <= config.StockMinimoUnidades;

                var lotesEnRiesgo = inventariosConStock
                    .Where(x => (x.Lote.FechaVencimiento.Date - hoyPeru).TotalDays <= config.DiasAnticipacionVencimiento)
                    .Select(x =>
                    {
                        int dias = (int)(x.Lote.FechaVencimiento.Date - hoyPeru).TotalDays;
                        return new LotePorVencerDto
                        {
                            InventarioSedeId = x.Inventario.Id,
                            CodigoLote = x.Lote.CodigoLote,
                            FechaVencimiento = x.Lote.FechaVencimiento,
                            DiasParaVencer = dias,
                            UnidadesEnLote = x.Inventario.StockFisicoUnidades,
                            YaVencido = dias < 0
                        };
                    })
                    .OrderBy(l => l.DiasParaVencer)
                    .ToList();

                bool alertaVencimiento = lotesEnRiesgo.Count > 0;

                if (!alertaStockBajo && !alertaVencimiento)
                    continue;

                int unidadesPorVencer = lotesEnRiesgo.Sum(l => l.UnidadesEnLote);
                int stockProyectado = Math.Max(0, stockUtil - unidadesPorVencer);
                int sugeridoReponer = Math.Max(0, config.StockDeseadoUnidades - stockProyectado);

                alertas.Add(new AlertaReposicionSedeDto
                {
                    ProductoId = config.ProductoId,
                    NombreProducto = config.Producto.NombreComercial,
                    StockActualUnidades = stockUtil,
                    StockMinimoUnidades = config.StockMinimoUnidades,
                    StockDeseadoUnidades = config.StockDeseadoUnidades,
                    CantidadSugeridaReponer = sugeridoReponer,
                    TieneAlertaStockBajo = alertaStockBajo,
                    TieneAlertaVencimiento = alertaVencimiento,
                    NivelPrioridad = DeterminarPrioridad(stockUtil, alertaStockBajo, alertaVencimiento),
                    LotesEnRiesgo = lotesEnRiesgo
                });
            }

            return alertas
                .OrderByDescending(a => a.TieneAlertaStockBajo && a.TieneAlertaVencimiento)
                .ThenBy(a => a.StockActualUnidades)
                .ToList();
        }

        private static string DeterminarPrioridad(int stockUtil, bool stockBajo, bool vencimiento)
        {
            if (stockUtil == 0 || (stockBajo && vencimiento))
                return "Critica";

            return stockBajo ? "Alta" : "Preventiva";
        }
    }
}
