using EasyEPlanner.PxcIolinkConfiguration.Models;
using IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    public class SensorDescriptionBuilder : ISensorDescriptionBuilder
    {
        public LinerecorderMultiSensor CreateModuleDescription(IIOModule module,
            string templateVersion, Dictionary<string, LinerecorderSensor> moduleTemplates,
            Dictionary<string, LinerecorderSensor> deviceTemplates)
        {
            Device moduleDescription;
            List<Device> devicesDescription;

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
        private Device CreateModuleFromTemplate(IIOModule module,
            Dictionary<string, LinerecorderSensor> moduleTemplates)
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
        private List<Device> CreateDevicesFromTemplate(IIOModule module,
            Dictionary<string, LinerecorderSensor> deviceTemplates)
        {
            var deviceList = new List<Device>();
            var iolEplanDevices = module.Devices
                .Where(x => x != null && x.Count > 0)
                .Select(x => new { Device = x.First(), Clamp = Array.IndexOf(module.Devices, x) });
            foreach(var deviceClampPair in iolEplanDevices)
            {
                int logicalPort = module.DevicesChannels[deviceClampPair.Clamp]
                    .Select(x => x.LogicalClamp).FirstOrDefault();
                string articleName = deviceClampPair.Device.ArticleName;
                bool templateNotFound = !deviceTemplates.ContainsKey(articleName);
                bool invalidLogicalClamp = logicalPort <= 0;

                if (templateNotFound || invalidLogicalClamp) continue;

                LinerecorderSensor deviceTemplate = deviceTemplates[articleName];
                var deviceDescription = CreateDeviceFromTemplate(deviceTemplate,
                    logicalPort, deviceClampPair.Device);

                deviceList.Add(deviceDescription);
            }

            return deviceList;
        }

        private Device CreateDeviceFromTemplate(LinerecorderSensor template,
            int logicalPort, EplanDevice.IIODevice eplanDevice)
        {
            var deviceIolConfProperties = eplanDevice.IolConfProperties;
            var copiedParameters = template.Parameters.Clone() as Parameters;
            foreach(var property in deviceIolConfProperties)
            {
                var originalTemplateProperty = copiedParameters.Param
                    .Where(x => x.Id == property.Key)
                    .FirstOrDefault();

                if (originalTemplateProperty == null)
                {
                    throw new ArgumentException($"В устройстве {eplanDevice.EplanName}" +
                        $" задано свойство {property.Key}, его не существует в шаблоне устройства" +
                        $" {eplanDevice.ArticleName}.");
                }

                double newInternalValue = CalculateNewInternalValue(originalTemplateProperty,
                    property, eplanDevice.ArticleName);

                originalTemplateProperty.Value = property.Value.ToString();
                originalTemplateProperty.InternalValue = newInternalValue.ToString();
            }

            var deviceDescription = new Device
            {
                Port = logicalPort,
                Sensor = template.Sensor.Clone() as Sensor,
                Parameters = copiedParameters
            };

            return deviceDescription;
        }

        private int CalculateNewInternalValue(Param originParam,
            KeyValuePair<string, double> deviceParamValuePair,
            string articleName)
        {
            int internalValue = 0;

            string correctParamValue = originParam.Value.Replace(',', '.');
            bool defaultValueParsed = double.TryParse(correctParamValue, out double defaultValue);
            bool defaultInternalValueParsed = double.TryParse(originParam.InternalValue, out double defaultInternalValue);
            double newValue = deviceParamValuePair.Value;
            if (defaultInternalValueParsed && defaultValueParsed)
            {
                if (defaultValue == 0 || defaultInternalValue == 0)
                {
                    string message = $"В шаблоне изделия {articleName}, параметр {deviceParamValuePair.Key}" +
                        $" содержит Value или InternalValue равное 0. Деление на 0. " +
                        $"Установите другие значения в шаблоне.";
                    throw new ArgumentException(message);
                }

                internalValue = Convert.ToInt32((defaultInternalValue * newValue) / defaultValue);
            }

            return internalValue;
        }
        #endregion
    }
}
