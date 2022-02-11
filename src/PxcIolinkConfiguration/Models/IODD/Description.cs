using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "Description")]
    public class Description
    {
        [XmlAttribute(AttributeName = "textId")]
        public string TextId { get; set; }
    }
}
