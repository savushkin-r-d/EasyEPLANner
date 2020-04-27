using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    public class IODeviceTest
    {
        [TestCaseSource(nameof(TestSortingChannelsForVDeviceData))]
        public void TestSortingChannelsForVDevice(Device.IODevice dev, 
            string subType, string[] expected)
        {
            dev.SetSubType(subType);
            dev.sortChannels();
            string[] actual = dev.Channels
                .Where(x => x.Comment != "")
                .Select(x => x.Comment ).ToArray();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// 1 - Устройство V (клапан) для тестирования
        /// 2 - Задаваемый подтип
        /// 3 - Ожидаемое значение
        /// </summary>
        /// <returns></returns>
        public static object[] TestSortingChannelsForVDeviceData()
        {
            return new object[] 
            {
                new object[] 
                {
                    VTest.GetRandomVDevice(), 
                    "V_DO2", 
                    new string[] {"Открыть", "Закрыть"} 
                },
                new object[] 
                {
                    VTest.GetRandomVDevice(),
                    "V_DO1_DI2",
                    new string[] {"Открыт", "Закрыт"} 
                },
                new object[] 
                {
                    VTest.GetRandomVDevice(), 
                    "V_DO2_DI2",
                    new string[] {"Открыть", "Закрыть", "Открыт", "Закрыт"} },
                new object[] 
                {
                    VTest.GetRandomVDevice(),
                    "V_MIXPROOF",
                    new string[] {"Открыть", "Открыть ВС", "Открыть НС", 
                        "Открыт", "Закрыт"} 
                },
                new object[] 
                {
                    VTest.GetRandomVDevice(),
                    "V_BOTTOM_MIXPROOF",
                    new string[] {"Открыть", "Открыть мини", "Открыть НС", 
                        "Открыт", "Закрыт"} 
                },
            };
        }
    }
}
