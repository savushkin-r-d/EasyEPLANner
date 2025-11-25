using EasyEPlanner.mpk.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace EasyEPlanner.mpk.Saver
{
    [ExcludeFromCodeCoverage]
    public class MpkxSaver
    {
        private Container container;

        public MpkxSaver(Container container)
        {
            this.container = container;
        }

        public void Save(string folderPath)
        {
            var mpkxPath = Path.Combine(folderPath, $"{container.Name}.mpkx");
            var componentsFolder = Path.Combine(folderPath, $"{container.Name}.files");
            var iconsFolder = Path.Combine(componentsFolder, "_Icons");

            if (File.Exists(mpkxPath))
            {
                if (MessageBox.Show(
                        "Контейнер с таким названием уже существует. Перезаписать существующий контейнер",
                        "Сохранение контейнера",
                        MessageBoxButtons.YesNo)
                    == DialogResult.No)
                {
                    return;
                }
                
                if (Directory.Exists(componentsFolder))
                    Directory.Delete(componentsFolder, true);
            }

            using var mpkx = new StreamWriter(mpkxPath, false);
            mpkx.Write(new ContainerSerializer(container).Serialize());

            Directory.CreateDirectory(componentsFolder);
            Directory.CreateDirectory(iconsFolder);
            foreach (var component in container.Components)
            {
                var componentFolder = Path.Combine(componentsFolder, $"{component.Name}.cmp");
                Directory.CreateDirectory(componentFolder);

                using var icon = new FileStream(Path.Combine(iconsFolder, $"{component.Name}.bmp"), FileMode.Create, FileAccess.Write);
                using var script = new StreamWriter(Path.Combine(componentFolder, "component.script"), false);
                using var cmp = new StreamWriter(Path.Combine(componentFolder, "component.xml"), false);

                Properties.Resources.mpkDefaultIcon.Save(icon, System.Drawing.Imaging.ImageFormat.Bmp);

                cmp.Write(new ComponentSerializer(component).Serialize());
            }
        }
    }
}
