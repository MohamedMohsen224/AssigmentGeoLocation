using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Geolocation.Core.IRepo;
using Geolocation.Core.Models;
using Geolocation.Core.Pagination;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Geolocation.Services
{
    public class BlockedCountryRepo : IBlockedCountryRepo
    {
        private readonly ConcurrentDictionary<string, BlockedCountries> _permanentBlocks;
        private readonly ConcurrentDictionary<string, TemporalBlock> _temporalBlocks;
        private readonly ConcurrentBag<BlockedAttemptLog> _attemptLogs;
        private readonly ILogger<BlockedCountryRepo> _logger;

        public BlockedCountryRepo(ILogger<BlockedCountryRepo> logger)
        {
            _permanentBlocks = new ConcurrentDictionary<string, BlockedCountries>(StringComparer.OrdinalIgnoreCase);
            _temporalBlocks = new ConcurrentDictionary<string, TemporalBlock>(StringComparer.OrdinalIgnoreCase);
            _attemptLogs = new ConcurrentBag<BlockedAttemptLog>();
            _logger = logger;
        }

        // ================================
        // Permanent Block Methods
        // ================================
        public void AddBlockedCountry(BlockedCountries blocked)
        {
            if (!_permanentBlocks.TryAdd(blocked.CountryCode.ToUpperInvariant(), blocked))
            {
                throw new InvalidOperationException($"Country {blocked.CountryCode} is already blocked.");
            }
            _logger.LogInformation("Blocked country added: {Code}", blocked.CountryCode);
        }

        public void RemoveBlockedCountry(string code)
        {
            code = code.ToUpperInvariant();

            if (!_permanentBlocks.TryRemove(code, out _))
            {
                throw new KeyNotFoundException($"Blocked country {code} not found.");
            }

            _logger.LogInformation("Removed blocked country: {Code}", code);
        }

        public IEnumerable<BlockedCountries> GetBlockedCountries(string search = null)
        {
            var allCountries = _permanentBlocks.Values
                .Concat(_temporalBlocks.Values)
                .DistinctBy(c => c.CountryCode);

            if (string.IsNullOrWhiteSpace(search))
                return allCountries;

            return allCountries.Where(c =>
                c.CountryCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.CountryName.Contains(search, StringComparison.OrdinalIgnoreCase));
        }


        // ================================
        // Temporal Block Methods
        // ================================
        public void AddTemporalBlock(TemporalBlock temporal)
        {
            if (!_temporalBlocks.TryAdd(temporal.CountryCode.ToUpperInvariant(), temporal))
            {
                throw new InvalidOperationException($"Country {temporal.CountryCode} is already temporarily blocked.");
            }

            _logger.LogInformation("Temporal block added: {Code} until {Expire}", temporal.CountryCode, temporal.ExpiredTime);
        }

        public void RemoveeExpiredTemporalBlocks()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in _temporalBlocks)
            {
                if (entry.Value.ExpiredTime <= now)
                {
                    _temporalBlocks.TryRemove(entry.Key, out _);
                    _logger.LogInformation("Expired temporal block removed: {Code}", entry.Key);
                }
            }
        }

        // ================================
        // Block Check
        // ================================
        public bool IsCountryBlocked(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode)) return false;

            countryCode = countryCode.ToUpperInvariant();

            if (_permanentBlocks.ContainsKey(countryCode))
                return true;

            if (_temporalBlocks.TryGetValue(countryCode, out var temporalBlock))
            {
                if (temporalBlock.ExpiredTime > DateTime.UtcNow)
                    return true;
                else
                    _temporalBlocks.TryRemove(countryCode, out _); // remove expired
            }

            return false;
        }

        // ================================
        // Logging
        // ================================
        public void LogAttempt(BlockedAttemptLog log)
        {
            _attemptLogs.Add(log);
            _logger.LogInformation("Logged blocked attempt from {IP} - Blocked: {Blocked}", log.IPAddress, log.WasBlocked);
        }

        public PaginatedList<BlockedAttemptLog> GetBlockedAttempts(int page, int size)
        {
            page = Math.Max(1, page);
            size = Math.Clamp(size, 1, 100);

            var allLogs = _attemptLogs;

            var items = allLogs
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

            return new PaginatedList<BlockedAttemptLog>(items, allLogs.Count, page, size);
        }
    }
}
