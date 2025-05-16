using EasyEPlanner.ModbusExchange;
using EasyEPlanner.ModbusExchange.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerTests.ModbusExchangeTest
{
    public class ModbusExchangeLoaderTest
    {

        [Test]
        public void GetGroup()
        {
            var subgroup = Mock.Of<IGroup>(g => 
                g.Description == "subgroup"
            );

            var group = Mock.Of<IGroup>(g => 
                g.Items == new List<IGatewayViewItem>() { subgroup });

            var exist_group = ModbusExchangeLoader.GetGroup(group, "subgroup");
            var new_group = ModbusExchangeLoader.GetGroup(group, "new_subgroup");
            var same_group = ModbusExchangeLoader.GetGroup(group, "");

            Assert.Multiple(() =>
            {
                Assert.AreSame(subgroup, exist_group);
                Assert.AreEqual("new_subgroup", new_group.Description);
                Assert.AreSame(group, same_group);
                Mock.Get(group).Verify(m => m.Add(It.IsAny<IGroup>()));
            });
        }

        [Test]
        public void AddSignal()
        {
            var group = Mock.Of<IGroup>();

            ModbusExchangeLoader.AddSignal(group, "description", "", "Bool", 10, 1);

            Mock.Get(group).Verify(m =>
                m.Add(It.Is<ISignal>(s =>
                    s.Description == "description" && s.DataType == "Bool" &&
                    s.Word == 10 && s.Bit == 1)));
        }
    }
}
