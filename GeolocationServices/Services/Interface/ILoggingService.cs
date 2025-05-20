using Geolocation.Core.Models;
using Geolocation.Core.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geolocation.Services.Services.Interface
{
    public interface ILoggingService
    {
        public void LogAttempt(BlockedAttemptLog attempt);
       

        PaginatedList<BlockedAttemptLog> GetBlockedAttemptts(int page = 1, int pageSize = 10);

    }
}
