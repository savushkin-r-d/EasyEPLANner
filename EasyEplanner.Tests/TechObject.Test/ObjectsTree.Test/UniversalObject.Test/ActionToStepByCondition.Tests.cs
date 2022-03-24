using NUnit.Framework;
using System.Collections.Generic;
using TechObject;
using System.Linq;

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
            List<EplanDevice.DeviceType[]> expectedTypes,
            List<EplanDevice.DeviceSubType[]> expectedSubTypes)
        {
            var action = new ActionToStepByCondition(string.Empty, null,
                string.Empty);

            List<IAction> actions = action.SubActions;

            for (int i = 0; i < actions.Count; i++)
            {
                EplanDevice.DeviceType[] actualTypes;
                EplanDevice.DeviceSubType[] actualSubTypes;

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
            var allowedOnoffDevTypes = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.V,
                EplanDevice.DeviceType.GS,
                EplanDevice.DeviceType.DI,
                EplanDevice.DeviceType.DO
            };

            var onDevsTypes = allowedOnoffDevTypes;
            var offDevsTypes = allowedOnoffDevTypes;

            return new object[]
            {
                new object[]
                {
                    new List<EplanDevice.DeviceType[]>()
                    {
                        onDevsTypes,
                        offDevsTypes
                    },
                    new List<EplanDevice.DeviceSubType[]>()
                    {
                        null,
                        null
                    },
                },
            };
        }

        [Test]
        public void Clone_NewAction_ReturnsClonedAction()
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
                Assert.AreNotEqual(cloned.GetHashCode(), action.GetHashCode());
            });
        }

        [TestCase(-1, "next_step_n", 0)]
        [TestCase(0, "next_step_n", 0)]
        [TestCase(1, "next_step_n", 0)]
        public void AddParam_NewAction_ChekParameterValue(object val,
            string paramName, int groupNumber)
        {
            var action = new ActionToStepByCondition(string.Empty, null,
                string.Empty);

            action.AddParam(val, paramName, groupNumber);
            var parameter = action.Parameters.Where(x => x.LuaName == paramName)
                .FirstOrDefault();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(parameter);
                if (parameter != null)
                {
                    Assert.AreEqual(val.ToString(), parameter.Value);
                }
            });
        }

        [TestCase(1, "wrongname_1", 0)]
        [TestCase(5, "wrongname_2", 0)]
        public void AddParam_NewAction_CheckAddingWrongParameter(object val,
            string paramName, int groupNumber)
        {
            var action = new ActionToStepByCondition(string.Empty, null,
                string.Empty);

            action.AddParam(val, paramName, groupNumber);
            var parameter = action.Parameters.Where(x => x.LuaName == paramName)
                .FirstOrDefault();

            Assert.IsNull(parameter);
        }
    }
}
