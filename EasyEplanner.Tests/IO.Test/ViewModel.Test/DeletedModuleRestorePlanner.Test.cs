using IO;
using IO.ViewModel;
using Moq;
using NUnit.Framework;
using StaticHelper;
using System.Collections.Generic;
using System.Linq;

namespace IOTests
{
    public class DeletedModuleRestorePlannerTest
    {
        [Test]
        public void GetRestorableModules_FreeUndefinedSlot_ReturnsTarget()
        {
            var deletedModule = CreateDeletedModule(338);
            var node = CreateNode(300, CreateModules(38, 37,
                CreateUndefinedModule()));

            var targets = DeletedModuleRestorePlanner
                .GetRestorableModules(new[] { deletedModule },
                    new[] { node })
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, targets.Count);
                Assert.AreSame(deletedModule, targets[0].Module);
                Assert.AreEqual(338, targets[0].TargetPhysicalNumber);
            });
        }

        [Test]
        public void GetRestorableModules_OccupiedSlot_ReturnsEmpty()
        {
            var deletedModule = CreateDeletedModule(338);
            var node = CreateNode(300, CreateModules(38, 37,
                CreateActiveModule(338)));

            var targets = DeletedModuleRestorePlanner
                .GetRestorableModules(new[] { deletedModule },
                    new[] { node });

            Assert.IsEmpty(targets);
        }

        [Test]
        public void GetRestorableModules_ExtensionNode_ReturnsTarget()
        {
            var deletedModule = CreateDeletedModule(338);
            var extensionNode = CreateNode(300, CreateModules(38, 37,
                CreateUndefinedModule()));
            var parentNode = Mock.Of<IIONode>(node =>
                node.NodeNumber == 100 &&
                node.IOModules == new List<IIOModule>() &&
                node.ExtensionModules == new List<IIONode>
                {
                    extensionNode
                });

            var targets = DeletedModuleRestorePlanner
                .GetRestorableModules(new[] { deletedModule },
                    new[] { parentNode })
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, targets.Count);
                Assert.AreSame(deletedModule, targets[0].Module);
            });
        }

        [Test]
        public void GetRestorableModules_DuplicateTarget_ReturnsFirstTargetOnly()
        {
            var firstDeletedModule = CreateDeletedModule(338);
            var secondDeletedModule = CreateDeletedModule(338);
            var node = CreateNode(300, CreateModules(38, 37,
                CreateUndefinedModule()));

            var targets = DeletedModuleRestorePlanner
                .GetRestorableModules(
                    new[] { firstDeletedModule, secondDeletedModule },
                    new[] { node })
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, targets.Count);
                Assert.AreSame(firstDeletedModule, targets[0].Module);
            });
        }

        private static IIONode CreateNode(int nodeNumber,
            List<IIOModule> modules)
        {
            return Mock.Of<IIONode>(node =>
                node.NodeNumber == nodeNumber &&
                node.IOModules == modules &&
                node.ExtensionModules == new List<IIONode>());
        }

        private static List<IIOModule> CreateModules(int count,
            int targetIndex, IIOModule targetModule)
        {
            var modules = Enumerable.Range(0, count)
                .Select(index => CreateActiveModule(index + 1))
                .ToList();

            modules[targetIndex] = targetModule;

            return modules;
        }

        private static IIOModule CreateDeletedModule(int physicalNumber)
        {
            return Mock.Of<IIOModule>(module =>
                module.PhysicalNumber == physicalNumber &&
                module.Function == Mock.Of<IEplanFunction>(function =>
                    function.IsValid));
        }

        private static IIOModule CreateUndefinedModule()
        {
            return Mock.Of<IIOModule>(module =>
                module.Info == Mock.Of<IIOModuleInfo>(info =>
                    info.Name == IOModuleInfo.Stub.Name) &&
                module.Function == null);
        }

        private static IIOModule CreateActiveModule(int physicalNumber)
        {
            return Mock.Of<IIOModule>(module =>
                module.PhysicalNumber == physicalNumber &&
                module.Function == Mock.Of<IEplanFunction>(function =>
                    function.IsValid));
        }
    }
}

