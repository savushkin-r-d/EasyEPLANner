using EasyEPlanner.mpk.Model;
using EasyEPlanner.mpk.ModelBuilder;
using EasyEPlanner.mpk.ViewModel;
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
using TechObject;

namespace EasyEPlanner.mpk.Saver
{
    [ExcludeFromCodeCoverage]
    public class MpkxSaver()
    {
        public void Save(IMpkSaverContext context)
        {
            var container = new TechObjectMpkBuilder(TechObjectManager.GetInstance(), context.MainContainerName).Build();

            var mpkxPath = Path.Combine(context.MpkDirectory, $"{container.Name}.mpkx");
            var componentsFolder = Path.Combine(context.MpkDirectory, $"{container.Name}.files");
            var iconsFolder = Path.Combine(componentsFolder, "_Icons");

            if (Directory.Exists(componentsFolder))
                Directory.Delete(componentsFolder, true);

            Directory.CreateDirectory(componentsFolder);
            Directory.CreateDirectory(iconsFolder);

            using var mpkx = new StreamWriter(mpkxPath, false);
            mpkx.Write(new ContainerSerializer(container).Serialize());

            foreach (var component in container.Components)
            {
                var componentFolder = Path.Combine(componentsFolder, $"{component.Name}.cmp");
                Directory.CreateDirectory(componentFolder);

                using var icon = new FileStream(Path.Combine(iconsFolder, $"{component.Name}.bmp"), FileMode.Create, FileAccess.Write);
                _ = new StreamWriter(Path.Combine(componentFolder, "component.script"), false);
                using var cmp = new StreamWriter(Path.Combine(componentFolder, "component.xml"), false);

                Properties.Resources.mpkDefaultIcon.Save(icon, System.Drawing.Imaging.ImageFormat.Bmp);

                cmp.Write(new ComponentSerializer(component).Serialize());
            }
        }
    }
}
