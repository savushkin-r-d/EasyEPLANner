using System;
using System.Collections.Generic;
using System.Text;
using TechObject;
using NUnit.Framework;

namespace Tests.TechObject
{
    class BaseTechObjectTest
    {
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
