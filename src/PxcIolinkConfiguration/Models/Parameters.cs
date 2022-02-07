using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models
{
    [XmlRoot(ElementName = "Parameters")]
    public class Parameters : ICloneable
    {
        [XmlElement(ElementName = "Param")]
        public List<Param> Param { get; set; }

        public Parameters()
        {
            Param = new List<Param>();
        }

        public bool IsEmpty()
        {
            return Param.Count == 0;
        }

        public object Clone()
        {
            var clone = new Parameters();
            var newList = new List<Param>();
            foreach(var parameter in Param)
            {
                newList.Add(parameter.Clone() as Param);
            }

            clone.Param = newList;
            return clone;
        }
    }
}
