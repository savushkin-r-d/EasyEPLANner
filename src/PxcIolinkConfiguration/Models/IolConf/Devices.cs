using System.Xml.Serialization;
using System.Collections.Generic;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IolConf
{
    [XmlRoot(ElementName = "Devices")]
    public class Devices
    {
        [XmlElement(ElementName = "Device")]
        public List<Device> Device { get; set; }

        public bool IsEmpty()
        {
            return Device == null || Device.Count == 0;
        }
    }
}
