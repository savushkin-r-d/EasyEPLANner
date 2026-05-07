using EasyEPlanner;
using IO;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;

namespace EasyEplannerTests.FileSaverTest
{
    public class ExcelDataCollectorTest
    {
        [SetUp]
        public void SetUp()
        {
            IOManager.GetInstance().Clear();
        }

        [TearDown]
        public void TearDown()
        {
            IOManager.GetInstance().Clear();
        }

        [Test]
        public void SaveIOAsConnectionArray_PhoenixControllerExists_SavesControllerFirst()
        {
            var ioManager = IOManager.GetInstance();
            var a1Node = CreateIoNodeMock(1, "750-352", "A1", "FIELD",
                "10.0.0.1", IONode.TYPES.T_ETHERNET);
            var controller = CreateIoNodeMock(2, "AXC F 2152", "A2", "PLC",
                "10.0.0.2", IONode.TYPES.T_PHOENIX_CONTACT_2152);
            var remoteNode = CreateIoNodeMock(3, "750-352", "A3", "FIELD",
                "10.0.0.3", IONode.TYPES.T_ETHERNET);
            ioManager.IONodes.Add(a1Node.Object);
            ioManager.IONodes.Add(controller.Object);
            ioManager.IONodes.Add(remoteNode.Object);

            var modulesCount = new Dictionary<string, int>();
            var modulesColor = new Dictionary<string, Color>();
            var asInterfaceConnection = new Dictionary<string, object[,]>();

            var result = ExcelDataCollector.SaveIOAsConnectionArray("TestProject",
                modulesCount, modulesColor, asInterfaceConnection);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(9, result.GetLength(0));
                Assert.AreEqual(7, result.GetLength(1));

                Assert.AreEqual("TestProject", result[0, 3]);
                StringAssert.StartsWith("'", result[1, 3] as string);
                Assert.AreEqual("Контроллер PLC-A2. Адрес: 10.0.0.2", result[1, 0]);
                Assert.AreEqual("Вход, бит", result[1, 4]);
                Assert.AreEqual("Выход, бит", result[1, 5]);
                Assert.AreEqual("Изделие", result[1, 6]);
                Assert.AreEqual(0, result[2, 0]);
                Assert.AreEqual("AXC F 2152", result[2, 1]);

                Assert.AreEqual("TestProject", result[3, 3]);
                Assert.AreEqual("Узел №0. FIELD-A1. Адрес: 10.0.0.1", result[4, 0]);
                Assert.AreEqual(0, result[5, 0]);
                Assert.AreEqual("352", result[5, 1]);

                Assert.AreEqual("TestProject", result[6, 3]);
                Assert.AreEqual("Узел №2. FIELD-A3. Адрес: 10.0.0.3", result[7, 0]);
                Assert.AreEqual(0, result[8, 0]);
                Assert.AreEqual("352", result[8, 1]);

                Assert.AreEqual(Color.Gray, modulesColor["AXC F 2152"]);
                Assert.AreEqual(Color.Gray, modulesColor["352"]);
            });

            VerifySaveAsConnectionArray(a1Node, modulesCount, modulesColor,
                asInterfaceConnection, 0);
            VerifySaveAsConnectionArray(controller, modulesCount, modulesColor,
                asInterfaceConnection, 1);
            VerifySaveAsConnectionArray(remoteNode, modulesCount, modulesColor,
                asInterfaceConnection, 2);
        }

        private static Mock<IIONode> CreateIoNodeMock(int n, string typeStr,
            string name, string location, string ip, IONode.TYPES type)
        {
            var nodeMock = new Mock<IIONode>();
            nodeMock.SetupGet(x => x.IOModules).Returns(new List<IIOModule>());
            nodeMock.SetupGet(x => x.N).Returns(n);
            nodeMock.SetupGet(x => x.NodeNumber).Returns(n);
            nodeMock.SetupGet(x => x.TypeStr).Returns(typeStr);
            nodeMock.SetupGet(x => x.Name).Returns(name);
            nodeMock.SetupGet(x => x.Location).Returns(location);
            nodeMock.SetupGet(x => x.IP).Returns(ip);
            nodeMock.SetupGet(x => x.Type).Returns(type);

            return nodeMock;
        }

        private static void VerifySaveAsConnectionArray(Mock<IIONode> nodeMock,
            Dictionary<string, int> modulesCount,
            Dictionary<string, Color> modulesColor,
            Dictionary<string, object[,]> asInterfaceConnection,
            int expectedNodeIdx)
        {
            nodeMock.Verify(x => x.SaveAsConnectionArray(
                ref It.Ref<object[,]>.IsAny,
                ref It.Ref<int>.IsAny,
                modulesCount,
                modulesColor,
                expectedNodeIdx,
                asInterfaceConnection),
                Times.Once);
        }
    }
}
