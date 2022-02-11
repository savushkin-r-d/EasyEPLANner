using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "ValueRange")]
    public class ValueRange
    {
        [XmlAttribute(AttributeName = "lowerValue")]
        public int LowerValue { get; set; }

        [XmlAttribute(AttributeName = "upperValue")]
        public int UpperValue { get; set; }
    }
}
