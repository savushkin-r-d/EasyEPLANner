using Editor;
using NUnit.Framework;
using System;
using TechObject;

namespace Tests.TechObject
{
    class BasePropertiesTest
    {
        [Test]
        public void Constructor_NewObject_EmptyProperties()
        {
            var obj = new BaseProperties();

            Assert.IsTrue(obj.Properties.Count == 0);
        }

        [Test]
        public void Count_EmptyObjectAdd2Properties_ReturnsNumber2()
        {
            var obj = new BaseProperties();
            obj.AddActiveParameter("active", "активный", string.Empty);
            obj.AddActiveBoolParameter("bool", "булевый", string.Empty);

            int expectedCount = 2;
            int actualCount = obj.Count;

            Assert.AreEqual(expectedCount, actualCount);
        }

        [Test]
        public void Count_ObjHas1PropertyAdd3Properties_ReturnsNumber4()
        {
            var obj = new BaseProperties();
            obj.AddActiveBoolParameter("beforeAdd", "До добавления", "false");
            int expectedBeforeAdd = 1;
            int actualBeforeAdd = obj.Count;

            Assert.AreEqual(expectedBeforeAdd, actualBeforeAdd);

            obj.AddActiveParameter("active1", "активный1", string.Empty);
            obj.AddActiveBoolParameter("bool", "булевый", string.Empty);
            obj.AddActiveParameter("active1", "активный2", string.Empty);

            int expectedCount = 4;
            int actualCount = obj.Count;

            Assert.AreEqual(expectedCount, actualCount);
        }

        [Test]
        public void Clone_ObjWith2Properties_ReturnsFullCopyOfObject()
        {
            var obj = new BaseProperties();
            string activeParName = "активный";
            string activeParLuaName = "active";
            string boolParName = "булевый";
            string boolParLuaName = "bool";

            obj.AddActiveParameter(activeParLuaName, activeParName,
                string.Empty);
            obj.AddActiveBoolParameter(boolParLuaName, boolParName, "true");

            var cloned = obj.Clone();

            var clonedActiveProperty = cloned.GetProperty(activeParLuaName);
            var clonedBoolProperty = cloned.GetProperty(boolParLuaName);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(obj.Count, cloned.Count);
                Assert.AreNotEqual(obj.GetHashCode(), cloned.GetHashCode());
                Assert.NotNull(clonedBoolProperty);
                Assert.NotNull(clonedActiveProperty);
            });

        }

        [TestCaseSource(nameof(DisplayTextTestCaseSource))]
        public void DisplayText_NewObject_ReturnsDisplayTextArr(
            int propertiesCount, string[] expectedDisplayText)
        {
            var obj = new BaseProperties();
            for(int i = 0; i < propertiesCount; i++)
            {
                obj.AddActiveParameter($"active{i}", $"активный{i}",
                    string.Empty);
            }

            string[] actualDisplayText = obj.DisplayText;

            Assert.AreEqual(expectedDisplayText, actualDisplayText);
        }

        private static object[] DisplayTextTestCaseSource()
        {
            var zeroProperties = new object[]
            { 
                0, 
                new string[] { "Доп. свойства (0)", string.Empty }
            };

            var threeProperties = new object[]
            {
                3,
                new string[] { "Доп. свойства (3)", string.Empty }
            };

            var tenProperties = new object[]
            {
                10,
                new string[] { "Доп. свойства (10)", string.Empty }
            };

            var sourceSet = new object[]
            { 
                zeroProperties,
                threeProperties,
                tenProperties
            };

            return sourceSet;
        }

        [TestCase(0, false, "")]
        [TestCase(2, false, "")]
        [TestCase(3, true, "val")]
        public void IsFilled_ObjectWithProperties_ReturnsTrueOrFalse(
            int propertiesCount, bool expectedValue, string valueForProperty)
        {
            var obj = new BaseProperties();
            for(int i = 0; i < propertiesCount; i++)
            {
                obj.AddActiveParameter($"act{i}", $"акт{i}", valueForProperty);
            }

            bool actualValue = obj.IsFilled;

            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestCase(0)]
        [TestCase(5)]
        public void Clear_ObjWithProperties_ClearAllProperties(
            int propertiesCount)
        {
            var obj = new BaseProperties();
            for (int i = 0; i < propertiesCount; i++)
            {
                obj.AddActiveParameter($"act{i}", $"акт{i}", string.Empty);
            }

            Assert.AreEqual(propertiesCount, obj.Count);

            obj.Clear();

            int expectedValue = 0;
            int actualValue = obj.Count;

            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Delete_ObjWithProperties_SetEmptyValueForProperty()
        {
            var obj = new BaseProperties();
            string valueBeforeDelete = "BeforeDel";
            ActiveParameter addedParameter = obj.AddActiveParameter("act1",
                "акт1", string.Empty);
            addedParameter.SetNewValue(valueBeforeDelete);

            Assert.AreEqual(valueBeforeDelete, addedParameter.Value);

            obj.Delete(addedParameter);

            Assert.AreEqual(string.Empty, addedParameter.Value);
        }

        [Test]
        public void SaveAsLuaTable_ObjWith2Properties_ReturnsExpectedString()
        {
            var obj = new BaseProperties();

            string activeParLuaName = "active";
            string activeParName = "активный";
            string activeParValue = "sss";
            ActiveParameter activePar = obj.AddActiveParameter(
                activeParLuaName, activeParName, string.Empty);
            activePar.SetNewValue(activeParValue);

            string boolParLuaName = "bool";
            string boolParName = "булевый";
            string boolParValue = "false";
            obj.AddActiveBoolParameter(boolParLuaName, boolParName,
                boolParValue);

            string prefix = string.Empty;
            string expectedSaveString = $"{prefix}properties =\n" +
                $"{prefix}\t{{\n" +
                $"{prefix}\tactive = \'sss\',\n" +
                $"{prefix}\tbool = \'false\',\n" +
                $"{prefix}\t}},\n";

            string actualSaveString = obj.SaveAsLuaTable(prefix);

            Assert.AreEqual(expectedSaveString, actualSaveString);
        }

        [Test]
        public void SaveAsLuaTable_TwoPropertiesOneEmpty_ReturnsExpectedString()
        {
            var obj = new BaseProperties();

            string activeParLuaName = "active";
            string activeParName = "активный";
            obj.AddActiveParameter(activeParLuaName, activeParName,
                string.Empty);

            string boolParLuaName = "bool";
            string boolParName = "булевый";
            string boolParValue = "false";
            obj.AddActiveBoolParameter(boolParLuaName, boolParName,
                boolParValue);

            string prefix = string.Empty;
            string expectedSaveString = $"{prefix}properties =\n" +
                $"{prefix}\t{{\n" +
                $"{prefix}\tbool = \'false\',\n" +
                $"{prefix}\t}},\n";

            string actualSaveString = obj.SaveAsLuaTable(prefix);

            Assert.AreEqual(expectedSaveString, actualSaveString);
        }

        [Test]
        public void SaveAsLuaTable_ObjNoProperties_ReturnsEmptyString()
        {
            var obj = new BaseProperties();

            string actualString = obj.SaveAsLuaTable(string.Empty);

            Assert.AreEqual(string.Empty, actualString);
        }
    }
}
