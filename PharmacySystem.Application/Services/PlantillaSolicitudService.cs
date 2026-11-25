using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class PlantillaSolicitudService : IPlantillaSolicitudService
    {
        private static readonly CultureInfo CulturaPeru = new("es-PE");
        private const string ZonaHorariaPeruId = "America/Lima";
        public string GenerarCartaFormal(SolicitudReposicion solicitud)
        {
            ArgumentNullException.ThrowIfNull(solicitud);

            string fechaTexto = FormatearFechaLocal(solicitud.FechaSolicitud);
            string sedeNombre = solicitud.SedeSolicitante.Nombre;
            string sedeDireccion = solicitud.SedeSolicitante.Direccion ?? "Dirección registrada en sistema";
            string listaProductos = FormatearListaProductos(solicitud.Detalles);
            string clausulaFechaLimite = FormatearClausulaFechaLimite(solicitud.FechaLimiteRequerida);
            string solicitanteNombre = solicitud.UsuarioSolicitante.NombreCompletoRazonSocial;
            string solicitanteCorreo = solicitud.UsuarioSolicitante.Email ?? "Sin correo registrado";

            return ConstruirCuerpoCarta(
            sedeNombre,
            sedeDireccion,
            fechaTexto,
            listaProductos,
            solicitud.MotivoSolicitud,
            clausulaFechaLimite,
            solicitanteNombre,
            solicitanteCorreo);
        }

        private static string FormatearFechaLocal(DateTime fechaUtc)
        {
            var zonaPeru = TimeZoneInfo.FindSystemTimeZoneById(ZonaHorariaPeruId);
            var utcNormalizada = DateTime.SpecifyKind(fechaUtc, DateTimeKind.Utc);
            var fechaLocal = TimeZoneInfo.ConvertTimeFromUtc(utcNormalizada, zonaPeru);

            return fechaLocal.ToString("dd 'de' MMMM 'de' yyyy", CulturaPeru);
        }

        private static string FormatearListaProductos(IEnumerable<DetalleSolicitudReposicion> detalles)
        {
            var items = detalles
                .Select(d => $"**{d.CantidadSolicitadaUnidades}** unidades de **{d.Producto.NombreComercial}** (Código de barras ${d.Producto.CodigoBarras})")
                .ToList();

            return items.Count switch
            {
                0 => "los productos detallados en el anexo",
                1 => items[0],
                _ => $"{string.Join(", ", items.Take(items.Count - 1))} y {items.Last()}"
            };
        }

        private static string FormatearClausulaFechaLimite(DateTime? fechaLimite)
        {
            if (!fechaLimite.HasValue)
                return string.Empty;

            string fechaFormateada = fechaLimite.Value.ToString("dd 'de' MMMM 'de' yyyy", CulturaPeru);
            return $", idealmente antes del **{fechaFormateada}**";
        }

        private static string ConstruirCuerpoCarta(
        string sedeNombre,
        string sedeDireccion,
        string fechaTexto,
        string listaProductos,
        string motivo,
        string clausulaFechaLimite,
        string solicitanteNombre,
        string solicitanteCorreo)
        {
            return $"""
                **SOLICITO: Reposición de productos para abastecimiento de tienda**

                {sedeNombre}, {fechaTexto}

                **Señores:**
                **Área de Logística y Almacén Central**
                Presente.-

                De mi mayor consideración:

                Por medio de la presente me dirijo a ustedes en representación de **{sedeNombre}**, ubicada en **{sedeDireccion}**, con el fin de solicitar formalmente la reposición de mercadería necesaria para mantener la operatividad y el abastecimiento adecuado de nuestro punto de venta.

                Tras realizar la revisión de nuestro inventario actual, hemos identificado la necesidad de reponer a la brevedad los siguientes artículos: {listaProductos}. *(Motivo de la solicitud: {motivo})*.

                Agradeceré que puedan confirmar la disponibilidad de los productos detallados, así como indicarnos la fecha estimada de despacho hacia nuestro local en **{sedeDireccion}**{clausulaFechaLimite}.

                Sin otro particular y agradeciendo de antemano su pronta atención, me despido.

                Atentamente,

                **{solicitanteNombre}**
                Químico Farmacéutico / Responsable de Turno
                **{sedeNombre}**
                {solicitanteCorreo}
                """;
        }
    }
}
