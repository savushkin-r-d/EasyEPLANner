using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "IODevice", Namespace = "http://www.io-link.com/IODD/2010/10")]
    public class IODevice
    {
        [XmlElement(ElementName = "ProfileBody")]
        public ProfileBody ProfileBody { get; set; }

        [XmlElement(ElementName = "ExternalTextCollection")]
        public ExternalTextCollection ExternalTextCollection { get; set; }
    }
}
