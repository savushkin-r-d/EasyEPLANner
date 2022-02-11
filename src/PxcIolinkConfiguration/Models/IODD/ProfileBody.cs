using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "ProfileBody")]
    public class ProfileBody
    {
        [XmlElement(ElementName = "DeviceIdentity")]
        public DeviceIdentity DeviceIdentity { get; set; }

        [XmlElement(ElementName = "DeviceFunction")]
        public DeviceFunction DeviceFunction { get; set; }
    }
}
