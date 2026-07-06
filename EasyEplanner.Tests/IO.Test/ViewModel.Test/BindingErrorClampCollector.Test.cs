using IO;
using IO.ViewModel;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace IOTests
{
    public class BindingErrorClampCollectorTest
    {
        [Test]
        public void Collect_FromClamp_ReturnsClampWithError()
        {
            var ioNode = BindingErrorTestHelper.CreateIoNode(
                modules: new List<IIOModule>
                {
                    BindingErrorTestHelper.CreateIoModuleWithInvalidClamp(),
                });
            var node = new Node(ioNode, null);
            var module = node.Items.OfType<Module>().Single();
            var clamp = module.Items.OfType<IClamp>().Single();

            var result = BindingErrorClampCollector.Collect(new object[] { clamp })
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, result.Count);
                Assert.AreSame(clamp, result[0]);
            });
        }

        [Test]
        public void Collect_FromValidClamp_ReturnsEmpty()
        {
            var ioNode = BindingErrorTestHelper.CreateIoNode(
                modules: new List<IIOModule>
                {
                    BindingErrorTestHelper.CreateIoModuleWithValidClamp(),
                });
            var node = new Node(ioNode, null);
            var clamp = node.Items.OfType<Module>().Single()
                .Items.OfType<IClamp>().Single();

            Assert.IsEmpty(BindingErrorClampCollector.Collect(new object[] { clamp }));
        }

        [Test]
        public void Collect_FromModule_ReturnsOnlyInvalidClamps()
        {
            var ioNode = BindingErrorTestHelper.CreateIoNode(
                modules: new List<IIOModule>
                {
                    BindingErrorTestHelper.CreateIoModuleWithInvalidClamp(1),
                    BindingErrorTestHelper.CreateIoModuleWithValidClamp(2),
                });
            var node = new Node(ioNode, null);
            var module = node.Items.OfType<Module>().First();

            var result = BindingErrorClampCollector.Collect(new object[] { module })
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, result.Count);
                Assert.IsTrue(result[0].HasBindingError);
            });
        }

        [Test]
        public void Collect_FromNode_ReturnsInvalidClampsFromAllModules()
        {
            var ioNode = BindingErrorTestHelper.CreateIoNode(
                modules: new List<IIOModule>
                {
                    BindingErrorTestHelper.CreateIoModuleWithInvalidClamp(1),
                    BindingErrorTestHelper.CreateIoModuleWithInvalidClamp(2),
                });
            var node = new Node(ioNode, null);

            var result = BindingErrorClampCollector.Collect(new object[] { node })
                .ToList();

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void Collect_FromLocation_ReturnsInvalidClampsFromNestedNodes()
        {
            var ioNode = BindingErrorTestHelper.CreateIoNode(
                modules: new List<IIOModule>
                {
                    BindingErrorTestHelper.CreateIoModuleWithInvalidClamp(),
                });
            var location = new Location(
                "+CAB1", "Шкаф 1", new List<IIONode> { ioNode });

            var result = BindingErrorClampCollector.Collect(new object[] { location })
                .ToList();

            Assert.AreEqual(1, result.Count);
        }
    }
}
