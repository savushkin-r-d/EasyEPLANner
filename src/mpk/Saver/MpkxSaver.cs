using EasyEPlanner.mpk.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace EasyEPlanner.mpk.Saver
{
    public class MpkxSaver
    {
        private Container container;

        public MpkxSaver(Container container) 
        {
            this.container = container;
        }

        public void Save(string folderPath)
        {
            using var mpkx = File.Open(Path.Combine(folderPath, $"{container.Name}.mpkx"), FileMode.Create);
            ContainerSerializer.Serialize(container, mpkx);

             

            var componentsFolder = Path.Combine(folderPath, $"{container.Name}.files");
            Directory.CreateDirectory(componentsFolder);
            foreach (var component in container.Components)
            {
                Directory.CreateDirectory(Path.Combine(componentsFolder, $"{component.Name}.cmp"));
                using var script = File.Open(Path.Combine(componentsFolder, $"{component.Name}.cmp", "component.script"), FileMode.Create);
                using var cmp = File.Open(Path.Combine(componentsFolder, $"{component.Name}.cmp", "component.xml"), FileMode.Create);
                ComponentSerializer.Serialize(component, cmp);
            }
        }
    }
}
