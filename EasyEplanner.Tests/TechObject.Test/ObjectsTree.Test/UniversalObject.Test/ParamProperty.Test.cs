using NUnit.Framework;
using TechObject;

namespace Tests.TechObject
{
    class ParamPropertyTest
    {
        ParamProperty property;
        
        [TestCase(0, 1, 1)]
        [TestCase(100, 200, 200)]
        [TestCase(-50, -700, -700)]
        [TestCase("0", "1", "1")]
        [TestCase("100", "200", "200")]
        [TestCase("ABCDE", "EDCBSSWE", "EDCBSSWE")]
        [TestCase("", "", "")]
        [TestCase("Один два три", "Два один четыре", "Два один четыре")]
        [TestCase("4.0", "4.5", "4.5")]
        [TestCase(4.0, 5.3, 5.3)]
        [TestCase(0, "строка", 0)]
        [TestCase("строка", 1, 1)]
        [TestCase(5.0, "строка", 5.0)]
        [TestCase("строка", 5.0, 5.0)]
        public void SetNewValue_NewObject_NewValueOrTheSame(object initValue,
            object setValue, object expectedValue)
        {
            string name = "Name";
            object defaultValue = null;
            bool editable = false; // Still allow value editing
            property = new ParamProperty(name, initValue, defaultValue,
                editable);
            property.SetNewValue(setValue.ToString());

            Assert.AreEqual(expectedValue.ToString(), property.Value);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void IsEditable_NewObject_TheSameValue(bool actualValue,
            bool expectedValue)
        {
            object value = 0;
            string name = "Name";
            object defaultValue = null;

            property =
                new ParamProperty(name, value, defaultValue, actualValue);

            Assert.AreEqual(expectedValue, property.IsEditable);
        }

        [TestCase(true, new int[] { -1, 1 })]
        [TestCase(false, new int[] { -1, -1 })]
        public void EditablePart_NewObject_TheSameValue(bool editable,
            int[] expectedValue)
        {
            object value = 0;
            string name = "Name";
            object defaultValue = null;

            property = new ParamProperty(name, value, defaultValue, editable);

            Assert.AreEqual(expectedValue, property.EditablePart);
        }

        [TestCase("-", "-", "-", true)]
        [TestCase(0.1, "-", "-", true)]
        [TestCase("-", 0.1, 0.1, true)]
        [TestCase("-", "abc", "-", false)]
        public void SetNewValue_GenericValueProperty(object initValue,
            object setValue, object expectedValue, bool expectedResult)
        {
            string name = Param.ValuePropertyName;
            object defaultValue = null;
            bool editable = false; // Still allow value editing
            
            property = new ParamProperty(name, initValue, defaultValue,
                editable);

            property.Parent = new Param(GetN => 1, "param")
            {
                Parent = new Params("params", "params", false, "params")
                {
                    Parent = new ParamsManager()
                    {
                        Parent = new GenericTechObject("", 0, "", -1, "", "", new BaseTechObject())
                    }
                }
            };       
            
            var result = property.SetNewValue(setValue.ToString());

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedValue.ToString(), property.Value);
                Assert.AreEqual(expectedResult, result);
            });
        }
    }
}
