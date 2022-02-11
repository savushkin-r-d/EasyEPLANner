using EasyEPlanner.PxcIolinkConfiguration.Models.IolConf;
using System.IO;
using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    public interface IXmlSensorSerializer
    {
        /// <summary>
        /// Десериализует одиночное устройство из xml описания
        /// </summary>
        /// <param name="xml">Содержание xml файла</param>
        /// <returns>Объект одиночного устройства</returns>
        LinerecorderSensor Deserialize(string xml);

        /// <summary>
        /// Сериализует мульти-устройство (модуль ввода-вывода + устройства) в
        /// xml файл, готовый для загрузки в IOL-Conf.
        /// </summary>
        /// <param name="template">Шаблон дла сериализации</param>
        /// <param name="path">Путь сохранения (в т.ч имя файла)</param>
        void Serialize(LinerecorderMultiSensor template, string path);
    }

    internal class XmlSensorSerializer : IXmlSensorSerializer
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
