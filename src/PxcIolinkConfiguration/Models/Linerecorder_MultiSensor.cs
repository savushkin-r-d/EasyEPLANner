using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models
{

	[XmlRoot(ElementName = "Linerecorder_MultiSensor")]
	public class Linerecorder_MultiSensor
	{
		[XmlElement(ElementName = "Version")]
		public string Version { get; set; }

		[XmlElement(ElementName = "Devices")]
		public Devices Devices { get; set; }

		[XmlAttribute(AttributeName = "xmlns")]
		public string xmlns { get; set; }

		[XmlText]
		public string text { get; set; }
	}
}
