using TechObject;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace Tests.TechObject
{
    class BaseTechObjectTest
    {
        [Test]
        public void Constructor_CreatingNewObject_ReturnsObjectWithDefaultData()
        {
            const int zeroValue = 0;
            
            var obj = new BaseTechObject();

            Assert.Multiple(() =>
            {
                Assert.IsFalse(obj.UseGroups);
                Assert.AreEqual(zeroValue, obj.SystemParams.Count);
                Assert.AreEqual(zeroValue, obj.S88Level);
                Assert.AreEqual(zeroValue, obj.Parameters.Items.Length);
                Assert.IsNull(obj.Owner);
                Assert.AreEqual(zeroValue, obj.ObjectGroupsList.Count);
                Assert.IsEmpty(obj.Name);
                Assert.IsNull(obj.MainAggregateParameter);
                Assert.IsFalse(obj.IsPID);
                Assert.IsFalse(obj.IsAttachable);
                Assert.AreEqual(zeroValue, obj.Equipment.Count);
                Assert.IsEmpty(obj.EplanName);
                Assert.IsEmpty(obj.BindingName);
                Assert.IsEmpty(obj.BasicName);
                Assert.AreEqual(zeroValue, obj.BaseOperationsList.Count);
                Assert.AreEqual(zeroValue, obj.BaseOperations.Count);
                Assert.AreEqual(zeroValue, obj.AggregateParameters.Count);
            });
        }

        [TestCase(1)]
        [TestCase(100)]
        public void AddEquipment_EmptyEquipment_AddToList(int parametersCount)
        {
            var obj = GetEmpty();
            string paramValueStr = paramValue.ToString();
            
            for (int i = 0; i < parametersCount; i++)
            {
                obj.AddEquipment(paramLuaName, paramName,
                    paramValueStr);
            }

            Assert.AreEqual(parametersCount, obj.Equipment.Count);
        }

        [TestCase(1)]
        [TestCase(100)]
        public void AddActiveParameter_EmptyList_AddToList(int parametersCount)
        {
            var obj = GetEmpty();
            string paramValueStr = paramValue.ToString();
            
            for (int i = 0; i < parametersCount; i++)
            {
                obj.AddActiveParameter(paramLuaName, paramName,
                    paramValueStr);
            }

            Assert.AreEqual(parametersCount, obj.AggregateParameters.Count);
        }

        [TestCase(1)]
        [TestCase(100)]
        public void AddActiveBoolParameter_EmptyList_AddToList(
            int parametersCount)
        {
            string defaultValue = "false";
            var obj = GetEmpty();

            for (int i = 0; i < parametersCount; i++)
            {
                obj.AddActiveBoolParameter(paramLuaName, paramName,
                    defaultValue);
            }

            Assert.AreEqual(parametersCount, obj.AggregateParameters.Count);
        }

        [Test]
        public void AddMainAggregateParameter_NullParameter_AddNewParameter()
        {
            var obj = GetEmpty();
            string defaultValue = paramValue.ToString();
            
            obj.AddMainAggregateParameter(paramLuaName, paramName,
                defaultValue);

            var mainParam = obj.MainAggregateParameter;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(paramLuaName, mainParam.LuaName);
                Assert.AreEqual(paramName, mainParam.Name);
                Assert.AreEqual(defaultValue, mainParam.Value);
                Assert.AreEqual(defaultValue, mainParam.DefaultValue);
            });
        }

        [TestCase(2, 1)]
        [TestCase(101, 100)]
        public void AddBaseOperation_EmptyList_AddBaseOperationToList(
            int expectedOperationsCount, int operationsCount)
        {
            var obj = GetEmpty();

            for (int i = 0; i < operationsCount; i++)
            {
                obj.AddBaseOperation(paramName, paramName, i + 1);
            }
            int baseOperationsCount = obj.BaseOperations.Count;

            Assert.AreEqual(expectedOperationsCount, baseOperationsCount);
        }

        [TestCaseSource(nameof(TestCaseSourceForAddObjectGroup))]
        public void AddObjectGroup_EmptyList_AddObjectGroupToList(
            int groupsCount, string allowedObjects,
            List<BaseTechObjectManager.ObjectType> expectedAllowedObjects)
        {
            var obj = GetEmpty();

            for (int i = 0; i < groupsCount; i++)
            {
                obj.AddObjectGroup(paramLuaName, paramName, allowedObjects);
            }

            var group = obj.ObjectGroupsList.First();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(groupsCount, obj.ObjectGroupsList.Count);
                Assert.AreEqual(expectedAllowedObjects, group.WorkStrategy.AllowedObjects);
            });
        }

        private static object[] TestCaseSourceForAddObjectGroup()
        {
            return new object[]
            {
                new object[]
                {
                    1, 
                    "all", 
                    new List<BaseTechObjectManager.ObjectType>
                    { 
                        BaseTechObjectManager.ObjectType.Aggregate,
                        BaseTechObjectManager.ObjectType.Unit
                    },
                },
                new object[]
                {
                    5,
                    "aggregates",
                    new List<BaseTechObjectManager.ObjectType>
                    {
                        BaseTechObjectManager.ObjectType.Aggregate,
                    },
                },
                new object[]
                {
                    3,
                    "units",
                    new List<BaseTechObjectManager.ObjectType>
                    {
                        BaseTechObjectManager.ObjectType.Unit
                    },
                },
                new object[]
                {
                    2,
                    "",
                    new List<BaseTechObjectManager.ObjectType>
                    {
                        BaseTechObjectManager.ObjectType.Aggregate,
                    },
                },
            };
        }

        [TestCase("Операция 1", false)]
        [TestCase("Операция 3", false)]
        [TestCase("Операция 5", true)]
        [TestCase("", false)]
        [TestCase(null, true)]
        public void GetBaseOperationByName_ListOperations_ReturnsBaseOperationOrNull(
            string expectedName, bool isOperationNull)
        {
            var obj = GetEmpty();
            var operations = TestCaseSourceForGetBaseOperation();
            foreach (var operation in operations)
            {
                obj.AddBaseOperation(operation.LuaName, operation.Name,
                    operation.DefaultPosition);
            }

            var baseOperation = obj.GetBaseOperationByName(expectedName);
            if (baseOperation == null)
            {
                Assert.IsTrue(isOperationNull);
            }
            else
            {
                Assert.Multiple(() =>
                {
                    Assert.IsFalse(isOperationNull);
                    Assert.AreEqual(expectedName, baseOperation.Name);
                });
            }
        }

        [TestCase("OPERATION1", false)]
        [TestCase("OPERATION3", false)]
        [TestCase("OPERATION5", true)]
        [TestCase("", false)]
        [TestCase(null, true)]
        public void GetBaseOperationByLuaName_ListOperations_ReturnsBaseOperationOrNull(
            string expectedLuaName, bool isOperationNull)
        {
            var obj = GetEmpty();
            var operations = TestCaseSourceForGetBaseOperation();
            foreach (var operation in operations)
            {
                obj.AddBaseOperation(operation.LuaName, operation.Name,
                    operation.DefaultPosition);
            }

            var baseOperation = obj.GetBaseOperationByLuaName(expectedLuaName);
            if (baseOperation == null)
            {
                Assert.Multiple(() =>
                {
                    Assert.IsTrue(isOperationNull);
                });
            }
            else
            {
                Assert.Multiple(() =>
                {
                    Assert.IsFalse(isOperationNull);
                    Assert.AreEqual(expectedLuaName, baseOperation.LuaName);
                });
            }
        }

        private List<BaseOperation> TestCaseSourceForGetBaseOperation()
        {
            return new List<BaseOperation>
            {
                new BaseOperation("Операция 1", "OPERATION1", null, null),
                new BaseOperation("Операция 2", "OPERATION2", null, null),
                new BaseOperation("Операция 3", "OPERATION3", null, null),
            };
        }

        public void CloneWithArgument_NormalBaseObject_ReturnsFullCopy()
        {
            //TODO: Write test
        }

        public void CloneNoArgument_NormalBaseObject_ReturnsFullCopyWithNullOwners()
        {
            //TODO: Write test
        }

        [Test]
        public void IsAttachable_HaveObjGorups_ReturnsTrueValue()
        {
            var obj = GetEmpty();

            obj.AddObjectGroup(paramLuaName, paramName, string.Empty);
        
            Assert.IsTrue(obj.IsAttachable);
        }

        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(0, false)]
        [TestCase(4, false)]
        public void IsAttachable_SetRightS88Level_ReturnsValueDependsOnS88level(
            int s88Level, bool expectedValue)
        {
            var obj = GetEmpty();

            obj.S88Level = s88Level;

            Assert.AreEqual(expectedValue, obj.IsAttachable);
            Assert.AreEqual(s88Level, obj.S88Level);
        }

        [TestCase(1, true)]
        [TestCase(0, false)]
        [TestCase(4, true)]
        public void UseGroups_BaseObject_ReturnsValueDependsOnObjectGroups(
            int countOfGroups, bool expectedValue)
        {
            var obj = GetEmpty();

            for(int i = 0; i < countOfGroups; i++)
            {
                obj.AddObjectGroup(paramLuaName, paramName, string.Empty);
            }
            
            Assert.AreEqual(expectedValue, obj.UseGroups);
        }

        //Save methods?

        [TestCase(1)]
        [TestCase(100)]
        public void AddSystemParameter_EmptyBaseTechObject_AddParametersToList(
            int paramsCount)
        {
            BaseTechObject emptyObject = GetEmpty();
            for (int i = 0; i < paramsCount; i++)
            {
                emptyObject.AddSystemParameter(paramLuaName, paramName,
                    paramValue, paramMeter);
            }

            var firstParam = emptyObject.SystemParams.GetParam(paramLuaName);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(paramsCount, emptyObject.SystemParams.Count);
                Assert.AreEqual(paramName, firstParam.Name);
                Assert.AreEqual(paramLuaName, firstParam.LuaName);
                Assert.AreEqual(paramMeter, firstParam.Meter);
                Assert.AreEqual(paramValue.ToString(), firstParam.Value.Value);
            });
        }

        [TestCase(1)]
        [TestCase(100)]
        public void AddParameter_EmptyBaseTechObject_AddParametersToList(
            int paramsCount)
        {
            BaseTechObject emptyObj = GetEmpty();
            for (int i = 0; i < paramsCount; i++)
            {
                emptyObj.AddParameter(paramLuaName, paramName,
                    paramValue, paramMeter);
            }

            var firstParam = emptyObj.Parameters.GetParam(paramLuaName);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(paramsCount, emptyObj.Parameters.Items.Length);
                Assert.AreEqual(paramName, firstParam.GetName());
                Assert.AreEqual(paramLuaName, firstParam.GetNameLua());
                Assert.AreEqual(paramMeter, firstParam.GetMeter());
                Assert.AreEqual(paramValue.ToString(), firstParam.GetValue());
            });
        }

        private BaseTechObject GetEmpty()
        {
            return new BaseTechObject();
        }

        string paramLuaName = "LuaName";
        string paramName = "Name";
        double paramValue = 0.5;
        string paramMeter = "шт";
    }
}
