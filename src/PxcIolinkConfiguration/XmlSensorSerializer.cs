using EasyEPlanner.PxcIolinkConfiguration.Models;
using System.IO;
using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    public class XmlSensorSerializer : IXmlSensorSerializer
    {
        public LinerecorderSensor Deserialize(string xml)
        {
            var serializer = new XmlSerializer(typeof(LinerecorderSensor));
            using (var reader = new StringReader(xml))
            {
                var info = (LinerecorderSensor)serializer.Deserialize(reader);
                return info;
            }
        }

        public void Serialize(LinerecorderMultiSensor template, string path)
        {
            var serializer = new XmlSerializer(typeof(LinerecorderMultiSensor));
            using (var fs = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(fs, template);
            }
        }
    }
}
