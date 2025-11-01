using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EasyEPlanner.FileSavers.XML
{
    public static class XmlExtensions
    {
        /// <summary>
        /// Добавить дочерний элемент
        /// </summary>
        /// <param name="xmlElement">Родительский элемент</param>
        /// <param name="prefix">префикс тега</param>
        /// <param name="attribute">название тега</param>
        /// <param name="ns">пространство имен</param>
        /// <param name="value">значение InnerText</param>
        /// <returns>Созданный тег</returns>
        public static XmlElement AddElement(this XmlElement xmlElement, string prefix, string attribute, string ns, string value)
        {
            var channelElm = xmlElement.OwnerDocument.CreateElement(prefix, attribute, ns);
            channelElm.InnerText = value;
            xmlElement.AppendChild(channelElm);

            return channelElm;
        }

        /// <summary>
        /// Добавить дочерний элемент без значения
        /// </summary>
        /// <param name="xmlElement">Родительский элемент</param>
        /// <param name="prefix">префикс тега</param>
        /// <param name="attribute">название тега</param>
        /// <param name="ns">пространство имен</param>
        /// <returns>Созданный тег</returns>
        public static XmlElement AddElement(this XmlElement xmlElement, string prefix, string attribute, string ns)
        {
            var channelElm = xmlElement.OwnerDocument.CreateElement(prefix, attribute, ns);
            xmlElement.AppendChild(channelElm);

            return channelElm;
        }
    }
}
