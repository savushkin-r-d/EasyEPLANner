using EasyEPlanner;
using IO;
using NUnit.Framework;
using StaticHelper;
using System.Reflection;

namespace EasyEPlanner.Devices.Tests
{
    public class DeviceBinderTest
    {
        [TestCase("text\r\nline", "text\nline")]
        [TestCase("  spaced  ", "spaced")]
        [TestCase(null, "")]
        public void NormalizeFunctionalText_TrimsCarriageReturns(string input,
            string expected)
        {
            var result = InvokeStatic<string>("NormalizeFunctionalText", input);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void FunctionalTextContainsDevice_FindsDeviceInFunctionalText()
        {
            const string functionalText = "+TANK2-DO1\r\nОписание";
            var binder = CreateBinder();

            var contains = Invoke<bool>(binder, "FunctionalTextContainsDevice",
                functionalText, "+TANK2-DO1");

            Assert.IsTrue(contains);
        }

        [Test]
        public void FunctionalTextContainsDevice_ReturnsFalseForMissingDevice()
        {
            const string functionalText = "OTHER\r\nОписание";
            var binder = CreateBinder();

            var contains = Invoke<bool>(binder, "FunctionalTextContainsDevice",
                functionalText, "+TANK2-DO1");

            Assert.IsFalse(contains);
        }

        private static DeviceBinder CreateBinder()
        {
            var apiHelper = new ApiHelper();
            return new DeviceBinder(apiHelper,
                new IOHelper(new ProjectHelper(apiHelper)));
        }

        private static T InvokeStatic<T>(string methodName, params object[] args)
        {
            var method = typeof(DeviceBinder).GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Static);
            return (T)method.Invoke(null, args);
        }

        private static T Invoke<T>(object instance, string methodName,
            params object[] args)
        {
            var method = instance.GetType().GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)method.Invoke(instance, args);
        }
    }
}
