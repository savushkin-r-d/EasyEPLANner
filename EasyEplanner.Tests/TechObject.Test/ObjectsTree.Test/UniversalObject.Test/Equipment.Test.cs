using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EplanDevice;
using Moq;
using NUnit.Framework;
using TechObject;

namespace TechObjectTests
{
    public class EquipmentTest
    {
        [Test]
        public void ModifyDevNames()
        {
            V OBJ1V1 = new V(nameof(OBJ1V1), "=OBJ1+V1", string.Empty, 1, "OBJ", 1, string.Empty);
            V OBJ2V2 = new V(nameof(OBJ2V2), "=OBJ2+V2", string.Empty, 2, "OBJ", 2, string.Empty);
            V OBJ3V1 = new V(nameof(OBJ3V1), "=OBJ3+V1", string.Empty, 1, "OBJ", 3, string.Empty);
            V OBJ3V2 = new V(nameof(OBJ3V2), "=OBJ3+V2", string.Empty, 2, "OBJ", 3, string.Empty);
            V OBJ3V3 = new V(nameof(OBJ3V3), "=OBJ3+V3", string.Empty, 3, "OBJ", 3, string.Empty);

            IODevice CAP = new V(string.Empty, string.Empty, StaticHelper.CommonConst.Cap, 1, string.Empty, 1, string.Empty);

            var deviceManagerMock = new Mock<IDeviceManager>();

            deviceManagerMock.Setup(dm => dm.GetDevice(It.IsAny<string>())).Returns(CAP);

            deviceManagerMock.Setup(dm => dm.GetDevice(nameof(OBJ1V1))).Returns(OBJ1V1);
            deviceManagerMock.Setup(dm => dm.GetDevice(nameof(OBJ2V2))).Returns(OBJ2V2);
            deviceManagerMock.Setup(dm => dm.GetDevice(nameof(OBJ3V1))).Returns(OBJ3V1);
            deviceManagerMock.Setup(dm => dm.GetDevice(nameof(OBJ3V2))).Returns(OBJ3V2);
            deviceManagerMock.Setup(dm => dm.GetDevice(nameof(OBJ3V3))).Returns(OBJ3V3);

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

            deviceManagerMock.Setup(dm => dm.GetDeviceByEplanName(It.IsAny<string>())).Returns(OBJ3V3);


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
    }
}
