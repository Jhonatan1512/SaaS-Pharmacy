using Mapster;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }
        public async Task CrearAsync(ClienteDto dto)
        {
            ValidarFormatoDocumento(dto.TipoDocumento, dto.NumeroDocumento);

            bool existeDocumento = await _clienteRepository.ExisteDocumentoAsync(dto.TipoDocumento, dto.NumeroDocumento);
            if (existeDocumento)
                throw new InvalidOperationException($"Ya existe un cliente registrado con el {dto.TipoDocumento} N° {dto.NumeroDocumento}.");

            var nuevoCliente = dto.Adapt<Cliente>();
            nuevoCliente.Id = Guid.NewGuid();

            await _clienteRepository.CrearAsync(nuevoCliente);
        }

        private static void ValidarFormatoDocumento(TipoDoc tipoDoc, string numeroDocumento)
        {
            if (tipoDoc == TipoDoc.DNI && numeroDocumento.Length != 8)
            {
                throw new ArgumentException("El DNI debe tener exactamente 8 dígitos.");
            }

            if (tipoDoc == TipoDoc.RUC && numeroDocumento.Length != 11)
            {
                throw new ArgumentException("El RUC debe tener exactamente 11 dígitos.");
            }
        }
    }
}
