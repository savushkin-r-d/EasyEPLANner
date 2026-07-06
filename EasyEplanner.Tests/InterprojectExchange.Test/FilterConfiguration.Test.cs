using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using InterprojectExchange;
using NUnit.Framework;

namespace EasyEplannerTests.InterprojectExchangeTest
{
    public class FilterConfigurationTest
    {
        [Test]
        public void GetDevicesList_VirtSubTypes_AreListedAfterBaseTypes()
        {
            var filter = CreateFilterConfiguration();
            List<string> devices = filter.GetDevicesList();

            int doIndex = devices.IndexOf("DO");
            int doVirtIndex = devices.IndexOf("DO_VIRT");
            int diVirtIndex = devices.IndexOf("DI_VIRT");
            int lastBaseTypeIndex = devices.FindLastIndex(d => !d.EndsWith("_VIRT"));

            Assert.Multiple(() =>
            {
                Assert.That(doVirtIndex, Is.GreaterThan(doIndex));
                Assert.That(diVirtIndex, Is.GreaterThan(devices.IndexOf("DI")));
                Assert.That(lastBaseTypeIndex, Is.LessThan(doVirtIndex));
                CollectionAssert.IsSubsetOf(
                    new[] { "DO_VIRT", "DI_VIRT", "AI_VIRT", "AO_VIRT" },
                    devices);
            });
        }

        [Test]
        public void FilterOut_OnlyVirtTypeSelected_ReturnsMatchingDevices()
        {
            var filter = CreateFilterConfiguration();
            filter.SetFilterParameter("currProjDevList", "DO_VIRT", true);

            var items = new List<ListViewItem>
            {
                CreateListViewItem("DO", "+TANK1DO1"),
                CreateListViewItem("DO_VIRT", "+TANK1DO2"),
                CreateListViewItem("DI_VIRT", "+TANK1DI1"),
            };

            var filtered = filter.FilterOut(items,
                FilterConfiguration.FilterList.CurrentProject);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, filtered.Length);
                Assert.AreEqual("DO_VIRT", filtered[0].Tag);
            });
        }

        [Test]
        public void FilterOut_BaseAndVirtTypesSelected_ReturnsBoth()
        {
            var filter = CreateFilterConfiguration();
            filter.SetFilterParameter("currProjDevList", "DO", true);
            filter.SetFilterParameter("currProjDevList", "DO_VIRT", true);

            var items = new List<ListViewItem>
            {
                CreateListViewItem("DO", "+TANK1DO1"),
                CreateListViewItem("DO_VIRT", "+TANK1DO2"),
                CreateListViewItem("DI", "+TANK1DI1"),
            };

            var filtered = filter.FilterOut(items,
                FilterConfiguration.FilterList.CurrentProject);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, filtered.Length);
                CollectionAssert.AreEquivalent(
                    new[] { "DO", "DO_VIRT" },
                    filtered.Select(x => x.Tag.ToString()).ToArray());
            });
        }

        [Test]
        public void FilterOut_NoTypesSelected_ReturnsAllDevices()
        {
            var filter = CreateFilterConfiguration();

            var items = new List<ListViewItem>
            {
                CreateListViewItem("DO", "+TANK1DO1"),
                CreateListViewItem("DO_VIRT", "+TANK1DO2"),
            };

            var filtered = filter.FilterOut(items,
                FilterConfiguration.FilterList.CurrentProject);

            Assert.AreEqual(2, filtered.Length);
        }

        private static FilterConfiguration CreateFilterConfiguration()
        {
            var filter = (FilterConfiguration)FormatterServices
                .GetUninitializedObject(typeof(FilterConfiguration));

            typeof(FilterConfiguration).GetMethod("SetUpFilterParameters",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(filter, null);

            return filter;
        }

        private static ListViewItem CreateListViewItem(string type, string name)
        {
            var item = new ListViewItem(new[] { "description", name });
            item.Tag = type;
            return item;
        }
    }
}
