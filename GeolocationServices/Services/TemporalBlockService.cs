using Geolocation.Core.IRepo;
using Geolocation.Core.Models;
using Geolocation.Services.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSharpFunctionalExtensions.Result;

namespace Geolocation.Services.Services
{
    public class TemporalBlockService : ITemporalBlockService
    {
        private readonly IBlockedCountryRepo _repo;
        private readonly ILogger<TemporalBlockService> _logger;
        private readonly HashSet<string> _validCodes;

        public TemporalBlockService(IBlockedCountryRepo repo, ILogger<TemporalBlockService> logger, IOptions<CountryCodeOptions> options)
        {
            _repo = repo;
            _logger = logger;
            _validCodes = new HashSet<string>(
            options.Value.ValidCountryCodes ?? new List<string>(),
            StringComparer.OrdinalIgnoreCase);
        }

        public void RemoveExpiredTemporalBlocks()
        {
            try
            {
                _repo.RemoveeExpiredTemporalBlocks();
                _logger.LogInformation("Expired temporal blocks removed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing expired temporal blocks");
            }
        }
    }
}
