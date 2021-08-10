using NUnit.Framework;
using TechObject;
using Editor;
using System.Collections.Generic;

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

        [TestCaseSource(nameof(CheckActionsDrawStyleTestSource))]
        public void Constructor_NewStep_CheckActionsDrawStyle(bool isMainStep,
            DrawInfo.Style[] drawStyles)
        {
            var step = new Step(string.Empty, null, null, isMainStep);

            var actions = step.GetActions;

            for(int i = 0; i < actions.Count; i++)
            {
                Assert.AreEqual(actions[i].DrawStyle, drawStyles[i]);
            }
        }

        private static object[] CheckActionsDrawStyleTestSource()
        {
            var greenBox = DrawInfo.Style.GREEN_BOX;
            var redBox = DrawInfo.Style.RED_BOX;
            var greenUpBox = DrawInfo.Style.GREEN_UPPER_BOX;
            var greenLowBox = DrawInfo.Style.GREEN_LOWER_BOX;

            var noMainStep = new object[]
            {
                false,
                new DrawInfo.Style[]
                {
                    greenBox,
                    greenBox,
                    greenBox,
                    redBox,
                    greenUpBox,
                    greenLowBox,
                    greenBox,
                    greenBox,
                    greenBox,
                    greenBox,
                    greenBox,
                }
            };

            var mainStep = new object[]
            {
                true,
                new DrawInfo.Style[]
                {
                    greenBox,
                    greenBox,
                    greenBox,
                    redBox,
                    greenUpBox,
                    greenLowBox,
                    greenBox,
                    greenBox,
                    greenBox,
                    greenBox,
                }
            };

            return new object[]
            {
                noMainStep,
                mainStep
            };
        }

        [TestCaseSource(nameof(CheckActionsAllowedDevTypes))]
        public void Constructor_NewStep_CheckActionsAllowedDevTypes(
            bool isMainStep, List<Device.DeviceType[]> devTypesList)
        {
            var step = new Step(string.Empty, null, null, isMainStep);

            var actions = step.GetActions;

            for(int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                action.GetDisplayObjects(out Device.DeviceType[] actualDevTypes,
                    out _, out _);
                Device.DeviceType[] expectedDevTypes = devTypesList[i];

                Assert.AreEqual(expectedDevTypes, actualDevTypes);
            }
        }

        private static object[] CheckActionsAllowedDevTypes()
        {
            // null - allowed all devices in action.
            // But action can contain other actions and it means, that main
            // action can't be changed itself, inner can. Null for main if u
            // have inner = it's ok.
            Device.DeviceType[] allTypesAllowed = null;

            var openDevice = new Device.DeviceType[]
            {
                Device.DeviceType.V,
                Device.DeviceType.DO,
                Device.DeviceType.M
            };

            var openReverse = new Device.DeviceType[]
            {
                Device.DeviceType.M
            };

            var closeDevice = new Device.DeviceType[]
            {
                Device.DeviceType.V,
                Device.DeviceType.DO,
                Device.DeviceType.M
            };

            var valveSeats = new Device.DeviceType[]
            {
                Device.DeviceType.V
            };
            Device.DeviceType[] openUpperSeats = valveSeats;
            Device.DeviceType[] openLowerSeats = valveSeats;

            var requiredFB = new Device.DeviceType[]
            {
                Device.DeviceType.DI,
                Device.DeviceType.GS
            };

            var groupDIDO = new Device.DeviceType[]
            {
                Device.DeviceType.DI,
                Device.DeviceType.SB,
                Device.DeviceType.DO,
                Device.DeviceType.HL,
                Device.DeviceType.GS
            };

            var groupAIAO = new Device.DeviceType[]
            {
                Device.DeviceType.AI,
                Device.DeviceType.AO,
                Device.DeviceType.M
            };

            var mainStep = new object[]
            {
                true,
                new List<Device.DeviceType[]>()
                {
                    allTypesAllowed,
                    openDevice,
                    openReverse,
                    closeDevice,
                    openUpperSeats,
                    openLowerSeats,
                    requiredFB,
                    allTypesAllowed,
                    groupDIDO,
                    groupAIAO
                }
            };

            var noMainStep = new object[]
            {
                true,
                new List<Device.DeviceType[]>()
                {
                    allTypesAllowed,
                    openDevice,
                    openReverse,
                    closeDevice,
                    openUpperSeats,
                    openLowerSeats,
                    requiredFB,
                    allTypesAllowed,
                    groupDIDO,
                    groupAIAO,
                    allTypesAllowed
                }
            };

            return new object[]
            {
                mainStep,
                noMainStep
            };
        }

        [TestCaseSource(nameof(CheckActionsAllowedDevSubTypesSource))]
        public void Constructor_NewStep_CheckActionsAllowedDevSubTypes(
            bool isMainStep, List<Device.DeviceSubType[]> devSubTypesList)
        {
            var step = new Step(string.Empty, null, null, isMainStep);

            var actions = step.GetActions;

            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                action.GetDisplayObjects(out _,
                    out Device.DeviceSubType[] actualDevSubTypes, out _);
                Device.DeviceSubType[] expectedDevTypes = devSubTypesList[i];

                Assert.AreEqual(expectedDevTypes, actualDevSubTypes);
            }
        }

        private static object[] CheckActionsAllowedDevSubTypesSource()
        {
            // null - it means, that we can use any subtype from defined types.
            Device.DeviceSubType[] allSubTypesAllowed = null;

            var openReverse = new Device.DeviceSubType[]
            {
                Device.DeviceSubType.M_REV_FREQ,
                Device.DeviceSubType.M_REV_FREQ_2,
                Device.DeviceSubType.M_REV_FREQ_2_ERROR,
                Device.DeviceSubType.M_ATV,
                Device.DeviceSubType.M_ATV_LINEAR,
                Device.DeviceSubType.M,
                Device.DeviceSubType.M_VIRT,
            };

            var valveSeats = new Device.DeviceSubType[]
            {
                Device.DeviceSubType.V_MIXPROOF,
                Device.DeviceSubType.V_AS_MIXPROOF,
                Device.DeviceSubType.V_IOLINK_MIXPROOF,
                Device.DeviceSubType.V_VIRT,
            };
            Device.DeviceSubType[] openUpperSeats = valveSeats;
            Device.DeviceSubType[] openLowerSeats = valveSeats;

            var mainStep = new object[]
            {
                true,
                new List<Device.DeviceSubType[]>()
                {
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    openReverse,
                    allSubTypesAllowed,
                    openUpperSeats,
                    openLowerSeats,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    allSubTypesAllowed
                }
            };

            var noMainStep = new object[]
            {
                true,
                new List<Device.DeviceSubType[]>()
                {
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    openReverse,
                    allSubTypesAllowed,
                    openUpperSeats,
                    openLowerSeats,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    allSubTypesAllowed
                }
            };

            return new object[]
            {
                mainStep,
                noMainStep
            };
        }
    }
}
