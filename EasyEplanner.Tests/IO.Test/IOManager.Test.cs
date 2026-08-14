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

        [Test]
        public void ApplyNtypesFromLua_DisablesNodesWithEmptyNtype()
        {
            var ioManager = IOManager.GetInstance();
            ioManager.ResetStoredNtypes();
            var controller = new IONode("AXC F 2152", 1, 100, StrStub, "A100", StrStub, StrStub);
            var coupler = new IONode("750-352", 2, 200, StrStub, "A200", StrStub, StrStub);
            var extension = new IONode("AXC F XT ETH 1TX", 1, 100, StrStub, "A100.1", StrStub, StrStub);
            controller.AddExtensionModule(extension);
            SetNodes(ioManager, new List<IIONode> { controller, coupler });

            ioManager.ApplyNtypesFromLua(
                "nodes =\n{\n" +
                "    {\n" +
                "    name    = 'A100',\n" +
                "    ntype   = 202, --AXC F 2152\n" +
                "    },\n" +
                "    {\n" +
                "    name    = 'A200',\n" +
                "    ntype   = -1, --выключен\n" +
                "    },\n" +
                "    {\n" +
                "    name    = 'A100.1',\n" +
                "    ntype   = -1, --выключен\n" +
                "    },\n" +
                "}\n");

            Assert.Multiple(() =>
            {
                Assert.IsTrue(controller.NtypeEnabled);
                Assert.IsFalse(coupler.NtypeEnabled);
                Assert.IsFalse(extension.NtypeEnabled);
            });
        }

        [Test]
        public void ApplyNtypesFromLua_ControllerWithEmptyNtype_StaysEnabled()
        {
            var ioManager = IOManager.GetInstance();
            ioManager.ResetStoredNtypes();
            var controller = new IONode("AXC F 2152", 1, 100, StrStub, "A100", StrStub, StrStub);
            SetNodes(ioManager, new List<IIONode> { controller });

            ioManager.ApplyNtypesFromLua(
                "name    = 'A100',\n" +
                "ntype   = -1, --выключен\n");

            Assert.IsTrue(controller.NtypeEnabled);
        }

        [Test]
        public void ApplyStoredNtypes_AfterClear_RestoresDisabledFlag()
        {
            var ioManager = IOManager.GetInstance();
            ioManager.ResetStoredNtypes();
            var coupler = new IONode("750-352", 2, 200, StrStub, "A200", StrStub, StrStub);
            coupler.NtypeEnabled = false;

            ioManager.Clear();
            var restored = new IONode("750-352", 2, 200, StrStub, "A200", StrStub, StrStub);
            SetNodes(ioManager, new List<IIONode> { restored });

            ioManager.ApplyStoredNtypes();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(restored.CanDisableNtype);
                Assert.IsFalse(restored.NtypeEnabled);
                StringAssert.Contains("ntype   = -1, --выключен",
                    restored.SaveAsLuaTable(""));
            });
        }

        [Test]
        public void ApplyStoredNtypes_AfterEnable_RestoresEnabledFlag()
        {
            var ioManager = IOManager.GetInstance();
            ioManager.ResetStoredNtypes();
            var coupler = new IONode("750-352", 2, 200, StrStub, "A200", StrStub, StrStub);
            coupler.NtypeEnabled = false;
            coupler.NtypeEnabled = true;

            ioManager.Clear();
            var restored = new IONode("750-352", 2, 200, StrStub, "A200", StrStub, StrStub);
            SetNodes(ioManager, new List<IIONode> { restored });

            ioManager.ApplyStoredNtypes();

            Assert.IsTrue(restored.NtypeEnabled);
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
