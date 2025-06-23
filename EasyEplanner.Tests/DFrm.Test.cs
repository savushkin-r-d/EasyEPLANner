using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Aga.Controls.Tree;
using EasyEPlanner;
using EplanDevice;
using NUnit.Framework;


namespace EasyEplannerTests
{
    public class DFrmTest
    {
        [Test]
        public void TreeSortTest()
        {
            var root = new Node("Устройства проекта")
            {
                Nodes =
                {
                    new Node("V")
                    {
                        Tag = DeviceType.V,
                        Nodes =
                        {
                            new Node ("TANK2")
                            {
                                Nodes =
                                {
                                    new Node("V2")
                                    {
                                        Tag = new V("TANK2V2", "+TANK2-V2", "", 2, "TANK", 2, ""),
                                        Nodes =
                                        {
                                            new Node("AO")
                                            {
                                                Tag = new IODevice.IOChannel("AO", -1, -1, -1, "AO")
                                            },
                                            new Node("DI")
                                            {
                                                Tag = new IODevice.IOChannel("DI", -1, -1, -1, "DI")
                                            }
                                        }
                                    },
                                    new Node("V1")
                                    {
                                        Tag = new V("TANK2V1", "+TANK2-V1", "", 1, "TANK", 2, "")
                                    }
                                }
                            },
                            new Node ("TANK1")
                            {
                                Nodes =
                                {
                                    new Node("V1")
                                    {
                                        Tag = new V("TANK1V1", "+TANK1-V1", "", 1, "TANK", 1, "")
                                    }
                                }
                            }
                        }
                    },
                    new Node("DI")
                    {
                        Tag = DeviceType.DI,
                    }
                }
            };

            DFrm.TreeSort(root.Nodes.ToList(), root);

            Assert.Multiple(() =>
            {
                var types = root.Nodes;
                var objects = types[1].Nodes;
                var devs = objects[1].Nodes;
                var channels = devs[1].Nodes;

                Assert.IsTrue(types.Select(n => n.Text).SequenceEqual(new[] { "DI", "V" }));
                Assert.IsTrue(objects.Select(n => n.Text).SequenceEqual(new[] { "TANK1", "TANK2" }));
                Assert.IsTrue(devs.Select(n => n.Text).SequenceEqual(new[] { "V1", "V2" }));
                Assert.IsTrue(channels.Select(n => n.Text).SequenceEqual(new[] { "DI", "AO" }));
            });
        }


        [Test]
        public void AddDevParametersTest()
        {
            var root_v = new Node("dev");
            var dev_v = new V("OBJ1V1", "+OBJ1-V1", "", 1, "OBJ", 1, "");
            dev_v.SetSubType("V_AS_MIXPROOF");

            var root_wt = new Node("dev");
            var dev_wt = new WT("OBJ1WT1", "+OBJ1-WT1", "", 1, "OBJ", 1, "");
            dev_wt.SetSubType("WT_ETH");

            var root_do = new Node("dev");
            var dev_do = new DO("OBJ1DO1", "+OBJ1-DO1", "", 1, "OBJ", 1);
            dev_do.SetSubType("DO_VIRT");

            DFrm.AddDevParametersAndProperties(root_v, dev_v);
            DFrm.AddDevParametersAndProperties(root_wt, dev_wt);
            DFrm.AddDevParametersAndProperties(root_do, dev_do);

            Assert.Multiple(() =>
            {
                Assert.AreEqual("Данные", root_v.Nodes[0].Text);
                Assert.AreEqual("Параметры", root_v.Nodes[1].Text);
                Assert.AreEqual("Рабочие параметры", root_v.Nodes[2].Text);
                Assert.AreEqual("P_ON_TIME Время включения", root_v.Nodes[1].Nodes[0].Text);
                Assert.AreEqual("P_FB      Обратная связь", root_v.Nodes[1].Nodes[1].Text);
                Assert.AreEqual("R_AS_NUMBER", root_v.Nodes[2].Nodes[0].Text);

                Assert.AreEqual("Данные", root_wt.Nodes[0].Text);
                Assert.AreEqual("Параметры", root_wt.Nodes[1].Text);
                Assert.AreEqual("Свойства", root_wt.Nodes[2].Text);
                Assert.AreEqual("P_C0 Сдвиг нуля", root_wt.Nodes[1].Nodes[0].Text);
                Assert.AreEqual("IP", root_wt.Nodes[2].Nodes[0].Text);

                Assert.AreEqual(1, root_do.Nodes.Count);
            });
        }

        [Test]
        public void SetUpCheckStateParameter_Test()
        {
            var root = new Node();

            var node = new Node()
            {
                Parent = root,
            };

            Assert.Multiple(() =>
            {
                DFrm.SetUpCheckStateParameter(node, new TechObject.Param(getN => 1, "Параметр", false, 0, "шт", "par", false), " 1 ");
                Assert.IsTrue(node.IsChecked);

                DFrm.SetUpCheckStateParameter(node, new TechObject.Param(getN => 1, "Параметр", false, 0, "шт", "par", false), " par ");
                Assert.IsTrue(node.IsChecked);

                DFrm.SetUpCheckStateParameter(node, new TechObject.Param(getN => 1, "Параметр", false, 0, "шт", "par", false), " 2 ");
                Assert.IsTrue(node.IsChecked);
            });


        }
    }
}
