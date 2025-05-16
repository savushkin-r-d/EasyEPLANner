using EasyEPlanner.ModbusExchange;
using EasyEPlanner.ModbusExchange.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerTests.ModbusExchangeTest
{
    public class ModbusExchangeCsvImporterAndSaverTest
    {
        [Test]
        public void SaveModel()
        {
            var csvData =
                "Static;;;\r\n" +
                "snd;UNKNOWN_TYPE;0;\r\n" +
                "Сигнал 1;Word;0;0\r\n" +
                "Группа 1;Struct;2;\r\n" +
                "Сигнал 2;Word;2;0\r\n" +
                "rcv;UNKNOWN_TYPE;200;\r\n" +
                "Сигнал 3;Word;200;0\r\n" +
                "Группа 1;Struct;202;\r\n" +
                "Сигнал 4;Word;202;0\r\n";

            var gateway = new Gateway("test")
            {
                IP = "127.0.0.1",
                Port = 500
            };
            gateway.Parse(csvData);

            var res = gateway.GetTextToSave("-- test version");

            var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory,
                "ModbusExchange.Test", "TestData", "gate_test.lua");
            var exchangeData = File.ReadAllText(filePath, Encoding.UTF8)
                .Replace("\r\n", "\n");

            Assert.AreEqual(exchangeData, res);
        }
    }
}
