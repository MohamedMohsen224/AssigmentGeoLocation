using Geolocation.Services.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeolocationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IPCheckController : ControllerBase
    {
        private readonly IIPCheckService _checkService;

        public IPCheckController(IIPCheckService checkService)
        {
            _checkService = checkService;
        }

        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock()
        {
            bool isBlocked = await _checkService.CheckIfCurrentIpIsBlocked(HttpContext);
            return Ok(new { IsBlocked = isBlocked });
        }
    }
}
