using Editor;
using NUnit.Framework;
using TechObject;

namespace Tests.TechObject
{
    class SystemParamsTest
    {
        [Test]
        public void Clone_ObjectsWith3Params_ReturnsFullObjectCopy()
        {
            SystemParams systemParams = GetDefaultWith3Params();

            SystemParams cloned = systemParams.Clone();

            Assert.AreEqual(cloned.Count, systemParams.Count);
            Assert.AreEqual(cloned.DisplayText[0], systemParams.DisplayText[0]);
            Assert.AreEqual(cloned.DisplayText[1], systemParams.DisplayText[1]);
            Assert.AreEqual(cloned.ImageIndex, systemParams.ImageIndex);
            Assert.AreEqual(cloned.IsFilled, systemParams.IsFilled);
            Assert.AreEqual(cloned.SaveAsLuaTable(string.Empty),
                systemParams.SaveAsLuaTable(string.Empty));

            var systemParamArr = (SystemParam[])systemParams.Items;
            var clonedSystemParamArr = (SystemParam[])cloned.Items;
            for(int i = 0; i < systemParamArr.Length; i++)
            {
                Assert.AreEqual(systemParamArr[i].Name,
                    clonedSystemParamArr[i].Name);
                Assert.AreEqual(systemParamArr[i].LuaName,
                    clonedSystemParamArr[i].LuaName);
                Assert.AreEqual(systemParamArr[i].Meter,
                    clonedSystemParamArr[i].Meter);
            }
        }

        [TestCase("")]
        [TestCase("\t")]
        public void SaveAsLuaTable_ObjectWith3Params_ReturnsCodeLuaForSave(
            string prefix)
        {
            SystemParams systemParams = GetDefaultWith3Params();
            string expectedCode = string.Empty;
            expectedCode += prefix + "system_parameters" + " =\n";
            expectedCode += prefix + "\t{\n";
            var systemParamArr = (SystemParam[])systemParams.Items;
            foreach(var systemParam in systemParamArr)
            {
                expectedCode += systemParam.SaveAsLuaTable(prefix + "\t");
            }
            expectedCode += prefix + "\t},\n";

            string savedCode = systemParams.SaveAsLuaTable(prefix);

            Assert.AreEqual(expectedCode, savedCode);
        }

        [TestCase("")]
        [TestCase("\t")]
        public void SaveAsLuaTable_EmptyObject_ReturnsEmptyString(
            string prefix)
        {
            var systemParam = new SystemParams();

            string savedCode = systemParam.SaveAsLuaTable(prefix);

            Assert.IsEmpty(savedCode);
        }

        [Test]
        public void GetParam_ObjectWithParam_GetByLuaNameOrNameReturnsParam()
        {
            var systemParams = new SystemParams();
            const string paramName = "Параметр 1";
            const string paramLuaName = "LuaName1";
            var systemParam = new SystemParam(systemParams.GetIdx,
                paramName, 0, "шт", paramLuaName);
            systemParams.AddParam(systemParam);

            SystemParam paramByName = systemParams.GetParam(paramName);
            SystemParam paramByLuaName = systemParams.GetParam(paramLuaName);

            Assert.AreEqual(systemParam, paramByName);
            Assert.AreEqual(systemParam, paramByLuaName);
        }

        [Test]
        public void GetParam_EmptyObject_GetByNameOrLuaNameReturnsNull()
        {
            var systemParams = new SystemParams();

            SystemParam paramByName = systemParams.GetParam("LuaName1");
            SystemParam paramByLuaName = systemParams.GetParam("Параметр 1");

            Assert.IsNull(paramByName);
            Assert.IsNull(paramByLuaName);
        }

        [Test]
        public void DisplayText_ObjectWith3Params_ReturnsTextWithObjectsCount()
        {
            SystemParams systemParams = GetDefaultWith3Params();
            var expectedDisplayText = new string[]
            {
                $"Системные параметры ({systemParams.Count})",
                string.Empty
            };

            Assert.AreEqual(expectedDisplayText[0],
                systemParams.DisplayText[0]);
            Assert.AreEqual(expectedDisplayText[1],
                systemParams.DisplayText[1]);
        }

        [Test]
        public void IsInsertableCopy_AnyObject_ReturnsTrue()
        {
            var systemParams = new SystemParams();
            Assert.IsTrue(systemParams.IsInsertableCopy);
        }

        [Test]
        public void InsertCopy_EmptyObject_AddNewSystemParam()
        {
            var systemParams = new SystemParams();
            const string paramName = "Параметр 1";
            var systemParam = new SystemParam(systemParams.GetIdx, paramName);
            const int expectedParametersCount = 1;

            var insertedItem = systemParams.InsertCopy(systemParam);

            Assert.IsTrue(insertedItem.DisplayText[0].Contains(paramName));
            Assert.AreEqual(expectedParametersCount, systemParams.Count);
        }

        [Test]
        public void InsertCopy_EmptyObject_AddNotSystemParam()
        {
            var systemParams = new SystemParams();
            int expectedCountOfParameters = 0;

            var insertedItem = systemParams.InsertCopy(new SystemParams());

            Assert.IsNull(insertedItem);
            Assert.AreEqual(expectedCountOfParameters, systemParams.Count);
        }

        [Test]
        public void IsFilled_EmptyObject_ReturnsFalse()
        {
            var systemParamsNoParams = new SystemParams();
            Assert.IsFalse(systemParamsNoParams.IsFilled);
        }

        [Test]
        public void IsFilled_ObjectWith3Params_ReturnsTrue()
        {
            var systemParamsWith3Params = GetDefaultWith3Params();
            Assert.IsTrue(systemParamsWith3Params.IsFilled);
        }

        [Test]
        public void ImageIndex_EmptyObject_ParamsManagerIndex()
        {
            var systemParams = new SystemParams();

            Assert.AreEqual(systemParams.ImageIndex,
                ImageIndexEnum.ParamsManager);
        }

        [Test]
        public void AddParam_EmptyObject_Add3Params()
        {
            var emptyDefaultParams = new SystemParams();
            SystemParams defaultWith3Params = GetDefaultWith3Params();

            foreach(SystemParam sysPar in defaultWith3Params.Items)
            {
                emptyDefaultParams.AddParam(sysPar);
            }

            Assert.AreEqual(defaultWith3Params.Count, emptyDefaultParams.Count);
        }

        [Test]
        public void SetUpFromBaseTechObject_EmptyObject_Add3Params()
        {
            var emptyDefaultParams = new SystemParams();
            SystemParams defaultWith3Params = GetDefaultWith3Params();

            emptyDefaultParams.SetUpFromBaseTechObject(defaultWith3Params);

            Assert.AreEqual(defaultWith3Params.Count, emptyDefaultParams.Count);
        }

        [Test]
        public void Count_DefaultObjectAddParams_ReturnsCountOfParams()
        {
            var systemParams0Params = new SystemParams();
            SystemParams systemParamsWith3Params = GetDefaultWith3Params();

            Assert.AreEqual(0, systemParams0Params.Count);
            Assert.AreEqual(3, systemParamsWith3Params.Count);
        }

        [Test]
        public void Clear_ThreeAnyParameters_DeleteAllParams()
        {
            var parameters = GetDefaultWith3Params();
            int beforeDeleteCount = parameters.Count;
            parameters.Clear();
            int afterDeleteCount = parameters.Count;
            int expectedCountAfterDelete = 0;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedCountAfterDelete, afterDeleteCount);
                Assert.AreNotEqual(afterDeleteCount, beforeDeleteCount);
            });
        }

        private SystemParams GetDefaultWith3Params()
        {
            var systemParams = new SystemParams();

            var firstParam = new SystemParam(systemParams.GetIdx,
                "Параметр 1", 1, "шт", "LuaName1");
            systemParams.AddParam(firstParam);

            var secondParam = new SystemParam(systemParams.GetIdx,
                "Параметр 2", 2, "шт", "LuaName2");
            systemParams.AddParam(firstParam);

            var thirdParam = new SystemParam(systemParams.GetIdx,
                "Параметр 3", 3, "шт", "LuaName3");
            systemParams.AddParam(thirdParam);

            return systemParams;
        }
    }
}
