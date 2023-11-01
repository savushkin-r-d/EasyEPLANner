using Editor;
using Moq;
using NUnit.Framework;
using TechObject;

namespace TestsTechObject
{
    class ParamOperationsPropertyTest
    {
        //[TestCase("", true)] Can't be tested, no dependency injection
        //[TestCase("1 2 3", true)] // The same case
        [TestCase("аыф", "-1", false)]
        [TestCase("2 3 -4", "-1", false)]
        [TestCase("abc 2 3 -4", "-1", false)]
        [TestCase("-1", "-1", false)]
        [TestCase("2", "2", true)]
        [TestCase("2 3", "2 3", true)]
        public void SetNewValue_Default_TrueOrFalse(string newValue, string expectedValue,
            bool expectedIsSetValue)
        {
            var property = new ParamOperationsProperty("Name", -1, -1);

            var techObject = new TechObject.TechObject("", GetN => 1, 1, 2, "", -1, "", "", null);
            techObject.ModesManager.Insert();
            techObject.ModesManager.Insert();
            techObject.ModesManager.Insert();

            var parametersManager = new Mock<ITreeViewItem>();
            parametersManager.Setup(i => i.Parent).Returns(techObject);

            var parameters = new Mock<ITreeViewItem>();
            parameters.Setup(i => i.Parent).Returns(parametersManager.Object);

            var parameter = new Param(GetN => 1, "param");

            parameter.Parent = parameters.Object;
            property.Parent = parameter;

            bool actualIsSetValue = property.SetNewValue(newValue);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedIsSetValue, actualIsSetValue);
                Assert.AreEqual(expectedValue, property.Value);
            });
            
        }
    }
}
