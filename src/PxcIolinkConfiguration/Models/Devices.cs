using System.Xml.Serialization;
using System.Collections.Generic;

namespace EasyEPlanner.PxcIolinkConfiguration.Models
{
    [XmlRoot(ElementName = "Devices")]
    public class Devices
    {
        [XmlElement(ElementName = "Device")]
        public List<Device> Device { get; set; }

        public Devices()
        {
            Device = new List<Device>();
        }

        public bool IsEmpty()
        {
            return Device.Count == 0;
        }
    }
}
