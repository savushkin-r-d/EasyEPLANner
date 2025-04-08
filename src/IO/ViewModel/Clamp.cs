using EplanDevice;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public class Clamp : IClamp, IHasIcon, IHasDescriptionIcon, IDeletable, IToolTip, IEditable
    {
        private IModule owner;

        private int clamp;

        public Clamp(IModule owner, int clamp)
        {
            this.owner = owner;
            this.clamp = clamp;
        }

        public string Name => clamp.ToString();

        string IToolTip.Description => Description.Replace(" || ", "\n");

        public string Description => string.Join(" || ",
            Module.Devices[clamp]?.Zip(Module.DevicesChannels[clamp], ChannelBinding) ?? []);

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

        public IEplanFunction ClampFunction => Module.ClampFunctions[clamp];

        public IIOModule Module => owner.IOModule;

        public IIONode Node => owner.IONode;

        Icon IHasIcon.Icon => Icon.Clamp;

        Icon IHasDescriptionIcon.Icon => (Module.Devices[clamp]?.Any() ?? false)? Icon.Cable : Icon.None;

        public string Value => ClampFunction.FunctionalText;

        public void Reset()
        {
            Module.ClearBind(clamp);
        }

        public void Delete()
        {
            ClampFunction.FunctionalText = "Резерв";

            Reset();
        }

        public bool SetValue(string value)
        {
            ClampFunction.FunctionalText = value;

            return true;
        }
    }
}
