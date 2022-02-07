using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models
{
    [XmlRoot(ElementName = "Linerecorder_Sensor", Namespace = "http://www.ifm.com/datalink/LinerecorderSensor4")]
    public class LinerecorderSensor
    {
        [XmlElement(ElementName = "Version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "Sensor")]
        public Sensor Sensor { get; set; }

        [XmlElement(ElementName = "Parameters")]
        public Parameters Parameters { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }

        [XmlText]
        public string Text { get; set; }

        public LinerecorderSensor()
        {
            Version = string.Empty;
            Sensor = new Sensor();
            Parameters = new Parameters();
            Xmlns = string.Empty;
            Text = string.Empty;
        }
    }
}