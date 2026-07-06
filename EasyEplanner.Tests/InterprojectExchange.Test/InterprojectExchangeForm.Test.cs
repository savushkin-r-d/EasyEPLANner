using System.Reflection;
using InterprojectExchange;
using NUnit.Framework;

namespace EasyEplannerTests.InterprojectExchangeTest
{
    public class InterprojectExchangeFormTest
    {
        [TestCase("DO_VIRT", "DO")]
        [TestCase("DI_VIRT", "DI")]
        [TestCase("AI_VIRT", "AI")]
        [TestCase("DO", "DO")]
        public void NormalizeChannelType_VirtAndBaseTypes_ReturnsBaseChannel(
            string deviceType, string expected)
        {
            var method = typeof(InterprojectExchangeForm).GetMethod(
                "NormalizeChannelType",
                BindingFlags.NonPublic | BindingFlags.Static);

            var result = method.Invoke(null, new object[] { deviceType });

            Assert.AreEqual(expected, result);
        }
    }
}
