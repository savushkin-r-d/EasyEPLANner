using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "Variable")]
    public class Variable
    {
        [XmlElement(ElementName = "Datatype")]
        public Datatype Datatype { get; set; }

        [XmlElement(ElementName = "Name")]
        public Name Name { get; set; }

        [XmlElement(ElementName = "Description")]
        public Description Description { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "defaultValue")]
        public int DefaultValue { get; set; }
    }
}
