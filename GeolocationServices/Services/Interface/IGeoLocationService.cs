using Geolocation.Core.GeoLocationConfig;
using Geolocation.Core.Models;
using Geolocation.Core.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Geolocation.Services.Services.Interface
{
    public interface IGeoLocationService
    {
        Task<GeoLocationResponse> GetCountryWithIpAddress(string ipAddress);

    }
}
