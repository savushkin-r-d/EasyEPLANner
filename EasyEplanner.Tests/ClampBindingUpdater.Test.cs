using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EasyEPlanner;
using Eplan.EplApi.DataModel;
using EplanDevice;
using IO;
using Moq;
using NUnit.Framework;
using StaticHelper;

namespace EasyEplannerTests
{
    [TestFixture]
    public class ClampBindingUpdaterTest
    {
        private FieldInfo deviceManagerInstanceField;
        private FieldInfo projectConfigurationInstanceField;

        static ClampBindingUpdaterTest()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveEplanAssembly;

            string eplanBin = FindEplanBinDirectory();
            if (eplanBin != null)
            {
                Assembly.LoadFrom(Path.Combine(eplanBin, "Eplan.EplApi.DataModelu.dll"));
            }
        }

        [SetUp]
        public void SetUp()
        {
            deviceManagerInstanceField = typeof(DeviceManager).GetField(
                "instance",
                BindingFlags.Static | BindingFlags.NonPublic);
            deviceManagerInstanceField.SetValue(null, null);

            projectConfigurationInstanceField = typeof(ProjectConfiguration).GetField(
                "instance",
                BindingFlags.Static | BindingFlags.NonPublic);
            projectConfigurationInstanceField.SetValue(null, null);
        }

        [Test]
        public void ResetDeviceChannels_ClearsChannelsForKnownDevices()
        {
            var device = CreateAoWithBoundChannel("OBJ1AO1", "Открыть");
            var manager = DeviceManager.GetInstance();
            manager.Devices.Add(device);
            manager.Sort();

            InjectDeviceReader(new StubDeviceReader(
                new Dictionary<string, string> { { "OBJ1AO1", "Открыть" } }));

            var moduleInfo = IOModuleInfo.Stub;
            moduleInfo.AddressSpaceType = IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI;

            var channel = GetAoChannels(device)[0];
            Assert.IsFalse(channel.IsEmpty());

            ClampBindingUpdater.ResetDeviceChannels(
                null,
                moduleInfo,
                "+OBJ1-AO1");

            Assert.IsTrue(channel.IsEmpty());
        }

        [Test]
        public void ResetDeviceChannels_UsesDiChannelNameFromComment()
        {
            var device = CreateLsWithBoundDiChannel("OBJ1LS1", "DI");
            var manager = DeviceManager.GetInstance();
            manager.Devices.Add(device);
            manager.Sort();

            InjectDeviceReader(new StubDeviceReader(
                new Dictionary<string, string> { { "OBJ1LS1", "DI" } }));

            var moduleInfo = IOModuleInfo.Stub;
            moduleInfo.AddressSpaceType = IOModuleInfo.ADDRESS_SPACE_TYPE.DODI;

            var channel = GetDiChannels(device)[0];
            Assert.IsFalse(channel.IsEmpty());

            ClampBindingUpdater.ResetDeviceChannels(
                null,
                moduleInfo,
                "+OBJ1-LS1");

            Assert.IsTrue(channel.IsEmpty());
        }

        [Test]
        public void ResetDeviceChannels_DoesNotThrowForUnknownDevice()
        {
            InjectDeviceReader(new StubDeviceReader(
                new Dictionary<string, string> { { "UNKNOWN", "Открыть" } }));

            Assert.DoesNotThrow(() => ClampBindingUpdater.ResetDeviceChannels(
                null,
                IOModuleInfo.Stub,
                "+OBJ1-V1"));
        }

        [Test]
        public void ReadClampBinding_DoesNotThrowWhenClampNotFound()
        {
            var clampFunction = Mock.Of<IEplanFunction>(f => f.Name == "+A1-1");

            Assert.DoesNotThrow(() => ClampBindingUpdater.ReadClampBinding(
                clampFunction,
                new ApiHelper()));
        }

        [Test]
        public void ApplyFunctionalText_InvokesClearModuleBind()
        {
            InjectDeviceReader(new StubDeviceReader(
                new Dictionary<string, string>()));

            var function = new Mock<Function>(MockBehavior.Loose);
            var clampFunction = new EplanFunction(function.Object);
            bool clearInvoked = false;

            Assert.Catch<Exception>(() =>
                ClampBindingUpdater.ApplyFunctionalText(
                    clampFunction,
                    IOModuleInfo.Stub,
                    "old text",
                    "new text",
                    () => clearInvoked = true));

            Assert.IsTrue(clearInvoked);
        }

        private static void InjectDeviceReader(DeviceReader deviceReader)
        {
            var configuration = ProjectConfiguration.GetInstance();
            typeof(ProjectConfiguration)
                .GetField("deviceReader", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(configuration, deviceReader);
        }

        private sealed class StubDeviceReader : DeviceReader
        {
            private readonly Dictionary<string, string> bindings;

            public StubDeviceReader(Dictionary<string, string> bindings)
                : base(
                    new ApiHelper(),
                    new DeviceHelper(new ApiHelper()),
                    new ProjectHelper(new ApiHelper()),
                    new IOHelper(new ProjectHelper(new ApiHelper())),
                    DeviceManager.GetInstance())
            {
                this.bindings = bindings;
            }

            public override Dictionary<string, string> GetBindingForResettingChannel(
                Function deviceClampFunction,
                IOModuleInfo moduleInfo,
                string devicesDescription = "")
            {
                return bindings;
            }
        }

        private static AO CreateAoWithBoundChannel(string name, string comment)
        {
            var device = new AO(name, $"+{name}", "test", 1, "OBJ", 1);
            device.SetSubType("AO");

            var channels = GetAoChannels(device);
            channels.Clear();
            channels.Add(new IODevice.IOChannel(IODevice.IOChannel.AO, 1, 1, 1, comment));
            channels[0].SetChannel(1, 1, 1, 101, 0, 0);

            return device;
        }

        private static LS CreateLsWithBoundDiChannel(string name, string comment)
        {
            var device = new LS(name, $"+{name}", "test", 1, "OBJ", 1, string.Empty);

            var channels = GetDiChannels(device);
            channels.Clear();
            channels.Add(new IODevice.IOChannel(IODevice.IOChannel.DI, 1, 1, 1, comment));
            channels[0].SetChannel(1, 1, 1, 101, 0, 0);

            return device;
        }

        private static List<IODevice.IOChannel> GetAoChannels(IODevice device)
        {
            return (List<IODevice.IOChannel>)typeof(IODevice)
                .GetField("AO", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(device);
        }

        private static List<IODevice.IOChannel> GetDiChannels(IODevice device)
        {
            return (List<IODevice.IOChannel>)typeof(IODevice)
                .GetField("DI", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(device);
        }

        private static Assembly ResolveEplanAssembly(object sender, ResolveEventArgs args)
        {
            string assemblyName = new AssemblyName(args.Name).Name;
            if (!assemblyName.StartsWith("Eplan.", StringComparison.Ordinal))
            {
                return null;
            }

            string eplanBin = FindEplanBinDirectory();
            if (eplanBin == null)
            {
                return null;
            }

            string path = Path.Combine(eplanBin, assemblyName + ".dll");
            return File.Exists(path) ? Assembly.LoadFrom(path) : null;
        }

        private static string FindEplanBinDirectory()
        {
            const string eplanRoot = @"C:\Program Files\Eplan\Platform";
            if (!Directory.Exists(eplanRoot))
            {
                return null;
            }

            return Directory.GetDirectories(eplanRoot)
                .OrderByDescending(path => path, StringComparer.OrdinalIgnoreCase)
                .Select(path => Path.Combine(path, "Bin"))
                .FirstOrDefault(bin => File.Exists(Path.Combine(bin, "Eplan.EplApi.DataModelu.dll")));
        }
    }
}
