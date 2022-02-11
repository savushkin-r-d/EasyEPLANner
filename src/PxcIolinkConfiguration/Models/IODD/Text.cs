using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "Text")]
    public class Text
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }
}
