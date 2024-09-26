using EplanDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.Markdown
{
    /// <summary>
    /// Markdown конструктор - адаптер <see cref="IODevice"/>
    /// </summary>
    public interface IMarkdownDevice
    {
        /// <summary>
        /// HTML-таблица описания устройства
        /// </summary>
        ITextBuilder Table();

        /// <summary>
        /// HTML-список каналов ввода-вывода устройства
        /// </summary>
        ITextBuilder ChannelsList();
        
        /// <summary>
        /// HTML-таблица параметров устройства
        /// </summary>
        ITextBuilder ParametersTable();

        /// <summary>
        /// HTML-таблица рабочих параметров устройства
        /// </summary>
        ITextBuilder RuntimeParametersTable();

        /// <summary>
        /// HTML-таблица свойств устройства
        /// </summary>
        ITextBuilder PropertiesTable();
        
        /// <summary>
        /// HTML-ссылка на устройство с его названием
        /// </summary>
        string Link { get; }
    }

    /// <summary>
    /// Markdown конструктор - адаптер <see cref="IODevice"/>
    /// </summary>
    public class MarkdownDevice : IMarkdownDevice
    {
        /// <summary>
        /// Адаптируемое устройство
        /// </summary>
        private IODevice device;

        public MarkdownDevice(IODevice device)
        {
            this.device = device;
        }

        public string Link => $"<a href=#device_{device.Name}>{device.Name}</a>";

        public ITextBuilder Table()
        {
            return new TextBuilder().Lines(
                $"<table id=device_{device.Name}>",
                $"    <tr> <td align=center> <b><code>{device.Name}</code></b> - <i>{device.GetDeviceSubTypeStr(device.DeviceType, device.DeviceSubType)}</i>",
                $"    <tr> <td>",
                $"        <b>Описание:</b> <i>{device.Description}</i> <br>",
                $"        <b>Изделие:</b> <i>{device.ArticleName}</i> <br>",
                ChannelsList()?.WithOffset("    "),
                ParametersTable()?.WithOffset("    "),
                RuntimeParametersTable()?.WithOffset("    "),
                PropertiesTable()?.WithOffset("    "),
                $"</table>");
        }

        public ITextBuilder ChannelsList()
        {
            if (device.Channels.Count == 0)
                return null;

            return new TextBuilder().Lines(
                $"<tr> <td>",
                $"<ul>",
                from channel in device.Channels
                select new TextBuilder().WithOffset("    ").Lines(
                    $"<li> <code>{channel.Name}</code> {channel.Comment} (<a href=#module_A{channel.FullModule}>-A{channel.FullModule}:{channel.PhysicalClamp}</a>)"),
                $"</ul>");
        }

        public ITextBuilder ParametersTable()
        {
            if (device.Parameters.Count == 0)
                return null;

            return new TextBuilder().Lines(
                $"<tr> <td>",
                $"<table>",
                $"    <tr> <th> Параметр <th> Описание <th> Значение",
                from parameter in device.Parameters
                select new TextBuilder().WithOffset("    ").Lines(
                     $"<tr> <td> {parameter.Key.Name} <td> {parameter.Key.Description} <td> {IODevice.Parameter.GetFormatValue(parameter.Key, parameter.Value ?? "", device)}"),
                $"</table>");
        }

        public ITextBuilder RuntimeParametersTable()
        {
            if (device.RuntimeParameters.Count == 0)
                return null;

            return new TextBuilder().Lines(
                $"<tr> <td>",
                $"<table>",
                $"    <tr> <th> Рабочий параметр <th> Значение",
                from parameter in device.RuntimeParameters
                select new TextBuilder().WithOffset("    ").Lines(
                    $"<tr> <td> {parameter.Key} <td> {parameter.Value}"),
                $"</table>");
        }

        public ITextBuilder PropertiesTable()
        {
            if (device.Properties.Count == 0)
                return null;

            return new TextBuilder().Lines(
                $"<tr> <td>",
                $"<table>",
                $"    <tr> <th> Свойство <th> Значение",
                from property in device.Properties
                select new TextBuilder().WithOffset("    ").Lines(
                    $"<tr> <td> {property.Key} <td> {property.Value}"),
                $"</table>");
        }
    }
}
