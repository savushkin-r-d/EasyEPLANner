using System;
using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models
{
    [XmlRoot(ElementName = "Sensor")]
    public class Sensor : ICloneable
    {
        [XmlElement(ElementName = "VendorId")]
        public int VendorId { get; set; }

        [XmlElement(ElementName = "DeviceId")]
        public int DeviceId { get; set; }

        [XmlElement(ElementName = "ProductId")]
        public string ProductId { get; set; }

        public bool IsEmpty()
        {
            return VendorId == 0 &&
                DeviceId == 0 &&
                ProductId == null;
        }

        public object Clone()
        {
            return new Sensor()
            {
                VendorId = VendorId,
                DeviceId = DeviceId,
                ProductId = ProductId
            };
        }
    }
}
