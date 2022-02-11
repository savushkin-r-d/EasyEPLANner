using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "RecordItem")]
    public class RecordItem
    {
        [XmlElement(ElementName = "SimpleDatatype")]
        public SimpleDatatype SimpleDatatype { get; set; }

        [XmlElement(ElementName = "Name")]
        public Name Name { get; set; }

        [XmlElement(ElementName = "Description")]
        public Description Description { get; set; }

        [XmlAttribute(AttributeName = "bitOffset")]
        public int BitOffset { get; set; }

        [XmlAttribute(AttributeName = "subindex")]
        public int Subindex { get; set; }
    }
}
