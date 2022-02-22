using EasyEPlanner.PxcIolinkConfiguration.Models;

namespace EasyEPlanner.PxcIolinkConfiguration.Interfaces
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
}
