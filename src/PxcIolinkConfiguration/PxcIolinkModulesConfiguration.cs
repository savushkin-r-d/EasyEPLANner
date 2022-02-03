using Device;
using IO;
using System.Collections.Generic;
using EasyEPlanner.PxcIolinkConfiguration.Models;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    internal class PxcIolinkModulesConfiguration : IPxcIolinkConfiguration
    {
        private List<string> errorsList;
        private bool generateDevices;
        private IDeviceManager deviceManager;
        private IIOManager ioManager;
        private Dictionary<string, Linerecorder_Sensor> moduleTemplates;
        private Dictionary<string, Linerecorder_Sensor> deviceTemplates;

        public PxcIolinkModulesConfiguration(bool generateDevices,
            IDeviceManager deviceManager, IIOManager ioManager)
        {
            errorsList = new List<string>();
            this.generateDevices = generateDevices;
            this.deviceManager = deviceManager;
            this.ioManager = ioManager;
            moduleTemplates = new Dictionary<string, Linerecorder_Sensor>();
            deviceTemplates = new Dictionary<string, Linerecorder_Sensor>();
        }

        public void Run()
        {
            //Check folders
                // -> If not exists -> create
                    // -> If no any files -> exception
                        // -> Return exception, stop.
                    // -> If has files -> Read templates
            //Generate for modules
                // -> Get modules with name, and channels
                // -> Define channel type (or get) by each clamp

            // using System.Xml.Serialization;
            // XmlSerializer serializer = new XmlSerializer(typeof(Linerecorder_Sensor));
            // using (StringReader reader = new StringReader(xml))
            // {
            //    var test = (Linerecorder_Sensor)serializer.Deserialize(reader);
            // }
        }

        public List<string> ErrorsList
        {
            get
            {
                return errorsList;
            }
        }

        public bool HasErrors
        {
            get
            {
                return ErrorsList.Count > 0;
            }
        }
    }
}
