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
    public class RootTest
    {
        [Test]
        public void Getters()
        {
            var context = Mock.Of<IIOViewModel>();

            var root = new Root(context);

            Assert.Multiple(() =>
            {
                Assert.AreEqual("ПЛК", root.Name);
                Assert.AreEqual("", root.Description);
                Assert.IsEmpty(root.Items);
                Assert.AreSame(context, root.Context);
            });
        }

        [Test]
        public void Items_DeletedModulesWithoutLocation_AddsDeletedModulesGroup()
        {
            var deletedModule = Mock.Of<IIOModule>(module =>
                module.Name == "DEL338" &&
                module.PhysicalNumber == 338 &&
                module.Location == string.Empty);
            var ioManager = Mock.Of<IIOManager>(manager =>
                manager.IONodes == new List<IIONode>() &&
                manager.DeletedModules == new List<IIOModule>
                {
                    deletedModule
                });
            var context = Mock.Of<IIOViewModel>(viewModel =>
                viewModel.IOManager == ioManager);

            var root = new Root(context);

            var deletedModulesGroup = root.Items
                .OfType<DeletedModulesGroup>()
                .Single();

            Assert.Multiple(() =>
            {
                Assert.AreEqual("Исключенные модули",
                    deletedModulesGroup.Name);
                Assert.AreEqual("DEL338",
                    deletedModulesGroup.Items.Single().Name);
            });
        }

        [Test]
        public void HasBindingError_WithInvalidClamp_ReturnsTrue()
        {
            var ioNode = BindingErrorTestHelper.CreateIoNode(
                modules: new List<IIOModule>
                {
                    BindingErrorTestHelper.CreateIoModuleWithInvalidClamp(),
                });
            var context = CreateContext(ioNode);

            var root = new Root(context);

            Assert.IsTrue(root.HasBindingError);
        }

        [Test]
        public void HasBindingError_WhenTreeValid_ReturnsFalse()
        {
            var ioNode = BindingErrorTestHelper.CreateIoNode(
                modules: new List<IIOModule>
                {
                    BindingErrorTestHelper.CreateIoModuleWithValidClamp(),
                });
            var context = CreateContext(ioNode);

            var root = new Root(context);

            Assert.IsFalse(root.HasBindingError);
        }

        [Test]
        public void Icon_WithInvalidClamp_ReturnsError()
        {
            var ioNode = BindingErrorTestHelper.CreateIoNode(
                modules: new List<IIOModule>
                {
                    BindingErrorTestHelper.CreateIoModuleWithInvalidClamp(),
                });
            var context = CreateContext(ioNode);

            var root = new Root(context);

            Assert.AreEqual(Icon.Error, (root as IHasDescriptionIcon).Icon);
        }

        [Test]
        public void Icon_WhenTreeValid_ReturnsNone()
        {
            var ioNode = BindingErrorTestHelper.CreateIoNode(
                modules: new List<IIOModule>
                {
                    BindingErrorTestHelper.CreateIoModuleWithValidClamp(),
                });
            var context = CreateContext(ioNode);

            var root = new Root(context);

            Assert.AreEqual(Icon.None, (root as IHasDescriptionIcon).Icon);
        }

        private static IIOViewModel CreateContext(IIONode ioNode)
        {
            var ioManager = Mock.Of<IIOManager>(manager =>
                manager.IONodes == new List<IIONode> { ioNode } &&
                manager.DeletedModules == new List<IIOModule>());

            return Mock.Of<IIOViewModel>(viewModel =>
                viewModel.IOManager == ioManager);
        }
    }
}
