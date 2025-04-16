using IO;
using IO.ViewModel;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTests
{
    public class ModuleTest
    {
        [Test]
        public void Getters()
        {
            var expanded = true;

            var node = Mock.Of<INode>();
            var ioModule = Mock.Of<IIOModule>(m =>
                m.Info.ChannelClamps == new int[]{ } &&
                m.Name == "A101" &&
                m.Info.Description == "description" &&
                m.Info.TypeName == "type_name" &&
                m.Info.Name == "123" && 
                m.Function.Expanded == expanded);
            

            var module = new Module(ioModule, node);


            Assert.Multiple(() =>
            {
                Assert.AreEqual("A101", module.Name);
                Assert.AreEqual("description", module.Description);
                Assert.IsTrue(module.Expanded);
                
                module.Expanded = false;
                Assert.IsFalse(module.Expanded);

                Assert.AreEqual("Артикул: 123\nОписание: description\ntype_name", (module as IToolTip).Description);

                Assert.IsEmpty(module.Items);
            });
        }


        [TestCaseSource(nameof(ColorIcon))]
        public void Icon(Color color, IO.ViewModel.Icon expectedIcon)
        {
            var node = Mock.Of<INode>();
            var ioModule = Mock.Of<IIOModule>(m => m.Info.ModuleColor == color);

            var module = new Module(ioModule, node);

            Assert.AreEqual(expectedIcon, (module as IHasIcon).Icon);
        }

        public static object[] ColorIcon = new object[]
        {
            new object[] { Color.Black, IO.ViewModel.Icon.BlackModule},
            new object[] { Color.Blue, IO.ViewModel.Icon.BlueModule},
            new object[] { Color.Gray, IO.ViewModel.Icon.GrayModule},
            new object[] { Color.Green, IO.ViewModel.Icon.GreenModule},
            new object[] { Color.Lime, IO.ViewModel.Icon.LimeModule},
            new object[] { Color.Orange, IO.ViewModel.Icon.OrangeModule},
            new object[] { Color.Red, IO.ViewModel.Icon.RedModule},
            new object[] { Color.Violet, IO.ViewModel.Icon.VioletModule},
            new object[] { Color.Yellow, IO.ViewModel.Icon.YellowModule },
            new object[] { Color.Aqua, IO.ViewModel.Icon.None },
        };
    }
}
