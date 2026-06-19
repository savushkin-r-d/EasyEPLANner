using EplanDevice;

namespace EasyEPlanner
{
    /// <summary>
    /// Источник устройства и канала для привязки к клемме.
    /// </summary>
    internal interface IDeviceBindingSource
    {
        IODevice Device { get; }

        IODevice.IOChannel Channel { get; }
    }
}
