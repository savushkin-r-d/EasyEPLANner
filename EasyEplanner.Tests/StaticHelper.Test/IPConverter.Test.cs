using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using EasyEplanner;
using StaticHelper;

namespace EasyEplannerTests.StaticHelperTest
{
    public class IPConverterTest
    {
        [TestCase("0.0.0.1", 1)]
        [TestCase("0.2", 2)]
        [TestCase("1.2", 258)]
        [TestCase("3", 3)]
        [TestCase("0.1.3", 259)]
        [TestCase("1.1.1.1", 16843009)]
        [TestCase("10.0.162.230", 167813862)]
        [TestCase("10.100.162.240", 174367472)]
        public void ConvertIPStrToLong_CheckParsedValue(string IPStr, long expectedIPLong)
        {
            Assert.AreEqual(expectedIPLong, IPConverter.ConvertIPStrToLong(IPStr));
        }
    }
}
