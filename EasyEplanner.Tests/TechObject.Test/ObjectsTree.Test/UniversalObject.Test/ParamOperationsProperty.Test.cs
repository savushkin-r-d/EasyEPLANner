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
        public void SetNewValue_Default_TrueOrFalse(string newValue,
            bool expectedIsSetValue)
        {
            ParamOperationsProperty property = GetDefault();

            bool actualIsSetValue = property.SetNewValue(newValue);

            Assert.AreEqual(expectedIsSetValue, actualIsSetValue);
        }

        private ParamOperationsProperty GetDefault()
        {
            return new ParamOperationsProperty("Name", -1, -1);
        }
    }
}
