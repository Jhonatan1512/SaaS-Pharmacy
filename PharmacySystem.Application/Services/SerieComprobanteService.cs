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
    public class SerieComprobanteService : ISerieComprobanteService
    {
        private readonly ISerieComprobanteRepository _serieRepository;
        private readonly ISedeRepository _sedeRepository;
        public SerieComprobanteService(ISerieComprobanteRepository serieRepository, ISedeRepository sedeRepository)
        {
            _serieRepository = serieRepository;
            _sedeRepository = sedeRepository;
        }
        public async Task CrearSerieAsync(SerieComprobanteDto dto)
        {
            var sedeExiste = await _sedeRepository.ObtenerSedeIdAsync(dto.SedeId);
            if (sedeExiste == null)
                throw new KeyNotFoundException("La sede no existe");

            var serieComprobantes = await _serieRepository.ObtenerCodigosSeriePorSedeYTipoAsync(dto.SedeId, dto.TipoComprobante);

            int siguienteNumero = CalcularSiguienteNumeroSerie(serieComprobantes);
            string codigoSerie = SerieComprobante.GenerarCodigoSerie(dto.TipoComprobante, siguienteNumero);

            var nuevaSerie = dto.Adapt<SerieComprobante>();
            sedeExiste.Id = Guid.NewGuid();
            nuevaSerie.Serie = codigoSerie;
            nuevaSerie.UltimoCorrelativo = 0;
            nuevaSerie.Activo = true;

            await _serieRepository.CrearAsync(nuevaSerie);
        }

        public async Task ActualizarEstadoSerieAsync(Guid id)
        {
            var serieExiste = await _serieRepository.ObtenerPorIdAsync(id);
            if (serieExiste == null) throw new KeyNotFoundException("La serie no en la BD");

            serieExiste.Activo = !serieExiste.Activo;
            await _serieRepository.ActualizarEstadoSerieAsync(serieExiste);
        }

        public async Task<IEnumerable<SerieComprobanteResponseDto>> ObtenerTodasAsync()
        {
            var seriesBd = await _serieRepository.ObtenerTodasAsync();
            return seriesBd.Adapt<List<SerieComprobanteResponseDto>>();
        }

        public async Task<IEnumerable<SerieComprobanteResponseDto>> ObtenerPorSedeIdAsync(Guid sedeId)
        {
            var existeSede = await _sedeRepository.ObtenerSedeIdAsync(sedeId);
            if (existeSede == null) 
                throw new KeyNotFoundException("La sede no existe");

            var seriesBd = await _serieRepository.ObtenerPorSedeIdAsync(sedeId);

            return seriesBd.Adapt<List<SerieComprobanteResponseDto>>();

        }

        private static int CalcularSiguienteNumeroSerie(IEnumerable<string> seriesExistentes)
        {
            int maxNumero = 0;

            foreach (var codigo in seriesExistentes)
            {
                if (codigo.Length > 1 && int.TryParse(codigo.AsSpan(1), out int numero))
                {
                    if (numero > maxNumero)
                        maxNumero = numero;
                }
            }

            return maxNumero + 1;
        }
    }
}
