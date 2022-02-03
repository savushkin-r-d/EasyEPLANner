using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models
{
    [XmlRoot(ElementName = "Devices")]
    public class Devices
    {

        [XmlElement(ElementName = "Device")]
        public Device Device { get; set; }
    }
}
