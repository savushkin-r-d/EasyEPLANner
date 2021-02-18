using TechObject;
using NUnit.Framework;

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
