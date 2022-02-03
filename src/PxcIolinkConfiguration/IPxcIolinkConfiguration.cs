using System.Collections.Generic;

namespace EasyEPlanner.PxcIolinkConfiguration
{
    public interface IPxcIolinkConfiguration
    {
        List<string> ErrorsList { get; }

        bool HasErrors { get; }

        void Run();
    }
}
