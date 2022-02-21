using EasyEPlanner.PxcIolinkConfiguration.Interfaces;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using IO;
using System.Collections.Generic;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    public class SensorDescriptionBuilder : ISensorDescriptionBuilder
    {
        private IModuleDescriptionBuilder _moduleDescriptionBuilder;
        private IDevicesDescriptionBuilder _devicesDescriptionBuilder;

        public SensorDescriptionBuilder(IModuleDescriptionBuilder moduleDescriptionBuilder,
            IDevicesDescriptionBuilder devicesDescriptionBuilder)
        {
            _moduleDescriptionBuilder = moduleDescriptionBuilder;
            _devicesDescriptionBuilder = devicesDescriptionBuilder;
        }

        public LinerecorderMultiSensor CreateModuleDescription(IIOModule module,
            string templateVersion, Dictionary<string, LinerecorderSensor> moduleTemplates,
            Dictionary<string, LinerecorderSensor> deviceTemplates)
        {
            Device moduleDescription;
            List<Device> devicesDescription;

            try
            {
                moduleDescription = _moduleDescriptionBuilder.Build(module, moduleTemplates);
                devicesDescription = _devicesDescriptionBuilder.Build(module, deviceTemplates);
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
    }
}
