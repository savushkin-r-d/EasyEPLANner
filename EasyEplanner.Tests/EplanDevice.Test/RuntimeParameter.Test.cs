using EplanDevice;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EplanDevices
{
    public class RuntimeParameterTest
    {

        [Test]
        public void ImplicitConvertToStringTest()
        {
            string R_EXTRA_OFFSET = IODevice.RuntimeParameter.R_EXTRA_OFFSET;
            Assert.AreEqual(nameof(IODevice.RuntimeParameter.R_EXTRA_OFFSET), R_EXTRA_OFFSET);
        }

        [Test]
        public void ExplicitConvertStringToParameterTest()
        {
            IODevice.RuntimeParameter R_EXTRA_OFFSET = (IODevice.RuntimeParameter)"R_EXTRA_OFFSET";
            Assert.AreSame(IODevice.RuntimeParameter.R_EXTRA_OFFSET, R_EXTRA_OFFSET);
        }


        [Test]
        public void ExplicitConvertStringToParameterTest_UNDEFINED()
        {
            IODevice.RuntimeParameter UNDEFINED = (IODevice.RuntimeParameter)"UNDEFINED";
            Assert.AreSame("UNDEFINED", (string)UNDEFINED);
        }
    }
}
