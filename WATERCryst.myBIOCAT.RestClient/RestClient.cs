using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
// ReSharper disable TemplateIsNotCompileTimeConstantProblem

// ReSharper disable MemberCanBePrivate.Global

namespace WATERCryst.myBIOCAT.RestClient
{
    public class RestClient
    {
        private readonly ILogger _logger;
        public string ApiServer { get; set; } = "https://appapi.watercryst.com/v1";
    
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;
    
    
        public RestClient(ILogger logger, string apiKey)
        {
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
        }

        public DeviceState? GetDeviceState()
        {
            try
            {
                var json = _client.GetStringAsync($"{ApiServer}/state").Result;
                var state = JsonSerializer.Deserialize<DeviceState>(json, _jsonOptions);
                return state;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
    
    }
}