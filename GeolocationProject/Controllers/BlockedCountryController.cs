using Geolocation.Services.Services.Interface;
using GeolocationProject.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeolocationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockedCountryController : ControllerBase
    {
        private readonly IBlockedCountryService _blockedService;

        public BlockedCountryController(IBlockedCountryService blockedService)
        {
            _blockedService = blockedService;
        }

        [HttpPost("block")]
        public IActionResult BlockCountry([FromBody] BlockCountryRequest request)
        {
            try
            {
                _blockedService.AddBlockedCountry(request.CountryCode);
                return Ok($"Country {request.CountryCode} blocked successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("block/{countryCode}")]
        public IActionResult UnblockCountry(string countryCode)
        {
            try
            {
                _blockedService.RemoveBlockedCountry(countryCode);
                return Ok($"Country {countryCode} unblocked.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("blocked")]
        public IActionResult GetBlockedCountries([FromQuery] string search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _blockedService.GetBlockedCountries(search);
            return Ok(result);
        }
    }
}
