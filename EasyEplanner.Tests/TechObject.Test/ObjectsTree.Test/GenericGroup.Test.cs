﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TechObject;

namespace TechObjectTests
{
    public class GenericGroupTest
    {
        [Test]
        public void GenericGroupPropertiesTest()
        {
            var techObjectManagerMock = new Mock<ITechObjectManager>();

            var techObject = new TechObject.TechObject("Танк", GetN => 1, 1, 2, "", -1, "", "", new BaseTechObject());
            var baseObject = new BaseObject("BTO", techObjectManagerMock.Object);
            
            var genericGroup = new GenericGroup(techObject, baseObject, techObjectManagerMock.Object);

            Assert.Multiple(() => 
            {
                Assert.IsTrue(genericGroup.IsMoveable);
                Assert.IsTrue(genericGroup.IsInsertableCopy);
                Assert.IsTrue(genericGroup.IsDeletable);
                Assert.IsTrue(genericGroup.ShowWarningBeforeDelete);
                Assert.IsTrue(genericGroup.IsInsertable);
                Assert.AreEqual($"{techObject.Name} (0)", genericGroup.DisplayText[0]);
            });
        }

        [Test]
        public void InsertCopy()
        {
            var techObjects = new List<TechObject.TechObject>();

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(tom => tom.TechObjects).Returns(techObjects);

            var baseTechObject = new BaseTechObject()
            {
                EplanName = "BTO",
            };
            var baseObject = new BaseObject("BTO", techObjectManagerMock.Object);

            var techObject1 = new TechObject.TechObject("Танк", GetN => 1, 1, 2, "TANK", -1, "", "", baseTechObject);
            var techObject2 = new TechObject.TechObject("Танк", GetN => 2, 2, 2, "TANK", -1, "", "", baseTechObject);
            techObject2.MarkToCut = true;
            techObject2.AddParent(baseObject);

            var techObject3 = new TechObject.TechObject("Танк", GetN => 2, 2, 2, "TANK", -1, "", "", baseTechObject);
            techObject3.MarkToCut = true;

            techObjects.Add(techObject1);
            techObjects.Add(techObject2);
            techObjects.Add(techObject3);

            var genericGroup = new GenericGroup(techObject1, baseObject, techObjectManagerMock.Object);


            Assert.Multiple(() =>
            {
                Assert.AreEqual(3, techObjects.Count);
                Assert.AreEqual(0, genericGroup.InheritedTechObjects.Count);

                var copyTechObject1 = genericGroup.InsertCopy(techObject1);
                Assert.AreNotSame(techObject1, copyTechObject1);
                Assert.AreEqual(4, techObjects.Count);
                Assert.AreEqual(1, genericGroup.InheritedTechObjects.Count);
                
                var cuttedCopyTechObject2 = genericGroup.InsertCopy(techObject2);
                Assert.AreSame(techObject2, cuttedCopyTechObject2);
                Assert.AreEqual(4, techObjects.Count);
                Assert.AreEqual(2, genericGroup.InheritedTechObjects.Count);

                var NotTechObjectNullResult = genericGroup.InsertCopy(1);
                Assert.IsNull(NotTechObjectNullResult);

                var TechObjectOtherParentNullResult = genericGroup.InsertCopy(techObject3);
                Assert.IsNull(TechObjectOtherParentNullResult);
            });
        }

        [Test]
        public void Delete()
        {
            var techObjects = new List<TechObject.TechObject>();

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(tom => tom.TechObjects).Returns(techObjects);
            var baseTechObject = new BaseTechObject()
            {
                EplanName = "BTO",
            };
            var baseObject = new BaseObject("BTO", techObjectManagerMock.Object);


            var techObject = new TechObject.TechObject("Танк", GetN => 1, 1, 2, "TANK", -1, "", "", baseTechObject);
            techObjects.Add(techObject);

            var genericGroup = new GenericGroup(techObject, baseObject, techObjectManagerMock.Object);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(0, genericGroup.InheritedTechObjects.Count);
                var techObject1 = genericGroup.Insert() as TechObject.TechObject;
                var techObject2 = genericGroup.Insert() as TechObject.TechObject;
                Assert.AreEqual(2, genericGroup.InheritedTechObjects.Count);

                Assert.IsTrue(genericGroup.Delete(techObject1));
                Assert.AreEqual(1, genericGroup.InheritedTechObjects.Count);

                Assert.IsTrue(genericGroup.Delete(techObject2));
                Assert.AreEqual(0, genericGroup.InheritedTechObjects.Count);

                Assert.IsFalse(genericGroup.Delete(1));
            });
        }

        [Test]
        public void Insert()
        {
            var techObjects = new List<TechObject.TechObject>();

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(tom => tom.TechObjects).Returns(techObjects);
            var baseTechObject = new BaseTechObject()
            {
                EplanName = "BTO",
            };
            var baseObject = new BaseObject("BTO", techObjectManagerMock.Object);

            var techObject = new TechObject.TechObject("Танк", GetN => 1, 1, 2, "TANK", -1, "", "", baseTechObject);

            var genericGroup = new GenericGroup(techObject, baseObject, techObjectManagerMock.Object);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(0, genericGroup.InheritedTechObjects.Count);
                var techObject1 = genericGroup.Insert() as TechObject.TechObject;
                Assert.AreEqual(1, genericGroup.InheritedTechObjects.Count);
                Assert.AreEqual(techObject.Name, techObject1.Name);
            });
        }

        [Test]
        public void Move()
        {
            var techObjects = new List<TechObject.TechObject>();

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(tom => tom.TechObjects).Returns(techObjects);
            var baseTechObject = new BaseTechObject()
            {
                EplanName = "BTO",
            };
            var baseObject = new BaseObject("BTO", techObjectManagerMock.Object);

            var techObject = new TechObject.TechObject("Танк", GetN => 1, 1, 2, "TANK", -1, "", "", baseTechObject);

            techObjects.Add(techObject);

            var genericGroup = new GenericGroup(techObject, baseObject, techObjectManagerMock.Object);

            var techObject1 = genericGroup.Insert() as TechObject.TechObject;
            var techObject2 = genericGroup.Insert() as TechObject.TechObject;
            var techObject3 = genericGroup.Insert() as TechObject.TechObject;

            Assert.Multiple(() => 
            {
                // UP
                var movedObject = genericGroup.MoveUp(techObject2);
                Assert.AreSame(techObject2, movedObject);
                Assert.AreEqual(1, techObjects.IndexOf(techObject2));
                Assert.AreEqual(2, techObject2.GetLocalNum);

                movedObject = genericGroup.MoveUp(techObject2);
                Assert.IsNull(movedObject);

                movedObject = genericGroup.MoveUp(1);
                Assert.IsNull(movedObject);

                // DOWN
                movedObject = genericGroup.MoveDown(techObject1);
                Assert.AreSame(techObject1, movedObject);
                Assert.AreEqual(3, techObjects.IndexOf(techObject1));
                Assert.AreEqual(1, techObject1.GetLocalNum);

                movedObject = genericGroup.MoveDown(techObject1);
                Assert.IsNull(movedObject);

                movedObject = genericGroup.MoveDown(1);
                Assert.IsNull(movedObject);
            });
        }
    }
}
