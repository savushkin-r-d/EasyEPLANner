using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;
using NUnit.Framework;
using Moq;
using TechObject.AttachedObjectStrategy;

namespace TechObjectTests
{
    public class AttachedObjectsTest
    {

        [Test]
        public void UpdateOnGenerictechObject()
        {
            var techObjectManagerMock = new Mock<ITechObjectManager>();

            var baseTechObject = new BaseTechObject()
            {
                S88Level = 2,
                EplanName = "base_tech_object"
            };

            var techObject_1 = new TechObject.TechObject("Танк", GetN => 1, 1, 2, "OBJ1", -1, "OBJ11", "", baseTechObject);
            var techObject_2 = new TechObject.TechObject("Танк", GetN => 2, 2, 2, "OBJ2", -1, "OBJ21", "", baseTechObject);
            var techObject_3 = new TechObject.TechObject("Танк", GetN => 3, 3, 3, "OBJ3", -1, "OBJ31", "", baseTechObject);


            techObjectManagerMock.Setup(m => m.GetTObject(1)).Returns(techObject_1);
            techObjectManagerMock.Setup(m => m.GetTObject(2)).Returns(techObject_2);
            techObjectManagerMock.Setup(m => m.GetTObject(3)).Returns(techObject_3);

            techObjectManagerMock.Setup(m => m.GetTechObjectN(It.IsAny<string>(), "OBJ1", 1)).Returns(1);
            techObjectManagerMock.Setup(m => m.GetTechObjectN(It.IsAny<string>(), "OBJ2", 2)).Returns(2);
            techObjectManagerMock.Setup(m => m.GetTechObjectN(It.IsAny<string>(), "OBJ3", 3)).Returns(3);

            techObjectManagerMock.Setup(m => m.TypeAdjacentTObjectIdByTNum(1, 2)).Returns(2);
            techObjectManagerMock.Setup(m => m.TypeAdjacentTObjectIdByTNum(3, 2)).Returns(3);

            var techObject = new TechObject.TechObject("Танк", GetN => 1, 2, 2, "TANK", -1, "TANK", "", baseTechObject);
            var genericTechObject = new GenericTechObject(techObject);

            var managerInstance = typeof(TechObject.TechObject).GetProperty("TechObjectManagerInstance",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);
            managerInstance.SetValue(techObject, techObjectManagerMock.Object);

            var attachedObjects = techObject.AttachedObjects;
            var genericAttachedObjects = genericTechObject.AttachedObjects;

            var attachedObjectsManagerField = typeof(AttachedObjects).GetField("techObjectManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            var TOManagerField = typeof(BaseStrategy).GetField("techObjectManager",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            attachedObjectsManagerField.SetValue(null, techObjectManagerMock.Object);
            TOManagerField.SetValue(attachedObjects.WorkStrategy, techObjectManagerMock.Object);

            genericAttachedObjects.SetValue("1 3");

            Assert.Multiple(() =>
            {
                attachedObjects.UpdateOnGenericTechObject(genericAttachedObjects);
                Assert.AreEqual("2 3", attachedObjects.Value);
            });
        }

        [TestCase("1 3", "\"Танк 1\", \"Танк 3\"")]
        [TestCase("2 1", "\"Танк 2\", \"Танк 1\"")]
        [TestCase("1 2 3", "\"Танк 1\", \"Танк 2\", \"Танк 3\"")]
        public void GenerateAttachedObjectsString(string objects, string expectedResultString)
        {
            var baseTechObject = new BaseTechObject()
            {
                S88Level = 2,
                EplanName = "base_tech_object"
            };
            var techObject = new TechObject.TechObject("Танк", GetN => 1, 1, 2, "TANK", -1, "TANK", "", baseTechObject);
            var attachedObjects = techObject.AttachedObjects;

            var method = typeof(AttachedObjects).GetMethod("GenerateAttachedObjectsString",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            var techObject_1 = new TechObject.TechObject("Танк", GetN => 1, 1, 2, "OBJ1", -1, "OBJ11", "", baseTechObject);
            var techObject_2 = new TechObject.TechObject("Танк", GetN => 2, 2, 2, "OBJ2", -1, "OBJ21", "", baseTechObject);
            var techObject_3 = new TechObject.TechObject("Танк", GetN => 3, 3, 2, "OBJ3", -1, "OBJ31", "", baseTechObject);

            techObjectManagerMock.Setup(m => m.GetTObject(1)).Returns(techObject_1);
            techObjectManagerMock.Setup(m => m.GetTObject(2)).Returns(techObject_2);
            techObjectManagerMock.Setup(m => m.GetTObject(3)).Returns(techObject_3);

            var attachedObjectsManagerField = typeof(AttachedObjects).GetField("techObjectManager",
                 System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            attachedObjectsManagerField.SetValue(null, techObjectManagerMock.Object);

            var res = method.Invoke(attachedObjects, new object[] { objects });

            Assert.AreEqual(expectedResultString, res);
        }

        [Test]
        public void InsertCopy()
        {
            var techObjectManagerMock = new Mock<ITechObjectManager>();

            var baseTechObject_tank = new BaseTechObject()
            {
                S88Level = 1,
                EplanName = "TANK",
                Name = "tank",
            };

            var baseTechObject_aggregate = new BaseTechObject()
            {
                S88Level = 2,
                EplanName = "TANK",
                Name = "aggregate_1",
            };

            var baseTechObject_aggregate2 = new BaseTechObject()
            {
                S88Level = 2,
                EplanName = "TANK",
                Name = "aggregate_2",
            };

            var techObject_1 = new TechObject.TechObject("Танк", GetN => 1, 1, 2, "", -1, "", "", baseTechObject_tank);
            var techObject_2 = new TechObject.TechObject("Агрегат", GetN => 2, 2, 2, "", -1, "", "", baseTechObject_tank);
            var techObject_3 = new TechObject.TechObject("Агрегат", GetN => 3, 3, 3, "", -1, "", "", baseTechObject_aggregate);
            var techObject_4 = new TechObject.TechObject("Агрегат", GetN => 4, 4, 3, "", -1, "", "", baseTechObject_aggregate);
            var techObject_5 = new TechObject.TechObject("Агрегат2", GetN => 5, 5, 3, "", -1, "", "", baseTechObject_aggregate2);

            techObjectManagerMock.Setup(m => m.GetTObject(1)).Returns(techObject_1);
            techObjectManagerMock.Setup(m => m.GetTObject(2)).Returns(techObject_2);
            techObjectManagerMock.Setup(m => m.GetTObject(3)).Returns(techObject_3);
            techObjectManagerMock.Setup(m => m.GetTObject(4)).Returns(techObject_4);
            techObjectManagerMock.Setup(m => m.GetTObject(5)).Returns(techObject_5);

            techObjectManagerMock.Setup(m => m.GetTechObjectN(techObject_1)).Returns(1);
            techObjectManagerMock.Setup(m => m.GetTechObjectN(techObject_2)).Returns(2);
            techObjectManagerMock.Setup(m => m.GetTechObjectN(techObject_3)).Returns(3);
            techObjectManagerMock.Setup(m => m.GetTechObjectN(techObject_4)).Returns(4);
            techObjectManagerMock.Setup(m => m.GetTechObjectN(techObject_5)).Returns(5);

            var managerInstance = typeof(TechObject.TechObject).GetProperty("TechObjectManagerInstance",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);
            managerInstance.SetValue(techObject_1, techObjectManagerMock.Object);
            managerInstance.SetValue(techObject_2, techObjectManagerMock.Object);
            managerInstance.SetValue(techObject_3, techObjectManagerMock.Object);
            managerInstance.SetValue(techObject_4, techObjectManagerMock.Object);
            managerInstance.SetValue(techObject_5, techObjectManagerMock.Object);

            var baseStrategyTechObjectManager = typeof(BaseStrategy).GetField("techObjectManager",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            baseStrategyTechObjectManager.SetValue(techObject_1.AttachedObjects.WorkStrategy,
                techObjectManagerMock.Object);
            baseStrategyTechObjectManager.SetValue(techObject_2.AttachedObjects.WorkStrategy,
                techObjectManagerMock.Object);

            Assert.Multiple(() =>
            {
                techObject_1.AttachedObjects.SetNewValue("3");
                techObject_2.AttachedObjects.SetNewValue("4 5");

                techObject_1.AttachedObjects.InsertCopy(techObject_2.AttachedObjects);
                Assert.AreEqual("3 5", techObject_1.AttachedObjects.Value);

                techObject_1.AttachedObjects.Replace(null, techObject_2.AttachedObjects);
                Assert.AreEqual("4 5", techObject_1.AttachedObjects.Value);
            });


        }
    }
}
