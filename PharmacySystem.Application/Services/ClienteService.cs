using Mapster;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;

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

        public async Task<IEnumerable<ClienteResponseDto>> ObtenerTodosAsync(string? busqueda = null)
        {
            var datosClientes = await _clienteRepository.ObtenerTodosAsync();
            return datosClientes.Adapt<List<ClienteResponseDto>>();
        }

        public async Task ActualizarAsync(Guid id, ClienteUpdateDto dto)
        {
            var clienteExiste = await _clienteRepository.ObtenerPorIdAsync(id);
            if (clienteExiste == null)
                throw new KeyNotFoundException("Cliente no encontrado");

            if (dto.TipoDocumento.HasValue || dto.NumeroDocumento != null)
            {
                var tipoDocFinal = dto.TipoDocumento ?? clienteExiste.TipoDocumento;
                var numeroDocFinal = dto.NumeroDocumento ?? clienteExiste.NumeroDocumento;

                ValidarFormatoDocumento(tipoDocFinal, numeroDocFinal);

                bool existeDocumento = await _clienteRepository.ExisteDocumentoAsync(
                    tipoDocFinal,
                    numeroDocFinal,
                    id 
                );

                if (existeDocumento)
                    throw new InvalidOperationException($"Ya existe un cliente registrado con el {tipoDocFinal} N° {numeroDocFinal}.");
            }

            dto.Adapt(clienteExiste);
            await _clienteRepository.ActualizarAsync(clienteExiste);
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

        public async Task<ClienteResponseDto> ObtenerPorDocumentoAsync(string numeroDocumento)
        {
            var cliente = await _clienteRepository.ObtenerPorDocumentoAsync(numeroDocumento);
            if (cliente == null)
                throw new KeyNotFoundException($"No se encontró ningún cliente con ese número de documento.");

            return cliente.Adapt<ClienteResponseDto>();
        }

        public async Task<ClienteResponseDto> ObtenerPorIdAsync(Guid id)
        {
            var cliente = await _clienteRepository.ObtenerPorIdAsync(id);
            if (cliente == null)
                throw new KeyNotFoundException("El cliente especificado no existe.");

            return cliente.Adapt<ClienteResponseDto>();
        }

        public async Task EliminarAsync(Guid id)
        {
            var cliente = await _clienteRepository.ObtenerPorIdAsync(id);
            if (cliente == null)
                throw new KeyNotFoundException("El cliente no fue encontrado.");

            await _clienteRepository.Eliminar(cliente);
        }
    }
}
