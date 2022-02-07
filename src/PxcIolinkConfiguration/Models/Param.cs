using System;
using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models
{
    [XmlRoot(ElementName = "Param")]
    public class Param : ICloneable
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "subindex")]
        public string Subindex { get; set; }

        [XmlAttribute(AttributeName = "internalValue")]
        public string InternalValue { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }

        [XmlAttribute(AttributeName = "unit")]
        public string Unit { get; set; }

        [XmlAttribute(AttributeName = "text")]
        public string Text { get; set; }

        public object Clone()
        {
            return new Param()
            {
                Id = Id,
                Subindex = Subindex,
                InternalValue = InternalValue,
                Name = Name,
                Value = Value,
                Unit = Unit,
                Text = Text
            };
        }
    }
}
