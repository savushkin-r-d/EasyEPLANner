using EplanDevice;
using IO;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IO.ViewModel
{
    public static class ClampBindingErrorChecker
    {
        public static bool HasBindingError(
            IIOModule module,
            int clamp,
            IEplanFunction clampFunction,
            IDeviceManager deviceManager = null) =>
            HasUndefinedDevice(module, clamp, clampFunction, deviceManager) ||
            HasInvalidChannelBinding(module, clamp, clampFunction, deviceManager);

        public static string GetErrorMessage(
            IIOModule module,
            int clamp,
            IEplanFunction clampFunction,
            IDeviceManager deviceManager = null)
        {
            if (HasUndefinedDevice(module, clamp, clampFunction, deviceManager))
                return "Ошибка: неопределённое устройство";

            return "Ошибка: неверная привязка канала";
        }

        public static bool HasUndefinedDevice(
            IIOModule module,
            int clamp,
            IEplanFunction clampFunction,
            IDeviceManager deviceManager = null)
        {
            if (module.Devices[clamp] != null)
            {
                foreach (var device in module.Devices[clamp])
                {
                    if (device?.Description == CommonConst.Cap)
                        return true;
                }
            }

            foreach (var device in GetDevicesFromFunctionalText(
                clampFunction, deviceManager))
            {
                if (device?.Description == CommonConst.Cap)
                    return true;
            }

            return false;
        }

        public static bool HasInvalidChannelBinding(
            IIOModule module,
            int clamp,
            IEplanFunction clampFunction,
            IDeviceManager deviceManager = null)
        {
            foreach (var (device, channel) in module.GetClampBinding(clamp) ?? [])
            {
                if (device?.Description == CommonConst.Cap)
                    continue;

                if (channel is null)
                    return true;

                if (!IsChannelAllowedOnClamp(module.Info, clamp, channel.Name))
                    return true;
            }

            foreach (var device in GetDevicesFromFunctionalText(
                clampFunction, deviceManager))
            {
                if (device?.Description == CommonConst.Cap)
                    continue;

                if (module.Devices[clamp]?.Contains(device) != true)
                    return true;
            }

            return false;
        }

        private static IEnumerable<IODevice> GetDevicesFromFunctionalText(
            IEplanFunction clampFunction,
            IDeviceManager deviceManager)
        {
            var functionalText = clampFunction?.FunctionalText;
            if (string.IsNullOrEmpty(functionalText) ||
                functionalText.Contains(CommonConst.Reserve))
            {
                yield break;
            }

            deviceManager ??= DeviceManager.GetInstance();

            var matches = Regex.Matches(
                functionalText,
                DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN,
                RegexOptions.None,
                TimeSpan.FromMilliseconds(100));

            foreach (Match match in matches.Cast<Match>())
            {
                yield return deviceManager.GetDevice(match.Groups["name"].Value);
            }
        }

        private static bool IsChannelAllowedOnClamp(
            IIOModuleInfo info,
            int clamp,
            string channelName)
        {
            var allowedTypes = GetAllowedChannelTypes(info, clamp);
            if (!allowedTypes.Any())
                return true;

            return allowedTypes.Contains(channelName);
        }

        private static IEnumerable<string> GetAllowedChannelTypes(
            IIOModuleInfo info,
            int clamp)
        {
            switch (info.AddressSpaceType)
            {
                case IOModuleInfo.ADDRESS_SPACE_TYPE.DO:
                    return [IODevice.IOChannel.DO];

                case IOModuleInfo.ADDRESS_SPACE_TYPE.DI:
                    return [IODevice.IOChannel.DI];

                case IOModuleInfo.ADDRESS_SPACE_TYPE.AO:
                    return [IODevice.IOChannel.AO];

                case IOModuleInfo.ADDRESS_SPACE_TYPE.AI:
                    return [IODevice.IOChannel.AI];

                case IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI:
                    return GetMixedModuleAllowedTypes(
                        info, clamp,
                        input: IODevice.IOChannel.AI,
                        output: IODevice.IOChannel.AO);

                case IOModuleInfo.ADDRESS_SPACE_TYPE.DODI:
                    return GetMixedModuleAllowedTypes(
                        info, clamp,
                        input: IODevice.IOChannel.DI,
                        output: IODevice.IOChannel.DO);

                case IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI:
                    return GetAoaIdoDiAllowedTypes(info, clamp);

                default:
                    return [];
            }
        }

        private static IEnumerable<string> GetMixedModuleAllowedTypes(
            IIOModuleInfo info,
            int clamp,
            string input,
            string output)
        {
            bool allowsIn = IsClampInputChannel(info, clamp);
            bool allowsOut = IsClampOutputChannel(info, clamp);

            if (allowsIn)
                yield return input;

            if (allowsOut)
                yield return output;
        }

        private static IEnumerable<string> GetAoaIdoDiAllowedTypes(
            IIOModuleInfo info,
            int clamp)
        {
            bool allowsIn = IsClampInputChannel(info, clamp);
            bool allowsOut = IsClampOutputChannel(info, clamp);

            if (allowsIn)
            {
                yield return IODevice.IOChannel.DI;
                yield return IODevice.IOChannel.AI;
            }

            if (allowsOut)
            {
                yield return IODevice.IOChannel.DO;
                yield return IODevice.IOChannel.AO;
            }
        }

        private static bool IsClampInputChannel(IIOModuleInfo info, int clamp) =>
            clamp >= 0 &&
            clamp < info.ChannelAddressesIn?.Length &&
            info.ChannelAddressesIn[clamp] >= 0;

        private static bool IsClampOutputChannel(IIOModuleInfo info, int clamp) =>
            clamp >= 0 &&
            clamp < info.ChannelAddressesOut?.Length &&
            info.ChannelAddressesOut[clamp] >= 0;
    }
}
