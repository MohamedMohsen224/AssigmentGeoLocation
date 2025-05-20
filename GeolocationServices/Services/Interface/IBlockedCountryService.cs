using Geolocation.Core.Models;
using Geolocation.Core.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geolocation.Services.Services.Interface
{
    public interface IBlockedCountryService
    {
        public void AddBlockedCountry(string countryCode, string addedBy = "system");
        public void RemoveBlockedCountry(string code);
        public IEnumerable<BlockedCountries> GetBlockedCountries(string search = null);
        public Task AddTemporalBlockAsync(string code, int minutes);




    }
}
