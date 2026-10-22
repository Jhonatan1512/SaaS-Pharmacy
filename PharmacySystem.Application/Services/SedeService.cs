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

        
    }
}
