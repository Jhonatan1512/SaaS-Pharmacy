using Mapster;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Services
{
    public class SedeService : ISedeService
    {
        private readonly ISedeRepository _sedeRepository;
        public SedeService(ISedeRepository sedeRepository) 
        {
            _sedeRepository = sedeRepository;
        }
        private async Task<string> contarSedes()
        {
            var cantidadSedes = await _sedeRepository.GetSedeAsync();
            int siguienteCodigo = cantidadSedes.Count() + 1;
            return siguienteCodigo.ToString("D4");
        }

        public async Task<SedeDto> CrearSedeAsync(SedeDto sede)
        {
            var nuevaSede = sede.Adapt<Sede>();
            nuevaSede.CodigoEstablecimientoAnexo = await contarSedes();
            var result = await _sedeRepository.CrearSedeAsync(nuevaSede);
            return result.Adapt<SedeDto>();
        }

        public async Task<IEnumerable<ObtenerSedeDto>> ObtenerSedeAsync()
        {
            var datosDto = await _sedeRepository.GetSedeAsync();
            var result = datosDto.Adapt<List<ObtenerSedeDto>>();
            return result;
        }

        public async Task<SedeDataDto?> ObtenerPersonalOficinaAsync(Guid sedeId)
        {
            var data = await _sedeRepository.ObtenerSedeIdAsync(sedeId);

            if(data == null)
            {
                throw new InvalidOperationException("No existen registro para el id indicado");
            }

            var result = data.Adapt<SedeDataDto>();
            return result;
        }

        public async Task<bool> ActualizarSedeAsync(ActualizarSedeDto dto, Guid id)
        {
            var sedeExiste = await _sedeRepository.ObtenerSedeIdAsync(id);
            if (sedeExiste == null) throw new KeyNotFoundException("Sede no encontrada");

            dto.Adapt(sedeExiste);
            await _sedeRepository.ActualizarSedeAsync(sedeExiste);

            return true;
        }

        public async Task<bool> ActualizarEstadoAsync(Guid id)
        {
            var sedeExiste = await _sedeRepository.ObtenerSedePorIdAsync(id);
            if(sedeExiste == null) throw new KeyNotFoundException("Sede no encontrada");

            sedeExiste.Activa = !sedeExiste.Activa;
            await _sedeRepository.ActualizarSedeAsync(sedeExiste);
            return sedeExiste.Activa;
        }
    }
}
