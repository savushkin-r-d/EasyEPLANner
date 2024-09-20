using Editor;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;


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


        [Test]
        public void InsertCopyAndReplace()
        {
            var baseTechObject = new BaseTechObject()
            {
            };

            baseTechObject.AddBaseOperation("operation_1", "операция_1", 0);
            baseTechObject.AddBaseOperation("operation_2", "операция_2", 0);

            var techObject = new TechObject("", getN => 1, 1, 2, "", -1, "", "", baseTechObject);

            var modesManager = new ModesManager(techObject);

            var copied_operation_1 = new Mode("Операция_1", getN => 1, modesManager,
                new BaseOperation("операция_1", "operation_1", new List<BaseParameter>() { }, new Dictionary<string, List<BaseStep>>() { }));
            var copied_operation_2 = new Mode("Операция_2", getN => 1, modesManager,
                new BaseOperation("операция_2", "operation_2", new List<BaseParameter>() { }, new Dictionary<string, List<BaseStep>>() { }));
            var copied_operation_3 = new Mode("Операция_3", getN => 1, modesManager,
               new BaseOperation("операция_3", "operation_3", new List<BaseParameter>() { }, new Dictionary<string, List<BaseStep>>() { }));
            var copied_operation_no_base = new Mode("Операция", getN => 1, modesManager, new BaseOperation(null));

            Assert.Multiple(() =>
            {
                // Вставка базовой операции 1
                var insertedOperation_1 = modesManager.InsertCopy(copied_operation_1) as Mode;
                Assert.IsNotNull(insertedOperation_1);
                Assert.AreNotSame(copied_operation_1, insertedOperation_1);
                Assert.AreEqual(copied_operation_1.BaseOperation.LuaName, insertedOperation_1.BaseOperation.LuaName);

                // Повторная вставка базовой операции 1
                insertedOperation_1 = modesManager.InsertCopy(copied_operation_1) as Mode;
                Assert.IsNotNull(insertedOperation_1);
                Assert.AreNotSame(copied_operation_1, insertedOperation_1);
                Assert.AreEqual(string.Empty, insertedOperation_1.BaseOperation.LuaName);

                // Вставка несуществующей базовой операции 3
                var insertedOperation_3 = modesManager.InsertCopy(copied_operation_3) as Mode;
                Assert.IsNotNull(insertedOperation_3);
                Assert.AreNotSame(copied_operation_3, insertedOperation_3);
                Assert.AreEqual(string.Empty, insertedOperation_3.BaseOperation.LuaName);

                // no operation
                var insertedOperation_null = modesManager.InsertCopy(null) as Mode;
                Assert.IsNull(insertedOperation_null);

                // Вставка операции без базовой операции
                var insertedOperation_no_base = modesManager.InsertCopy(copied_operation_no_base) as Mode;
                Assert.IsNotNull(insertedOperation_no_base);
                Assert.AreNotSame(copied_operation_no_base, insertedOperation_no_base);
                Assert.AreEqual(copied_operation_no_base.BaseOperation.LuaName, insertedOperation_no_base.BaseOperation.LuaName);

                // Замена предыдущей операции на базовую операцию 2
                var replacedOperation_2 = modesManager.Replace(insertedOperation_no_base, copied_operation_2) as Mode;
                Assert.IsNotNull(replacedOperation_2);
                Assert.AreNotSame(copied_operation_2, replacedOperation_2);
                Assert.AreEqual(copied_operation_2.BaseOperation.LuaName, replacedOperation_2.BaseOperation.LuaName);

                // Замена предыдущей операции на базовую операцию 1
                var replacedOperation_1 = modesManager.Replace(replacedOperation_2, copied_operation_1) as Mode;
                Assert.IsNotNull(replacedOperation_1);
                Assert.AreNotSame(copied_operation_1, replacedOperation_1);
                Assert.AreEqual(string.Empty, replacedOperation_1.BaseOperation.LuaName);

                // Замена предыдущей операции на несуществующую базовую операцию 3
                var replacedOperation_3 = modesManager.Replace(replacedOperation_1, copied_operation_3) as Mode;
                Assert.IsNotNull(replacedOperation_3);
                Assert.AreNotSame(copied_operation_3, replacedOperation_3);
                Assert.AreEqual(string.Empty, replacedOperation_3.BaseOperation.LuaName);

                // no operation
                var replacedOperation_null = modesManager.Replace(null, null) as Mode;
                Assert.IsNull(replacedOperation_null);
            });
        }
    }
}
