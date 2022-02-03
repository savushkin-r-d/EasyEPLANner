using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models
{
    [XmlRoot(ElementName = "Param")]
	public class Param
	{

		[XmlAttribute(AttributeName = "id")]
		public string id { get; set; }

		[XmlAttribute(AttributeName = "subindex")]
		public object subindex { get; set; }

		[XmlAttribute(AttributeName = "internalValue")]
		public int internalValue { get; set; }

		[XmlAttribute(AttributeName = "name")]
		public string name { get; set; }

		[XmlAttribute(AttributeName = "value")]
		public int value { get; set; }

		[XmlAttribute(AttributeName = "unit")]
		public string unit { get; set; }

		[XmlAttribute(AttributeName = "text")]
		public string text { get; set; }
	}
}
