using Geolocation.Services.Services.Interface;
using GeolocationProject.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeolocationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemporalBlockController : ControllerBase
    {
        private readonly IBlockedCountryService _blockedService;

        public TemporalBlockController(IBlockedCountryService blockedService)
        {
            _blockedService = blockedService;
        }

        [HttpPost("temporal-block")]
        public async Task<IActionResult> TemporalBlock([FromBody] TemporalBlockRequest request)
        {
            try
            {
                await _blockedService.AddTemporalBlockAsync(request.CountryCode, request.DurationMinutes);
                return Ok($"Country {request.CountryCode} temporarily blocked for {request.DurationMinutes} minutes.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
