using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            saver = new InterprojectExchangeSaver(interprojectExchange, "");
        }

        [TestCaseSource(nameof(SaveProjectRemoteGateWays_Cases))]
        public void SaveProjectRemoteGateWays_CheckResultData(string projectName,
            PacInfo pacInfo, DeviceSignalsInfo signals, bool invertSignals, string expected)
        {
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

        private static DeviceSignalsInfo SetUpDeviceSourceSingnal()
        {
            var deviceSignalsInfo = new DeviceSignalsInfo();
            deviceSignalsInfo.DI.Add("OBJ1DI1");

            return deviceSignalsInfo;
        }

        private static List<string> SharedFileAsStringList = new List<string>()
        {
            "shared_devices =\n",
            "{\n",
            "    [10] =\n",
            "    {\n",
            "        projectName = \"PROJECT1\",\n",
            "        DO =\n",
            "        {\n",
            "        __OBJ2DO1,\n",
            "        },\n",
            "    },\n",
            "}\n",
            "remote_gateways =\n",
            "{\n",
            "    ['PROJECT1'] =\n",
            "    {\n",
            "        ip         = '10.125.162.180', -- адрес удаленного контроллера\n",
            "        ipemulator = '127.0.0.1',      -- адрес удаленного контроллера при эмуляции на столе\n",
            "        emulation  = false,            -- включение эмуляции\n",
            "        cycletime  = 500,              -- время ожидания между опросами контроллера\n",
            "        timeout    = 1000,             -- таймаут для modbus клиента\n",
            "        port       = 10502,            -- modbus - порт удаленного контроллера\n",
            "        enabled    = true,             -- включить/выключить шлюз\n",
            "        station    = 2,                -- номер станции modbus удаленного клиента\n",
            "        DO =\n",
            "        {\n",
            "        OBJ1DEV1,\n",
            "        },\n",
            "    },\n",
            "\n",
            "    ['OTHER_PROJECT'] =\n",
            "    {\n",
            "        SOME DATA",
            "    },\n",
            "},"
        };

        private static List<string> SharedFileAsStringList_Copy = new List<string>(SharedFileAsStringList);

        private const int PROJECT1_StartIndex = 13;
        private const int PROJECT1_Lencht = 15;
        private static string PROJECT1_DataAfterUpdateRemoteGateway =
            "    ['PROJECT1'] =\n" +
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
            "    },";

        private static List<string> SharedFileAsStringList_ExpectedAfterUpdateRemoteGateway()
        {
            SharedFileAsStringList_Copy.RemoveRange(PROJECT1_StartIndex, PROJECT1_Lencht);
            SharedFileAsStringList_Copy.Insert(PROJECT1_StartIndex, PROJECT1_DataAfterUpdateRemoteGateway);

            return SharedFileAsStringList_Copy;
        }

        private const int STATION10_StartIndex = 2;
        private const int STATION10_Lencht = 8;
        private static string STATION10_DataAfterUpdateSharedDevices =
            "    [10] =\n" +
            "    {\n" +
            "        projectName = \"PROJECT1\",\n" +
            "        DI =\n" +
            "        {\n" +
            "        __OBJ1DI1,\n" +
            "        },\n" +
            "    },";

        private static List<string> SharedFileAsStringList_ExpectedAfterUpdateSharedDevices()
        {
            SharedFileAsStringList_Copy.RemoveRange(STATION10_StartIndex, STATION10_Lencht);
            SharedFileAsStringList_Copy.Insert(STATION10_StartIndex, STATION10_DataAfterUpdateSharedDevices);

            return SharedFileAsStringList_Copy;
        }

        private static Mock<IProjectModel> MockSetUpSavingProjectModel() 
        {
            var projectModel = new Mock<IProjectModel>();

            projectModel.Setup(obj => obj.SharedFileAsStringList)
                .Returns(SharedFileAsStringList);

            projectModel.Setup(obj => obj.PacInfo)
                .Returns(
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
                });

            projectModel.Setup(obj => obj.ReceiverSignals)
                .Returns(SetUpDeviceSignalInfo());

            projectModel.Setup(obj => obj.SourceSignals)
                .Returns(SetUpDeviceSourceSingnal());

            return projectModel;
        }

        private static Mock<IProjectModel> MockSetUpOppositeProjectModel()
        {
            var projectModel = new Mock<IProjectModel>();

            projectModel.Setup(obj => obj.ProjectName).Returns("PROJECT1");

            projectModel.Setup(obj => obj.PacInfo)
                .Returns(
                new PacInfo()
                {
                    Station = 10
                });



            return projectModel;
        }

        private static IProjectModel savingProject = MockSetUpSavingProjectModel().Object;
        private static IProjectModel oppositeProject = MockSetUpOppositeProjectModel().Object;

        [Test]
        public void UpdateModelRemoteGateWaysAndSharedDevices_CheckSavingModelSharedFileData()
        {
            var UpdateModelRemoteGateWays = typeof(InterprojectExchangeSaver).GetMethod(
                "UpdateModelRemoteGateWays", BindingFlags.NonPublic | BindingFlags.Instance);
            UpdateModelRemoteGateWays.Invoke(saver, new object[] { savingProject, oppositeProject, false });

            CollectionAssert.AreEqual(SharedFileAsStringList_ExpectedAfterUpdateRemoteGateway(), SharedFileAsStringList);

            var UpdateModelSharedDevices = typeof(InterprojectExchangeSaver).GetMethod(
                "UpdateModelSharedDevices", BindingFlags.NonPublic | BindingFlags.Instance);
            UpdateModelSharedDevices.Invoke(saver, new object[] { savingProject, oppositeProject, false });

            CollectionAssert.AreEqual(SharedFileAsStringList_ExpectedAfterUpdateSharedDevices(), SharedFileAsStringList);
        }

        private InterprojectExchangeSaver saver;
        private InterprojectExchange.InterprojectExchange interprojectExchange 
            = InterprojectExchange.InterprojectExchange.GetInstance();
    }
}
