using NUnit.Framework;

namespace Tests.EplanDevices
{
    public class DeviceTest
    {
        [TestCaseSource(nameof(CompareTestData))]
        public void CompareTest(int expected, EplanDevice.IODevice x, 
            EplanDevice.IODevice y)
        {
            Assert.AreEqual(expected, EplanDevice.Device.Compare(x, y));
        }

        [TestCaseSource(nameof(CompareToTestData))]
        public void CompareToTest(int expected, EplanDevice.IODevice dev, 
            object otherDevice)
        {
            Assert.AreEqual(expected, dev.CompareTo(otherDevice));
        }

        static object[] EqualsTestSource =
        {
            new object[]
            {
                new EplanDevice.FQT("TANK1FQT2", "+TANK1-FQT2",
                    "Test device", 2, "TANK", 1, "DeviceArticle"),
                new EplanDevice.QT("KOAG4QT1", "+KOAG4-QT1",
                    "Test device", 1, "KOAG", 4, "Test article"),
                false
            },
            new object[]
            {
                new EplanDevice.QT("KOAG4QT1", "+KOAG4-QT1",
                    "Test device", 1, "KOAG", 4, "Test article"),
                new EplanDevice.QT("KOAG4QT1", "+KOAG4-QT1",
                    "Test device", 1, "KOAG", 4, "Test article"),
                true
            },
            new object[]
            {
                new EplanDevice.FQT("TANK1FQT2", "+TANK1-FQT2",
                    "Test device", 2, "TANK", 1, "DeviceArticle"),
                null,
                false
            },
        };

        [TestCaseSource(nameof(EqualsTestSource))]
        public void EqualsTest(EplanDevice.Device device1,
            EplanDevice.Device device2, bool expected)
        {
            Assert.AreEqual(expected, device1.Equals(device2));
        }

        


        /// <summary>
        /// 1 - ожидаемое значение,
        /// 2 - первое устройство
        /// 3 - второе устройство
        /// </summary>
        /// <returns></returns>
        public static object[] CompareTestData()
        {
            return new object[]
            {
                new object[] 
                { 
                    0, 
                    null , 
                    null
                },
                new object[]
                { 
                    -1, 
                    null, 
                    new EplanDevice.AI("KOAG4AI1", "+KOAG4-AI1", "Test device",
                        1, "KOAG", 4)
                },
                new object[] 
                { 
                    1, 
                    new EplanDevice.AI("KOAG4AI1", "+KOAG4-AI1", "Test device",
                        1, "KOAG", 4),
                    null
                },
                new object[] 
                {
                    -1, 
                    new EplanDevice.LT("LINE1LT2", "+LINE1-LT2", "Test device",
                        2, "LINE", 1, "DeviceArticle"), 
                    new EplanDevice.AI("KOAG4AI1", "+KOAG4-AI1", "Test device",
                        1, "KOAG", 4) 
                },
                new object[] 
                { 
                    1, 
                    new EplanDevice.AI("KOAG4AI1", "+KOAG4-AI1", "Test device",
                        1, "KOAG", 4),
                    new EplanDevice.LT("LINE1LT2", "+LINE1-LT2", "Test device",
                        2, "LINE", 1, "DeviceArticle") 
                },
                new object[] 
                { 
                    0, 
                    new EplanDevice.LT("LINE1LT2", "+LINE1-LT2", "Test device",
                        2, "LINE", 1, "DeviceArticle"),
                    new EplanDevice.LT("LINE1LT2", "+LINE1-LT2", "Test device",
                        2, "LINE", 1, "DeviceArticle") 
                },
                new object[]
                {
                    -1,
                    new EplanDevice.FQT("TANK1FQT2", "+TANK1-FQT2","Test device",
                        2, "TANK", 1, "DeviceArticle"),
                    new EplanDevice.LT("LINE1LT2", "+LINE1-LT2", "Test device",
                        2, "LINE", 1, "DeviceArticle")
                },
                new object[]
                {
                    -1,
                    new EplanDevice.LT("LINE1LT2", "+LINE1-LT2", "Test device",
                        2, "LINE", 1, "DeviceArticle"),
                    new EplanDevice.QT("KOAG4QT1", "+KOAG4-QT1", "Test device",
                        1, "KOAG", 4, "Test article")
                },
                new object[]
                {
                    -1,
                    new EplanDevice.LT("LINE1LT1", "+LINE1-LT1", "Test device",
                        1, "LINE", 1, "DeviceArticle"),
                    new EplanDevice.HA("CW_TANK3HA3", "+CW_TANK3-HA3", "Test device",
                        3, "CW_TANK", 3, "DeviceArticle")
                },
            };
        }

        /// <summary>
        /// 1 - ожидаемое значение,
        /// 2 - устройство на котором будет вызываться CompareTO
        /// 3 - устройство которое будет проверяться
        /// </summary>
        /// <returns></returns>
        public static object[] CompareToTestData()
        {
            return new object[] 
            {
                new object[] 
                {
                    1,
                    new EplanDevice.HA("CW_TANK3HA3", "+CW_TANK3-HA3", "Test device",
                        3, "CW_TANK", 3, "DeviceArticle"), 
                    null
                },
                new object[]
                {
                    -1,
                    new EplanDevice.VC("TANK2VC1", "+TANK2-VC1", "Test device",
                        1, "TANK", 2, "DeviceArticle"),
                    new EplanDevice.HA("CW_TANK3HA3", "+CW_TANK3-HA3", "Test device",
                        3, "CW_TANK", 3, "DeviceArticle"),
                },
                new object[]
                {
                    1,
                    new EplanDevice.VC("TANK2VC2", "+TANK2-VC2", "Test device",
                        2, "TANK", 2, "DeviceArticle"),
                    new EplanDevice.VC("TANK2VC1", "+TANK2-VC1", "Test device",
                        1, "TANK", 2, "DeviceArticle"),
                }
            };
        }
    }
}
