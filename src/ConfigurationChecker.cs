using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner
{
    public class ConfigurationChecker
    {
        public ConfigurationChecker()
        {
            this.deviceManager = Device.DeviceManager.GetInstance();
            this.IOManager = IO.IOManager.GetInstance();
            this.techObjectManager = TechObject.TechObjectManager.GetInstance();
        }

        public void Check() 
        {
            errors = string.Empty;
            errors = deviceManager.Check();
            errors += IOManager.Check();
            errors += techObjectManager.Check();
        }

        public string Errors 
        { 
            get
            {
                return errors;
            } 
        }

        string errors;

        Device.DeviceManager deviceManager;
        IO.IOManager IOManager;
        TechObject.TechObjectManager techObjectManager;
    }
}
