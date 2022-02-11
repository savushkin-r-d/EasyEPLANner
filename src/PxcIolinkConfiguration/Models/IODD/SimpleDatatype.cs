using System.Collections.Generic;
using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "SimpleDatatype")]
    public class SimpleDatatype
    {
        [XmlElement(ElementName = "ValueRange")]
        public ValueRange ValueRange { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "bitLength")]
        public int BitLength { get; set; }

        [XmlElement(ElementName = "SingleValue")]
        public List<SingleValue> SingleValue { get; set; }
    }
}
