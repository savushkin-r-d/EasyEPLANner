using InterprojectExchange;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace Tests.InterprojectExchangeTest
{
    internal static class InterprojectExchangeStarterTestHelper
    {
        public static string SystemLuaDirectory => Path.GetFullPath(Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "..", "..", "..", "..", "src", "Lua"));

        public static string ProjectTestDataDirectory => Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "InterprojectExchange.Test",
            "TestData",
            "project");

        public static void InitLuaWithSystemScripts(InterprojectExchangeStarter starter)
        {
            InvokeInstanceMethod(starter, "InitLuaInstance");
            InvokeLoadScript(starter, Path.Combine(SystemLuaDirectory,
                "sys_interproject_io.lua"));
            InvokeLoadScript(starter, Path.Combine(SystemLuaDirectory,
                "sys_shared_initializer.lua"));
        }

        public static void InvokeLoadScript(InterprojectExchangeStarter starter,
            string scriptPath)
        {
            InvokeInstanceMethod(starter, "LoadScript", scriptPath);
        }

        public static void InvokeLoadProjectsData(
            InterprojectExchangeStarter starter,
            string projectFolder,
            string projectName)
        {
            typeof(InterprojectExchangeStarter).GetMethod(
                "LoadProjectsData",
                BindingFlags.Instance | BindingFlags.Public)
                .Invoke(starter, new object[] { projectFolder, projectName });
        }

        public static string InvokeSetIPFromMainModel(
            InterprojectExchangeStarter starter,
            string projectName)
        {
            return (string)typeof(InterprojectExchangeStarter).GetMethod(
                "SetIPFromMainModel",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(starter, new object[] { projectName });
        }

        public static bool InvokeLoadCurrentInterprojectExchange(
            InterprojectExchangeStarter starter,
            bool isReadDevices)
        {
            return (bool)typeof(InterprojectExchangeStarter).GetMethod(
                "LoadCurrentInterprojectExchange",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(starter, new object[] { isReadDevices });
        }

        public static string BuildProjectsNotOpenedSummaryText(
            InterprojectExchangeStarter starter)
        {
            return (string)typeof(InterprojectExchangeStarter).GetMethod(
                "BuildProjectsNotOpenedSummaryText",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(starter, null);
        }

        public static System.Collections.Generic.List<string> GetProjectsNotOpenedList(
            InterprojectExchangeStarter starter)
        {
            return typeof(InterprojectExchangeStarter).GetField(
                "_projectsNotOpenedOnLoad",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(starter) as System.Collections.Generic.List<string>;
        }

        public static bool InvokeTryResolveProjectFolder(
            string projectName,
            out string folder)
        {
            var method = typeof(InterprojectExchangeStarter).GetMethod(
                "TryResolveProjectFolder",
                BindingFlags.NonPublic | BindingFlags.Static);
            object[] args = { projectName, null };
            bool resolved = (bool)method.Invoke(null, args);
            folder = (string)args[1];
            return resolved;
        }

        private static void InvokeInstanceMethod(
            InterprojectExchangeStarter starter,
            string methodName,
            params object[] args)
        {
            typeof(InterprojectExchangeStarter).GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(starter, args);
        }
    }
}
