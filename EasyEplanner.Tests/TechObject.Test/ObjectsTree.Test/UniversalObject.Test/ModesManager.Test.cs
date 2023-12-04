using Editor;
using Moq;
using NUnit.Framework;


namespace TechObject.Tests
{
    class ModesManagerTest
    {

        [Test]
        public void SetUpFromBaseTechObject_CheckModes()
        {
            var techObject = new TechObject("name", getN => 1, 1, 1, "eplanName", 1, "NameBc", "", null);
            var modesManager = new ModesManager(techObject);
            var baseTechObject = new BaseTechObject();
            
            baseTechObject.AddBaseOperation("LuaName1", "Name1", 1);
            baseTechObject.AddBaseOperation("LuaName2", "Name2", 2);
            baseTechObject.AddBaseOperation("LuaName3", "Name3", 3);

            modesManager.SetUpFromBaseTechObject(baseTechObject);
            Assert.AreEqual(3, modesManager.Modes.Count);
        }

        [Test]
        public void UpdateOnGenericTechObject()
        {
            var modeSetNameMethodCalled = false;
            var modeSetBaseOperationMethodCalled = false;

            var modesManager = new ModesManager(null);
            var genericModesManager = new ModesManager(null);

            genericModesManager.AddMode("operation 1", "");
            genericModesManager.AddMode("operation 2", "");

            modesManager.AddMode("operation", "");

            var modeMock = new Mock<Mode>("operation", new GetN(n => 1), modesManager, null);
            modeMock.Setup(x => x.SetNewValue(It.IsAny<string>()))
                .Callback<string>(name =>
                {
                    Assert.AreEqual(genericModesManager.Modes[0].Name, name);
                    modeSetNameMethodCalled = true;
                });
            modeMock.Setup(x => x.SetNewValue(It.IsAny<string>(), true))
                .Callback<string, bool>((baseOperationLuaName, _) =>
                {
                    Assert.AreEqual(genericModesManager.Modes[0].BaseOperation.LuaName,
                        baseOperationLuaName);
                    modeSetBaseOperationMethodCalled = true;
                });

            modesManager.Modes[0] = modeMock.Object;

            Assert.Multiple(() =>
            {
                modesManager.UpdateOnGenericTechObject(genericModesManager);
                Assert.IsTrue(modeSetNameMethodCalled);
                Assert.IsTrue(modeSetBaseOperationMethodCalled);
            });
        }

        [Test]
        public void CanMove()
        {
            var modesManager = new ModesManager(null);
            
            var mode1 = modesManager.AddMode("operation 1", "");
            var mode2 = modesManager.AddMode("operation 2", "");

            Assert.Multiple(() =>
            {
                Assert.IsTrue(modesManager.CanMoveDown(mode1));
                Assert.IsFalse(modesManager.CanMoveDown(mode2));
                Assert.IsFalse(modesManager.CanMoveDown(0));
                Assert.IsFalse(modesManager.CanMoveUp(mode1));
                Assert.IsTrue(modesManager.CanMoveUp(mode2));
                Assert.IsFalse(modesManager.CanMoveUp(0));
            });
        }

        [Test]
        public void Move()
        {
            var editorMock = new Mock<IEditor>();

            var modesManager = new ModesManager(null);
            var mode1 = modesManager.AddMode("operation 1", "");
            var mode2 = modesManager.AddMode("operation 2", "");

            Assert.Multiple(() =>
            {
                Assert.AreSame(mode2, modesManager.MoveUp(mode2));
                Assert.IsNull(modesManager.MoveUp(mode2));
                Assert.IsNull(modesManager.MoveUp(0));

                Assert.AreSame(mode2, modesManager.MoveDown(mode2));
                Assert.IsNull(modesManager.MoveDown(mode2));
                Assert.IsNull(modesManager.MoveDown(0));
            });
        }
    }
}
