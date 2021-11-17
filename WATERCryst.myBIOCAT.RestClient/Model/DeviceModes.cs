// ReSharper disable InconsistentNaming
namespace WATERCryst.myBIOCAT.RestClient
{
    public enum DeviceModes
    {
        SU,	// Start Up	
        RS,	// Rinse	After finishing the TD, the hot water is rinsed out of the cartridge via the flushing line. The multi-chamber valve moves to the backflush position. Cold water flows into the cartridge and removes the hot water, which reaches the drain via the flushing line. As soon as the cartridge has cooled down, the flushing (RS) stops and the unit returns to normal operation (WT or ST).
        ST,	// Self Test	Automatically checks all actuators and sensors and fills the active unit with drinking water over a defined flushing time.
        UD,	// Firmware Update	
        FS,	// Failsafe	
        ER,	// Error Mode	In the event of an error, the device will resort to error mode.
        WO,	// Water Off	
        WT,	// Water Treatment	Default mode of operation. During the WT, water flows through the limescale protection active unit (cartridge), and drinking water temperature monitoring takes place at the same time.
        TD	// Thermal Disinfection	Thermal disinfection prevents contamination of the BIOCAT limescale protection device (microbiological intrinsic safety).
    }
}