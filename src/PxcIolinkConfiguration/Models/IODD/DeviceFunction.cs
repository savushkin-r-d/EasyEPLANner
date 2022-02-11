using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "DeviceFunction")]
    public class DeviceFunction
    {
        [XmlElement(ElementName = "VariableCollection")]
        public VariableCollection VariableCollection { get; set; }
    }
}
