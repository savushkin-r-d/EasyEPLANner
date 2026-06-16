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
    public class LocationTest
    {
        [Test]
        public void Getters()
        {
            var nodes = new List<IIONode>() { };

            var loc = new Location("+CAB1", "Шкаф 1", nodes);

            Assert.Multiple(() =>
            {
                Assert.AreEqual("+CAB1", loc.Name);
                Assert.AreEqual("Шкаф 1", loc.Description);
                Assert.IsEmpty(loc.Items);
            });
        }

        [Test]
        public void Items_DeletedModules_AddsDeletedModulesGroup()
        {
            var deletedModule = Mock.Of<IIOModule>(module =>
                module.Name == "DEL338" &&
                module.PhysicalNumber == 338);

            var loc = new Location("+CAB1", "Шкаф 1", new List<IIONode>(),
                new[] { deletedModule });

            var deletedModulesGroup = loc.Items
                .OfType<DeletedModulesGroup>()
                .Single();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(loc.Expanded);
                Assert.AreEqual("Исключенные модули",
                    deletedModulesGroup.Name);
                Assert.AreEqual(Icon.EmptyModule,
                    (deletedModulesGroup as IHasIcon).Icon);
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
            var location = new Location(
                "+CAB1", "Шкаф 1", new List<IIONode> { ioNode });

            Assert.IsTrue(location.HasBindingError);
        }

        [Test]
        public void HasBindingError_WhenTreeValid_ReturnsFalse()
        {
            var ioNode = BindingErrorTestHelper.CreateIoNode(
                modules: new List<IIOModule>
                {
                    BindingErrorTestHelper.CreateIoModuleWithValidClamp(),
                });
            var location = new Location(
                "+CAB1", "Шкаф 1", new List<IIONode> { ioNode });

            Assert.IsFalse(location.HasBindingError);
        }

        [Test]
        public void Icon_WithInvalidClamp_ReturnsError()
        {
            var ioNode = BindingErrorTestHelper.CreateIoNode(
                modules: new List<IIOModule>
                {
                    BindingErrorTestHelper.CreateIoModuleWithInvalidClamp(),
                });
            var location = new Location(
                "+CAB1", "Шкаф 1", new List<IIONode> { ioNode });

            Assert.AreEqual(Icon.Error, (location as IHasDescriptionIcon).Icon);
        }

        [Test]
        public void Icon_WhenTreeValid_ReturnsNone()
        {
            var ioNode = BindingErrorTestHelper.CreateIoNode(
                modules: new List<IIOModule>
                {
                    BindingErrorTestHelper.CreateIoModuleWithValidClamp(),
                });
            var location = new Location(
                "+CAB1", "Шкаф 1", new List<IIONode> { ioNode });

            Assert.Multiple(() =>
            {
                Assert.AreEqual(Icon.Cab, (location as IHasIcon).Icon);
                Assert.AreEqual(Icon.None, (location as IHasDescriptionIcon).Icon);
            });
        }
    }
}
