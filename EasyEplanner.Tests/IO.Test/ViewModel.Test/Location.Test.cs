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
    }
}
