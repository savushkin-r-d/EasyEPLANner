using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models
{
    [XmlRoot(ElementName = "Device")]
    public class Device
    {
        [XmlElement(ElementName = "Sensor")]
        public Sensor Sensor { get; set; }

        [XmlElement(ElementName = "Port")]
        public int Port { get; set; }

        [XmlElement(ElementName = "Parameters")]
        public Parameters Parameters { get; set; }

        [XmlElement(ElementName = "Devices")]
        public Devices Devices { get; set; }

        public Device()
        {
            Sensor = new Sensor();
            Parameters = new Parameters();
            Devices = new Devices();
            Port = 0;
        }

        public bool IsEmpty()
        {
            return Sensor.IsEmpty() &&
                Port == 0 &&
                Parameters.IsEmpty() &&
                Devices.IsEmpty();
        }
    }
}
