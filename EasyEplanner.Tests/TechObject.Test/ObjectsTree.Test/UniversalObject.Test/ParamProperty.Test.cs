using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TechObject;

namespace EasyEplanner.TechObject.Tests
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
        [TestCase(4.0, 5.0, 5.0)]
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

    }
}
