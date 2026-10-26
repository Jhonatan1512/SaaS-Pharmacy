using Mapster;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Domain.Entities;
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
        }
    }
}
