using System.Collections.Generic;
using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models.IODD
{
    [XmlRoot(ElementName = "VariableCollection")]
    public class VariableCollection
    {
        [XmlElement(ElementName = "Variable")]
        public List<Variable> Variable { get; set; }
    }
}
