using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aga.Controls.Tree;
using EasyEPlanner;
using NUnit.Framework;

namespace EasyEplannerTests
{
    public class ModeFrmTest
    {
        [Test]
        public void UncheckedTest()
        {
            var root = new Node()
            {
                Nodes =
                {
                    new Node()
                    {
                        CheckState = CheckState.Checked,
                        Nodes =
                        {
                            new Node() { CheckState = CheckState.Checked},
                            new Node() { CheckState = CheckState.Checked},
                        }
                    }
                }
            };

            ModeFrm.Unchecked(root, root.Nodes[0].Nodes[1]);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(CheckState.Unchecked, root.Nodes[0].CheckState);
                Assert.AreEqual(CheckState.Unchecked, root.Nodes[0].Nodes[0].CheckState);
                Assert.AreEqual(CheckState.Checked, root.Nodes[0].Nodes[1].CheckState);
            });
        }

    }
}
