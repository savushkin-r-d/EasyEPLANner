using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterprojectExchange;
using NUnit.Framework;

namespace EasyEplannerTests.InterprojectExchangeTest
{
    public class DeviceSignalsInfoTest
    {
        [TestCaseSource(nameof(CountCompareTest_Cases))]
        public void CountCompareTest_CheckErrorString(DeviceSignalsInfo current,
            DeviceSignalsInfo other, string expectedError)
        {
            Assert.AreEqual(expectedError, current.CountCompare(other));
        }

        private static object[] CountCompareTest_Cases()
        {
            var signals_1 = new DeviceSignalsInfo();
            var signals_2 = new DeviceSignalsInfo();
            var signals_3 = new DeviceSignalsInfo();
            var signals_4 = new DeviceSignalsInfo();

            signals_1.AO.Add("AO1");
            signals_1.AO.Add("AO2");

            signals_2.AO.Add("AI1");
            signals_2.AO.Add("AI2");

            signals_3.AI.Add("AI1");
            signals_3.AI.Add("AI2");

            signals_4.AO.Add("AI1");
            signals_4.AO.Add("AI2");
            signals_4.DO.Add("DO1");
            signals_4.DI.Add("DI1");
            signals_4.AI.Add("AI1");

            return new object[]
            {
                new object[]
                {
                    signals_1,
                    signals_2,
                    "",
                },
                new object[]
                {
                    signals_1,
                    signals_3,
                    "AO, AI",
                },
                new object[]
                {
                    signals_1,
                    signals_4,
                    "AI, DO, DI",
                },
            };
        }
    }
}
