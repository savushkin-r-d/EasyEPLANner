using EasyEPlanner.PxcIolinkConfiguration.Models;
using EplanDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.Markdown
{
    /// <summary>
    /// Markdown конструктор - адаптер <see cref="IDeviceManager"/>
    /// </summary>
    public interface IMarkdownDeviceManger
    {
        /// <summary>
        /// Текст списка устройств
        /// </summary>
        ITextBuilder TreeOfContent();

        /// <summary>
        /// Текст описания устройств
        /// </summary>
        ITextBuilder Description();
    }


    /// <summary>
    /// Markdown конструктор - адаптер <see cref="IDeviceManager"/>
    /// </summary>
    public class MarkdownDeviceManager : IMarkdownDeviceManger
    {
        /// <summary>
        /// Сгруппированный список md-адаптеров устройств
        /// </summary>
        private List<(string Location, List<(DeviceType Type, List<IMarkdownDevice> Devices)> Types)> groupedDevices;

        /// <summary>
        /// Название раздела со списком устройств
        /// </summary>
        public static readonly string TreeOfContentName = "Список устройств";

        /// <summary>
        /// Название раздела с описанием устройств
        /// </summary>
        public static readonly string DescriptionName = "Описание устройств";

        /// <summary>
        /// markdown-ссылка на раздел со списком устройств
        /// </summary>
        public static string TreeOfContentLink => TreeOfContentName.ToLower().Split(' ').Aggregate((a, b) => $"{a}-{b}");

        /// <summary>
        /// markdown-ссылка на раздел с описанием устройств
        /// </summary>
        public static string DescriptionLink => DescriptionName.ToLower().Split(' ').Aggregate((a, b) => $"{a}-{b}");


        public MarkdownDeviceManager(IDeviceManager deviceManager)
        {
            groupedDevices = (
                from device in deviceManager.Devices
                orderby device.ObjectName, device.ObjectNumber
                group device by device.ObjectName + (device.ObjectNumber == 0 ? "-" : $"{device.ObjectNumber}")
                into objGroup
                select (
                    Location: objGroup.Key,
                    Types: (
                        from device in objGroup
                        group device by device.DeviceType
                        into typeGroup
                        select (
                            Type: typeGroup.Key,
                            Devices: (
                                from device in typeGroup
                                orderby device.ObjectName, device.ObjectNumber, device.DeviceNumber
                                select new MarkdownDevice(device) as IMarkdownDevice)
                                .ToList()))
                        .ToList()))
                .ToList();
        }


        public ITextBuilder TreeOfContent()
        {
            return new TextBuilder().Lines(
                $"",
                $"## {TreeOfContentName}",
                $"",
                from objectGroup in groupedDevices
                select new TextBuilder().Lines(
                    $"<details>",
                    $"    <summary>{objectGroup.Location}</summary>",
                    $"    <ul>",
                    from typeGroup in objectGroup.Types
                    select new TextBuilder().WithOffset("        ").Lines(
                        $"<details>",
                        $"    <summary>{typeGroup.Type}</summary>",
                        $"    <ul>",
                        from device in typeGroup.Devices
                        select $"        <li> {device.Link}",
                        $"    </ul>",
                        $"</details>"),
                    $"    </ul>",
                    $"</details>"));
        }


        public ITextBuilder Description()
        {
            return new TextBuilder().Lines(
                $"",
                $"## {DescriptionName}",
                $"",
                (from objectGroup in groupedDevices
                 from typeGroup in objectGroup.Types
                 from device in typeGroup.Devices
                 select new TextBuilder().Lines(
                     "",
                     device.Table(),
                     ""))
                 .Aggregate((a, b) => a.Lines("---", b)));
        }
    }
}
