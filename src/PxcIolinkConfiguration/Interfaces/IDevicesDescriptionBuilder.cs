using EasyEPlanner.PxcIolinkConfiguration.Models;
using IO;
using System.Collections.Generic;

namespace EasyEPlanner.PxcIolinkConfiguration.Interfaces
{
    public interface IDevicesDescriptionBuilder
    {
        List<Device> Build(IIOModule module, Dictionary<string, LinerecorderSensor> deviceTemplates);
    }
}
