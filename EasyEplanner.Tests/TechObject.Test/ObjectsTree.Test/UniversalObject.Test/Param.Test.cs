using NUnit.Framework;
using TechObject;

namespace Tests.TechObject
{
    class ParamTest
    {
        // LuaNameProperty and Delete(object child) have not to test because,
        // these methods delegate his work to dependencies and we have not to
        // test dependencies.

        // GetParameterNumber works with delegate which we have to mock and
        // return only delegate value, we can skip testing this property.

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void IsUseOperation_DefaultParameter_ReturnsTrueOrFalse(
            bool isUseOperation, bool expectedIsUseOperation)
        {
            bool runtime = false;

            var param = GetEmptyWithGetNReturns0(runtime, isUseOperation);

            Assert.AreEqual(expectedIsUseOperation, param.IsUseOperation());
        }

        // -1 because we have no filled objects and we can't make full test.
        // There is a problem in dependency ParamOperationsProperty
        [TestCase(true, "-1", "")]
        [TestCase(true, "-1", "3 4")]
        [TestCase(true, "-1", null)]
        [TestCase(false, "", "")]
        [TestCase(false, "", "4")]
        [TestCase(false, "", null)]
        [TestCase(true, "-1", "-1")]
        [TestCase(false, "", "-1")]
        public void OperationsGetSet_DefaultParameter(bool isUseOperation,
            string expectedValue, string setValue)
        {
            var runtime = false;
            var param = GetEmptyWithGetNReturns0(runtime, isUseOperation);

            param.Operations = setValue;

            Assert.AreEqual(expectedValue, param.Operations);
        }

        [TestCase("", "")]
        [TestCase("шт", "шт")]
        [TestCase("", null)]
        [TestCase("1", "1")]
        public void GetMeter_NewParam_ReturnsMeter(string expected, string actual)
        {
            var runtime = false;
            double value = 0;
            var param = new Param(stubGetN, string.Empty, runtime, value,
                actual);

            string meter = param.GetMeter();

            Assert.AreEqual(expected, meter);
        }

        [TestCase(true, "0", 0)]
        [TestCase(true, "0", 20)]
        [TestCase(false, "20", 20)]
        [TestCase(false, "0", 0)]
        [TestCase(true, "0", null)]
        [TestCase(false, "0", null)]
        public void GetValue_NewParamRuntimeOrNo_ReturnsValueOrZero(
            bool isRuntime, string expected, double actual)
        {
            var param = new Param(stubGetN, string.Empty, isRuntime, actual);

            string value = param.GetValue();

            Assert.AreEqual(expected, value);
        }

        [TestCase("P", "")]
        [TestCase("P", "P")]
        [TestCase("ONE TWO", "ONE TWO")]
        [TestCase("P", null)]
        public void GetNameLua_NewParam_ReturnsNameLuaOrP(string expected,
            string actual)
        {
            var runtime = false;
            double value = 0;
            var param = new Param(stubGetN, string.Empty, runtime, value,
                string.Empty, actual);

            string luaName = param.GetNameLua();

            Assert.AreEqual(expected, luaName);
        }

        [TestCase("", "")]
        [TestCase("Name", "Name")]
        [TestCase("ONE TWO", "ONE TWO")]
        [TestCase(null, null)]
        public void GetName_NewParam_ReturnsNameOrNull(string expected,
            string actual)
        {
            var param = new Param(stubGetN, actual);

            string name = param.GetName();

            Assert.AreEqual(expected, name);
        }

        [Test]
        public void ConstProperties_NewParam_ReturnsConstValues()
        {
            var param = GetEmpty();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(param.IsDeletable);
                Assert.IsTrue(param.IsReplaceable);
                Assert.IsTrue(param.IsMoveable);
                Assert.IsTrue(param.IsCopyable);
                Assert.AreEqual(new int[] { 0, -1 }, param.EditablePart);
                Assert.IsTrue(param.IsEditable);
            });
        }

        [TestCase("", "")]
        [TestCase("1", "1")]
        [TestCase("ONE", "ONE")]
        [TestCase(null, null)]
        public void EditText_NewParam_ReturnsNameInArrEditText(string expected,
            string actual)
        {
            var param = GetEmpty(actual);

            string[] expectedEditText = new string[] { expected, string.Empty };
            Assert.AreEqual(expectedEditText, param.EditText);
        }

        [TestCase("", "")]
        [TestCase("1", "1")]
        [TestCase("ONE", "ONE")]
        [TestCase(null, null)]
        public void SetNewValue_NewParam_ChangeName(string expected,
            string actual)
        {
            var param = GetEmpty();

            param.SetNewValue(actual);
            string name = param.GetName();

            Assert.AreEqual(expected, name);
        }

        [Test]
        public void Items_NoRuntimeNoUseOperation_ItemsCountAndItemsSequence()
        {
            var param = GetEmptyWithGetNReturns0(false, false);
            int expectedItemsCount = 3;

            var items = param.Items;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedItemsCount, param.Items.Length);
                Assert.IsTrue(items[0] is ParamProperty v && 
                    v.Name == "Значение");
                Assert.IsTrue(items[1] is ParamProperty m &&
                    m.Name == "Размерность");
                Assert.IsTrue(items[2] is ParamProperty n &&
                    n.Name == "Lua имя");
            });
        }

        [Test]
        public void Items_RuntimeUseOperation_ItemsCountAndItemsSequence()
        {
            var param = GetEmptyWithGetNReturns0(true, true);
            int expectedItemsCount = 3;

            var items = param.Items;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedItemsCount, param.Items.Length);
                Assert.IsTrue(items[0] is ParamProperty m &&
                    m.Name == "Размерность");
                Assert.IsTrue(items[1] is ParamOperationsProperty o &&
                    o.Name == "Операция");
                Assert.IsTrue(items[2] is ParamProperty n &&
                    n.Name == "Lua имя");
            });
        }

        [Test]
        public void Items_NoRuntimeUseOperation_ItemsCountAndItemsSequence()
        {
            var param = GetEmptyWithGetNReturns0(false, true);
            int expectedItemsCount = 4;

            var items = param.Items;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedItemsCount, param.Items.Length);
                Assert.IsTrue(items[0] is ParamProperty v &&
                    v.Name == "Значение");
                Assert.IsTrue(items[1] is ParamProperty m &&
                    m.Name == "Размерность");
                Assert.IsTrue(items[2] is ParamOperationsProperty o &&
                    o.Name == "Операция");
                Assert.IsTrue(items[3] is ParamProperty n &&
                    n.Name == "Lua имя");
            });
        }

        [Test]
        public void Items_RuntimeNoUseOperation_ItemsCountAndItemsSequence()
        {
            var param = GetEmptyWithGetNReturns0(true, false);
            int expectedItemsCount = 2;

            var items = param.Items;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedItemsCount, param.Items.Length);
                Assert.IsTrue(items[0] is ParamProperty m &&
                    m.Name == "Размерность");
                Assert.IsTrue(items[1] is ParamProperty n &&
                    n.Name == "Lua имя");
            });
        }


        private Param GetEmptyWithGetNReturns0(bool isRuntime,
            bool isUseOperation)
        {
            return new Param(stubGetN, string.Empty, isRuntime, 0, string.Empty,
                string.Empty, isUseOperation);
        }

        private Param GetEmpty(string name = "")
        {
            return new Param(stubGetN, name);
        }

        GetN stubGetN = delegate (object obj)
        {
            return 0;
        };
    }
}
