using EasyEPlanner.mpk.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EasyEPlanner.mpk.Saver
{
    public class ComponentSerializer(IComponent component) : IXmlSerializer
    { 
        public string Serialize()
        {
            var xDoc = new XDocument();

            xDoc.Add(BuildComponent(component));
            return xDoc.ToString();
        }

        private static XElement BuildComponent(IComponent component)
        {
            return new XElement("component",
                new XElement("imageslist",
                    new XElement("width", 0),
                    new XElement("heigth", 0),
                    new XElement("startx", 0),
                    new XElement("starty", 0),
                    new XElement("wallpaper", false.ToString()),
                    new XElement("animation", false.ToString()),
                    new XElement("animationstart", 1),
                    new XElement("animbationend", 1),
                    new XElement("animationspeed", 1)
                ),
                BuildPropertiesList(component.Properties),
                BuildMassagesList(component.Messages)
            );
        }


        private static XElement BuildPropertiesList(List<IProperty> properties)
        {
            if (properties?.Any() is null or false)
                return null;

            return new XElement("propertieslist",
                properties.Select(BuildProperty)
            );
        }

        private static XElement BuildProperty(IProperty property)
        {
            return new XElement("property",
                new XElement("name", property.Name),
                new XElement("caption", property.Caption),
                new XElement("visible", property.Visible.ToString()),
                new XElement("report", property.Report.ToString()),
                new XElement("saved", property.Saved.ToString()),
                new XElement("tagname", property.TagName),
                new XElement("propmodel", (int)property.PropModel),
                new XElement("proptype", (int)property.PropType),
                new XElement("value", property.Value),
                new XElement("channelid", property.ChannelId),
                new XElement("priority", property.Priority)
            );
        }

        private static XElement BuildMassagesList(List<IMessage> messages)
        {
            if (messages?.Any() is null or false)
                return null;

            return new XElement("propertieslist",
                messages.Select(BuildMessage)
            );
        }

        private static XElement BuildMessage(IMessage message)
        {
            return new XElement("property",
                new XElement("name", message.Name),
                new XElement("caption", message.Caption),
                new XElement("visible", message.Visible.ToString()),
                new XElement("report", message.Report.ToString()),
                new XElement("model", (int)message.Type),
                new XElement("priority", message.Priority)
            );
        }
    }
}
