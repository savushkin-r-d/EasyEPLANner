using NUnit.Framework;
using TechObject;

namespace Tests.TechObject
{
    class SystemParamTest
    {
        GetN GetNReturn1;

        [SetUp]
        public void SetUpTheTest()
        {
            GetNReturn1 += delegate (object obj) { return 1; };
        }

        [TestCase("")]
        [TestCase("\t")]
        public void SaveAsLuatable_DefaultParam_ReturnsStringAsLuaTable(
            string prefix)
        {
            SystemParam defaultParam = GetDefault();
            string expectedLuaCode = string.Empty;
            expectedLuaCode += prefix + $"{defaultParam.LuaName} =\n";
            expectedLuaCode += prefix + "\t{\n";
            expectedLuaCode += prefix + "\tvalue = " +
                defaultParam.Value.Value + ",\n";
            expectedLuaCode += prefix + "\t},\n";

            Assert.AreEqual(expectedLuaCode,
                defaultParam.SaveAsLuaTable(prefix));
        }

        [Test]
        public void DisplayText_DefaultParamGetNReturns1_ReturnsDisplayText()
        {
            SystemParam defaultParam = GetDefault();
            string expectedDisplayText =
                $"1. {defaultParam.Name} - {defaultParam.Value.Value} " +
                    $"{defaultParam.Meter}.";

            Assert.AreEqual(expectedDisplayText, defaultParam.DisplayText[0]);
            Assert.AreEqual(string.Empty, defaultParam.DisplayText[1]);
        }

        [Test]
        public void Items_DefaultParam_ReturnsItemsList()
        {
            SystemParam defaultParam = GetDefault();

            int countOfItems = 3;
            string firstItemName = "Значение";
            string secondItemName = "Размерность";
            string thirdItemName = "Lua имя";

            var items = defaultParam.Items;

            Assert.AreEqual(countOfItems, defaultParam.Items.Length);
            Assert.AreEqual(firstItemName, items[0].DisplayText[0]);
            Assert.AreEqual(secondItemName, items[1].DisplayText[0]);
            Assert.AreEqual(thirdItemName, items[2].DisplayText[0]);
        }

        [TestCase("", "")]
        [TestCase("Meter", "Meter")]
        [TestCase(null, "")]
        public void Meter_NewParam_ReturnsMeterValue(string actualMeter,
            string expectedMeter)
        {
            var systemParam = new SystemParam(GetNReturn1, string.Empty, 0,
                actualMeter);

            Assert.AreEqual(expectedMeter, systemParam.Meter);
        }

        [Test]
        public void Meter_DefaultParam_Returns_CountOfPiecesValue()
        {
            Assert.AreEqual("шт", GetDefault().Meter);
        }

        [TestCase("", "")]
        [TestCase("Name", "Name")]
        [TestCase(null, null)]
        public void Name_NewParam_ReturnsNameValue(string actualName,
            string expectedName)
        {
            var systemParam = new SystemParam(GetNReturn1, actualName);

            Assert.AreEqual(expectedName, systemParam.Name);
        }

        [TestCase("", "")]
        [TestCase("LuaName", "LuaName")]
        [TestCase(null, "")]
        public void LuaName_NewParam_ReturnsLuaNameValue(
            string actualLuaName, string expectedLuaName)
        {
            var systemParam = new SystemParam(GetNReturn1, string.Empty,
                0, string.Empty, actualLuaName);

            Assert.AreEqual(expectedLuaName, systemParam.LuaName);
        }

        [Test]
        public void LuaName_DefaultParam_ReturnsEmptyValue()
        {
            Assert.IsEmpty(GetDefault().LuaName);
        }

        private SystemParam GetDefault()
        {
            return new SystemParam(
                delegate (object obj) { return 1; }, "Default param");
        }
    }
}
