using Geolocation.Core.IRepo;
using Geolocation.Core.Models;
using Geolocation.Services.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geolocation.Services.Services
{
    public class IPCheckService : IIPCheckService
    {
        private readonly IGeoLocationService _geo;
        private readonly IBlockedCountryRepo _repo;
        private readonly ILogger<IPCheckService> _logger;

        public IPCheckService(IGeoLocationService geo, IBlockedCountryRepo repo, ILogger<IPCheckService> logger)
        {
            _geo = geo;
            _repo = repo;
            _logger = logger;
        }

        public async Task<bool> CheckIfCurrentIpIsBlocked(HttpContext context)
        {

            try
            {
                string ip = context.Connection.RemoteIpAddress?.ToString();
                string userAgent = context.Request.Headers["User-Agent"].ToString();

                if (string.IsNullOrWhiteSpace(ip))
                {
                    _logger.LogWarning("Could not determine caller IP address.");
                    return true; // Fail secure
                }

                var geo = await _geo.GetCountryWithIpAddress(ip);

                if (geo == null || string.IsNullOrWhiteSpace(geo.CountryCode))
                {
                    _logger.LogWarning("Failed to get country code from IP: {IP}", ip);
                    return true; // Fail secure
                }

                // Check both permanent and temporal blocks
                bool isPermanentlyBlocked = _repo.IsCountryBlocked(geo.CountryCode);
                bool isTemporarilyBlocked = _repo.IsCountryBlocked(geo.CountryCode);
                bool isBlocked = isPermanentlyBlocked || isTemporarilyBlocked;

                // Enhanced logging
                _repo.LogAttempt(new BlockedAttemptLog
                {
                    IPAddress = ip,
                    Timestamp = DateTime.UtcNow,
                    CountryCode = geo.CountryCode,
                    CountryName = geo.CountryName,
                    WasBlocked = isBlocked,
                    UserAgent = userAgent,
                   
                });

                if (isBlocked)
                {
                    _logger.LogInformation("Blocked request from {Country} ({CountryCode})",
                        geo.CountryName, geo.CountryCode);
                }

                return isBlocked;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if IP is blocked");
                return true; // Fail secure
            }
        }
    }
}



