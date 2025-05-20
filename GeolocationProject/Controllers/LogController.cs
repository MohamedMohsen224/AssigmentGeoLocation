using Geolocation.Services.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeolocationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILoggingService _logService;

        public LogController(ILoggingService logService)
        {
            _logService = logService;
        }

        [HttpGet("blocked-attempts")]
        public IActionResult GetAttempts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _logService.GetBlockedAttemptts(page, pageSize);
            return Ok(result);
        }   
    }
}
