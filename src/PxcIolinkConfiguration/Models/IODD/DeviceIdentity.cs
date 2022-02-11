using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "DeviceIdentity")]
    public class DeviceIdentity
    {
        [XmlAttribute(AttributeName = "vendorId")]
        public int VendorId { get; set; }

        [XmlAttribute(AttributeName = "deviceId")]
        public int DeviceId { get; set; }
    }
}
