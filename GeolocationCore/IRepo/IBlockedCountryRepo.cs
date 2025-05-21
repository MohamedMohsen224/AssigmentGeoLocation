using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Geolocation.Core.Models;
using Geolocation.Core.Pagination;
using Microsoft.AspNetCore.Http;


namespace Geolocation.Core.IRepo
{
   public interface IBlockedCountryRepo
   {

        void AddBlockedCountry(BlockedCountries blocked);
        void AddTemporalBlock(TemporalBlock temporal);
        void RemoveBlockedCountry(string code);
        IEnumerable<BlockedCountries> GetBlockedCountries(string search = null);
        PaginatedList<BlockedAttemptLog> GetBlockedAttempts(int page, int size);
        bool IsCountryBlocked(string countryCode);
        void RemoveeExpiredTemporalBlocks();
        void LogAttempt(BlockedAttemptLog log);
        string GetCountryNameFromCode(string countryCode);
    }
}
