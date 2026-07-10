using EasyEPlanner.ModbusExchange.Model;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using NUnit.Framework;
using StaticHelper;

namespace EasyEplannerTests.StaticHelperTest
{
    public class SuppFieldsTest
    {
        [Test]
        public void Constants()
        {
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, SuppField.Off);
                Assert.AreEqual(2 ,SuppField.Subtype);
                Assert.AreEqual(3 ,SuppField.Parameters);
                Assert.AreEqual(4 ,SuppField.Properties);
                Assert.AreEqual(5 ,SuppField.RuntimeParameters);
                Assert.AreEqual(10 ,SuppField.OldDeviceName);
                Assert.AreEqual(13 ,SuppField.Expanded);
                Assert.AreEqual(15, SuppField.Gateway);
            });
        }
    }
}
