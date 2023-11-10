using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;
using NUnit.Framework;
using Moq;
using System.Threading;
using Editor;
using System.Windows.Forms;

namespace TechObjectTests
{
    public class BaseObjectTest
    {
        [Test]
        public void AddObjectWhenLoadFromLua()
        {
            var baseTechObject = new BaseTechObject();
            baseTechObject.EplanName = "BaseTechObjectName";

            var genericTechObject = new GenericTechObject("", 2, "", -1, "", string.Empty, baseTechObject);
            

            var techObjects = new List<TechObject.TechObject>();

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(obj => obj.TechObjects).Returns(techObjects);
            techObjectManagerMock.Setup(obj => obj.GetGenericTObject(It.IsAny<int>()))
                .Returns((GenericTechObject)null);
            techObjectManagerMock.Setup(obj => obj.GetGenericTObject(1))
                .Returns(genericTechObject);

            var baseObject = new BaseObject("BaseTechObjectName", techObjectManagerMock.Object);
            genericTechObject.GenericGroup = new GenericGroup(genericTechObject, baseObject, techObjectManagerMock.Object);

            var techObject = new TechObject.TechObject("", GetN => 1, 1, 2, "TANK", -1, "", string.Empty, baseTechObject);

            Assert.Multiple(() =>
            {
                baseObject.AddObjectWhenLoadFromLua(techObject, 1);
                Assert.AreEqual(techObject, genericTechObject.GenericGroup.InheritedTechObjects.First());

                baseObject.AddObjectWhenLoadFromLua(techObject, 2);
                Assert.IsTrue(baseObject.TechObjects.Contains(techObject));
            });
        }

        [Test]
        public void AddGenericObjectWhenLoadFromLua()
        {
            var baseTechObject = new BaseTechObject();
            baseTechObject.EplanName = "BaseTechObjectName";

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(obj => obj.TechObjects).Returns(new List<TechObject.TechObject>());
            techObjectManagerMock.Setup(obj => obj.GetGenericTObject(It.IsAny<int>()))
                .Returns((GenericTechObject)null);
            
            var baseObject = new BaseObject("BaseTechObjectName", techObjectManagerMock.Object);

            var genericTechObject = new GenericTechObject("", 2, "", -1, "", string.Empty, baseTechObject);
            baseObject.AddGenericObjectWhenLoadFromLua(genericTechObject);

            Assert.IsTrue(baseObject.GenericGroups.Any());
        }

        [Test]
        public void GetItems()
        {
            var techObjects = new List<TechObject.TechObject>() 
            { 
                new TechObject.TechObject("", GetN => 1, 1, 2, "TANK", -1, "", string.Empty, new BaseTechObject()) 
            };

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(obj => obj.TechObjects).Returns(techObjects);
            
            var baseObject = new BaseObject("BaseTechObjectName", techObjectManagerMock.Object);
            baseObject.AddObjectWhenLoadFromLua(techObjects[0], 0);

            Assert.IsTrue(baseObject.Items.Any());
        }

        [Test]
        public void AddObject()
        {
            var techObjects = new List<TechObject.TechObject>();
            
            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(obj => obj.TechObjects).Returns(techObjects);
            
            var baseObject = new BaseObject("BaseTechObjectName", techObjectManagerMock.Object);

            var techObject = new TechObject.TechObject("", GetN => 1, 1, 2, "TANK", -1, "", string.Empty, new BaseTechObject());

            baseObject.AddObject(techObject);

            Assert.IsTrue(baseObject.TechObjects.Any() is false && techObjects.Any());
        }

        [Test]
        public void CreateGenericGroup()
        {
            var techObjects = new List<TechObject.TechObject>();
            var genericTechObjects = new List<TechObject.GenericTechObject>();
            var techObject = new TechObject.TechObject("", GetN => 1, 1, 2, "TANK", -1, "", string.Empty, new BaseTechObject());

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(obj => obj.TechObjects).Returns(techObjects);
            techObjectManagerMock.Setup(obj => obj.GenericTechObjects).Returns(genericTechObjects);

            var baseObject = new BaseObject("BaseTechObjectName", techObjectManagerMock.Object);

            var genericGroup = baseObject.CreateGenericGroup(techObject);

            Assert.IsTrue(baseObject.TechObjects.Any() is false &&
                genericGroup != null && genericTechObjects.Any());
        }

        [Test]
        public void AddAndRemoveLocalObjects()
        {
            var techObject = new TechObject.TechObject("", GetN => 1, 1, 2, "TANK", -1, "", string.Empty, new BaseTechObject());
            var techObjectManagerMock = new Mock<ITechObjectManager>();
            var baseObject = new BaseObject("BaseTechObjectName", techObjectManagerMock.Object);

            Assert.Multiple(() =>
            {
                baseObject.AddLocalObject(techObject);
                Assert.AreEqual(1, baseObject.Count);

                baseObject.RemoveLocalObject(techObject);
                Assert.AreEqual(0, baseObject.Count);
            });
        }

        [TestCase(DialogResult.Yes)]
        [TestCase(DialogResult.No)]
        public void Delete(DialogResult deleteTechObjectInGroupDialogResult)
        {
            var techObjects = new List<TechObject.TechObject>();
            var genericTechObjects = new List<TechObject.GenericTechObject>();
            var techObject = new TechObject.TechObject("", GetN => 1, 1, 2, "TANK", -1, "", string.Empty, new BaseTechObject()); 
            var techObjectOther = new TechObject.TechObject("", GetN => 2, 1, 2, "TANK", -1, "", string.Empty, new BaseTechObject());

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(obj => obj.TechObjects).Returns(techObjects);
            techObjectManagerMock.Setup(obj => obj.GenericTechObjects).Returns(genericTechObjects);

            var editorMock = new Mock<IEditor>();
            editorMock.Setup(obj => obj.DialogDeletingGenericGroup())
                .Returns(deleteTechObjectInGroupDialogResult);

            var baseObject = new BaseObject("BaseTechObjectName", techObjectManagerMock.Object);
            var editorProperty = typeof(BaseObject).GetProperty("editor",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            editorProperty.SetValue(baseObject, editorMock.Object);

            baseObject.AddObject(techObject);
            baseObject.AddObject(techObjectOther);

            var genericGroup = baseObject.CreateGenericGroup(techObject);

            baseObject.Delete(genericGroup);

            if (deleteTechObjectInGroupDialogResult == DialogResult.Yes)
            {
                Assert.AreEqual(1, baseObject.Count);
            }
            else
            {
                Assert.AreEqual(2, baseObject.Count);
            }
        }

        [Test]
        public void MoveTest()
        {
            var techObjects = new List<TechObject.TechObject>();
            var genericTechObjects = new List<TechObject.GenericTechObject>();

            var techObject1 = new TechObject.TechObject("1", GetN => 1, 1, 2, "TANK", -1, "", string.Empty, new BaseTechObject());
            var techObject2 = new TechObject.TechObject("2", GetN => 2, 1, 2, "TANK", -1, "", string.Empty, new BaseTechObject());
            var techObject3 = new TechObject.TechObject("3", GetN => 3, 1, 2, "TANK", -1, "", string.Empty, new BaseTechObject());
            var techObject4 = new TechObject.TechObject("4", GetN => 4, 1, 2, "TANK", -1, "", string.Empty, new BaseTechObject());

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(obj => obj.TechObjects).Returns(techObjects);
            techObjectManagerMock.Setup(obj => obj.GenericTechObjects).Returns(genericTechObjects);

            var baseObject = new BaseObject("BaseTechObjectName", techObjectManagerMock.Object);

            baseObject.AddObject(techObject1);
            baseObject.AddObject(techObject2);

            baseObject.TechObjects.Add(techObject1);
            baseObject.TechObjects.Add(techObject2);

            baseObject.AddObject(techObject3);
            baseObject.AddObject(techObject4);

            var genericGroup_1 = baseObject.CreateGenericGroup(techObject3);
            var genericGroup_2 = baseObject.CreateGenericGroup(techObject4);

            Assert.Multiple(() =>
            {
                // UP
                var globlaNumMovedTO = techObject2.GlobalNum;
                var globalNumOtherTO = techObject1.GlobalNum;

                var movedUpTO = baseObject.MoveUp(techObject2);
                Assert.AreSame(techObject2, movedUpTO);
                Assert.AreEqual(globalNumOtherTO, techObject2.GlobalNum);

                var movedUpTOnull = baseObject.MoveUp(techObject2);
                Assert.IsNull(movedUpTOnull);
                Assert.AreEqual(globalNumOtherTO, techObject2.GlobalNum);

                var movedUpGG = baseObject.MoveUp(genericGroup_2);
                Assert.AreSame(genericGroup_2, movedUpGG);

                var movedUpGGnull = baseObject.MoveUp(genericGroup_2);
                Assert.IsNull(movedUpGGnull);

                var NoTypeMove = baseObject.MoveUp(1);
                Assert.IsNull(NoTypeMove);

                // DOWN
                globlaNumMovedTO = techObject2.GlobalNum;
                globalNumOtherTO = techObject1.GlobalNum;

                var movedDownTO = baseObject.MoveDown(techObject2);
                Assert.AreSame(techObject2, movedDownTO);
                Assert.AreEqual(globalNumOtherTO, techObject2.GlobalNum);

                var movedDownTOnull = baseObject.MoveDown(techObject2);
                Assert.IsNull(movedDownTOnull);
                Assert.AreEqual(globalNumOtherTO, techObject2.GlobalNum);

                var movedDownGG = baseObject.MoveDown(genericGroup_2);
                Assert.AreSame(genericGroup_2, movedDownGG);

                var movedDownGGnull = baseObject.MoveDown(genericGroup_2);
                Assert.IsNull(movedDownGGnull);

                NoTypeMove = baseObject.MoveDown(1);
                Assert.IsNull(NoTypeMove);
            });
        }

        [Test]
        public void Cut()
        {
            var techObjects = new List<TechObject.TechObject>();
            var genericTechObjects = new List<TechObject.GenericTechObject>();

            var techObject1 = new TechObject.TechObject("1", GetN => 1, 1, 2, "TANK", -1, "", string.Empty, new BaseTechObject());
            var techObject2 = new TechObject.TechObject("2", GetN => 2, 1, 2, "TANK", -1, "", string.Empty, new BaseTechObject());

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(obj => obj.TechObjects).Returns(techObjects);
            techObjectManagerMock.Setup(obj => obj.GenericTechObjects).Returns(genericTechObjects);

            var baseObject = new BaseObject("BaseTechObjectName", techObjectManagerMock.Object);

            baseObject.AddObject(techObject1);
            baseObject.AddObject(techObject2);

            baseObject.TechObjects.Add(techObject1);
            baseObject.TechObjects.Add(techObject2);

            Assert.Multiple(() =>
            {
                Assert.IsFalse(baseObject.IsCuttable);

                baseObject.CreateGenericGroup(techObject2);
                Assert.IsTrue(baseObject.IsCuttable);

                var cuttedObject = baseObject.Cut(techObject1);
                Assert.AreSame(techObject1, cuttedObject);
                Assert.IsEmpty(baseObject.TechObjects);

                Assert.IsNull(baseObject.Cut(baseObject));
            });
        }

        [Test]
        public void Replace()
        {
            var techObjects = new List<TechObject.TechObject>();

            var techObject1 = new TechObject.TechObject("1", GetN => 1, 1, 2, "TANK", -1, "", string.Empty, new BaseTechObject());
            var techObject2 = new TechObject.TechObject("2", GetN => 2, 2, 3, "TANK_COPY", -1, "", string.Empty, new BaseTechObject());

            techObjects.Add(techObject1);
            techObjects.Add(techObject2);

            var techObjectManagerMock = new Mock<ITechObjectManager>();
            techObjectManagerMock.Setup(obj => obj.TechObjects).Returns(techObjects);

            var baseObject = new BaseObject("bto", techObjectManagerMock.Object);

            var baseTechObjectField = typeof(BaseObject).GetField("baseTechObject",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            baseTechObjectField.SetValue(baseObject, new BaseTechObject());

            baseObject.AddObject(techObject1);
            baseObject.TechObjects.Add(techObject1);

            baseObject.Replace(techObject1, techObject2);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(techObject2.TechType, baseObject.TechObjects[0].TechType);
                Assert.AreEqual(techObject2.NameEplan, baseObject.TechObjects[0].NameEplan);
            });
        }
    }
}
