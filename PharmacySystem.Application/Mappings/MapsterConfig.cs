using Mapster;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Mappings
{
    public class MapsterConfig
    {
        public void RegisterMappings()
        {
            TypeAdapterConfig<Usuario, SedePersonalDto>
                .NewConfig()
                .Map(dest => dest.UsuarioNombreCompletoRazonSocial, src => src.NombreCompletoRazonSocial)
                .Map(dest => dest.UsuarioDocumento, src => src.Documento.ToString())
                .Map(dest => dest.UsuarioNumeroDoc, src => src.NumeroDoc)
                .Map(dest => dest.UsuarioEmail, src => src.Email);

            TypeAdapterConfig<Sede, SedeDataDto>
                .NewConfig()
                .Map(dest => dest.Personal, src => src.Usuarios);

            TypeAdapterConfig<ActualizarSedeDto, Sede>
                .NewConfig()
                .IgnoreNullValues(true);

            TypeAdapterConfig<ActualizarProductoDto, Producto>
                .NewConfig()
                .IgnoreNullValues(true);

            TypeAdapterConfig<actualizarLoteDto, Lote>
                .NewConfig()
                .IgnoreNullValues(true);

            TypeAdapterConfig<InventarioSede, InvetarioSedeResponseDto>.NewConfig()
                .Map(dest => dest.NombreSede, src => src.Sede.Nombre)
                .Map(dest => dest.CodigoLote, src => src.Lote.CodigoLote) 
                .Map(dest => dest.NombreProducto, src => src.Lote.Producto.NombreComercial)
                .Map(dest => dest.NombreUsuarioRecibe, src => src.UsuarioRecibe != null ? src.UsuarioRecibe.NombreCompletoRazonSocial : null);

            TypeAdapterConfig<ClienteUpdateDto, Cliente>
                .NewConfig()
                .IgnoreNullValues(true);

            TypeAdapterConfig<SerieComprobante, SerieComprobanteResponseDto>.NewConfig()
                .Map(dest => dest.NombreSede, src => src.Sede.Nombre);

            TypeAdapterConfig<TurnoCaja, TurnoCajaResponseDto>.NewConfig()
                .Map(dest => dest.NombreSede, src => src.Sede != null ? src.Sede.Nombre : string.Empty)
                .Map(dest => dest.NombreUsuario, src => src.Usuario != null ? src.Usuario.NombreCompletoRazonSocial : string.Empty);

            TypeAdapterConfig<TotalesVentasTurno, ResumenTotalVentasTurnoActivoDto>.NewConfig()
                .Map(dest => dest.TotalEfectivo, src => src.Efectivo)
                .Map(dest => dest.TotalYape, src => src.Yape)
                .Map(dest => dest.TotalPlin, src => src.Plin)
                .Map(dest => dest.TotalTarjeta, src => src.Tarjeta);

            TypeAdapterConfig<Venta, VentaDelDiaDto>.NewConfig()
                .Map(dest => dest.Comprobante, src => $"{src.Serie}-{src.Numero}")
                .Map(dest => dest.Cliente, src => src.Cliente != null ? src.Cliente.NombresRazonSocial : "Público General")
                .Map(dest => dest.MetodosPago, src => src.Pagos.Select(p => p.MedioPago.ToString()).ToList());

        }
    }
}
