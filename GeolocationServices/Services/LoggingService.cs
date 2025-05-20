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
    public class LoggingService : ILoggingService
    {
        private readonly IBlockedCountryRepo _repo;

        public LoggingService(IBlockedCountryRepo repo)
        {
            _repo = repo;
        }

            

        public PaginatedList<BlockedAttemptLog> GetBlockedAttemptts(int page = 1, int pageSize = 10)
          => _repo.GetBlockedAttempts(page, pageSize);

        public void LogAttempt(BlockedAttemptLog attempt)
        {
            if (attempt == null)
            {
                throw new ArgumentNullException(nameof(attempt));
            }
            _repo.LogAttempt(attempt);
        }
    }
}
