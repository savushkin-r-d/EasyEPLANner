using EplanDevice;
using Eplan.EplApi.DataModel;
using System;
using System.Text.RegularExpressions;

namespace StaticHelper
{
    class DeviceBindingHelper
    {
        /// <summary>
        /// Возвращает имя канала (IO-Link, DI, DO) из строки для IO-Link
        /// модуля.
        /// </summary>
        /// <param name="source">Строка для поиска</param>
        /// <returns></returns>
        public static string GetChannelNameForIOLinkModuleFromString(
            string source)
        {
            const string IOLink = "IO-Link";
            const string DI = "DI";
            const string DO = "DO";

            if (source.Contains(DI) &&
                !source.Contains(IOLink) &&
                !source.Contains(DO))
            {
                return DI;
            }

            if (source.Contains(DO) &&
                !source.Contains(IOLink) &&
                !source.Contains(DI))
            {
                return DO;
            }

            return string.Empty;
        }

        public static string GetClampNumberAsString(
            Function deviceClampFunction, IIOHelper ioHelper)
        {
            string clampNumberAsString = deviceClampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();

            bool haveValveTerminal = deviceClampFunction.Name
                .Contains(global::EplanDevice.DeviceManager.ValveTerminalName);
            if (haveValveTerminal)
            {
                Function IOModuleFunction =
                    ioHelper.GetIOModuleFunction(deviceClampFunction);
                string bindedDevice = deviceClampFunction.Name;
                Function IOModuleClampFunction =
                    ioHelper.GetClampFunction(IOModuleFunction, bindedDevice);
                clampNumberAsString = IOModuleClampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
            }

            return clampNumberAsString ?? string.Empty;
        }

        public static string ReplaceRusBigLettersByEngBig(string replacingStr)
        {
            return Regex.Replace(replacingStr,
                CommonConst.RusAsEngPattern, CommonConst.RusAsEngEvaluator);
        }

        public static Match FindCorrectClampCommentMatch(string comment)
        {
            string replaceCarriageToNewLine = Regex.Replace(comment,
                CommonConst.NewLineWithCarriageReturn,
                CommonConst.NewLine);
            string[] splitBySeparator = replaceCarriageToNewLine.Split(
                new string[] { CommonConst.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            int arrEndIndex = splitBySeparator.Length - 1;
            int maxDeep = 2;
            for (int i = arrEndIndex; i >= 0; i--)
            {
                if (maxDeep > 0)
                {
                    var match = Regex.Match(splitBySeparator[i],
                    IODevice.IOChannel.ChannelCommentPattern,
                    RegexOptions.IgnoreCase);
                    if (match.Value.Length == splitBySeparator[i].Length)
                    {
                        return match;
                    }
                }
                maxDeep--;
            }

            return Match.Empty;
        }
    }
}
