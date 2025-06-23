using EasyEPlanner;
using EplanDevice;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerTests
{
    public class ColumnNodesTest
    {
        [Test]
        public void TestUpdateSubtype()
        {
            var device = Mock.Of<IIODevice>(d =>
                d.Function == Mock.Of<IEplanFunction>() &&
                d.DeviceType == DeviceType.V);

            var node = new DeviceSubTypeNode("V_DO1");

            Assert.Multiple(() =>
            {
                node.Update(device, "DO", "V_DO1");
                Assert.AreEqual("V_DO1", node.Value);

                node.Update(device, "V_DO1", "V_DO1");
                Assert.AreEqual("V_DO1", node.Value);

                node.Update(device, "V_DO2", "V_DO1");
                Assert.AreEqual("Обновите проект", node.Value);
                Mock.Get(device.Function).VerifySet(f => f.SubType = "V_DO2");
            });
        }


        [Test]
        public void TestUpdateDescription()
        {
            var device = Mock.Of<IIODevice>(d =>
                d.Function == Mock.Of<IEplanFunction>());

            var node = new DeviceDescriptionNode("Описание 1");

            Assert.Multiple(() =>
            {
                node.Update(device, "Описание 1", "Описание 1");
                Assert.AreEqual("Описание 1", node.Value);
                Mock.Get(device.Function).VerifySet(f => f.Description = It.IsAny<string>(), Times.Never);

                node.Update(device, "Описание 2", "Описание 1");
                Assert.AreEqual("Описание 2", node.Value);
                Mock.Get(device.Function).VerifySet(f => f.Description = "Описание 2");
            });
        }

        [Test]
        public void TestUpdateArticle()
        {
            var device = Mock.Of<IIODevice>(d =>
                d.Function == Mock.Of<IEplanFunction>());

            var node = new DeviceArticleNode("Изделие 1");

            Assert.Multiple(() =>
            {
                node.Update(device, "Изделие 2", "Изделие 1");
                Assert.AreEqual("Изделие 1", node.Value);
            });
        }

        [Test]
        public void TestUpdateParameter()
        {
            var device = Mock.Of<IIODevice>(d =>
                d.Function == Mock.Of<IEplanFunction>());

            var node = new ParameterNode("P_DT", "0.1")
            {
                Tag = IODevice.Parameter.P_DT
            };

            Assert.Multiple(() =>
            {
                node.Update(device, "0.1", "0.1");
                Assert.AreEqual("0.1", node.Value);
                Mock.Get(device).Verify(d => d.UpdateParameters(), Times.Never);

                node.Update(device, "0.2", "0.1");
                Assert.AreEqual("0.2", node.Value);
                Mock.Get(device).Verify(d => d.SetParameter("P_DT", 0.2));
                Mock.Get(device).Verify(d => d.UpdateParameters());
            });
        }

        [Test]
        public void TestUpdateProperty()
        {
            var device = Mock.Of<IIODevice>(d =>
                d.Function == Mock.Of<IEplanFunction>() &&
                d.MultipleProperties == new List<string>());

            var node = new PropertyNode("property", "свойство 1");

            Assert.Multiple(() =>
            {
                node.Update(device, "свойство 1", "свойство 1");
                Assert.AreEqual("свойство 1", node.Value);
                Mock.Get(device).Verify(d => d.UpdateProperties(), Times.Never);

                node.Update(device, "свойство 1, свойство 2", "свойство 1");
                Assert.AreEqual("свойство 1", node.Value);
                Mock.Get(device).Verify(d => d.UpdateProperties(), Times.Never);

                Mock.Get(device).Setup(d => d.MultipleProperties).Returns(new List<string>() { "property" });
                node.Update(device, "свойство 1, свойство 2", "свойство 1");
                Assert.AreEqual("свойство 1, свойство 2", node.Value);
                Mock.Get(device).Verify(d => d.SetProperty("property", "свойство 1, свойство 2"));
                Mock.Get(device).Verify(d => d.UpdateProperties());

                node.Update(device, "свойство 2", "свойство 1");
                Assert.AreEqual("свойство 2", node.Value);
                Mock.Get(device).Verify(d => d.SetProperty("property", "свойство 2"));
                Mock.Get(device).Verify(d => d.UpdateProperties());
            });
        }

        [Test]
        public void TestUpdateRuntimeParameter()
        {
            var device = Mock.Of<IIODevice>(d =>
                d.Function == Mock.Of<IEplanFunction>());

            var node = new RuntimeParameterNode("par", "1");

            Assert.Multiple(() =>
            {
                node.Update(device, "1", "1");
                Assert.AreEqual("1", node.Value);
                Mock.Get(device).Verify(d => d.UpdateRuntimeParameters(), Times.Never);

                node.Update(device, "2", "1");
                Assert.AreEqual("2", node.Value);
                Mock.Get(device).Verify(d => d.SetRuntimeParameter("par", 2));
                Mock.Get(device).Verify(d => d.UpdateRuntimeParameters());
            });
        }
    }
}
