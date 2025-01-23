using EasyEPlanner.ProjectImportICP;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerTests.ProjectImportICPTest
{
    public class DeviceNameMatchingSaverTest
    {
        [Test]
        public void SaveImportDevices()
        {
            var devices = new List<ImportDevice>()
            {
                new ImportDevice() { WagoType = "V", FullNumber = 101, Object = "LINE1",  Type = "V", Number = 1, Description = "desc LINE1V1"},
                new ImportDevice() { WagoType = "V", FullNumber = 102, Object = "LINE1",  Type = "V", Number = 2, Description = "desc; LINE1V2"},
            };

            var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "DeviceNameMatching.csv");

            DeviceNameMatchingSaver.Save(filePath, devices);

            Assert.AreEqual(
                "Название в ICP CON;Новое название;Описание\n" +
                "V101;LINE1V1;desc LINE1V1\n" +
                "V102;LINE1V2;\"desc; LINE1V2\"\n",
                File.ReadAllText(filePath));
        }
    }
}
