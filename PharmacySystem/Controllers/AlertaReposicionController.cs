using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlertaReposicionController : ControllerBase
    {
        private readonly IAlertaReposicionService _alertaReposicionService;
        public AlertaReposicionController(IAlertaReposicionService alertaReposicionService)
        {
            _alertaReposicionService = alertaReposicionService;
        }

        [HttpGet("alertas-reposicion")]
        [Authorize(Roles = "Farmaceutico")]
        public async Task<IActionResult> GenerarAlertas()
        {
            var result = await _alertaReposicionService.GenerarAlertaAsync();
            return Ok(result);
        }
    }
}
