using System.Collections.Generic;
using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models
{
    [XmlRoot(ElementName = "Parameters")]
    public class Parameters
    {
        [XmlElement(ElementName = "Param")]
        public List<Param> Param { get; set; }
    }
}
