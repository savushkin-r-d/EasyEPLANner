namespace EplanDevice
{
    public interface ISubtypeControlChannels
    {
        int AI { get; }
        int AO { get; }
        int[] ChannelsCount { get; }
        int DI { get; }
        int DO { get; }
        DeviceSubType SubType { get; }
    }
}