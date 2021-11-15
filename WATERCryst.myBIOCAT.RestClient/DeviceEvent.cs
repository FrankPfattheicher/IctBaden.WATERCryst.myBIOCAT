using System;

namespace WATERCryst.myBIOCAT.RestClient
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DeviceEvent
    {
        public string? Type { get; set; }
        public int EventId { get; set; }
        public string? Category { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}