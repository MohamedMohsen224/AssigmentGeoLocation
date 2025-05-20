using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geolocation.Core.Enums
{
    public enum ErrorCodes
    {
        None = 0,
        InvalidIP = 1,
        InvalidCountryCode = 2,
        InvalidUserAgent = 3,
        InvalidRequest = 4,
        DatabaseError = 5,
        NotFound = 6,
        Unauthorized = 7,
        Forbidden = 8,
        InternalServerError = 9,
        BadRequest = 10,
        NotBlockedCountry = 11,


    }
}
