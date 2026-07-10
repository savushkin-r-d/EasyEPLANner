using System.Collections.Generic;
using System.Linq;

namespace IO.ViewModel
{
    public static class DeletedModuleRestorePlanner
    {
        public static IEnumerable<DeletedModuleRestoreTarget>
            GetRestorableModules(IEnumerable<IIOModule> deletedModules,
                IEnumerable<IIONode> nodes)
        {
            var usedTargets = new HashSet<int>();

            foreach (var module in deletedModules ??
                Enumerable.Empty<IIOModule>())
            {
                if (!TryGetRestoreTarget(module, nodes, out var target) ||
                    !usedTargets.Add(target.TargetPhysicalNumber))
                {
                    continue;
                }

                yield return target;
            }
        }

        private static bool TryGetRestoreTarget(IIOModule deletedModule,
            IEnumerable<IIONode> nodes,
            out DeletedModuleRestoreTarget restoreTarget)
        {
            restoreTarget = null;

            if (deletedModule?.Function?.IsValid != true)
            {
                return false;
            }

            var node = GetNodesWithExtensions(nodes)
                .FirstOrDefault(ioNode =>
                    ioNode.NodeNumber == deletedModule.PhysicalNumber /
                    100 * 100);
            if (node == null)
            {
                return false;
            }

            int moduleIndex = deletedModule.PhysicalNumber -
                node.NodeNumber - 1;
            if (moduleIndex < 0 ||
                moduleIndex >= node.IOModules.Count ||
                !IsUndefinedModule(node.IOModules[moduleIndex]))
            {
                return false;
            }

            restoreTarget = new DeletedModuleRestoreTarget(deletedModule,
                deletedModule.PhysicalNumber);
            return true;
        }

        private static IEnumerable<IIONode> GetNodesWithExtensions(
            IEnumerable<IIONode> nodes)
        {
            foreach (var node in nodes ?? Enumerable.Empty<IIONode>())
            {
                if (node == null)
                {
                    continue;
                }

                yield return node;

                foreach (var extensionNode in GetNodesWithExtensions(
                    node.ExtensionModules))
                {
                    yield return extensionNode;
                }
            }
        }

        private static bool IsUndefinedModule(IIOModule module)
        {
            return module is not null &&
                module.Info?.Name == IOModuleInfo.Stub.Name &&
                module.Function is null;
        }
    }

    public class DeletedModuleRestoreTarget
    {
        public DeletedModuleRestoreTarget(IIOModule module,
            int targetPhysicalNumber)
        {
            Module = module;
            TargetPhysicalNumber = targetPhysicalNumber;
        }

        public IIOModule Module { get; }

        public int TargetPhysicalNumber { get; }
    }
}

