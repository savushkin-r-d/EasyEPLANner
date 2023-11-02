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
            var techObject_3 = new TechObject.TechObject("Танк", GetN => 3, 3, 2, "OBJ3", -1, "OBJ31", "", baseTechObject);


            techObjectManagerMock.Setup(m => m.GetTObject(1)).Returns(techObject_1);
            techObjectManagerMock.Setup(m => m.GetTObject(2)).Returns(techObject_2);
            techObjectManagerMock.Setup(m => m.GetTObject(3)).Returns(techObject_3);

            techObjectManagerMock.Setup(m => m.GetTechObjectN(It.IsAny<string>(), "OBJ1", 1)).Returns(1);
            techObjectManagerMock.Setup(m => m.GetTechObjectN(It.IsAny<string>(), "OBJ2", 2)).Returns(2);
            techObjectManagerMock.Setup(m => m.GetTechObjectN(It.IsAny<string>(), "OBJ3", 3)).Returns(3);

            var techObject = new TechObject.TechObject("Танк", GetN => 1, 1, 2, "TANK", -1, "TANK", "", baseTechObject);
            var genericTechObject = new GenericTechObject(techObject);

            var managerInstance = typeof(TechObject.TechObject).GetProperty("TechObjectManagerInstance",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);
            managerInstance.SetValue(techObject, techObjectManagerMock.Object);

            var attachedObjects = techObject.AttachedObjects;
            var genericAttachedObjects = genericTechObject.AttachedObjects;

            var attachedObjectsManagerField = typeof(AttachedObjects).GetField("techObjectManager",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            var TOManagerField = typeof(BaseStrategy).GetField("techObjectManager",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            attachedObjectsManagerField.SetValue(attachedObjects, techObjectManagerMock.Object);
            attachedObjectsManagerField.SetValue(genericAttachedObjects, techObjectManagerMock.Object);
            TOManagerField.SetValue(attachedObjects.WorkStrategy, techObjectManagerMock.Object);

            genericAttachedObjects.SetValue("1 2 3");

            Assert.Multiple(() =>
            {
                attachedObjects.UpdateOnGenericTechObject(genericAttachedObjects);
                Assert.AreEqual("1 2 3", attachedObjects.Value);
            });
        }
    }
}
