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
    public class IOViewModelTest
    {
        [Test]
        public void Getters()
        {
            var nodes = new List<IIONode>() { };

            var context = new IOViewModel(null);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(context.Root);
                CollectionAssert.AreEqual(new List<IRoot> { context.Root }, context.Roots);

                Assert.IsNull(context.IOManager);

                Assert.IsNull(context.SelectedClampFunction);
                Assert.IsNull(context.SelectedClamp);

                var root = context.Root;
                context.RebuildTree();

                Assert.AreNotSame(root, context.Root);
            });
        }
    }
}
