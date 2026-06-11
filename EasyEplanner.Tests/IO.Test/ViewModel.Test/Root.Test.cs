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
    }
}
