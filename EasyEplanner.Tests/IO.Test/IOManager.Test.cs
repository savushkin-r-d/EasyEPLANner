using IO;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IOTests
{
    public class IOManagerTest
    {
        [Test]
        public void GetNodesWithExtensions_ReturnsNodesWithExtensionModules()
        {
            var ioManager = IOManager.GetInstance();
            var parentNode = new IONode(StrStub, 1, 100, StrStub, "A100", StrStub, StrStub);
            var extensionNode = new IONode(StrStub, 1, 100, StrStub, "A100.1", StrStub, StrStub);
            var secondNode = new IONode(StrStub, 2, 200, StrStub, "A200", StrStub, StrStub);

            parentNode.AddExtensionModule(extensionNode);
            SetNodes(ioManager, new List<IIONode>
            {
                parentNode,
                null,
                secondNode
            });

            var nodes = GetNodesWithExtensions(ioManager).ToList();

            CollectionAssert.AreEqual(
                new[] { parentNode, extensionNode, secondNode },
                nodes);
        }

        [Test]
        public void DeletedModules_AddAndClear_UpdatesDeletedModules()
        {
            var ioManager = IOManager.GetInstance();
            ioManager.Clear();
            var module = new IOModule(0, 0, null);

            ioManager.AddDeletedModule(module);

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(new[] { module },
                    ioManager.DeletedModules);

                ioManager.Clear();

                Assert.IsEmpty(ioManager.DeletedModules);
            });
        }

        private static IEnumerable<IIONode> GetNodesWithExtensions(IOManager ioManager)
        {
            var method = typeof(IOManager).GetMethod("GetNodesWithExtensions",
                BindingFlags.NonPublic | BindingFlags.Instance);

            return (IEnumerable<IIONode>)method.Invoke(ioManager, null);
        }

        private static void SetNodes(IOManager ioManager, List<IIONode> nodes)
        {
            typeof(IOManager).GetField("iONodes",
                BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(ioManager, nodes);
        }

        const string StrStub = "";
    }
}
