using NUnit.Framework;
using TechObject;

namespace EasyEplanner.Tests
{
    class StepTest
    {
        [TestCase(true, new int[] { 0, -1 })]
        [TestCase(false, new int[] { 0, 1 })]
        public void EditablePart_StepFromCaseSource_ReturnsArray(
            bool isMainStep, int[] expectedArray)
        {
            var step = new Step(string.Empty, null, null, isMainStep);

            Assert.AreEqual(expectedArray, step.EditablePart);
        }
    }
}
