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
    public class LIFE_DEVICETest
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
                new object[] { nameof(DeviceSubType.LIFEBIT), nameof(DeviceSubType.LIFEBIT), Random_LIFE_DEVICE() },
                new object[] { nameof(DeviceSubType.LIFECOUNTER), nameof(DeviceSubType.LIFECOUNTER), Random_LIFE_DEVICE() },
                new object[] { string.Empty, "Incorrect", Random_LIFE_DEVICE() },
            };
        }

        [Test]
        public void GetDeviceProperties()
        {
            var lifebit = Random_LIFE_DEVICE();
            lifebit.SetSubType(nameof(DeviceSubType.LIFEBIT));

            var lifecounter = Random_LIFE_DEVICE();
            lifecounter.SetSubType(nameof(DeviceSubType.LIFECOUNTER));

            var expected = new Dictionary<string, int> {
                { IODevice.Tag.ST , 1 },
                { IODevice.Tag.M , 1 },
                { IODevice.Parameter.P_DT , 1 }
            };

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(expected, lifebit.GetDeviceProperties(lifebit.DeviceType, lifebit.DeviceSubType));
                CollectionAssert.AreEqual(expected, lifecounter.GetDeviceProperties(lifecounter.DeviceType, lifecounter.DeviceSubType));
            });
        }


        [TestCaseSource(nameof(Check_CaseSource))]
        public void Check(IODevice device, string subtype, string dev, string expected)
        {
            device.SetSubType(subtype);
            device.Properties[IODevice.Property.DEV] = dev;
            
            Assert.IsTrue(device.Check().Contains(expected));
        }

        private static object[] Check_CaseSource()
        {
            return new object[]
            {
                new object[] { Random_LIFE_DEVICE(), nameof(DeviceSubType.LIFEBIT), "DI1", string.Empty },
                new object[] { Random_LIFE_DEVICE(), nameof(DeviceSubType.LIFECOUNTER), "AI1", string.Empty },

                new object[] { Random_LIFE_DEVICE(), nameof(DeviceSubType.LIFEBIT), "AI1", "привязано устройство неверного типа"},
                new object[] { Random_LIFE_DEVICE(), nameof(DeviceSubType.LIFECOUNTER), "DI1", "привязано устройство неверного типа" },

                new object[] { Random_LIFE_DEVICE(), nameof(DeviceSubType.LIFEBIT), "STUB", "привязано неизвестное устройство" },
                new object[] { Random_LIFE_DEVICE(), nameof(DeviceSubType.LIFEBIT), "", string.Empty },
            };
        }


        private static IODevice Random_LIFE_DEVICE()
        {
            var dmanager = Mock.Of<IDeviceManager>(m =>
                m.GetDevice("DI1") == new DI("", "", "", 1, "", 1) &&
                m.GetDevice("AI1") == new AI("", "", "", 1, "", 1) &&
                m.GetDevice("STUB") == new AI("", "", StaticHelper.CommonConst.Cap, 1, "", 1));

            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new LIFE_DEVICE("KOAG4LIFE_DEVICE1", "+KOAG4-LIFE_DEVICE1",
                        "Test device", 1, "KOAG", 4, dmanager);
                case 2:
                    return new LIFE_DEVICE("LINE1G2", "+LINE1-G2",
                        "Test device", 2, "LINE", 1, dmanager);
                case 3:
                    return new LIFE_DEVICE("TANK2LIFE_DEVICE1", "+TANK2-LIFE_DEVICE1",
                        "Test device", 1, "TANK", 2, dmanager);
                default:
                    return new LIFE_DEVICE("CW_TANK3LIFE_DEVICE3", "+CW_TANK3-LIFE_DEVICE3",
                        "Test device", 3, "CW_TANK", 3, dmanager);
            }
        }
    }
}
