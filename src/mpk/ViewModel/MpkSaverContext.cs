using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.mpk.ViewModel
{
    public class MpkSaverContext(string projectName) : IMpkSaverContext
    {
        private const string BaseMpkDirectory = "P:\\Monitor\\Mpk";

        public string MpkDirectory { get; set; } = Path.Combine(BaseMpkDirectory, projectName);

        public string MainContainerName { get; set; } = projectName;

        public bool Rewrite { get; set; } = true;
    }
}
