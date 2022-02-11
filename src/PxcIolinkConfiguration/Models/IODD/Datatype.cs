using System.Collections.Generic;
using System.Xml.Serialization;
using EasyEPlanner.PxcIolinkConfiguration.Models.IODD.Datatypes;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlInclude(typeof(ArrayT))]
    [XmlInclude(typeof(BooleanT))]
    [XmlInclude(typeof(Float32T))]
    [XmlInclude(typeof(IntegerT))]
    [XmlInclude(typeof(OctetStringT))]
    [XmlInclude(typeof(OctetStringT))]
    [XmlInclude(typeof(StringT))]
    [XmlInclude(typeof(TimeSpanT))]
    [XmlInclude(typeof(TimeT))]
    [XmlInclude(typeof(UIntegerT))]
    [XmlRoot(ElementName = "Datatype", Namespace = "http://www.io-link.com/IODD/2010/10")]
    public class Datatype
    {
        [XmlElement(ElementName = "ValueRange")]
        public ValueRange ValueRange { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "bitLength")]
        public int BitLength { get; set; }

        [XmlElement(ElementName = "SingleValue")]
        public List<SingleValue> SingleValue { get; set; }

        [XmlElement(ElementName = "RecordItem")]
        public List<RecordItem> RecordItem { get; set; }

        [XmlAttribute(AttributeName = "subindexAccessSupported")]
        public bool SubindexAccessSupported { get; set; }
    }
}
