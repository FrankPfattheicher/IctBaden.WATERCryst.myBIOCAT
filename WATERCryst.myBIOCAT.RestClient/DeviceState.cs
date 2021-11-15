namespace WATERCryst.myBIOCAT.RestClient
{
    public class DeviceState
    {
        public bool Online { get; set; }
        public DeviceMode? Mode { get; set; }
        public DeviceEvent? Event { get; set; }
    }
}