using Geolocation.Core.IRepo;
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
        private readonly IBlockedCountryRepo repo;

        public LogController(ILoggingService logService , IBlockedCountryRepo repo)
        {
            _logService = logService;
            this.repo = repo;
        }

        [HttpGet("blocked-attempts")]
        public IActionResult GetBlockedAttempts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = repo.GetBlockedAttempts(pageNumber, pageSize);
            return Ok(result);
        }
    }
}
