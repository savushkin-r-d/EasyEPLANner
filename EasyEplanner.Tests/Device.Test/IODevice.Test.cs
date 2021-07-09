using NUnit.Framework;
using System.Linq;
using System.Windows.Forms;

namespace Tests.Devices
{
    public class IODeviceTest
    {
        [TestCaseSource(nameof(TestSortingChannelsForVDeviceData))]
        public void SortChannels_NewValveDevices_ReturnsSortedArrayOfChannels(
            Device.IODevice dev, string subType, string[] expected)
        {
            dev.SetSubType(subType);
            dev.SortChannels();
            string[] actual = dev.Channels
                .Where(x => x.Comment != "")
                .Select(x => x.Comment ).ToArray();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// 1 - Устройство V (клапан) для тестирования
        /// 2 - Задаваемый подтип
        /// 3 - Ожидаемое значение
        /// </summary>
        /// <returns></returns>
        public static object[] TestSortingChannelsForVDeviceData()
        {
            return new object[] 
            {
                new object[] 
                {
                    VTest.GetRandomVDevice(), 
                    "V_DO2", 
                    new string[] {"Открыть", "Закрыть"} 
                },
                new object[] 
                {
                    VTest.GetRandomVDevice(),
                    "V_DO1_DI2",
                    new string[] {"Открыт", "Закрыт"} 
                },
                new object[] 
                {
                    VTest.GetRandomVDevice(), 
                    "V_DO2_DI2",
                    new string[] {"Открыть", "Закрыть", "Открыт", "Закрыт"} },
                new object[] 
                {
                    VTest.GetRandomVDevice(),
                    "V_MIXPROOF",
                    new string[] {"Открыть", "Открыть ВС", "Открыть НС", 
                        "Открыт", "Закрыт"} 
                },
                new object[] 
                {
                    VTest.GetRandomVDevice(),
                    "V_BOTTOM_MIXPROOF",
                    new string[] {"Открыть", "Открыть мини", "Открыть НС", 
                        "Открыт", "Закрыт"} 
                },
            };
        }

        [TestCaseSource(nameof(GenerateDeviceTagsCaseSource))]
        public void GenerateDeviceTags_DeviceAODefault_ReturnsTree(
            Device.IODevice dev, TreeNode expectedNode)
        {
            var actualNode = new TreeNode();
            
            dev.GenerateDeviceTags(actualNode);
            
            for(int i = 0; i < actualNode.Nodes.Count; i++)
            {
                TreeNode actualSubNode = actualNode.Nodes[i];
                TreeNode expectedSubNode = expectedNode.Nodes[i];
                Assert.AreEqual(expectedSubNode.Text, actualSubNode.Text);
                for (int j = 0; j < actualNode.Nodes.Count; j++)
                {
                    Assert.AreEqual(expectedSubNode.Nodes[j].Text,
                        actualSubNode.Nodes[j].Text);
                }
            }
        }

        public static object[] GenerateDeviceTagsCaseSource()
        {
            string devName = "TANK1AO1";
            string eplanName = "+TANK1-AO1";
            string descr = "Сигнал AO";
            string objName = "TANK";
            int objNum = 1;
            int devNum = 1;

            string devMTag = "M";
            string devVTag = "V";
            string devPMinVTag = "P_MIN_V";
            string devPMaxVTag = "P_MAX_V";

            var expectedNode = new TreeNode();
            var mNode = new TreeNode($"AO_{devMTag}");
            mNode.Nodes.Add($"{devName}.{devMTag}");
            var vNode = new TreeNode($"AO_{devVTag}");
            vNode.Nodes.Add($"{devName}.{devVTag}");
            var pMinVNode = new TreeNode($"AO_{devPMinVTag}");
            pMinVNode.Nodes.Add($"{devName}.{devPMinVTag}");
            var pMaxVNode = new TreeNode($"AO_{devPMaxVTag}");
            pMaxVNode.Nodes.Add($"{devName}.{devPMaxVTag}");
            expectedNode.Nodes.Add(mNode);
            expectedNode.Nodes.Add(vNode);
            expectedNode.Nodes.Add(pMinVNode);
            expectedNode.Nodes.Add(pMaxVNode);

            var dev = new Device.AO(devName, eplanName, descr, devNum, objName,
                objNum);
            var defaultAODev = new object[] { dev, expectedNode };

            return new object[] { defaultAODev };
        }
    }
}
