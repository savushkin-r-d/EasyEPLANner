using EasyEPlanner.PxcIolinkConfiguration.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    public interface ISensorSerializer
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

    internal class SensorSerializer : ISensorSerializer
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
