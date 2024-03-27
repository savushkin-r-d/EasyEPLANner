using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;
using NUnit.Framework;
using Moq;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using EplanDevice;

namespace TechObjectTests
{
    public class ActionGroupWashTest
    {

        [Test]
        public void UpdateOnGenerictTechObject()
        {
            var actionGroupWash = new ActionGroupWash("Устройства", null, "devices");
            var genericActionGroupWash = new ActionGroupWash("Устройства", null, "devices");

            genericActionGroupWash.Insert();

            var actionWashMock = new Mock<IAction>();
            actionWashMock.Setup(a => a.UpdateOnGenericTechObject(It.IsAny<IAction>()))
                .Callback<IAction>((sa) => Assert.AreSame(genericActionGroupWash.SubActions[0], sa));

            actionGroupWash.SubActions[0] = actionWashMock.Object;

            Assert.Multiple(() =>
            {
                // null object
                actionGroupWash.UpdateOnGenericTechObject(null);
                // wrong type object
                actionGroupWash.UpdateOnGenericTechObject(new TechObject.Action("action", null, "action"));

                Assert.AreEqual(1, actionGroupWash.SubActions.Count());

                actionGroupWash.UpdateOnGenericTechObject(genericActionGroupWash);

                Assert.AreEqual(2, actionGroupWash.SubActions.Count());
            });
        }

        [Test]
        public void SaveAsLuaTable_Test()
        {
            var actionGroupWash = new ActionGroupWash("Устройства", null, "devices");

            var deviceManagerMock = new Mock<IDeviceManager>();

            var DO1 = new DO("DO1", "DO1", "desc", 1, "", 0);
            var DI1 = new DI("DI1", "DI1", "desc", 1, "", 0);

            deviceManagerMock.Setup(m => m.GetDeviceByIndex(0)).Returns(DO1);
            deviceManagerMock.Setup(m => m.GetDeviceByIndex(1)).Returns(DI1);

            typeof(TechObject.Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManagerMock.Object);

            actionGroupWash.AddDev(1, 1, "DI");
            actionGroupWash.AddDev(0, 0, "DO");



            Assert.AreEqual(
                "    devices_data = --Устройства\n" +
                "        {\n" +
                "         --Группа\n" +
                "            {\n" +
                "            DO = --DO\n" +
                "                {\n" +
                "                'DO1'\n" +
                "                },\n" +
                "            },\n" +
                "         --Группа\n" +
                "            {\n" +
                "            DI = --DI\n" +
                "                {\n" +
                "                'DI1'\n" +
                "                },\n" +
                "            },\n" +
                "        },\n",
                actionGroupWash.SaveAsLuaTable("\t").Replace("\t", "    "));
        }

        [Test]
        public void SaveAsLueTable_EmptySubActions()
        {
            var actionGroupWash = new ActionGroupWash("Устройства", null, "devices");
            actionGroupWash.SubActions.Clear();
            Assert.AreEqual(string.Empty, actionGroupWash.SaveAsLuaTable("\t"));
        }
    }
}
