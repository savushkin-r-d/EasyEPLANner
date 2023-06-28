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

            var signals_1 = new DeviceSignalsInfo();
            signals_1.AI.Add("AI1");

            var signals_2 = new DeviceSignalsInfo();
            signals_2.AI.Add("AI2");

            var signals_error = new DeviceSignalsInfo();


            var mainModel = new Mock<CurrentProjectModel>();

            mainModel.Setup(obj => obj.ReceiverSignals).Returns(signals_1);
            mainModel.Setup(obj => obj.SourceSignals).Returns(signals_error);
            mainModel.Setup(obj => obj.ProjectName).Returns("main_prj");

            var advancedModel = new Mock<IProjectModel>();

            advancedModel.Setup(obj => obj.ReceiverSignals).Returns(signals_2);
            advancedModel.Setup(obj => obj.SourceSignals).Returns(signals_error);
            advancedModel.Setup(obj => obj.ProjectName).Returns("adv_prj");

            var interprojectExchangeMock = new Mock<InterprojectExchange.InterprojectExchange>();

            interprojectExchangeMock.Setup(obj => obj.MainModel).Returns(mainModel.Object);

            var interprojectExchange = interprojectExchangeMock.Object;

            interprojectExchange.AddModel(mainModel.Object);
            interprojectExchange.AddModel(advancedModel.Object);

            Assert.AreEqual(expected, interprojectExchange.CheckBindingSignals());
        }

    }
}
