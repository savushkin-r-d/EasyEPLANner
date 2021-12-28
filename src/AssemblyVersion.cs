using System.Reflection;

namespace EasyEPlanner
{
    internal static class AssemblyVersion
    {
        public static string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().
                Version.ToString();
        }
    }
}
