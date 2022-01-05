using IO;
using NUnit.Framework;

namespace Tests.IO
{
    internal class IONodeInfoTest
    {
        [SetUp]
        public void SetUpBeforeEachTest()
        {
            IONodeInfo.Nodes.Clear();
        }

        [Test]
        public void AddNodeInfo_Adds_New_NodeInfo()
        {
            int expectedCount = 4;
            var nodes = new[]
            {
                new { Name = "750-362", Type = IONode.TYPES.T_ETHERNET, IsCoupler = true },
                new { Name = "AXC F BK ETH", Type = IONode.TYPES.T_PHOENIX_CONTACT, IsCoupler = false },
                new { Name = "750-352", Type = IONode.TYPES.T_ETHERNET, IsCoupler = true },
                new { Name = "AXC F 1552", Type = IONode.TYPES.T_PHOENIX_CONTACT_MAIN, IsCoupler = false },
            };

            foreach(var node in nodes)
            {
                IONodeInfo.AddNodeInfo(node.Name, node.Type, node.IsCoupler);
            }

            int actualCount = IONodeInfo.Count;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedCount, actualCount);
                Assert.AreEqual(expectedCount, IONodeInfo.Nodes.Count);
                for(int i = 0; i < IONodeInfo.Nodes.Count; i++)
                {
                    var addedNodes = IONodeInfo.Nodes;
                    Assert.AreEqual(nodes[i].Name, addedNodes[i].Name);
                    Assert.AreEqual(nodes[i].Type, addedNodes[i].Type);
                    Assert.AreEqual(nodes[i].IsCoupler, addedNodes[i].IsCoupler);
                }
            });
        }

        [Test]
        public void AddNodeInfo_Do_Not_Add_Already_Added_Nodes()
        {
            int expectedCount = 2;
            var nodes = new[]
            {
                new { Name = "750-362", Type = IONode.TYPES.T_ETHERNET, IsCoupler = true },
                new { Name = "AXC F BK ETH", Type = IONode.TYPES.T_PHOENIX_CONTACT, IsCoupler = false },
            };

            int repeatCount = 2;
            for(int i = 0; i < repeatCount; i++)
            {
                foreach (var node in nodes)
                {
                    IONodeInfo.AddNodeInfo(node.Name, node.Type, node.IsCoupler);
                }
            }


            int actualCount = IONodeInfo.Count;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedCount, actualCount);
                Assert.AreEqual(expectedCount, IONodeInfo.Nodes.Count);
                for (int i = 0; i < IONodeInfo.Nodes.Count; i++)
                {
                    var addedNodes = IONodeInfo.Nodes;
                    Assert.AreEqual(nodes[i].Name, addedNodes[i].Name);
                    Assert.AreEqual(nodes[i].Type, addedNodes[i].Type);
                    Assert.AreEqual(nodes[i].IsCoupler, addedNodes[i].IsCoupler);
                }
            });
        }

        [Test]
        public void GetNodeInfo_Returns_Stub()
        {
            // There is no any node info right now and we will always get a stub
            string nodeName = "somename";
            var nodeInfo = IONodeInfo.GetNodeInfo(nodeName, out bool isStub);

            var stub = IONodeInfo.Stub;
            Assert.Multiple(() =>
            {
                Assert.IsTrue(isStub);
                Assert.AreNotEqual(nodeName, nodeInfo.Name);
                Assert.AreEqual(stub.Name, nodeInfo.Name);
                Assert.AreEqual(stub.Type, nodeInfo.Type);
                Assert.AreEqual(stub.IsCoupler, nodeInfo.IsCoupler);
                Assert.AreEqual(stub.GetHashCode(), nodeInfo.GetHashCode());
            });
        }

        [Test]
        public void GetNodeInfo_Returns_Concrete_NodeInfo()
        {
            string name = "AXC F BK ETH NET2";
            IONode.TYPES type = IONode.TYPES.T_PHOENIX_CONTACT;
            bool isCoupler = true;
            IONodeInfo.AddNodeInfo(name, type, isCoupler);
            
            var node = IONodeInfo.GetNodeInfo(name, out _);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(name, node.Name);
                Assert.AreEqual(type, node.Type);
                Assert.AreEqual(isCoupler, node.IsCoupler);
            });
        }

        [TestCase("750-362", IONode.TYPES.T_ETHERNET, true)]
        [TestCase("AXC F BK ETH", IONode.TYPES.T_PHOENIX_CONTACT, false)]
        [TestCase("750-352", IONode.TYPES.T_ETHERNET, true)]
        public void Clone_Returns_Full_Copy(string name, IONode.TYPES type, bool isCoupler)
        {
            IONodeInfo.AddNodeInfo(name, type, isCoupler);
            var nodeForClone = IONodeInfo.GetNodeInfo(name, out _);

            var cloned = nodeForClone.Clone() as IONodeInfo;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(name, cloned.Name);
                Assert.AreEqual(type, cloned.Type);
                Assert.AreEqual(isCoupler, cloned.IsCoupler);
                Assert.AreNotEqual(nodeForClone.GetHashCode(), cloned.GetHashCode());
            });
        }
    }
}
