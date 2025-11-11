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
    public class ContainerSerializer
    {
        public static void Serialize(Container container, Stream stream)
        {
            var xDoc = new XDocument();

            xDoc.Add(BuildContainer(container));
            xDoc.Save(stream);
        }

        public static XElement BuildContainer(Container container)
        {
            return new XElement("container",
                new XElement("build", container.Build),
                new XElement("version", container.Version),
                BuildAttributes(container.Attributes),
                BuildComponentList(container.Components)
            );
        }

        public static XElement BuildAttributes(Attributes attributes)
        {
            if (attributes is null)
                return new XElement("attributes");

            return new XElement("attributes",
                new XElement("theme", attributes.Theme),
                new XElement("author", attributes.Author),
                new XElement("organization", attributes.Organization),
                new XElement("telefon", attributes.PhoneNumber),
                new XElement("comment", attributes.Comment),
                new XElement("lastdate", attributes.CurrentDate)
            );
        }


        public static XElement BuildComponentList(List<Component> components)
        {
            if (components?.Any() is null or false)
                return new XElement("components");

            return new XElement("components",
                components.Select(c => c.Name)
            );
        }
    }
}
