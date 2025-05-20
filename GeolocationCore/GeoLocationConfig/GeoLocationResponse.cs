using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geolocation.Core.GeoLocationConfig
{
        public class GeoLocationResponse
        {
        [JsonProperty("ip")]
        public string IpAddress { get; set; }

        [JsonProperty("country_code2", NullValueHandling = NullValueHandling.Ignore)]
        public string CountryCode { get; set; }

        [JsonProperty("country_name", NullValueHandling = NullValueHandling.Ignore)]
        public string CountryName { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("isp")]
        public string Isp { get; set; }
    }
}
