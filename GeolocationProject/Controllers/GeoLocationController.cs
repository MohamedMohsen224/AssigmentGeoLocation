using Geolocation.Core.GeoLocationConfig;
using Geolocation.Services.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GeolocationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeoLocationController : ControllerBase
    {
        private readonly IGeoLocationService _geoService;

        public GeoLocationController(IGeoLocationService geoService)
        {
            _geoService = geoService;
        }

        [HttpGet("lookup")]
        [ProducesResponseType(typeof(GeoLocationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Lookup([FromQuery] string ipAddress = null)
        {


            ipAddress ??= HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ipAddress))
                return BadRequest("IP address could not be determined");

            if (!IPAddress.TryParse(ipAddress, out _))
                return BadRequest("Invalid IP address format");

            var result = await _geoService.GetCountryWithIpAddress(ipAddress);

            if (result == null)
                return StatusCode(503, "Geolocation service unavailable");

            return Ok(new
            {
                ipAddress = result.IpAddress ?? ipAddress,
                country = result.CountryName,
                countryCode = result.CountryCode,
                city = result.City,
                isp = result.Isp,
                timestamp = DateTime.UtcNow
            });

        }
    }
}
