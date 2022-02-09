using EasyEPlanner.PxcIolinkConfiguration.Models;
using IO;
using System;
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

        #region генерация описания модуля
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

            const string clampNamePattern = "V_PortConfig_{0:d2}";
            int clampsCount = module.Info.ChannelClamps.Length;
            for(int i = 1; i <= clampsCount; i++)
            {
                string paramName = string.Format(clampNamePattern, i);
                var param = moduleDescription.Parameters.Param
                    .Where(x => x.Id == paramName)
                    .FirstOrDefault();
                var channel = channels
                    .Where(x => x.LogicalClamp == i)
                    .FirstOrDefault();

                if(param == null)
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
        #endregion

        #region генерация описания устройств для добавления в модуль
        private List<Models.Device> CreateDevicesFromTemplate(IOModule module,
            Dictionary<string, LinerecorderSensor> deviceTemplates)
        {
            var deviceList = new List<Models.Device>();
            var devices = module.Devices
                .Where(x => x != null && x.Count > 0)
                .Select(x => new { Device = x.First(), Clamp = Array.IndexOf(module.Devices, x) });
            foreach(var deviceClampPair in devices)
            {
                int logicalPort = module.DevicesChannels[deviceClampPair.Clamp]
                    .Select(x => x.LogicalClamp).FirstOrDefault();
                string articleName = deviceClampPair.Device.ArticleName;
                bool templateNotFound = !deviceTemplates.ContainsKey(articleName);
                bool invalidLogicalClamp = logicalPort <= 0;

                if (templateNotFound || invalidLogicalClamp) continue;

                LinerecorderSensor deviceTemplate = deviceTemplates[articleName];
                var deviceDescription = new Models.Device
                {
                    Port = logicalPort,
                    Sensor = deviceTemplate.Sensor.Clone() as Sensor,
                    Parameters = deviceTemplate.Parameters.Clone() as Parameters
                };

                deviceList.Add(deviceDescription);
            }

            //TODO: Manage parameters

            return deviceList;
        }
        #endregion
    }
}
