using EasyEPlanner.ModbusExchange.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerTests.ModbusExchangeTest
{
    public class SignalsSizeCalculatorTest
    {
        [Test]
        public void Size()
        {
            var signals = new List<ISignal>
            {
                Mock.Of<ISignal>(s => s.DataType == "Other" && s.Word == 0),
                Mock.Of<ISignal>(s => s.DataType == "Word" && s.Word == 1),
                Mock.Of<ISignal>(s => s.DataType == "Byte" && s.Word == 5),
                Mock.Of<ISignal>(s => s.DataType == "Real" && s.Word == 6),
            };

            Assert.AreEqual(5, signals.Size());
        }

    }
}
