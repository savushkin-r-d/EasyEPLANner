using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models
{
    [XmlRoot(ElementName = "Sensor")]
    public class Sensor
    {
        [XmlElement(ElementName = "VendorId")]
        public int VendorId { get; set; }

        [XmlElement(ElementName = "DeviceId")]
        public int DeviceId { get; set; }

        [XmlElement(ElementName = "ProductId")]
        public string ProductId { get; set; }
    }
}
