using EplanDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.XML
{
    public class Driver : IDriver
    {
        public Driver(IDeviceManager deviceManager)
        {
            this.deviceManager = deviceManager;
            PrepareChannelsLogg();
        }

        private IDeviceManager deviceManager;

        private void PrepareChannelsLogg()
        {
            var tags = deviceManager.Devices
                .Where(d => d.DeviceType is DeviceType.WATCHDOG)
                .SelectMany<IODevice, string>(d => [
                    d.Properties[IODevice.Property.DO_dev]?.ToString(),
                    d.Properties[IODevice.Property.DI_dev]?.ToString(),
                    d.Properties[IODevice.Property.AO_dev]?.ToString(),
                    d.Properties[IODevice.Property.AI_dev]?.ToString()])
                .Where(devName => devName != null && devName != "")
                .Select(deviceManager.GetDevice)
                .Where(dev => dev.Description is not StaticHelper.CommonConst.Cap)
                .Select(dev => dev.DeviceType switch
                {
                    DeviceType.DO or DeviceType.DI => $"{dev.Name}.ST",
                    DeviceType.AO or DeviceType.AI => $"{dev.Name}.V",
                    _ => ""
                })
                .Where(tag => tag != "");

            foreach (var tag in tags)
            {
                Subtype.ChannelsLogging[tag] = false;
            }
        }

        public List<ISubtype> Subtypes { get; } = [];

        public ISubtype this[string subtype] => Subtypes.Find(st => st.Description == subtype);

        public IChannel AddChannel(string stDescription, IChannel channel, int count = 1)
        {
            if (this[stDescription] is { } subtype)
            {
                return subtype.AddChannel(channel, count);
            }

            var newSubtype = new Subtype(stDescription);
            Subtypes.Add(newSubtype);
            
            return newSubtype.AddChannel(channel, count);
        }

        public IChannel AddChannel(string stDescription, string channelName, string comment = "", int count = 1)
        {
            return AddChannel(stDescription, new Channel(channelName, comment), count);
        }
    }
}
