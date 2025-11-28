using EasyEPlanner;
using EplanDevice;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public class Clamp(IModule owner, int clamp) : IClamp, IHasIcon, IHasDescriptionIcon, IDeletable, IToolTip, IEditable
    {
        public string Name => clamp.ToString();

        string IToolTip.Description => Description.Replace(" || ", "\n");

        public string Description => string.Join(" || ",
            Module.GetClampBinding(clamp)?
                .Select(g => ChannelBinding(g.Item1, g.Item2)) ?? [ Value ]);

        /// <summary>
        /// Генерация текста привязки канала устройства к клемме
        /// </summary>
        /// <param name="device">Устройство</param>
        /// <param name="channel">Канал</param>
        /// <returns>Текст привязки канала</returns>
        private string ChannelBinding(IIODevice device, IODevice.IIOChannel channel)
        {
            var type = (Module.Info.AddressSpaceType is
                IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI or
                IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI or
                IOModuleInfo.ADDRESS_SPACE_TYPE.DODI) ? $":{channel.Name}" : "";

            var ioLinkSize = "";
            if (device.IOLinkProperties.GetMaxIOLinkSize() > 0)
            {
                if (channel.Name is "AI" or "DI")
                    ioLinkSize = $"({device.IOLinkProperties.SizeIn * 16})";

                if (channel.Name is "AO" or "DO")
                    ioLinkSize = $"({device.IOLinkProperties.SizeOut * 16})";
            }


            var comment = (string.IsNullOrEmpty(channel.Comment)) ? "" : $":{channel.Comment}";

            return $"{device.Name}{type}{ioLinkSize}{comment}"; 
        }

        public IEplanFunction ClampFunction 
            => Module.ClampFunctions.TryGetValue(clamp, out var function)? function : null;

        public IIOModule Module => owner.IOModule;

        public IIONode Node => owner.IONode;

        Icon IHasIcon.Icon => Icon.Clamp;

        Icon IHasDescriptionIcon.Icon => Bound? Icon.Cable : Icon.None;

        public string Value => ClampFunction?.FunctionalText;

        public bool Bound => Module.Devices[clamp]?.Any() ?? false;

        public void Reset()
        {
            Module.ClearBind(clamp);
        }

        public void Delete()
        {
            foreach (var (dev, channel) in Module.GetClampBinding(clamp))
            {
                dev.ClearChannel(Module.Info.AddressSpaceType, channel.Comment, channel.Name);
            }
            ClampFunction?.FunctionalText = "Резерв";
            Reset();
        }

        public bool SetValue(string value)
        {
            if (ClampFunction is null || ClampFunction.FunctionalText == value)
                return false;

            ClampFunction.FunctionalText = value;
            Reset();

            return true;
        }
    }
}
