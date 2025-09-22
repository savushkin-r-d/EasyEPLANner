using EplanDevice;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EplanDevice.IODevice;

namespace Tests.EplanDevices
{
    public class EYTest
    {

        [TestCase("", DeviceSubType.DEV_CONV_AO2)]
        [TestCase("DEV_CONV_AO2", DeviceSubType.DEV_CONV_AO2)]
        [TestCase("INCORECT", DeviceSubType.NONE)]
        public void SetSubType(string subtypestring, DeviceSubType expectedSubType)
        {
            var dev = GetEY();
            dev.SetSubType(subtypestring);

            Assert.AreEqual(expectedSubType, dev.DeviceSubType);
        }

        [Test]
        public void Type()
        {
            var dev = GetEY();
            dev.SetSubType("");

            Assert.AreEqual(DeviceType.EY, dev.DeviceType);
        }

        [Test]
        public void GetDeviceSubTypeString()
        {
            var dev = GetEY();
            dev.SetSubType(DeviceSubType.DEV_CONV_AO2.ToString());

            Assert.AreEqual(DeviceSubType.DEV_CONV_AO2.ToString(),
                dev.GetDeviceSubTypeStr(dev.DeviceType, dev.DeviceSubType));
        }

        [Test]
        public void GetDeiceProperties()
        {
            var dev = GetEY();
            dev.SetSubType(DeviceSubType.DEV_CONV_AO2.ToString());

            var properties = dev.GetDeviceProperties(dev.DeviceType, dev.DeviceSubType);

            CollectionAssert.AreEqual(new Dictionary<ITag, int>() {
                { Tag.ST, 1 },
                { Tag.V, 1 },
                { Tag.V2, 1 },
                { Tag.ERR, 1 },
            }, properties);
        }


        private static IODevice GetEY() =>  new EY("OBJ1EY1", "+OBJ1-EY1", "descr", 1, "OBJ", 1, "");
    }
}
