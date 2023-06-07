using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InterprojectExchange;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using TechObject;

namespace EasyEplannerTests.InterprojectExchangeTest
{
    public class InterprojectExchangeSaverTest
    {
        [TestCaseSource(nameof(SaveProjectRemoteGateWays_Cases))]
        public void SaveProjectRemoteGateWays_CheckResultData(string projectName,
            PacInfo pacInfo, DeviceSignalsInfo signals, bool invertSignals, string expected)
        {
            var interprojectExchange = InterprojectExchange.InterprojectExchange.GetInstance();
            InterprojectExchangeSaver saver = new InterprojectExchangeSaver(interprojectExchange, "");
            var method = typeof(InterprojectExchangeSaver).GetMethod(
                "SaveProjectRemoteGateWays", BindingFlags.NonPublic | BindingFlags.Instance);
            object res = method.Invoke(saver, new object[] { projectName, pacInfo, signals, invertSignals });
            Assert.AreEqual(expected, res);
        }

        private static object[] SaveProjectRemoteGateWays_Cases = new object[]
        {
            new object[]
            {
                "ProjectName",
                new PacInfo()
                {
                    IP = "10.0.162.1",
                    IPEmulator = "10.0.0.127",
                    EmulationEnabled = true,
                    CycleTime = 400,
                    TimeOut = 500,
                    Port = 10512,
                    GateEnabled = true,
                    Station = 5
                },
                SetUpDeviceSignalInfo(),
                false,
                "    ['ProjectName'] =\n" +
                "    {\n" +
                "        ip         = '10.0.162.1', -- адрес удаленного контроллера\n" +
                "        ipemulator = '10.0.0.127', -- адрес удаленного контроллера при эмуляции на столе\n" +
                "        emulation  = true,         -- включение эмуляции\n" +
                "        cycletime  = 400,          -- время ожидания между опросами контроллера\n" +
                "        timeout    = 500,          -- таймаут для modbus клиента\n" +
                "        port       = 10512,        -- modbus - порт удаленного контроллера\n" +
                "        enabled    = true,         -- включить/выключить шлюз\n" +
                "        station    = 5,            -- номер станции modbus удаленного клиента\n" +
                "        DO =\n" +
                "        {\n" +
                "        OBJ1DO1,\n" +
                "        },\n" +
                "    },"
            }
        };


        [TestCaseSource(nameof(SaveProjectSharedDevices_Cases))]
        public void SaveProjectSharedDevices_CheckResultData(string projectName,
            int stationNum, DeviceSignalsInfo signals, bool invertSignals, string expected)
        {
            var interprojectExchange = InterprojectExchange.InterprojectExchange.GetInstance();
            InterprojectExchangeSaver saver = new InterprojectExchangeSaver(interprojectExchange, "");
            var method = typeof(InterprojectExchangeSaver).GetMethod(
                "SaveProjectSharedDevices", BindingFlags.NonPublic | BindingFlags.Instance);
            object res = method.Invoke(saver, new object[] { projectName, stationNum, signals, invertSignals });
            Assert.AreEqual(expected, res);
        }

        private static object[] SaveProjectSharedDevices_Cases = new object[]
        {
            new object[]
            {
                "ProjectName",
                5,
                SetUpDeviceSignalInfo(),
                false,
                "    [5] =\n" +
                "    {\n" +
                "        projectName = \"ProjectName\",\n" +
                "        DO =\n" +
                "        {\n" +
                "        OBJ1DO1,\n" +
                "        },\n" +
                "    },"
            }
        };

        private static DeviceSignalsInfo SetUpDeviceSignalInfo()
        {
            var deviceSignalsInfo = new DeviceSignalsInfo();
            deviceSignalsInfo.DO.Add("OBJ1DO1");

            return deviceSignalsInfo;
        }
    }
}
