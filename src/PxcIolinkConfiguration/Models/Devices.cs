using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration.Models
{
	[XmlRoot(ElementName = "Devices")]
	public class Devices
	{

		[XmlElement(ElementName = "Device")]
		public Device Device { get; set; }
	}
}
