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

            string ip = context.Connection.RemoteIpAddress?.ToString();
            string userAgent = context.Request.Headers["User-Agent"].ToString();

            if (string.IsNullOrWhiteSpace(ip))
            {
                _logger.LogWarning("Could not determine caller IP address.");
                return false;
            }

            var geo = await _geo.GetCountryWithIpAddress(ip);

            if (geo == null || string.IsNullOrWhiteSpace(geo.CountryCode))
            {
                _logger.LogWarning("Failed to get country code from IP: {IP}", ip);
                return false;
            }

            bool isBlocked = _repo.IsCountryBlocked(geo.CountryCode);

            // THIS IS WHERE THE LOG ENTRY GETS CREATED AND ADDED TO THE IN-MEMORY LIST
            _repo.LogAttempt(new BlockedAttemptLog
            {
                IPAddress = ip,
                Timestamp = DateTime.UtcNow,
                CountryCode = geo.CountryCode,
                CountryName = geo.CountryName,
                WasBlocked = isBlocked,
                UserAgent = userAgent
            });

            return isBlocked;
        }
    }
}



