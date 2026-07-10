using EasyEPlanner.FileSavers.XML;
using EplanDevice;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerTests.FileSaverTest.XMLTest
{
    public class DriverTest
    {
        [Test]
        public void Driver_ExcludeLoggTest()
        {
            var watchdog = new WATCHDOG("WATCHDOG1", "WATCHDOG1", "", 1, "", 0, Mock.Of<IDeviceManager>());
            watchdog.SetSubType("WATCHDOG");
            
            var signalDO = new DO("DO1", "-DO1", "", 1, "", 0);
            signalDO.SetSubType("DO");

            var signalDI = new DI("DI1", "-DI1", "", 1, "", 0);
            signalDI.SetSubType("DI");

            var signalAO = new AO("AO1", "-AO1", "", 1, "", 0);
            signalAO.SetSubType("AO");

            var signalAI = new AI("AI1", "-AI1", "", 1, "", 0);
            signalAI.SetSubType("AI");

            watchdog.Properties[IODevice.Property.DO_dev] = "DO1";
            watchdog.Properties[IODevice.Property.DI_dev] = "DI1";
            watchdog.Properties[IODevice.Property.AO_dev] = "AO1";
            watchdog.Properties[IODevice.Property.AI_dev] = "AI1";

            var devicemanager = Mock.Of<IDeviceManager>(dm => 
                dm.Devices == new List<IODevice> { watchdog, signalDO, signalDI, signalAO, signalAI } &&
                dm.GetDevice("DO1") == signalDO &&
                dm.GetDevice("DI1") == signalDI &&
                dm.GetDevice("AO1") == signalAO &&
                dm.GetDevice("AI1") == signalAI);

            var driver = new Driver(devicemanager);

            Assert.Multiple(() =>
            {
                Assert.IsFalse(Subtype.ChannelsLogging["DO1.ST"]);
                Assert.IsFalse(Subtype.ChannelsLogging["DI1.ST"]);
                Assert.IsFalse(Subtype.ChannelsLogging["AO1.V"]);
                Assert.IsFalse(Subtype.ChannelsLogging["AI1.V"]);
            });
        }

    }
}
