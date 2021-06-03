using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class DeviceTest
    {
        [TestCaseSource(nameof(CompareTestData))]
        public void CompareTest(int expected, Device.IODevice x, 
            Device.IODevice y)
        {
            Assert.AreEqual(expected, Device.Device.Compare(x, y));
        }

        [TestCaseSource(nameof(CompareToTestData))]
        public void CompareToTest(int expected, Device.IODevice dev, 
            object otherDevice)
        {
            Assert.AreEqual(expected, dev.CompareTo(otherDevice));
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
                    new Device.AI("KOAG4AI1", "+KOAG4-AI1", "Test device",
                        1, "KOAG", 4)
                },
                new object[] 
                { 
                    1, 
                    new Device.AI("KOAG4AI1", "+KOAG4-AI1", "Test device",
                        1, "KOAG", 4),
                    null
                },
                new object[] 
                {
                    -1, 
                    new Device.LT("LINE1LT2", "+LINE1-LT2", "Test device",
                        2, "LINE", 1, "DeviceArticle"), 
                    new Device.AI("KOAG4AI1", "+KOAG4-AI1", "Test device",
                        1, "KOAG", 4) 
                },
                new object[] 
                { 
                    1, 
                    new Device.AI("KOAG4AI1", "+KOAG4-AI1", "Test device",
                        1, "KOAG", 4),
                    new Device.LT("LINE1LT2", "+LINE1-LT2", "Test device",
                        2, "LINE", 1, "DeviceArticle") 
                },
                new object[] 
                { 
                    0, 
                    new Device.LT("LINE1LT2", "+LINE1-LT2", "Test device",
                        2, "LINE", 1, "DeviceArticle"),
                    new Device.LT("LINE1LT2", "+LINE1-LT2", "Test device",
                        2, "LINE", 1, "DeviceArticle") 
                },
                new object[]
                {
                    -1,
                    new Device.FQT("TANK1FQT2", "+TANK1-FQT2","Test device",
                        2, "TANK", 1, "DeviceArticle"),
                    new Device.LT("LINE1LT2", "+LINE1-LT2", "Test device",
                        2, "LINE", 1, "DeviceArticle")
                },
                new object[]
                {
                    -1,
                    new Device.LT("LINE1LT2", "+LINE1-LT2", "Test device",
                        2, "LINE", 1, "DeviceArticle"),
                    new Device.QT("KOAG4QT1", "+KOAG4-QT1", "Test device",
                        1, "KOAG", 4, "Test article")
                },
                new object[]
                {
                    -1,
                    new Device.LT("LINE1LT1", "+LINE1-LT1", "Test device",
                        1, "LINE", 1, "DeviceArticle"),
                    new Device.HA("CW_TANK3HA3", "+CW_TANK3-HA3", "Test device",
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
                    new Device.HA("CW_TANK3HA3", "+CW_TANK3-HA3", "Test device",
                        3, "CW_TANK", 3, "DeviceArticle"), 
                    null
                },
                new object[]
                {
                    -1,
                    new Device.VC("TANK2VC1", "+TANK2-VC1", "Test device",
                        1, "TANK", 2, "DeviceArticle"),
                    new Device.HA("CW_TANK3HA3", "+CW_TANK3-HA3", "Test device",
                        3, "CW_TANK", 3, "DeviceArticle"),
                },
                new object[]
                {
                    1,
                    new Device.VC("TANK2VC2", "+TANK2-VC2", "Test device",
                        2, "TANK", 2, "DeviceArticle"),
                    new Device.VC("TANK2VC1", "+TANK2-VC1", "Test device",
                        1, "TANK", 2, "DeviceArticle"),
                }
            };
        }
    }
}
