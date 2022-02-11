using System.Collections.Generic;
using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "PrimaryLanguage")]
    public class PrimaryLanguage
    {
        [XmlElement(ElementName = "Text")]
        public List<Text> Text { get; set; }
    }
}
