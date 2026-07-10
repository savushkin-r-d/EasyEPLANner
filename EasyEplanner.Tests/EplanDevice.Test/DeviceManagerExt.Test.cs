using EplanDevice;
using Moq;
using NUnit.Framework;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerTests.EplanDeviceTest
{
    public class DeviceManagerExt
    {
        [Test]
        public void GetTypesCount()
        {
            var V1 = new V("OBJ1V", "+OBJ1-V1", "", 1, "OBJ", 1, "");
            V1.SetSubType(DeviceSubType.V_DO1.ToString());
            var V2 = new V("OBJ1V2", "+OBJ1-V2", "", 2, "OBJ", 1, "");
            V2.SetSubType(DeviceSubType.V_DO1.ToString());
            var V3 = new V("OBJ1V3", "+OBJ1-V3", "", 3, "OBJ", 1, "");
            V3.SetSubType(DeviceSubType.V_DO2.ToString());
            var CapV = new V("OBJ1V4", "+OBJ1-V4", CommonConst.Cap, 4, "OBJ", 1, "");
            CapV.SetSubType(DeviceSubType.V_DO2.ToString());

            var manager = Mock.Of<IDeviceManager>(dm => 
                dm.Devices == new List<IODevice>() { V1, V2, V3, CapV });

            var summary = new SummaryDevices(manager);

            var result = summary.NumberUsedTypes();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, result["V"].Count);
                Assert.AreEqual(2, result["V"]["V_DO1"]);
                Assert.AreEqual(1, result["V"]["V_DO2"]);
            });
        }

        [Test]
        public void ControlChannelsCounter()
        {

            var summary = Mock.Of<ISummaryDevices>(s => 
                s.NumberUsedTypes() == new Dictionary<string, Dictionary<string, int>>()
                {
                    { "V", new Dictionary<string, int>()
                        {
                            { "V_DO1", 5 },
                            { "V_DO2", 10 },
                            { "V_VIRT", 3 },
                        }
                    },
                });
            
            var counter = new ControlChannelsCounter(summary);
            counter.AddChannelsCount("V_DO1", 0, 1, 0, 0);
            counter.AddChannelsCount("V_DO2", 0, 2, 0, 0);
            counter.AddChannelsCount("V_VIRT", 0, 0, 1, 1);

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(new int[] { 0, 1, 0, 0 }, counter.GetChannelsCount("V_DO1"));
                CollectionAssert.AreEqual(new int[] { 0, 0, 0, 0 }, counter.GetChannelsCount("UNDEFINED"));
                CollectionAssert.AreEqual(new int[] { 0, 0, 0, 0 }, counter.GetChannelsCount("V_DO2_DI2"));

                Assert.AreEqual(DeviceSubType.NONE, counter.GetSubtypeControlChannels("UNDEFINED").SubType);
                Assert.AreEqual(DeviceSubType.V_DO1, counter.GetSubtypeControlChannels("V_DO1").SubType);
                CollectionAssert.AreEqual(new int[] { 0, 1, 0, 0 }, counter.GetSubtypeControlChannels("V_DO1").ChannelsCount);
                Assert.AreEqual(DeviceSubType.V_DO2_DI2, counter.GetSubtypeControlChannels("V_DO2_DI2").SubType);
                CollectionAssert.AreEqual(new int[] { 0, 0, 0, 0 }, counter.GetSubtypeControlChannels("V_DO2_DI2").ChannelsCount);

                (int DI, int DO, int AI, int AO) = counter.CalculateUsedChannelsCount();

                Assert.AreEqual(0, DI);
                Assert.AreEqual(25, DO);
                Assert.AreEqual(3, AI);
                Assert.AreEqual(3, AO);
            });
        }
    }
}
