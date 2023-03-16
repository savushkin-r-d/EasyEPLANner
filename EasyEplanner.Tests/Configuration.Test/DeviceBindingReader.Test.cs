using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EasyEPlanner;
using EasyEPlanner.PxcIolinkConfiguration.Models;
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
    }
}
