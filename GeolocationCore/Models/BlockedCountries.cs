using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geolocation.Core.Models
{
    public class BlockedCountries : BaseClass
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
    }
}
