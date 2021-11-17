using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
// ReSharper disable TemplateIsNotCompileTimeConstantProblem

// ReSharper disable MemberCanBePrivate.Global

namespace WATERCryst.myBIOCAT.RestClient
{
    public class RestClient : IDisposable
    {
        private readonly ILogger _logger;
        private readonly CallbackServer _callbackServer;
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

            _callbackServer = new CallbackServer(logger, this, 8888);
            _callbackServer.Start();
        }

        public void Dispose()
        {
            _client.Dispose();
            _callbackServer.Dispose();
        }
        

        /// <summary>
        /// Returns the current state of the device. The event property may be null if there is no current event.
        /// </summary>
        /// <returns>DeviceState</returns>
        public DeviceState? GetCurrentDeviceState()
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


        private bool ExecuteGetRequest(string route)
        {
            try
            {
                var response = _client.GetAsync($"{ApiServer}/{route}").Result;
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return false;
        }
        
        /// <summary>
        /// Enables or disables absence mode and raises or reverts leakage detector sensitivity.
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public bool SetAbsenceMode(bool enabled)
        {
            var mode = enabled ? "enable" : "disable";
            return ExecuteGetRequest($"absence/{mode}");
        }

        /// <summary>
        /// Acknowledges the current device warning or error.
        /// </summary>
        /// <returns></returns>
        public bool AcknowledgeCurrentDeviceEvent() => ExecuteGetRequest($"ackevent");

        /// <summary>
        /// Enables absence mode and raises leakage detector sensitivity.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool PauseLeakageProtection(int minutes)
        {
            if (minutes is < 1 or > 4320) throw new ArgumentException("Invalid pause duration", nameof(minutes));
            
            return ExecuteGetRequest($"leakageProtection/pause?minutes={minutes}");
        }

        /// <summary>
        /// Disables absence mode and reverts leakage detector sensitivity to its default level.
        /// </summary>
        /// <returns></returns>
        public bool UnpauseLeakageProtection() => ExecuteGetRequest("leakageProtection/unpause");

        /// <summary>
        /// Starts the micro-leakage measurement to check the leak- tightness of the piping.
        /// This allows the detection of micro- leaks such as dripping taps or pipe fittings.
        /// For this measurements, water supply is briefly shut off.
        /// An unexpected water consumption during the measuring process,
        /// e.g. flushing the toilet or opening a tap, is automatically detected and the water supply is
        /// restored within a few seconds. However, the test fails and the API call has to be repeated.
        /// Notice
        /// If you use a drip irrigation system in your household, this can be detected as a micro leak.
        /// </summary>
        /// <returns></returns>
        public bool StartMicroLeakageMeasurement() => ExecuteGetRequest("mlmeasurement/start");

        
        /// <summary>
        /// Triggers a command to fetch current measurement data from the device.
        /// On arrival, the data will be forwarded to your webhook endpoint as a MeasurementResponse.
        /// </summary>
        /// <returns></returns>
        public bool LoadCurrentMeasurementData() => ExecuteGetRequest("measurements/now");

        /// <summary>
        /// Starts the self test routine. Automatically checks all actuators and sensors and fills the active unit with drinking water over a defined flushing time.
        /// Requires 2 minutes for completion.
        /// The device will return to water treatment if no errors could be detected. In the case of errors, the current error will be reported via the webhook endpoint.
        /// Additionally it can be queried at any time with GET v1/state.
        /// </summary>
        /// <returns></returns>
        public bool StartSelfTest() => ExecuteGetRequest("selftest");
        
        /// <summary>
        /// Triggers a command to fetch daily statistics data from the device.
        /// On arrival, the data will be forwarded to your webhook endpoint as a StatisticsResponse.
        /// </summary>
        /// <returns></returns>
        public bool LoadDailyStatistics() => ExecuteGetRequest("statistics/daily");
        
        /// <summary>
        /// Connects the device with the water supply. Will confirm pending warnings and errors.
        /// </summary>
        /// <returns></returns>
        public bool OpenWaterSupply() => ExecuteGetRequest("watersupply/open");
        
        /// <summary>
        /// Disconnects the device from the water supply.
        /// </summary>
        /// <returns></returns>
        public bool ShutOffWaterSupply() => ExecuteGetRequest("watersupply/close");

        public void OnCallback(Stream content)
        {
            try
            {
                using var rdr = new StreamReader(content);
                var json = rdr.ReadToEnd();

                _logger.LogTrace(json);
                
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}