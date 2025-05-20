using Geolocation.Core.IRepo;
using Geolocation.Core.Models;
using Geolocation.Core.Pagination;
using Geolocation.Services.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geolocation.Services.Services
{
    public class BlockedCountryService : IBlockedCountryService
    {
        private readonly Dictionary<string, BlockedCountries> _blockedCountries = new();
        private readonly Dictionary<string, TemporalBlock> _temporalBlocks = new();
        private readonly IBlockedCountryRepo _repo;
        private readonly IGeoLocationService _geo;

        public BlockedCountryService(IBlockedCountryRepo repo, IGeoLocationService geo)
        {
            _repo = repo;
            _geo = geo;
        }

        public void AddBlockedCountry(string countryCode)
        {
            countryCode = countryCode.ToUpperInvariant();
            if (_repo.IsCountryBlocked(countryCode))
                throw new InvalidOperationException("Country is already blocked.");

            var name = GetCountryNameFromCode(countryCode);
            var entity = new BlockedCountries
            {
                CountryCode = countryCode,
                CountryName = name
            };

            _repo.AddBlockedCountry(entity);
        }

        public void RemoveBlockedCountry(string code)
        {
            _repo.RemoveBlockedCountry(code);
        }

        public IEnumerable<BlockedCountries> GetBlockedCountries(string search = null)
        {
            return _repo.GetBlockedCountries(search);
        }

        public async Task AddTemporalBlockAsync(string code, int minutes)
        {
            code = code.ToUpperInvariant();

            if (minutes < 1 || minutes > 1440)
                throw new ArgumentException("Duration must be between 1 and 1440 minutes.");

            if (_repo.IsCountryBlocked(code))
                throw new InvalidOperationException("Country already blocked.");

            var name = GetCountryNameFromCode(code);
            var temporal = new TemporalBlock
            {
                CountryCode = code,
                CountryName = name,
                ExpiredTime = DateTime.UtcNow.AddMinutes(minutes)
            };

            _repo.AddTemporalBlock(temporal);
            await Task.CompletedTask;
        }

        private string GetCountryNameFromCode(string code)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"US", "United States"},
            {"EG", "Egypt"},
            {"GB", "United Kingdom"},
            {"FR", "France"},
            {"DE", "Germany"},
            {"IN", "India"},
            {"CN", "China"},
            {"JP", "Japan"},
            {"RU", "Russia"}
        };

            return dict.TryGetValue(code, out var name) ? name : code;
        }
    }
}
