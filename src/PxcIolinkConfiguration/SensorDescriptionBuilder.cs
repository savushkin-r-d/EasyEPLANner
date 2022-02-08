using EasyEPlanner.PxcIolinkConfiguration.Models;
using IO;
using System.Collections.Generic;

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
            Models.Device moduleDescription = CreateModuleFromTemplate(module, moduleTemplates);
            List<Models.Device> devicesDescription = CreateDevicesFromTemplate(module, deviceTemplates);
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

            //var channels = module.DevicesChannels.SelectMany(x => x)
            //    .Where(x => x != null);
            // TODO: Change module parameters

            return moduleDescription;
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
