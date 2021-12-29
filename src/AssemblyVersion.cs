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

        public static string GetStringForFileWithVersion()
        {
            return $"{StrForFilePattern} = {GetVersion()}";
        }

        public const string StrForFilePattern = "Eplanner version";
    }
}
