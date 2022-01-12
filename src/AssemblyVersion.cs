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

        public static string GetVersionAsLuaComment()
        {
            return string.Format("--Eplanner version = {0}", GetVersion());
        }

        public const string StrForFilePattern = "Eplanner version";
    }
}
