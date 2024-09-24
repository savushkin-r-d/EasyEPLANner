using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ProjectImportICP
{
    /// <summary>
    /// Импортер привязки устройств к модулям
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BindingImporter
    {
        private readonly Dictionary<int, List<ImportModule>> modules;
        private readonly List<ImportDevice> devices;


        public BindingImporter(Dictionary<int, List<ImportModule>> modules, List<ImportDevice> devices)
        {
            this.modules = modules;
            this.devices = devices;
        }

        /// <summary>
        /// Настроить привязку импортированных устройств к модулям
        /// </summary>
        public void Bind()
        {
            Logs.AddMessage("\n\n");
            Logs.AddMessage("Настройка привязки устройств к модулям:\n");

            foreach (var device in devices)
            {
                foreach (var channel in device.Channels)
                {
                    var node = modules[channel.node + 1];

                    var offset = channel.offset;

                    foreach (var module in node.Where(m => m.ModuleInfo.AddressSpaceType.ToString().Contains(channel.type)))
                    {
                        if (offset - module.AddressSpace(channel.type) >= 0)
                        {
                            offset -= module.AddressSpace(channel.type);
                            continue;
                        }

                        if (module.Clamps.TryGetValue(offset, out var clamp))
                        {
                            clamp.LockObject();

                            clamp.Properties.FUNC_TEXT = $"+{device.Object}-{device.Type}{device.Number}\r\n" +
                                $"{device.Description}\r\n" +
                                $"{channel.comment}";

                            Logs.AddMessage($"\tКанал {device.Object}{device.Type}{device.Number}:{channel.type}{channel.comment}" +
                                $" привязан к клемме {module.Function.VisibleName}:{clamp.Properties.FUNC_ADDITIONALIDENTIFYINGNAMEPART}\n");
                        }
                        else
                            Logs.AddMessage($"\tНе удалось привязать канал {device.Object}{device.Type}{device.Number}:{channel.type}{channel.comment}\n");

                        break;
                    }
                }
            }
        }
    }
}
