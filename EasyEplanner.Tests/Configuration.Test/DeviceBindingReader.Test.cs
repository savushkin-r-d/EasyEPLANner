using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EasyEPlanner;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using IO;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using StaticHelper;

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
    }
}
