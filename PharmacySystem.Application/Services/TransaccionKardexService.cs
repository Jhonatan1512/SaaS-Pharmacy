using Mapster;
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
    public class TransaccionKardexService : ITransaccionKardexService
    {
        private readonly ITransaccionKardexRepository _transaccionKardexRepository;

        public TransaccionKardexService(ITransaccionKardexRepository transaccionKardexRepository)
        {
            _transaccionKardexRepository = transaccionKardexRepository;
        }
        public async Task<List<KardexResponseDto>> ObtenerRegistrsoKardexAsync()
        {
            var datosBd = await _transaccionKardexRepository.ObtenerRegistrosKardexAsync();
            return datosBd.Adapt<List<KardexResponseDto>>();
        }
    }
}
