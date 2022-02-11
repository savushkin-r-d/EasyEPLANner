using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "Name")]
    public class Name
    {
        [XmlAttribute(AttributeName = "textId")]
        public string TextId { get; set; }
    }
}
