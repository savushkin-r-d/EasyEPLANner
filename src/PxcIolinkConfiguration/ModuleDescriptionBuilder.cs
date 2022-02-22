using EasyEPlanner.PxcIolinkConfiguration.Interfaces;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    public class ModuleDescriptionBuilder : IModuleDescriptionBuilder
    {
        public Device Build(IIOModule module, Dictionary<string, LinerecorderSensor> moduleTemplates)
        {
            var moduleDescription = new Device();
            LinerecorderSensor template;
            if (moduleTemplates.ContainsKey(module.ArticleName))
            {
                template = moduleTemplates[module.ArticleName];
            }
            else
            {
                return moduleDescription;
            }

            moduleDescription.Sensor = template.Sensor.Clone() as Sensor;
            moduleDescription.Parameters = template.Parameters.Clone() as Parameters;
            try
            {
                ConfigureModuleParameters(module, moduleDescription);
            }
            catch
            {
                throw;
            }

            return moduleDescription;
        }

        private void ConfigureModuleParameters(IIOModule module, Device moduleDescription)
        {
            var channels = module.DevicesChannels
                .Where(x => x != null && x.Count > 0)
                .SelectMany(x => x);

            const string clampNamePattern = "V_PortConfig_{0:d2}";
            int clampsCount = module.Info.ChannelClamps.Length;
            for (int i = 1; i <= clampsCount; i++)
            {
                string paramName = string.Format(clampNamePattern, i);
                var param = moduleDescription.Parameters.Param
                    .Where(x => x.Id == paramName)
                    .FirstOrDefault();
                var channel = channels
                    .Where(x => x.LogicalClamp == i)
                    .FirstOrDefault();

                if (param == null)
                {
                    throw new InvalidDataException($"Параметр клеммы {paramName} " +
                        $"модуля {module.Name} {module.ArticleName} не найден.");
                }

                string channelName = channel?.Name ?? string.Empty;
                ConfigureParam(param, channelName);
            }
        }

        private void ConfigureParam(Param param, string channelName)
        {
            switch (channelName)
            {
                case "AI":
                case "AO":
                    SetIoLink(param);
                    break;

                case "DI":
                    SetDi(param);
                    break;

                case "DO":
                    SetDo(param);
                    break;

                default:
                    SetDisabled(param);
                    break;
            }
        }

        private void SetDisabled(Param parameter)
        {
            const string disabledValue = "3";
            parameter.InternalValue = disabledValue;
            parameter.Value = disabledValue;
            parameter.Text = "Disabled";
        }

        private void SetIoLink(Param parameter)
        {
            const string ioLinkValue = "0";
            parameter.InternalValue = ioLinkValue;
            parameter.Value = ioLinkValue;
            parameter.Text = "IO-Link";
        }

        private void SetDi(Param parameter)
        {
            const string diValue = "1";
            parameter.InternalValue = diValue;
            parameter.Value = diValue;
            parameter.Text = "DI";
        }

        private void SetDo(Param parameter)
        {
            const string doValue = "2";
            parameter.InternalValue = doValue;
            parameter.Value = doValue;
            parameter.Text = "DO";
        }
    }
}
