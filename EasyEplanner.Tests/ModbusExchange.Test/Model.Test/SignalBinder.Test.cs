using EasyEPlanner.ModbusExchange.Model;
using EplanDevice;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerTests.ModbusExchangeTest.ModelTest
{
    public class SignalBinderTest
    {
        [TestCase("Bool", DeviceType.DO, false, true)]
        [TestCase("Bool", DeviceType.DI, true, false)]
        [TestCase("Bool", DeviceType.AO, false, false)]
        [TestCase("Bool", DeviceType.AI, false, false)]

        [TestCase("Word", DeviceType.DO, false, true)]
        [TestCase("Word", DeviceType.DI, true, false)]
        [TestCase("Word", DeviceType.AO, false, true)]
        [TestCase("Word", DeviceType.AI, true, false)]
        public void Bind(string dataType, DeviceType deviceType, bool bindedToRead, bool bindedToWrite)
        {
            var readSignal = Mock.Of<ISignal>(s => s.DataType == dataType);
            var writeSignal = Mock.Of<ISignal>(s => s.DataType == dataType);

            var gate = Mock.Of<IGateway>(g => 
                g.Write.NestedSignals == new List<ISignal>() { writeSignal } &&
                g.Read.NestedSignals == new List<ISignal>() { readSignal }
            );

            var device = Mock.Of<IIODevice>(d => d.DeviceType == deviceType);

            gate.Bind(readSignal, device);
            gate.Bind(writeSignal, device);

            Assert.Multiple(() =>
            {
                Mock.Get(readSignal)
                    .SetupSet(s => s.Device = device)
                    .Verifiable(Times.Exactly(bindedToRead ? 1 : 0));
                Mock.Get(writeSignal)
                    .SetupSet(content => content.Device = device)
                    .Verifiable(Times.Exactly(bindedToWrite ? 1 : 0));
            });
        }
    }
}
