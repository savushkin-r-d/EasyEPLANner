using System.Collections.Generic;
using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IolConf
{
    [XmlRoot(ElementName = "Device")]
    public class Device
    {
        [XmlElement(ElementName = "Sensor")]
        public Sensor Sensor { get; set; }

        [XmlElement(ElementName = "Port")]
        public int Port { get; set; }

        [XmlElement(ElementName = "Parameters")]
        public Parameters Parameters { get; set; }

        [XmlElement(ElementName = "Devices")]
        public Devices Devices { get; set; }

        public void Add(List<Device> devices)
        {
            if (Devices == null) Devices = new Devices();

            Devices.Device = devices;
        }

        public bool ShouldSerializePort()
        {
            return Port != 0;
        }

        public bool ShouldSerializeDevices()
        {
            return Devices != null &&
                Devices.Device != null &&
                Devices.Device.Count > 0;
        }

        public bool IsEmpty()
        {
            return (Sensor == null || Sensor.IsEmpty()) &&
                Port == 0 &&
                (Parameters == null || Parameters.IsEmpty()) &&
                (Devices == null || Devices.IsEmpty());
        }
    }
}
