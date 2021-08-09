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

        [TestCase("", "")]
        [TestCase(null, "")]
        [TestCase("Name", "Name")]
        [TestCase("Имя", "Имя")]
        public void GetStepName_NewStep_ReturnExpectedName(string defaultName,
            string expectedName)
        {
            var step = new Step(defaultName, null, null);

            var actualName = step.GetStepName();

            Assert.AreEqual(actualName, expectedName);
        }

        [TestCase(true, 10)]
        [TestCase(false, 11)]
        public void Constructor_NewStep_CheckActionsCount(bool isMainStep,
            int expectedCount)
        {
            var step = new Step(string.Empty, null, null, isMainStep);

            var actualCount = step.GetActions.Count;

            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestCase(true, 10)]
        [TestCase(false, 13)]
        public void Constructor_NewStep_CheckItemsCount(bool isMainStep,
            int expectedCount)
        {
            var step = new Step(string.Empty, null, null, isMainStep);

            var actualCount = step.Items.Length;

            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestCaseSource(nameof(CheckActionsSequenceByLuaNameTestSource))]
        public void Constructor_NewStep_CheckActionsSequenceByLuaname(
            bool isMainStep, string[] expectedLuaNames)
        {
            var step = new Step(string.Empty, null, null, isMainStep);

            var actions = step.GetActions;

            for(int i = 0; i < actions.Count; i++)
            {
                Assert.AreEqual(expectedLuaNames[i], actions[i].LuaName);
            }
        }

        private static object[] CheckActionsSequenceByLuaNameTestSource()
        {
            var checkedDevices = "checked_devices";
            var toStepIfDevicesInSpecificState = 
                "to_step_if_devices_in_specific_state";

            object[] sequenceIfNoMainStep = new object[]
            {
                false,
                new string[]
                {
                    checkedDevices,
                    Action.OpenDevices,
                    Action.OpenReverseDevices,
                    Action.CloseDevices,
                    ActionGroup.OpenedUpperSeats,
                    ActionGroup.OpenedLowerSeats,
                    Action.RequiredFB,
                    ActionGroupWash.SingleGroupAction,
                    ActionGroup.DIDO,
                    ActionGroup.AIAO,
                    toStepIfDevicesInSpecificState
                }
            };

            object[] sequenceIfMainStep = new object[]
            {
                true,
                new string[]
                {
                    checkedDevices,
                    Action.OpenDevices,
                    Action.OpenReverseDevices,
                    Action.CloseDevices,
                    ActionGroup.OpenedUpperSeats,
                    ActionGroup.OpenedLowerSeats,
                    Action.RequiredFB,
                    ActionGroupWash.SingleGroupAction,
                    ActionGroup.DIDO,
                    ActionGroup.AIAO
                }
            };

            return new object[]
            {
                sequenceIfMainStep,
                sequenceIfNoMainStep
            };
        }
    }
}
