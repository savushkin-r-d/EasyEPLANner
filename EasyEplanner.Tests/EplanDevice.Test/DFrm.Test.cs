using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Aga.Controls.Tree;
using EasyEPlanner;
using EplanDevice;
using NUnit.Framework;


namespace Tests.EplanDevices
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

    }
}
