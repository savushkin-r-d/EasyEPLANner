using EplanDevice;
using IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.FileSavers.Markdown
{
    /// <summary>
    /// Markdown конструктор - адаптер <see cref="IIOModule"/>
    /// </summary>
    public interface IMarkdownModule
    {
        /// <summary>
        /// HTML-таблица описания модуля
        /// </summary>
        ITextBuilder Table();

        /// <summary>
        /// HTML-таблица с описанием привязки модуля
        /// </summary>
        ITextBuilder BindingTable();

        /// <summary>
        /// Строка с привязкой клеммы для таблицы <see cref="BindingTable"/>
        /// </summary>
        /// <param name="clamp">Номер клеммы</param>
        /// <param name="bind">Информация о привязке к клемме</param>
        ITextBuilder ClampBindingLine(int clamp, IEnumerable<(IODevice device, IEnumerable<IODevice.IIOChannel> channels)> bind);
    }

    /// <summary>
    /// Markdown конструктор - адаптер <see cref="IIOModule"/>
    /// </summary>
    public class MarkdownModule : IMarkdownModule
    {
        /// <summary>
        /// Узел адаптируемого модуля
        /// </summary>
        private readonly IIONode node;

        /// <summary>
        /// Адаптируемый модуль ввода-вывода
        /// </summary>
        private readonly IIOModule module;

        public MarkdownModule(IIONode node, IIOModule module) 
        {
            this.module = module;
            this.node = node;
        }


        public ITextBuilder Table()
        {
            return new TextBuilder().Lines(
                $"<table id=module_{module.Name}>",
                $"    <tr> <td> <code><b>{node.Location}-{module.Name}</b></code> <td>",
                $"        {module.Info.TypeName}<br>",
                $"        {module.Info.Description}<br>",
                $"    <tr> <td colspan=2>",
                BindingTable().WithOffset("    "),
                $"</table>");
        }


        public ITextBuilder BindingTable()
        {
            return new TextBuilder().Lines(
                "<table>",
                from clamp in module.Info.ChannelClamps
                join binding in module.Devices
                    .Select((d, i) => (clamp: i, devices: d)) // indexing clamps
                    .Where(i => i.devices != null)            // exclude empty clamps
                    .Zip(module.DevicesChannels.Where(c => c != null), (idds, channels) => (idds.clamp, idds.devices, channels)) // join devices and channels by clamps
                    .Select(c => (
                        c.clamp,
                        bind: c.devices
                            .Zip(c.channels, (dev, ch) => (device: dev as IODevice, channel: ch)) // join device with channel
                            .GroupBy(g => g.device)                                               // grouping channels by device
                            .Select(g => (
                                device: g.Key,
                                channels: g.Key.Channels.All((from dc in g select dc.channel).Contains) ? 
                                    null : from dc in g select dc.channel))))
                on clamp equals binding.clamp into bindingList from binding in bindingList.DefaultIfEmpty() // Left join clamp to binding
                orderby clamp
                select ClampBindingLine(clamp, binding.bind).WithOffset("    "),
                "</table>");
        }

        public ITextBuilder ClampBindingLine(int clamp, IEnumerable<(IODevice device, IEnumerable<IODevice.IIOChannel> channels)> bind)
        {
            string clampBind = bind?
                .SelectMany(b => (b.channels?.Select(c => $"{b.device.Name}: {c.Name} {c.Comment}") ?? new List<string>() { b.device.Name })
                    .Select(s => $"<a href=#device_{b.device.Name}>{s}</a>{ValveParameter(b.device)}"))
                .Aggregate((a, b) => $"{a} <br> {b}")
                ?? "-";

            return new TextBuilder(string.Format("<tr> <td> {0} <td> {1}", clamp, clampBind));
        }

        public string ValveParameter(IODevice device)
        {
            if (device.RuntimeParameters.TryGetValue(IODevice.RuntimeParameter.R_AS_NUMBER, out var r_as_n))
                return $" (AS-{r_as_n})"; 

            if (device.RuntimeParameters.TryGetValue(IODevice.RuntimeParameter.R_VTUG_NUMBER, out var r_vtug_n))
                return $" (Y-{r_vtug_n})";

            return string.Empty;
        }
    }
}
