using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EplanDevice;
using Moq;
using NUnit.Framework;
using StaticHelper;
using TechObject;

namespace TechObjectTests
{
    public class EquipmentTest
    {
        [Test]
        public void ModifyDevNames()
        {
            V OBJ1V1 = new V(nameof(OBJ1V1), "+OBJ1-V1", string.Empty, 1, "OBJ", 1, string.Empty);
            V OBJ2V2 = new V(nameof(OBJ2V2), "+OBJ2-V2", string.Empty, 2, "OBJ", 2, string.Empty);
            V OBJ3V1 = new V(nameof(OBJ3V1), "+OBJ3-V1", string.Empty, 1, "OBJ", 3, string.Empty);
            V OBJ3V2 = new V(nameof(OBJ3V2), "+OBJ3-V2", string.Empty, 2, "OBJ", 3, string.Empty);
            V OBJ3V3 = new V(nameof(OBJ3V3), "+OBJ3-V3", string.Empty, 3, "OBJ", 3, string.Empty);

            IODevice CAP = new V(string.Empty, string.Empty, StaticHelper.CommonConst.Cap, 1, string.Empty, 1, string.Empty);

            var deviceManagerMock = new Mock<IDeviceManager>();

            deviceManagerMock.Setup(dm => dm.GetDeviceByEplanName(It.IsAny<string>())).Returns(CAP);

            deviceManagerMock.Setup(dm => dm.GetDeviceByEplanName(nameof(OBJ1V1))).Returns(OBJ1V1);
            deviceManagerMock.Setup(dm => dm.GetDeviceByEplanName(nameof(OBJ2V2))).Returns(OBJ2V2);
            deviceManagerMock.Setup(dm => dm.GetDeviceByEplanName(nameof(OBJ3V1))).Returns(OBJ3V1);
            deviceManagerMock.Setup(dm => dm.GetDeviceByEplanName(nameof(OBJ3V2))).Returns(OBJ3V2);
            deviceManagerMock.Setup(dm => dm.GetDeviceByEplanName(nameof(OBJ3V3))).Returns(OBJ3V3);

            deviceManagerMock.Setup(dm => dm.GetDeviceIndex(nameof(OBJ1V1))).Returns(0);
            deviceManagerMock.Setup(dm => dm.GetDeviceIndex(nameof(OBJ2V2))).Returns(1);
            deviceManagerMock.Setup(dm => dm.GetDeviceIndex(nameof(OBJ3V1))).Returns(2);
            deviceManagerMock.Setup(dm => dm.GetDeviceIndex(nameof(OBJ3V2))).Returns(3);
            deviceManagerMock.Setup(dm => dm.GetDeviceIndex(nameof(OBJ3V3))).Returns(4);

            deviceManagerMock.Setup(dm => dm.GetDeviceByIndex(0)).Returns(OBJ1V1);
            deviceManagerMock.Setup(dm => dm.GetDeviceByIndex(1)).Returns(OBJ2V2);
            deviceManagerMock.Setup(dm => dm.GetDeviceByIndex(2)).Returns(OBJ3V1);
            deviceManagerMock.Setup(dm => dm.GetDeviceByIndex(3)).Returns(OBJ3V2);
            deviceManagerMock.Setup(dm => dm.GetDeviceByIndex(4)).Returns(OBJ3V3);

            var equipment = new Equipment(
                new TechObject.TechObject(string.Empty, GetN => 1, 3, 2,
                "OBJ", -1, string.Empty, string.Empty, new BaseTechObject()));

            var equipmentParameter = new EquipmentParameter("devs", "устройства", "");
            typeof(BaseParameter).GetField("deviceManager",
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManagerMock.Object);

            typeof(Equipment).GetProperty("deviceManager",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Static)
                .SetValue(null, deviceManagerMock.Object);

            equipment.AddItems(new List<BaseParameter>() { equipmentParameter });

            equipment.SetEquipmentValue("devs", "OBJ1V1 OBJ2V2 OBJ3V3");

            equipment.ModifyDevNames();

            Assert.AreEqual($"{nameof(OBJ3V1)} {nameof(OBJ3V2)} {nameof(OBJ3V3)}", equipmentParameter.Value);
        }

        [Test]
        public void Autocomplete()
        {
            var stubDevice = new LS("", "", "", 1, "", 1, "");

            var deviceManager = Mock.Of<IDeviceManager>(m =>
                m.GetDevice("OBJ1LS1") == stubDevice
            );

            var equipment = new Equipment(
                new TechObject.TechObject(string.Empty, GetN => 1, 1, 2,
                "OBJ", -1, string.Empty, string.Empty, new BaseTechObject()));

            typeof(Equipment).GetProperty("deviceManager",
               System.Reflection.BindingFlags.NonPublic |
               System.Reflection.BindingFlags.Static)
               .SetValue(null, deviceManager);

            var p1 = new ActiveParameter("P1", "P1", "");
            var p2 = new ActiveParameter("P2", "P2", "");
            p2.SetNewValue("LS1");
            var p3 = new ActiveParameter("P3", "P3", "LS1");
            p3.SetNewValue("LS1");
            var p4 = new ActiveParameter("P4", "P4", "LS1");
            p4.SetNewValue("LS2");
            var p5 = new ActiveParameter("P4", "P4", "LS1");

            equipment.AddItems(new List<BaseParameter>()
            {
                p1, p2, p3, p4, p5
            });

            equipment.Autocomplete();

            CollectionAssert.AreEqual(
                new List<string>() { "", "LS1", "OBJ1LS1", "LS2", "OBJ1LS1" },
                equipment.Items.OfType<BaseParameter>().Select(p => p.Value));
        }


        [Test]
        public void Check()
        {
            var techObject = new TechObject.TechObject(string.Empty, GetN => 1, 1, 2,
                "OBJ", -1, string.Empty, string.Empty, new BaseTechObject());
            techObject.GetParamsManager().AddFloatParam("", 0, "", "PARAMETER_1");
            techObject.GetParamsManager().AddFloatParam("", 0, "", "PARAMETER_2");

            var equipment = new Equipment(techObject);

            var par = new EquipmentParameter("P_SET_VALUE", "")
            {
                Owner = equipment
            };
            par.SetNewValue("PARAMETER_1");

            equipment.AddItems(new List<BaseParameter>()
            {
                par
            });

            var res = equipment.Check();

            par.SetNewValue("PARAMETER_1 PARAMETER_2");
            res += equipment.Check();

            par.SetValue("UNKNOWN");
            res += equipment.Check();

            par.SetValue("UNKNOWN1 UNKNOWN2");
            res += equipment.Check();

            Assert.AreEqual(
                "Проверьте оборудование: \"\" в объекте \"1.  №1 (#0)\". Некорректные значения: UNKNOWN.\n" +
                "Проверьте оборудование: \"\" в объекте \"1.  №1 (#0)\". Некорректные значения: UNKNOWN1, UNKNOWN2.\n",
                res);
        }

        [Test]
        public void SaveAsLuaTable_Test()
        {
            var techObject = new TechObject.TechObject(string.Empty, GetN => 1, 1, 2,
                "OBJ", -1, string.Empty, string.Empty, new BaseTechObject());
            techObject.GetParamsManager().AddFloatParam("PAR 1", 10, "м.", "PARAMETER_1");

            var LS1 = new LS("DEV1LS1", "+DEV1-LS1", "description", 1, "DEV", 1, "");
            var STUB = new LS(CommonConst.Cap, "+DEV1-LS1", CommonConst.Cap, 1, "DEV", 1, "");
            typeof(BaseParameter).GetField("deviceManager",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Static)
                .SetValue(null, Mock.Of<IDeviceManager>(m =>
                    m.GetDevice("DEV1LS1") == LS1 &&
                    m.GetDeviceByEplanName("DEV1LS1") == LS1 &&
                    m.GetDeviceIndex("DEV1LS1") == 1 &&
                    m.GetDeviceByIndex(1) == LS1 &&
                    m.GetDeviceByIndex(0) == STUB
                ));

            var equipment = new Equipment(techObject);

            var equip = new EquipmentParameter("EQ", "name");
            var par = equip.AddEquipment("EQ_SET_VALUE", "name", "");
            equip.Owner = equipment;

            equip.SetNewValue("DEV1LS1");
            par.SetNewValue("PARAMETER_1");

            equipment.AddItems(new List<BaseParameter>() { equip });

            Assert.AreEqual(
                "equipment =\n" +
                "\t{\n" +
                "\tEQ = 'DEV1LS1',\n" +
                "\tEQ_SET_VALUE = 'PARAMETER_1',\n" +
                "\t},\n",
                equipment.SaveAsLuaTable(""));
        }

        [Test]
        public void Delete_Test()
        {
            var equipment = new Equipment(null);
            var equip = new EquipmentParameter("EQ", "name");
            equip.SetValue("value");

            Assert.Multiple(() =>
            {
                Assert.AreEqual("value", equip.Value);
                Assert.IsTrue(equipment.Delete(equip));
                Assert.AreEqual("", equip.Value);

                Assert.IsFalse(equipment.Delete(1));
            });
        }

        [Test]
        public void Props_Test()
        {
            var equipment = new Equipment(null);


            Assert.Multiple(() =>
            {
                Assert.IsTrue(equipment.IsCopyable);
                Assert.IsTrue(equipment.IsDeletable);
                Assert.IsTrue(equipment.ShowWarningBeforeDelete);
                Assert.IsTrue((equipment as IAutocompletable)?.CanExecute);

                Assert.AreEqual("", equipment.DisplayText[1]);
                Assert.AreEqual("Оборудование", equipment.DisplayText[0]);

                equipment.AddItems(new List<BaseParameter>() { new EquipmentParameter("", "") });
                Assert.AreEqual("Оборудование (1)", equipment.DisplayText[0]);
            });
        }
    }
}
