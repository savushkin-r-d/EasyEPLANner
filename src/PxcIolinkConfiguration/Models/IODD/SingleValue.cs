using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "SingleValue")]
    public class SingleValue
    {
        [XmlElement(ElementName = "Name")]
        public Name Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public int Value { get; set; }
    }
}
