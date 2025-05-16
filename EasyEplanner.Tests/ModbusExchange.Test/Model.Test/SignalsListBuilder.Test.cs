using EasyEPlanner.ModbusExchange.Model;
using EplanDevice;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEplannerTests.ModbusExchangeTest.ModelTest
{
    public class SignalsListBuilderTest
    {
        [Test]
        public void GetSignals()
        {
            var dev1 = new MockDevice(DeviceType.AO, "f1", "f");
            var dev2 = new MockDevice(DeviceType.AI, "f2");
            var dev3 = new MockDevice(DeviceType.V, "f3");
            var dev4 = new MockDevice(DeviceType.DO, "n1");
            var dev5 = new MockDevice(DeviceType.DI, "n1");
            var dev6 = new MockDevice(DeviceType.DO, "f4");
            var dev7 = new MockDevice(DeviceType.DI, "f5");

            var deviceManager = Mock.Of<IDeviceManager>(m => 
                m.Devices == new List<IODevice>()
                {
                    dev1, dev2, dev3, dev4, dev5, dev6, dev7
                });

            var gate = Mock.Of<IGateway>(g =>
                g.Read.NestedSignals == new List<ISignal>() 
                {
                    Mock.Of<ISignal>(s => s.Device == dev7)
                } &&
                g.Write.NestedSignals == new List<ISignal>()
                {
                    Mock.Of<ISignal>(s => s.Device == dev6)
                });


            CollectionAssert.AreEqual(
                new List<IIODevice>() { dev1, dev2 },
                deviceManager.GetModbusExchangeSignals(gate, "f"));
        }



        public class MockDevice : IODevice
        {
            public MockDevice(DeviceType type, string name = "", string description = "")
                :this(name, "", description, "", 0, "", 1)
            {
                dType = type;
            }

            private MockDevice(
                string name, string eplanName, string description, string deviceType,
                int deviceNumber, string objectName,int objectNumber) 
                : base(name, eplanName, description, deviceType,
                      deviceNumber, objectName, objectNumber)
            {
            }
        }
    }



}
