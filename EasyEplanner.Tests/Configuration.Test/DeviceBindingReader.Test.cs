using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EasyEPlanner;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using EplanDevice;
using IO;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using StaticHelper;
using static TechObjectTests.DeviceManagerMock;

namespace EasyEplannerTests.ConfigurationTest
{
    public class DeviceBindingReaderTest
    {
        [Test]
        public void CorrectDataMultipleBindingToValveTerminal_CheckDescription()
        {
            string description = "+A1-Y1\r\n+A1-V1\r\nОткрыть\t\r\n+A1-V2\r\nОткрыть ВС";
            string expectedDescription = "+A1-Y1\r\n+A1-V1\r\n+A1-V2";
            var expectedActions = new List<string>() { "", "Открыть", "Открыть ВС" };
            var expectedComments = new List<string>() { "", "+A1-V1\r.", ". +A1-V2\r." };

            var apiHelper = new ApiHelper();
            var deviceBindingReader = new DeviceBindingReader(new ProjectHelper(apiHelper), apiHelper);

            var method = typeof(DeviceBindingReader).GetMethod(
                    "CorrectDataMultipleBindingToValveTerminal", BindingFlags.NonPublic | BindingFlags.Instance);

            object[] arguments = new object[] { description, null, null };
            object res = method.Invoke(deviceBindingReader, arguments);

            description = arguments[0] as string;
            var actions = arguments[1] as List<string>;
            var comments = arguments[2] as List<string>;

            Assert.Multiple(
            () =>
            {
                Assert.AreEqual(expectedDescription, description);
                CollectionAssert.AreEqual(expectedActions, actions);
                CollectionAssert.AreEqual(expectedComments, comments);
            });
        }

        [TestCaseSource(nameof(AlternateClampBinded_Cases))]
        public void AlternateClampBinded(List<List<int>> AlternateChannelClamps, int clamp, Dictionary<int, string> currentBinding, int? expected)
        {
            var apiHelper = new ApiHelper();
            var deviceBindingReader = new DeviceBindingReader(new ProjectHelper(apiHelper), apiHelper);

            var method = typeof(DeviceBindingReader).GetMethod("AlternateClampBinded",
                BindingFlags.NonPublic | BindingFlags.Instance);

            var ioModuleMock = new Mock<IIOModuleInfo>();
            ioModuleMock.Setup(m => m.AlternateChannelsClamps).Returns(AlternateChannelClamps);

            var arguments = new object[] { ioModuleMock.Object, clamp, currentBinding };

            int? result = method.Invoke(deviceBindingReader, arguments) as int?;

            Assert.AreEqual(expected, result);
        }

        private static readonly object[] AlternateClampBinded_Cases = new object[]
        {
            new object[] { new List<List<int>> { new List<int> { 0, 20 }, new List<int> { 1, 21 } }, 21, new Dictionary<int, string> { { 1, "bind" } }, 1 },
            new object[] { new List<List<int>> { new List<int> { 0, 20 }, new List<int> { 1, 21 } }, 20, new Dictionary<int, string> { { 0, "bind" } }, 0 },
            new object[] { new List<List<int>> { new List<int> { 0, 20 }, new List<int> { 1, 21 } }, 20, new Dictionary<int, string> { { 0, "" } }, null },
            new object[] { new List<List<int>> { new List<int> { 0 }, new List<int> { 1 } }, 1, new Dictionary<int, string> { { 0, "bind" } }, null },
        };

        [TestCase("V_DO1", false)]
        [TestCase("V_DO2", false)]
        [TestCase("V_DO1_DI1_FB_OFF", false)]
        [TestCase("V_DO1_DI1_FB_ON", false)]
        [TestCase("V_DO1_DI2", false)]
        [TestCase("V_DO2_DI2", false)]
        [TestCase("V_IOLINK_DO1_DI2", true)]
        [TestCase("V_IOLINK_VTUG_DO1", true)]
        [TestCase("V_IOLINK_VTUG_DO1_FB_OFF", true)]
        [TestCase("V_IOLINK_VTUG_DO1_FB_ON", true)]
        [TestCase("V_IOLINK_VTUG_DO1_DI2", true)]
        [TestCase("V_IOL_TERMINAL_MIXPROOF_DO3", true)]
        [TestCase("V_IOLINK_MIXPROOF", true)]
        public void CheckBindAllowedSubTypesToValveTerminal_Tests(string subtype, bool withoutError)
        {
            var device = new V("OBJ1V1", "+OBJ1-V1", "", 1, "OBJ", 1, "");
            device.SetSubType(subtype);



            Assert.AreEqual(withoutError,
                DeviceBindingReader.CheckBindAllowedSubTypesToValveTerminal(device, "ValveTerminalName") == string.Empty);
        }

        [Test]
        public void CheckBindAllowedSubTypesToValveTerminal_NotValve()
        {
            var device = new HLA("OBJ1V1", "+OBJ1-V1", "", 1, "OBJ", 1, "");

            Assert.IsTrue(
                DeviceBindingReader.CheckBindAllowedSubTypesToValveTerminal(device, "ValveTerminalName") == string.Empty);
        }

        [TestCase("V_DO1", true)]
        [TestCase("V_DO2", true)]
        [TestCase("V_DO1_DI1_FB_OFF", true)]
        [TestCase("V_DO1_DI1_FB_ON", true)]
        [TestCase("V_DO1_DI2", true)]
        [TestCase("V_DO2_DI2", true)]
        [TestCase("V_IOLINK_DO1_DI2", false)]
        [TestCase("V_IOLINK_VTUG_DO1", false)]
        [TestCase("V_IOLINK_VTUG_DO1_FB_OFF", true)]
        [TestCase("V_IOLINK_VTUG_DO1_FB_ON", true)]
        [TestCase("V_IOLINK_VTUG_DO1_DI2", false, "")]
        [TestCase("V_IOLINK_VTUG_DO1_DI2", true, "Открыт")]
        [TestCase("V_IOLINK_VTUG_DO1_DI2", true, "Закрыт")]
        [TestCase("V_IOL_TERMINAL_MIXPROOF_DO3", false)]
        [TestCase("V_IOLINK_MIXPROOF", false)]
        public void CheckBindAllowedSubTypesToDOModule_Tests(string subtype, bool withoutError, string comment = "")
        {
            var device = new V("OBJ1V1", "+OBJ1-V1", "", 1, "OBJ", 1, "");
            device.SetSubType(subtype);

            var ioModuleInfo = IOModuleInfo.Stub;
            ioModuleInfo.AddressSpaceType = IOModuleInfo.ADDRESS_SPACE_TYPE.DODI;
            var moduleMock = new Mock<IIOModule>();
            moduleMock.Setup(m => m.Name).Returns("Module");
            moduleMock.Setup(m => m.Info).Returns(ioModuleInfo);

            Assert.AreEqual(withoutError,
                DeviceBindingReader.CheckBindAllowedSubTypesToDOModule(device, moduleMock.Object, comment) == string.Empty);
        }

        [Test]
        public void CheckBindAllowedSubTypesToDOModule_NotValve()
        {
            var device = new HLA("OBJ1V1", "+OBJ1-V1", "", 1, "OBJ", 1, "");

            Assert.IsTrue(
                DeviceBindingReader.CheckBindAllowedSubTypesToDOModule(device, null, "") == string.Empty);
        }

        [Test]
        public void CheckBindAllowedSubTypesToDOModule_ModuleDoesNotDIDO()
        {
            var device = new V("OBJ1V1", "+OBJ1-V1", "", 1, "OBJ", 1, "");

            var ioModuleInfo = IOModuleInfo.Stub;
            ioModuleInfo.AddressSpaceType = IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI;
            var moduleMock = new Mock<IIOModule>();
            moduleMock.Setup(m => m.Name).Returns("Module");
            moduleMock.Setup(m => m.Info).Returns(ioModuleInfo);

            Assert.IsTrue(
                DeviceBindingReader.CheckBindAllowedSubTypesToDOModule(device, moduleMock.Object, "") == string.Empty);
        }


        [Test]
        public void CheckAndSetExtraOffset()
        {
            var ioModuleInfo = IOModuleInfo.Stub;
            ioModuleInfo.Number = 657;

            var module = Mock.Of<IIOModule>(m => 
                m.Info == ioModuleInfo);

            var valve = new V("V1", "-V1", "", 1, "", -1, "");
            valve.SetSubType(DeviceSubType.V_IOLINK_MIXPROOF.ToString());

            Assert.Multiple(() =>
            {
                DeviceBindingReader.CheckAndSetExtraOffset(module, valve, 1);
                Assert.IsTrue(valve.RuntimeParameters.TryGetValue(IODevice.RuntimeParameter.R_EXTRA_OFFSET, out var extraOffset));
                Assert.AreEqual(0, extraOffset);

                DeviceBindingReader.CheckAndSetExtraOffset(module, valve, 3);
                valve.RuntimeParameters.TryGetValue(IODevice.RuntimeParameter.R_EXTRA_OFFSET, out extraOffset);
                Assert.AreEqual(-2, extraOffset);
            });
        }
    }
}
