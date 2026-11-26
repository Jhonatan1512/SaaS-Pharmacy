using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Interfaces
{
    public interface IInventarioSedeRepository
    {
        Task<bool> ExisteInventarioEnSedeAsync(Guid sedeId, Guid loteId);
        Task<InventarioSede?> ObtenerPorIdAsync(Guid id);
        Task<IEnumerable<InventarioSede>> ObtenerPorSedeIdAsync(Guid sedeId);
        Task<List<InventarioSede>> ObtenerPorIdsParaVentaAsync(IEnumerable<Guid> ids);

        // Operaciones transaccionales mediante Stored Procedures
        Task DespacharLoteASedeSpAsync(
            Guid sedeId,
            Guid loteId,
            int cantidadUnidades,
            decimal? precioVentaUnidad,
            decimal? precioVentaBlister,
            decimal? precioVentaCaja,
            Guid usuarioId);

        Task ConfirmarRecepcionSpAsync(
            Guid inventarioSedeId,
            Guid sedeUsuarioId,
            Guid usuarioRecibeId,
            int cantidadDaniada,
            string? observacion);

        Task ActualizarPrecioSpAsync(
            Guid inventarioSedeId,
            decimal? precioVentaUnidad,
            decimal? precioVentaBlister,
            decimal? precioVentaCaja);

        Task CorregirMermaRecepcionSpAsync(
            Guid inventarioSedeId,
            Guid sedeUsuarioId,
            Guid usuarioId,
            int nuevaCantidadDanada,
            string? motivoCorreccion);

        Task DescontarStockSpAsync(
            Guid inventarioSedeId,
            Guid sedeId,
            int cantidadUnidadesMinimas);
    }
}