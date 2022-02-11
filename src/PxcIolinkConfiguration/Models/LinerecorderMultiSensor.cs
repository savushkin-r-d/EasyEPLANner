using System.Collections.Generic;
using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models
{
    [XmlRoot(ElementName = "Linerecorder_MultiSensor", Namespace = "http://www.ifm.com/datalink/LinerecorderSensor4")]
    public class LinerecorderMultiSensor
    {
        [XmlElement(ElementName = "Version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "Devices")]
        public Devices Devices { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }

        [XmlText]
        public string Text { get; set; }

        public void Add(Device device)
        {
            if (Devices == null) Devices = new Devices();
            if (Devices.Device == null) Devices.Device = new List<Device>();

            Devices.Device.Add(device);
        }

        public bool IsEmpty()
        {
            return Devices == null || Devices.IsEmpty();
        }
    }
}
