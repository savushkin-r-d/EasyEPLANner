using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.PxcIolinkConfiguration
{

    internal class PxcIolinkModulesConfiguration : IPxcIolinkConfiguration
    {
        private List<string> errorsList;

        public PxcIolinkModulesConfiguration()
        {
            errorsList = new List<string>();
        }

        public void Run()
        {

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
