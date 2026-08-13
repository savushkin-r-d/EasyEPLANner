using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterprojectExchange;
using NUnit.Framework;
using Moq;
using System.Reflection;
using EasyEPlanner;

namespace EasyEplannerTests.InterprojectExchangeTest
{
    public class InterprojectExchangeTest
    {
        [Test]
        public void CheckBindingSignalsTest_CheckErrorString()
        {
            string expected = "remote_gateways: adv_prj - AI\nshared_devices: adv_prj - AI\n";

            var mainReceiver = new DeviceSignalsInfo();
            mainReceiver.AI.Add("AI1");

            var mainSource = new DeviceSignalsInfo();
            var advancedSource = new DeviceSignalsInfo();

            var advancedReceiver = new DeviceSignalsInfo();
            advancedReceiver.AI.Add("AI2");

            var mainModel = new Mock<CurrentProjectModel>();

            mainModel.Setup(obj => obj.ReceiverSignals).Returns(mainReceiver);
            mainModel.Setup(obj => obj.SourceSignals).Returns(mainSource);
            mainModel.Setup(obj => obj.ProjectName).Returns("main_prj");

            var advancedModel = new Mock<IProjectModel>();

            advancedModel.Setup(obj => obj.ReceiverSignals).Returns(advancedReceiver);
            advancedModel.Setup(obj => obj.SourceSignals).Returns(advancedSource);
            advancedModel.Setup(obj => obj.ProjectName).Returns("adv_prj");
            advancedModel.Setup(obj => obj.Loaded).Returns(true);
            advancedModel.SetupProperty(obj => obj.HasBindingError);

            var interprojectExchangeMock = new Mock<InterprojectExchange.InterprojectExchange>();

            interprojectExchangeMock.Setup(obj => obj.MainModel).Returns(mainModel.Object);

            var interprojectExchange = interprojectExchangeMock.Object;

            interprojectExchange.AddModel(mainModel.Object);
            interprojectExchange.AddModel(advancedModel.Object);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expected, interprojectExchange.CheckBindingSignals());
                Assert.IsTrue(advancedModel.Object.HasBindingError);
                Assert.IsTrue(advancedModel.Object.Loaded);
                CollectionAssert.AreEqual(
                    new[] { "AI1" }, mainReceiver.AI);
                CollectionAssert.AreEqual(
                    new[] { DeviceSignalsInfo.UnpairedSignal },
                    advancedSource.AI);
                CollectionAssert.AreEqual(
                    new[] { DeviceSignalsInfo.UnpairedSignal },
                    mainSource.AI);
                CollectionAssert.AreEqual(
                    new[] { "AI2" }, advancedReceiver.AI);
            });
        }

        [Test]
        public void PairUnpairedSignals_RestoresPairAndClearsError()
        {
            typeof(InterprojectExchange.InterprojectExchange)
                .GetField("eProjectManager", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, Mock.Of<IEProjectManager>(
                    m => m.GetModifyingCurrentProjectName() == "main"));

            typeof(InterprojectExchange.InterprojectExchange)
                .GetField("interprojectExchange", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, null);

            var interprojectExchange = InterprojectExchange.InterprojectExchange
                .GetInstance();

            var mainModel = new CurrentProjectModel
            {
                ProjectName = "main",
                SelectedAdvancedProject = "adv",
                Loaded = true
            };
            mainModel.SourceSignals.DO.AddRange(new[] { "DO1", "-", "-" });

            var advancedModel = new AdvancedProjectModel
            {
                ProjectName = "adv",
                Loaded = true,
                HasBindingError = true,
                Selected = true
            };
            advancedModel.ReceiverSignals.DO.AddRange(new[] { "-", "DI1", "DI2" });

            interprojectExchange.AddModel(mainModel);
            interprojectExchange.AddModel(advancedModel);
            interprojectExchange.ChangeEditMode(0);

            bool paired = interprojectExchange.PairUnpairedSignals(
                "DO", "DO1", "DI1");

            Assert.Multiple(() =>
            {
                Assert.IsTrue(paired);
                CollectionAssert.AreEqual(
                    new[] { "DO1", "-" }, mainModel.SourceSignals.DO);
                CollectionAssert.AreEqual(
                    new[] { "DI1", "DI2" },
                    advancedModel.ReceiverSignals.DO);
                Assert.IsTrue(advancedModel.HasBindingError);
            });

            bool filled = interprojectExchange.FillUnpairedSignal(
                "DO", "DI2", "DO2", fillAdvancedSide: false);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(filled);
                CollectionAssert.AreEqual(
                    new[] { "DO1", "DO2" }, mainModel.SourceSignals.DO);
                CollectionAssert.AreEqual(
                    new[] { "DI1", "DI2" },
                    advancedModel.ReceiverSignals.DO);
                Assert.IsFalse(advancedModel.HasBindingError);
            });

            typeof(InterprojectExchange.InterprojectExchange)
                .GetField("interprojectExchange", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, null);
        }


        [TestCaseSource(nameof(GetSignalsPairs_Test_CaseSrc))]
        public void GetSignalsPairs_Test(List<string> currSignals,
            List<string> advSignals, List<string[]> expected)
        {
            var result = typeof(InterprojectExchange.InterprojectExchange)
                .GetMethod("GetSignalsPairs", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { currSignals, advSignals }) as List<string[]>;

            CollectionAssert.AreEqual(expected, result);
        }

        public static object[] GetSignalsPairs_Test_CaseSrc = new object[]
        {
            new object[]
            {
                new List<string>{ "S1", "S2" },
                new List<string>{ "S1", "S2" },
                new List<string[]>{ new []{"S1", "S1"}, new []{"S2", "S2"}, },
            },
            new object[]
            {
                new List<string>{ "S1", "S2" },
                new List<string>{ "S1" },
                new List<string[]>{
                    new []{"S1", DeviceSignalsInfo.UnpairedSignal},
                    new []{"S2", DeviceSignalsInfo.UnpairedSignal},
                    new []{DeviceSignalsInfo.UnpairedSignal, "S1"},
                },
            },
            new object[]
            {
                new List<string>{ "S1", },
                new List<string>{ "S1", "S2" },
                new List<string[]>{
                    new []{"S1", DeviceSignalsInfo.UnpairedSignal},
                    new []{DeviceSignalsInfo.UnpairedSignal, "S1"},
                    new []{DeviceSignalsInfo.UnpairedSignal, "S2"},
                },
            },
            new object[]
            {
                new List<string>{ "S1", },
                new List<string>{ },
                new List<string[]>{
                    new []{"S1", DeviceSignalsInfo.UnpairedSignal},
                },
            }
        };


        [Test]
        public void GetMainModel()
        {
            var mainModel = Mock.Of<ICurrentProjectModel>(m => m.ProjectName == "T1-MAIN_PROJECT");
            var altModel = Mock.Of<IProjectModel>(m => m.ProjectName == "T1-ALT_PROJECT");

            typeof(InterprojectExchange.InterprojectExchange).GetField("eProjectManager", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, Mock.Of<IEProjectManager>(m => m.GetModifyingCurrentProjectName() == "T1-MAIN_PROJECT"));

            var interprojectExchange = InterprojectExchange.InterprojectExchange.GetInstance();

            interprojectExchange.AddModel(mainModel);
            interprojectExchange.AddModel(altModel);

            Assert.AreSame(mainModel, interprojectExchange.MainModel);

            typeof(InterprojectExchange.InterprojectExchange)
                .GetField("interprojectExchange", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, null);
        }

        [Test]
        public void SelectModel()
        {
            var mainModel = Mock.Of<ICurrentProjectModel>(m => m.ProjectName == "T1-MAIN_PROJECT");
            var altModel = Mock.Of<IProjectModel>(m => m.ProjectName == "T1-ALT_PROJECT");

            typeof(InterprojectExchange.InterprojectExchange).GetField("eProjectManager", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, Mock.Of<IEProjectManager>(m => m.GetModifyingCurrentProjectName() == "T1-MAIN_PROJECT"));

            var interprojectExchange = InterprojectExchange.InterprojectExchange.GetInstance();

            interprojectExchange.AddModel(mainModel);
            interprojectExchange.AddModel(altModel);

            interprojectExchange.SelectModel(altModel);

            Assert.Multiple(() =>
            {
                Mock.Get(altModel).VerifySet(m => m.Selected = true);
                Mock.Get(mainModel).VerifySet(m => m.Selected = false);
                Mock.Get(mainModel).VerifySet(m => m.SelectedAdvancedProject = altModel.ProjectName);
            });

            typeof(InterprojectExchange.InterprojectExchange)
                .GetField("interprojectExchange", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, null);
        }
    }
}
