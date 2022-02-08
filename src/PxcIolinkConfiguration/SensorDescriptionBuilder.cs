using EasyEPlanner.PxcIolinkConfiguration.Models;
using IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    internal interface ISensorDescriptionBuilder
    {
        LinerecorderMultiSensor CreateModuleDescription(IOModule module,
            string templateVersion, Dictionary<string, LinerecorderSensor> moduleTemplates,
            Dictionary<string, LinerecorderSensor> deviceTemplates);
    }

    internal class SensorDescriptionBuilder : ISensorDescriptionBuilder
    {
        public LinerecorderMultiSensor CreateModuleDescription(IOModule module,
            string templateVersion, Dictionary<string, LinerecorderSensor> moduleTemplates,
            Dictionary<string, LinerecorderSensor> deviceTemplates)
        {
            Models.Device moduleDescription;
            List<Models.Device> devicesDescription;

            try
            {
                moduleDescription = CreateModuleFromTemplate(module, moduleTemplates);
                devicesDescription = CreateDevicesFromTemplate(module, deviceTemplates);
            }
            catch
            {
                throw;
            }
            
            moduleDescription.Add(devicesDescription);

            var sensor = new LinerecorderMultiSensor();
            if (moduleDescription.IsEmpty()) return sensor;

            sensor.Version = templateVersion;
            sensor.Add(moduleDescription);

            return sensor;
        }

        private Models.Device CreateModuleFromTemplate(IOModule module,
            Dictionary<string, LinerecorderSensor> moduleTemplates)
        {
            var moduleDescription = new Models.Device();
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

        private void ConfigureModuleParameters(IOModule module, Models.Device moduleDescription)
        {
            var channels = module.DevicesChannels
                .Where(x => x != null && x.Count > 0)
                .SelectMany(x => x);

            const string clampNamePattern = "V_PortConfig_{0}";
            int clampsCount = module.Info.ChannelClamps.Length;
            for(int i = 1; i <= clampsCount; i++)
            {
                string paramName = string.Format(clampNamePattern, i);
                var param = moduleDescription.Parameters.Param
                    .Where(x => x.Name == paramName)
                    .FirstOrDefault();
                var channel = channels
                    .Where(x => x.LogicalClamp == i)
                    .FirstOrDefault();

                if(param == null)
                {
                    throw new InvalidDataException($"Параметр клеммы {paramName} " +
                        $"модуля {module.Name} {module.ArticleName} не найден.");
                }

                ConfigureParam(param, channel);
            }
        }

        private void ConfigureParam(Param param, Device.IODevice.IOChannel channel)
        {
            if (channel == null)
            {
                SetDisabled(param);
            }
            else
            {
                switch (channel.Name)
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
                }
            }
        }

        private void SetDisabled(Param parameter)
        {
            parameter.InternalValue = "3";
            parameter.Value = "3";
            parameter.Text = "Disabled";
        }

        private void SetIoLink(Param parameter)
        {
            parameter.InternalValue = "0";
            parameter.Value = "0";
            parameter.Text = "IO-Link";
        }

        private void SetDi(Param parameter)
        {
            parameter.InternalValue = "1";
            parameter.Value = "1";
            parameter.Text = "DI";
        }

        private void SetDo(Param parameter)
        {
            parameter.InternalValue = "2";
            parameter.Value = "2";
            parameter.Text = "DO";
        }

        private List<Models.Device> CreateDevicesFromTemplate(IOModule module,
            Dictionary<string, LinerecorderSensor> deviceTemplates)
        {
            var deviceList = new List<Models.Device>();

            //TODO: Generate device info
            //TODO: Change device parameters somehow

            return deviceList;
        }
    }
}
