using EasyEPlanner.PxcIolinkConfiguration.Interfaces;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    public class DevicesDescriptionBuilder : IDevicesDescriptionBuilder
    {
        public List<Device> Build(IIOModule module, Dictionary<string, LinerecorderSensor> deviceTemplates)
        {
            var deviceList = new List<Device>();
            var iolEplanDevices = module.Devices
                .Where(x => x != null && x.Count > 0)
                .Select(x => new { Device = x.First(), Clamp = Array.IndexOf(module.Devices, x) });
            foreach (var deviceClampPair in iolEplanDevices)
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
            foreach (var property in deviceIolConfProperties)
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
            KeyValuePair<string, double> deviceParamValuePair, string articleName)
        {
            int internalValue = 0;

            string correctParamValue = originParam.Value.Replace(',', '.');
            bool defaultValueParsed = double.TryParse(correctParamValue, out double defaultValue);
            bool defaultInternalValueParsed = double.TryParse(originParam.InternalValue, out double defaultInternalValue);
            double newValue = deviceParamValuePair.Value;
            if (defaultInternalValueParsed && defaultValueParsed)
            {
                if (Math.Abs(defaultValue) <= double.Epsilon || Math.Abs(defaultInternalValue) <= double.Epsilon)
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
    }
}
