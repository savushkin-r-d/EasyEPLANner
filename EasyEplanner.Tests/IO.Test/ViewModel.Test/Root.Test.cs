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
    }
}
