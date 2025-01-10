using EasyEPlanner.ProjectImportICP;
using EplanDevice;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerTests.ProjectImportICPTest
{
    public class ImportDefaultDeviceParameterTest
    {

        [Test]
        public void CreateInstance()
        {
            var par = new ImportDefaultDeviceParamter("V", IODevice.Parameter.P_DT);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(par);
                Assert.AreEqual("V", par.DeviceType);
                Assert.AreEqual(IODevice.Parameter.P_DT, par.Parameter);
                Assert.AreEqual(0, par.DefaultValue);
            });
        }
    }
}
