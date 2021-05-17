using NUnit.Framework;
using TechObject;

namespace Tests.TechObject
{
    class ParamOperationsPropertyTest
    {
        //[TestCase("", true)] Can't be tested, no dependency injection
        //[TestCase("1 2 3", true)] // The same case
        [TestCase("аыф", false)]
        [TestCase("2 3 -4", false)]
        [TestCase("abc 2 3 -4", false)]
        [TestCase("-1", false)]
        public void SetNewValue_Default_TrueOrFalse(string newValue,
            bool expectedIsSetValue)
        {
            var property = new ParamOperationsProperty("Name", -1, -1);

            bool actualIsSetValue = property.SetNewValue(newValue);

            Assert.AreEqual(expectedIsSetValue, actualIsSetValue);
        }
    }
}
