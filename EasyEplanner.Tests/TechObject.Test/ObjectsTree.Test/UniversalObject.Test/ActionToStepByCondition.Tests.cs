using NUnit.Framework;
using System.Collections.Generic;
using TechObject;

namespace Tests.TechObject
{
    class ActionToStepByConditionTests
    {
        [Test]
        public void Constructor_NewAction_ReturnsItemCountEqualTwo()
        {
            const int expectedCount = 2;
            var action = new ActionToStepByCondition(string.Empty,
                null, string.Empty);

            int actualCount = action.Items.Length;

            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestCase("LuaName1")]
        [TestCase("OneTwoThree")]
        public void LuaName_NewAction_ReturnsRightLuaName(string expectedValue)
        {
            var action = new ActionToStepByCondition(string.Empty, null,
                expectedValue);

            string actualValue = action.LuaName;

            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestCase("Name1")]
        [TestCase("ОдинДваТри")]
        public void Name_NewAction_ReturnRightName(string expectedValue)
        {
            var action = new ActionToStepByCondition(expectedValue, null,
                   string.Empty);

            string actualValue = action.Name;

            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestCaseSource(nameof(CheckActionsSequenceByLuaName))]
        public void Constructor_NewAction_CheckActionsSequenceByLuaName(
            string[] expectedSequence)
        {
            var action = new ActionToStepByCondition(string.Empty, null,
                string.Empty);

            List<IAction> actions = action.SubActions;

            for(int i = 0; i < actions.Count; i++)
            {
                Assert.AreEqual(expectedSequence[i], actions[i].LuaName);
            }
        }

        private static object[] CheckActionsSequenceByLuaName()
        {
            string firstAction = "on_devices";
            string secondAction = "off_devices";

            return new object[]
            {
                new string[]
                {
                    firstAction,
                    secondAction
                },
            };
        }

        [TestCaseSource(nameof(CheckAllowedDevicesInEveryAction))]
        public void Constructor_NewAction_CheckAlloweDevicesInEveryAction(
            List<Device.DeviceType[]> expectedTypes,
            List<Device.DeviceSubType[]> expectedSubTypes)
        {
            var action = new ActionToStepByCondition(string.Empty, null,
                string.Empty);

            List<IAction> actions = action.SubActions;

            for (int i = 0; i < actions.Count; i++)
            {
                Device.DeviceType[] actualTypes;
                Device.DeviceSubType[] actualSubTypes;

                actions[i].GetDisplayObjects(out actualTypes,
                    out actualSubTypes, out _);

                Assert.Multiple(() =>
                {
                    Assert.AreEqual(expectedTypes[i], actualTypes);
                    Assert.AreEqual(expectedSubTypes[i], actualSubTypes);
                });
            }
        }

        private static object[] CheckAllowedDevicesInEveryAction()
        {
            var allowedOnoffDevTypes = new Device.DeviceType[]
            {
                Device.DeviceType.V,
                Device.DeviceType.GS,
                Device.DeviceType.DI
            };

            var onDevsTypes = allowedOnoffDevTypes;
            var offDevsTypes = allowedOnoffDevTypes;

            return new object[]
            {
                new object[]
                {
                    new List<Device.DeviceType[]>()
                    {
                        onDevsTypes,
                        offDevsTypes
                    },
                    new List<Device.DeviceSubType[]>()
                    {
                        null,
                        null
                    },
                },
            };
        }

        [Test]
        public void Clone_NewAction_ReturnsFullClonedAction()
        {
            string name = "Действие 1";
            string luaName = "ActionLuaName";

            var action = new ActionToStepByCondition(name, null, luaName);
            int actionsCount = action.SubActions.Count;

            IAction cloned = action.Clone();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(name, cloned.Name);
                Assert.AreEqual(luaName, cloned.LuaName);
                Assert.AreEqual(actionsCount, action.SubActions.Count);
            });

        }
    }
}
