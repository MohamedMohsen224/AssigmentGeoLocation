using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geolocation.Services.Services.Interface
{
    public interface IIPCheckService
    {
        Task<bool> CheckIfCurrentIpIsBlocked(HttpContext httpContext);


    }
}
