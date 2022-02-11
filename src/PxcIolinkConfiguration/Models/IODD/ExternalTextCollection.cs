using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "ExternalTextCollection")]
    public class ExternalTextCollection
    {
        [XmlElement(ElementName = "PrimaryLanguage")]
        public PrimaryLanguage PrimaryLanguage { get; set; }
    }
}
