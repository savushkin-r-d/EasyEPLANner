using InterprojectExchange;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerTests.InterprojectExchangeTest
{
    public class PacInfoTest
    {
        [Test]
        public void CloneTest()
        {
            var pacInfo = new PacInfo()
            {
                ModelLoaded = true,
                IP = "10.0.0.10",
            };

            var pacInfoClone = pacInfo.Clone();

            Assert.Multiple(() =>
            {
                Assert.AreNotSame(pacInfoClone, pacInfo);
                Assert.AreEqual(pacInfo.ModelLoaded, pacInfoClone.ModelLoaded);
                Assert.AreEqual(pacInfo.Port, pacInfoClone.Port);
                Assert.AreEqual(pacInfo.IP, pacInfoClone.IP);
                Assert.AreEqual(pacInfo.CycleTime, pacInfoClone.CycleTime);
                Assert.AreEqual(pacInfo.Station, pacInfoClone.Station);
                Assert.AreEqual(pacInfo.GateEnabled, pacInfoClone.GateEnabled);
                Assert.AreEqual(pacInfo.EmulationEnabled, pacInfoClone.EmulationEnabled);
            });
        }
    }
}
