using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Geolocation.Core.GeoLocationConfig;
using Geolocation.Core.IRepo;
using Geolocation.Core.Models;
using Geolocation.Core.Pagination;
using Geolocation.Services.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Geolocation.Services.Services
{
    public class GeoLocationService : IGeoLocationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<GeoLocationService> _logger;

        public GeoLocationService(HttpClient httpClient, IConfiguration config, ILogger<GeoLocationService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<GeoLocationResponse> GetCountryWithIpAddress(string ipAddress)
        {
            try
            {
                var apiKey = _config["GeoLocationApi:ApiKey"];
                var baseUrl = _config["GeoLocationApi:BaseUrl"];

                if (!IPAddress.TryParse(ipAddress, out _))
                {
                    _logger.LogWarning("Invalid IP address format: {ip}", ipAddress);
                    return null;
                }

                _logger.LogInformation("Requesting geo data for IP: {ip}", ipAddress);

                var response = await _httpClient.GetAsync($"{baseUrl}?apiKey={apiKey}&ip={ipAddress}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API request failed. Status: {status}, Response: {response}",
                        response.StatusCode, errorContent);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Raw API response: {json}", json); 

                var geo = JsonConvert.DeserializeObject<GeoLocationResponse>(json);

                if (geo == null)
                {
                    _logger.LogError("Deserialization failed for IP: {ip}", ipAddress);
                }

                return geo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching geo data for IP: {ip}", ipAddress);
                return null;
            }
        }
    }
}
