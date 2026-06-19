using EasyEPlanner.Devices.ViewModel;
using NUnit.Framework;
using System.Linq;

namespace EasyEPlanner.Devices.Tests
{
    public class DevicesViewModelTest
    {
        [Test]
        public void Root_IsInitializedWithEmptyProjectDevices()
        {
            var context = new DevicesViewModel(null);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(context.Root);
                Assert.AreEqual("Устройства проекта (0)", context.Root.Name);
                CollectionAssert.AreEqual(new[] { context.Root }, context.Roots.ToArray());
                Assert.IsEmpty(((FilterableViewItemBase)context.Root).Items);
            });
        }

        [Test]
        public void RebuildTree_ReplacesRoot()
        {
            var context = new DevicesViewModel(null);
            var previousRoot = context.Root;

            context.RebuildTree();

            Assert.AreNotSame(previousRoot, context.Root);
            Assert.AreEqual("Устройства проекта (0)", context.Root.Name);
        }

        [Test]
        public void GroupingMode_DefaultIsTypeThenObject()
        {
            var context = new DevicesViewModel(null);

            Assert.AreEqual(DevicesGroupingMode.TypeThenObject, context.GroupingMode);
        }
    }
}
