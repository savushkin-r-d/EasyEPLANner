using EplanDevice;
using IO.ViewModel;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EplanDevices
{
    public class WATCHDOGTest
    {
        [TestCaseSource(nameof(GetDeviceSubTypeStrTestData))]
        public void GetDeviceSubTypeStr_NewDev_ReturnsDevType(
            string expectedType, string subType, IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedType, device.GetDeviceSubTypeStr(
                device.DeviceType, device.DeviceSubType));
        }

        private static object[] GetDeviceSubTypeStrTestData()
        {
            return new object[]
            {
                new object[] { nameof(DeviceSubType.WATCHDOG), nameof(DeviceSubType.WATCHDOG), Random_WATCHDOG() },
                new object[] { string.Empty, "Incorrect", Random_WATCHDOG() },
            };
        }

        [Test]
        public void GetDeviceProperties()
        {
            var watchcdog = Random_WATCHDOG();
            watchcdog.SetSubType(nameof(DeviceSubType.WATCHDOG));

            var expected = new Dictionary<ITag, int> {
                { IODevice.Tag.ST , 1 },
                { IODevice.Tag.M , 1 },
                { IODevice.Parameter.P_T_GEN , 1 },
                { IODevice.Parameter.P_T_ERR , 1 },
            };


            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(expected, watchcdog.GetDeviceProperties(watchcdog.DeviceType, watchcdog.DeviceSubType));
            });
        }


        [TestCaseSource(nameof(Check_CaseSource))]
        public void Check(IODevice device, string subtype, string dev, string expected)
        {
            device.SetSubType(subtype);
            device.Properties[IODevice.Property.AO_dev] = dev;
            
            Assert.IsTrue(device.Check().Contains(expected));
        }

        private static object[] Check_CaseSource()
        {
            return new object[]
            {
                new object[] { Random_WATCHDOG(), nameof(DeviceSubType.WATCHDOG), "DI1", string.Empty },
                new object[] { Random_WATCHDOG(), nameof(DeviceSubType.WATCHDOG), "AI1", string.Empty },
                new object[] { Random_WATCHDOG(), nameof(DeviceSubType.WATCHDOG), "STUB", "привязано неизвестное устройство" },
            };
        }


        private static IODevice Random_WATCHDOG()
        {
            var dmanager = Mock.Of<IDeviceManager>(m =>
                m.GetDevice("DI1") == new DI("", "", "", 1, "", 1) &&
                m.GetDevice("AI1") == new AI("", "", "", 1, "", 1) &&
                m.GetDevice("STUB") == new AI("", "", StaticHelper.CommonConst.Cap, 1, "", 1));

            var randomizer = new Random();
            int value = randomizer.Next(1, 4);
            switch (value)
            {
                case 1:
                    return new WATCHDOG("KOAG4WATCHDOG1", "+KOAG4-WATCHDOG1",
                        "Test device", 1, "KOAG", 4, dmanager);
                case 2:
                    return new WATCHDOG("LINE1G2", "+LINE1-G2",
                        "Test device", 2, "LINE", 1, dmanager);
                case 3:
                    return new WATCHDOG("TANK2WATCHDOG1", "+TANK2-WATCHDOG1",
                        "Test device", 1, "TANK", 2, dmanager);
                default:
                    return new WATCHDOG("CW_TANK3WATCHDOG3", "+CW_TANK3-WATCHDOG3",
                        "Test device", 3, "CW_TANK", 3, dmanager);
            }
        }
    }
}
