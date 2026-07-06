using Eplan.EplApi.DataModel;
using EplanDevice;
using IO;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EasyEPlanner
{
    /// <summary>
    /// Общие операции сброса и пересчёта привязки клеммы по FUNC_TEXT.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ClampBindingUpdater
    {
        /// <summary>
        /// Сброс каналов устройств по функциональному тексту клеммы.
        /// </summary>
        public static void ResetDeviceChannels(
            Function clampFunction,
            IOModuleInfo moduleInfo,
            string functionalText)
        {
            Dictionary<string, string> devicesComments =
                ProjectConfiguration.GetInstance().GetBindingForResettingChannel(
                    clampFunction,
                    moduleInfo,
                    functionalText);

            foreach (KeyValuePair<string, string> pair in devicesComments)
            {
                IIODevice device = DeviceManager.GetInstance().GetDevice(pair.Key);
                if (device is null)
                {
                    continue;
                }

                string channelName = DeviceBindingHelper
                    .GetChannelNameForIOLinkModuleFromString(pair.Value);
                device.ClearChannel(
                    moduleInfo.AddressSpaceType,
                    pair.Value,
                    channelName);
            }
        }

        /// <summary>
        /// Пересчёт привязки клеммы по текущему FUNC_TEXT.
        /// </summary>
        public static void ReadClampBinding(
            IEplanFunction clampFunction,
            IApiHelper apiHelper)
        {
            var reader = new DeviceBindingReader(
                new ProjectHelper(apiHelper),
                apiHelper);
            reader.ReadModuleClampBinding(clampFunction);
        }

        /// <summary>
        /// Сброс старой привязки, запись нового FUNC_TEXT и пересчёт привязки.
        /// </summary>
        public static void ApplyFunctionalText(
            EplanFunction clampFunction,
            IOModuleInfo moduleInfo,
            string oldFunctionalText,
            string newFunctionalText,
            Action clearModuleBind)
        {
            ResetDeviceChannels(
                clampFunction.Function,
                moduleInfo,
                oldFunctionalText);
            clearModuleBind?.Invoke();
            clampFunction.FunctionalText = newFunctionalText;
            ReadClampBinding(clampFunction, new ApiHelper());
        }
    }
}
