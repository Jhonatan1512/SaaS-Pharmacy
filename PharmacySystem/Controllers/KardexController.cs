using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KardexController : ControllerBase
    {
        private readonly ITransaccionKardexService _transaccionKardexService;
        public KardexController(ITransaccionKardexService transaccionKardexService)
        {
            _transaccionKardexService = transaccionKardexService;
        }

        [HttpGet("registros-kardex")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ObtenerRegistrosKardex()
        {
            var reult = await _transaccionKardexService.ObtenerRegistrsoKardexAsync();
            return Ok(reult);
        }
    }
}
