using IO;
using IO.ViewModel;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTests
{
    public class NodeTest
    {
        [Test]
        public void Getters()
        {
            var expanded = true;

            var ioNode = Mock.Of<IIONode>(n =>
                n.N == 1 &&
                n.Name == "A100" &&
                n.TypeStr == "AO" &&
                n.IOModules == new List<IIOModule>() &&
                n.Function.IP == "ip" &&
                n.Function.SubnetMask == "mask" &&
                n.Function.Gateway == "gateway" && 
                n.Function.Expanded == expanded);

            var location = Mock.Of<ILocation>();

            var node = new Node(ioNode, location);

            Assert.Multiple(() =>
            {
                Assert.AreEqual("1. A100", node.Name);
                Assert.AreEqual("AO", node.Description);
                Assert.IsTrue(node.Expanded);

                node.Expanded = false;
                Assert.IsFalse(node.Expanded);

                CollectionAssert.AreEqual(
                    new List<string>() { "IP-адрес", "Маска подсети", "Сетевой шлюз" },
                    node.Items.Select(i => i.Name));

                CollectionAssert.AreEqual(
                    new List<string>() { "ip", "mask", "gateway" },
                    node.Items.Select(i => i.Description));
            });
        }
    }
}
