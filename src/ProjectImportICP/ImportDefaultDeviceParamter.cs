using EplanDevice;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ProjectImportICP
{
    public class ImportDefaultDeviceParamter
    {
        public ImportDefaultDeviceParamter(string deviceType, IODevice.Parameter parameter, int defaultValue = 0) 
        {
            DeviceType = deviceType;
            Parameter = parameter;
            DefaultValue = defaultValue;
        }

        public string DeviceType { get; }
        
        public IODevice.Parameter Parameter { get; }

        public int DefaultValue { get; set; }
    }
}
