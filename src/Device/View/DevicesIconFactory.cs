using EasyEPlanner.Devices.ViewModel.ViewInterface;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace EasyEPlanner.Devices.View
{
    [ExcludeFromCodeCoverage]
    internal static class DevicesIconFactory
    {
        private static readonly IReadOnlyDictionary<DevicesIcon, Bitmap> Icons =
            new Dictionary<DevicesIcon, Bitmap>
            {
                [DevicesIcon.Root] = Properties.Resources.devicesTreeRoot,
                [DevicesIcon.Type] = Properties.Resources.devicesTreeType,
                [DevicesIcon.Object] = Properties.Resources.devicesTreeObject,
                [DevicesIcon.Device] = Properties.Resources.devicesTreeDevice,
                [DevicesIcon.Data] = Properties.Resources.devicesTreeData,
                [DevicesIcon.Parameters] = Properties.Resources.devicesTreeParameters,
                [DevicesIcon.RuntimeParameters] = Properties.Resources.devicesTreeRuntimeParameters,
                [DevicesIcon.Properties] = Properties.Resources.devicesTreeProperties,
                [DevicesIcon.Channels] = Properties.Resources.io_plc_cable,
                [DevicesIcon.Channel] = Properties.Resources.io_plc_cable,
                [DevicesIcon.Clamp] = Properties.Resources.io_plc_clamp,
                [DevicesIcon.GoToFas] = Properties.Resources.go_to_fas,
            };

        public static void Populate(ImageList imageList)
        {
            imageList.Images.Clear();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(16, 16);

            foreach (DevicesIcon icon in System.Enum.GetValues(typeof(DevicesIcon)))
            {
                if (icon is DevicesIcon.None || !Icons.TryGetValue(icon, out var bitmap))
                    continue;

                imageList.Images.Add(icon.ToString(), (Bitmap)bitmap.Clone());
            }
        }
    }
}
