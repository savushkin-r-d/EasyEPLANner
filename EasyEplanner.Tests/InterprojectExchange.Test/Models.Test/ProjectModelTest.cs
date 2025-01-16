using InterprojectExchange;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerEasyEplannerTests.InterprojectExchangeTest
{

    public class ProjectModelTest
    {
        [Test]
        public void LoadedGetSetTest()
        {
            var model = new AdvancedProjectModel
            {
                Loaded = true
            };

            Assert.IsTrue(model.Loaded);
        }

        [Test]
        public void AddPlcData()
        {
            var model = new AdvancedProjectModel
            {
                Loaded = true
            };

            var currentModel = new CurrentProjectModel
            {
                Loaded = true
            };

            model.AddPLCData(true, 1, 1, 1, true, 1, "pac_name");
            currentModel.AddPLCData(true, 1, 1, 1, true, 1, "pac_name");
            currentModel.SelectedAdvancedProject = "pac_name";

            Assert.Multiple(() =>
            {
                Assert.IsTrue(model.PacInfo.ModelLoaded);
                Assert.IsTrue(model.PacInfo.EmulationEnabled);
                Assert.IsTrue(model.PacInfo.GateEnabled);

                Assert.IsTrue(currentModel.PacInfo.ModelLoaded);
                Assert.IsTrue(currentModel.PacInfo.EmulationEnabled);
                Assert.IsTrue(currentModel.PacInfo.GateEnabled);

                Assert.AreEqual(1, model.PacInfo.CycleTime);
                Assert.AreEqual(1, model.PacInfo.TimeOut);
                Assert.AreEqual(1, model.PacInfo.Port);
                Assert.AreEqual(1, model.PacInfo.Station);

                Assert.AreEqual(1, currentModel.PacInfo.CycleTime);
                Assert.AreEqual(1, currentModel.PacInfo.TimeOut);
                Assert.AreEqual(1, currentModel.PacInfo.Port);
                Assert.AreEqual(1, currentModel.PacInfo.Station);

                Assert.AreEqual("pac_name", model.ProjectName);
                Assert.AreEqual("pac_name", currentModel.ProjectName);
            });
        }
    }
}
