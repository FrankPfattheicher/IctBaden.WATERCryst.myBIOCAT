using System.Text.Json.Serialization;

namespace WATERCryst.myBIOCAT.RestClient
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DeviceMode
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeviceModes Id { get; set; }
        public string? Name { get; set; }
    }
}