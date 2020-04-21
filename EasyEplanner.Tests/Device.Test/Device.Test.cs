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
                    new Device.AI("KOAG4AI1", "Test device", 1, "KOAG", 4)
                },
                new object[] 
                { 
                    1, 
                    new Device.AI("KOAG4AI1", "Test device", 1, "KOAG", 4),
                    null
                },
                new object[] 
                {
                    -1, 
                    new Device.LT("LINE1LT2", "Test device", 2, "LINE", 1, 
                    "DeviceArticle"), 
                    new Device.AI("KOAG4AI1", "Test device", 1, "KOAG", 4) 
                },
                new object[] 
                { 
                    1, 
                    new Device.AI("KOAG4AI1", "Test device", 1, "KOAG", 4),
                    new Device.LT("LINE1LT2", "Test device", 2, "LINE", 1, 
                    "DeviceArticle") 
                },
                new object[] 
                { 
                    0, 
                    new Device.LT("LINE1LT2", "Test device", 2, "LINE", 1, 
                    "DeviceArticle"),
                    new Device.LT("LINE1LT2", "Test device", 2, "LINE", 1, 
                    "DeviceArticle") 
                },
                new object[]
                {
                    -1,
                    new Device.FQT("TANK1FQT2", "Test device", 2, "TANK", 1,
                    "DeviceArticle"),
                    new Device.LT("LINE1LT2", "Test device", 2, "LINE", 1, 
                    "DeviceArticle")
                },
                new object[]
                {
                    -1,
                    new Device.LT("LINE1LT2", "Test device", 2, "LINE", 1, 
                    "DeviceArticle"),
                    new Device.QT("KOAG4QT1", "Test device", 1, "KOAG", 4, 
                    "Test article")
                },
                new object[]
                {
                    -1,
                    new Device.LT("LINE1LT1", "Test device", 1, "LINE", 1, 
                    "DeviceArticle"),
                    new Device.HA("CW_TANK3HA3", "Test device", 3, "CW_TANK", 
                    3, "DeviceArticle")
                },
            };
        }
    }
}
