using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geolocation.Core.Models
{
    public class BlockedAttemptLog : BaseClass
    {
        public string IPAddress { get; set; }
        public DateTime Timestamp { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public bool WasBlocked { get; set; }
        public string UserAgent { get; set; }

    }

}
