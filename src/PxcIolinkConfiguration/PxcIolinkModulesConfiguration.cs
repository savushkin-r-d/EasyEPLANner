using System.Collections.Generic;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    internal class PxcIolinkModulesConfiguration : IPxcIolinkConfiguration
    {
        private List<string> errorsList;
        private bool generateDevices;

        public PxcIolinkModulesConfiguration(bool generateDevices)
        {
            errorsList = new List<string>();
            this.generateDevices = generateDevices;
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
