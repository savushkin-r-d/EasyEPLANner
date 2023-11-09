using NUnit.Framework;
using TechObject;
using Editor;
using System.Collections.Generic;
using System;
using System.Linq;
using Moq;
using EplanDevice;

namespace EasyEplanner.Tests
{
    class StepTest
    {
        private static State RUN  = new State(State.StateType.RUN, null);
        private static State STOP = new State(State.StateType.STOP, null);
        private static State PAUSE = new State(State.StateType.PAUSE, null);
        private static State IDLE = new State(State.StateType.IDLE, null);

        [TestCase(true, new int[] { 0, -1 })]
        [TestCase(false, new int[] { 0, 1 })]
        public void EditablePart_StepFromCaseSource_ReturnsArray(
            bool isMainStep, int[] expectedArray)
        {
            var step = new Step(string.Empty, null, RUN, isMainStep);

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

        static object[] Constructor_NewStep_CheckActionsCountCases =
        {
            new object[] { RUN, true, 15 },
            new object[] { RUN, false, 15 },
            new object[] { STOP, true, 14 },
            new object[] { STOP, false, 15 },
            new object[] { PAUSE, true, 14 },
            new object[] { PAUSE, false, 15 },
            new object[] { IDLE, true, 15 },
            new object[] { IDLE, false, 15 },
        };

        [TestCaseSource(nameof(Constructor_NewStep_CheckActionsCountCases))]
        public void Constructor_NewStep_CheckActionsCount(State state,
            bool isMainStep, int expectedCount)
        {
            var step = new Step(string.Empty, null, state, isMainStep);

            var actualCount = step.GetActions.Count;

            Assert.AreEqual(expectedCount, actualCount);
        }

        private static readonly int NotMainStepActionsCount = 18;
        private static readonly int MainStepActionsCount = 14;
        private static readonly int MainStepActionsWithToStateCount = 15;

        static object[] Constructor_NewStep_CheckItemsCountCases =
        {
            new object[] { RUN, true, MainStepActionsWithToStateCount },
            new object[] { RUN, false, NotMainStepActionsCount },
            new object[] { STOP, true, MainStepActionsCount },
            new object[] { STOP, false, NotMainStepActionsCount },
            new object[] { PAUSE, true, MainStepActionsCount },
            new object[] { PAUSE, false, NotMainStepActionsCount },
            new object[] { IDLE, true, MainStepActionsWithToStateCount },
            new object[] { IDLE, false, NotMainStepActionsCount },
        };

        [TestCaseSource(nameof(Constructor_NewStep_CheckItemsCountCases))]
        public void Constructor_NewStep_CheckItemsCount(State state,
            bool isMainStep, int expectedCount)
        {
            var step = new Step(string.Empty, null, state, isMainStep);

            var actualCount = step.Items.Length;

            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestCaseSource(nameof(CheckActionsSequenceByLuaNameTestSource))]
        public void Constructor_NewStep_CheckActionsSequenceByLuaname(
            bool isMainStep, string[] expectedLuaNames)
        {
            var step = new Step(string.Empty, null, RUN, isMainStep);

            var actions = step.GetActions;

            for (int i = 0; i < actions.Count; i++)
            {
                Assert.AreEqual(expectedLuaNames[i], actions[i].LuaName);
            }
        }

        private static object[] CheckActionsSequenceByLuaNameTestSource()
        {
            var checkedDevices = "checked_devices";
            var openedDevices = "opened_devices";
            var delayOpenedDevices = "delay_opened_devices";
            var openedReverseDevices = "opened_reverse_devices";
            var closedDevices = "closed_devices";
            var delayClosedDevices = "delay_closed_devices";
            var openedUpperSeats = "opened_upper_seat_v";
            var openedLowerSeats = "opened_lower_seat_v";
            var requiredFB = "required_FB";
            var devices = "devices_data";
            var pairsDIDO = "DI_DO";
            var pairsInvertedDIDO = "inverted_DI_DO";
            var pairsAIAO = "AI_AO";
            var enableStepBySignal = "enable_step_by_signal";
            var toStepIfDevicesInSpecificState =
                "jump_if";
            var toStateIfDevicesInSpecificState = "jump_if";

            object[] sequenceIfNoMainStep = new object[]
            {
                false,
                new string[]
                {
                    checkedDevices,
                    openedDevices,
                    delayOpenedDevices,
                    openedReverseDevices,
                    closedDevices,
                    delayClosedDevices,
                    openedUpperSeats,
                    openedLowerSeats,
                    requiredFB,
                    devices,
                    pairsDIDO,
                    pairsInvertedDIDO,
                    pairsAIAO,
                    enableStepBySignal,
                    toStepIfDevicesInSpecificState,
                }
            };

            object[] sequenceIfMainStep = new object[]
            {
                true,
                new string[]
                {
                    checkedDevices,
                    openedDevices,
                    delayOpenedDevices,
                    openedReverseDevices,
                    closedDevices,
                    delayClosedDevices,
                    openedUpperSeats,
                    openedLowerSeats,
                    requiredFB,
                    devices,
                    pairsDIDO,
                    pairsInvertedDIDO,
                    pairsAIAO,
                    enableStepBySignal,
                    toStateIfDevicesInSpecificState,
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
            var step = new Step(string.Empty, null, RUN, isMainStep);

            var actions = step.GetActions;

            for (int i = 0; i < actions.Count; i++)
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

            DrawInfo.Style checkedDevices = greenBox;
            DrawInfo.Style openedDevices = greenBox;
            DrawInfo.Style delayOpenedDevices = greenBox;
            DrawInfo.Style openedReverseDevices = greenBox;
            DrawInfo.Style closedDevices = redBox;
            DrawInfo.Style delayClosedDevices = greenBox;
            DrawInfo.Style openedUpperSeats = greenUpBox;
            DrawInfo.Style openedLowerSeats = greenLowBox;
            DrawInfo.Style requiredFB = greenBox;
            DrawInfo.Style devices = greenBox;
            DrawInfo.Style pairsDIDO = greenBox;
            DrawInfo.Style pairsInvertedDIDO = greenBox;
            DrawInfo.Style pairsAIAO = greenBox;
            DrawInfo.Style enableStepBySignal = greenBox;
            DrawInfo.Style toStepByCondition = greenBox;
            DrawInfo.Style toStateIfDevicesInSpecificState = greenBox;

            var noMainStep = new object[]
            {
                false,
                new DrawInfo.Style[]
                {
                    checkedDevices,
                    openedDevices,
                    delayOpenedDevices,
                    openedReverseDevices,
                    closedDevices,
                    delayClosedDevices,
                    openedUpperSeats,
                    openedLowerSeats,
                    requiredFB,
                    devices,
                    pairsDIDO,
                    pairsInvertedDIDO,
                    pairsAIAO,
                    enableStepBySignal,
                    toStepByCondition,
                }
            };

            var mainStep = new object[]
            {
                true,
                new DrawInfo.Style[]
                {
                    checkedDevices,
                    openedDevices,
                    delayOpenedDevices,
                    openedReverseDevices,
                    closedDevices,
                    delayClosedDevices,
                    openedUpperSeats,
                    openedLowerSeats,
                    requiredFB,
                    devices,
                    pairsDIDO,
                    pairsInvertedDIDO,
                    pairsAIAO,
                    enableStepBySignal,
                    toStateIfDevicesInSpecificState,
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
            bool isMainStep, List<EplanDevice.DeviceType[]> devTypesList)
        {
            var step = new Step(string.Empty, null, RUN, isMainStep);

            var actions = step.GetActions;

            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                action.GetDisplayObjects(out EplanDevice.DeviceType[] actualDevTypes,
                    out _, out _);
                EplanDevice.DeviceType[] expectedDevTypes = devTypesList[i];

                Assert.AreEqual(expectedDevTypes, actualDevTypes);
            }
        }

        private static object[] CheckActionsAllowedDevTypes()
        {
            // null - allowed all devices in action.
            // But action can contain other actions and it means, that main
            // action can't be changed itself, inner can. Null for main if u
            // have inner = it's ok.
            EplanDevice.DeviceType[] allTypesAllowed = null;

            var openDevice = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.V,
                EplanDevice.DeviceType.DO,
                EplanDevice.DeviceType.M
            };

            var delayOpenDevice = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.V,
                EplanDevice.DeviceType.DO,
                EplanDevice.DeviceType.M
            };

            var openReverse = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.M
            };

            var closeDevice = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.V,
                EplanDevice.DeviceType.DO,
                EplanDevice.DeviceType.M
            };

            var delayCloseDevice = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.V,
                EplanDevice.DeviceType.DO,
                EplanDevice.DeviceType.M
            };

            var valveSeats = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.V,
                EplanDevice.DeviceType.DO
            };
            EplanDevice.DeviceType[] openUpperSeats = valveSeats;
            EplanDevice.DeviceType[] openLowerSeats = valveSeats;

            var requiredFb = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.DI,
                EplanDevice.DeviceType.GS
            };

            var groupDiDo = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.DI,
                EplanDevice.DeviceType.SB,
                EplanDevice.DeviceType.DO,
                EplanDevice.DeviceType.HL,
                EplanDevice.DeviceType.GS,
                EplanDevice.DeviceType.LS,
                EplanDevice.DeviceType.FS
            };

            var groupInvertedDIDO = groupDiDo;

            var groupAiAo = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.AI,
                EplanDevice.DeviceType.AO,
                EplanDevice.DeviceType.M,
                EplanDevice.DeviceType.PT,
                EplanDevice.DeviceType.LT,
                EplanDevice.DeviceType.FQT,
                EplanDevice.DeviceType.QT,
                EplanDevice.DeviceType.TE,
                EplanDevice.DeviceType.VC
            };

            var enableStepBySignal = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.DI,
                EplanDevice.DeviceType.LS,
                EplanDevice.DeviceType.GS,
                EplanDevice.DeviceType.SB,
                EplanDevice.DeviceType.FS,
                EplanDevice.DeviceType.TS,
                EplanDevice.DeviceType.PDS,
            };

            var byCondition = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.V,
                EplanDevice.DeviceType.GS,
                EplanDevice.DeviceType.DI,
                EplanDevice.DeviceType.DO,
                EplanDevice.DeviceType.SB,
                EplanDevice.DeviceType.LS,
            };

            var mainStep = new object[]
            {
                true,
                new List<EplanDevice.DeviceType[]>()
                {
                    allTypesAllowed,
                    openDevice,
                    delayOpenDevice,
                    openReverse,
                    closeDevice,
                    delayCloseDevice,
                    openUpperSeats,
                    openLowerSeats,
                    requiredFb,
                    allTypesAllowed,
                    groupDiDo,
                    groupInvertedDIDO,
                    groupAiAo,
                    enableStepBySignal,
                    byCondition,
                }
            };

            var noMainStep = new object[]
            {
                true,
                new List<EplanDevice.DeviceType[]>()
                {
                    allTypesAllowed,
                    openDevice,
                    delayOpenDevice,
                    openReverse,
                    closeDevice,
                    delayCloseDevice,
                    openUpperSeats,
                    openLowerSeats,
                    requiredFb,
                    allTypesAllowed,
                    groupDiDo,
                    groupInvertedDIDO,
                    groupAiAo,
                    enableStepBySignal,
                    byCondition,
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
            bool isMainStep, List<EplanDevice.DeviceSubType[]> devSubTypesList)
        {
            var step = new Step(string.Empty, null, RUN, isMainStep);

            var actions = step.GetActions;

            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                action.GetDisplayObjects(out _,
                    out EplanDevice.DeviceSubType[] actualDevSubTypes, out _);
                EplanDevice.DeviceSubType[] expectedDevTypes = devSubTypesList[i];

                Assert.AreEqual(expectedDevTypes, actualDevSubTypes);
            }
        }

        private static object[] CheckActionsAllowedDevSubTypesSource()
        {
            // null - it means, that we can use any subtype from defined types.
            EplanDevice.DeviceSubType[] allSubTypesAllowed = null;

            var openReverse = new EplanDevice.DeviceSubType[]
            {
                EplanDevice.DeviceSubType.M_REV_FREQ,
                EplanDevice.DeviceSubType.M_REV_FREQ_2,
                EplanDevice.DeviceSubType.M_REV_FREQ_2_ERROR,
                EplanDevice.DeviceSubType.M_ATV,
                EplanDevice.DeviceSubType.M_ATV_LINEAR,
                EplanDevice.DeviceSubType.M,
                EplanDevice.DeviceSubType.M_VIRT,
            };

            var openUpperSeats = new EplanDevice.DeviceSubType[]
            {
                EplanDevice.DeviceSubType.V_MIXPROOF,
                EplanDevice.DeviceSubType.V_AS_MIXPROOF,
                EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF,
                EplanDevice.DeviceSubType.V_IOL_TERMINAL_MIXPROOF_DO3,
                EplanDevice.DeviceSubType.V_VIRT,
                EplanDevice.DeviceSubType.DO,
                EplanDevice.DeviceSubType.DO_VIRT
            };
            var openLowerSeats = new EplanDevice.DeviceSubType[]
            {
                EplanDevice.DeviceSubType.V_MIXPROOF,
                EplanDevice.DeviceSubType.V_AS_MIXPROOF,
                EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF,
                EplanDevice.DeviceSubType.V_MINI_FLUSHING,
                EplanDevice.DeviceSubType.V_IOL_TERMINAL_MIXPROOF_DO3,
                EplanDevice.DeviceSubType.V_VIRT,
                EplanDevice.DeviceSubType.DO,
                EplanDevice.DeviceSubType.DO_VIRT
            };
            var mainStep = new object[]
            {
                true,
                new List<EplanDevice.DeviceSubType[]>()
                {
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    openReverse,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    openUpperSeats,
                    openLowerSeats,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                }
            };

            var noMainStep = new object[]
            {
                true,
                new List<EplanDevice.DeviceSubType[]>()
                {
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    openReverse,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
                    openUpperSeats,
                    openLowerSeats,
                    allSubTypesAllowed,
                    allSubTypesAllowed,
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

        [TestCaseSource(nameof(ImageIndexInActionsTest))]
        public void ImageIndex_SetOfActions_ReturnCorrectImageIndexSequence(
            bool isMainStep, List<ImageIndexEnum> expectedImageIndexes)
        {
            var step = new Step(string.Empty, null, RUN, isMainStep);

            var actualImageIndexes = new List<ImageIndexEnum>();
            foreach (var action in step.GetActions)
            {
                var treeViewItem = (ITreeViewItem)action;
                ImageIndexEnum imageIndex = treeViewItem.ImageIndex;
                actualImageIndexes.Add(imageIndex);
            }

            Assert.AreEqual(expectedImageIndexes, actualImageIndexes);
        }

        private static object[] ImageIndexInActionsTest()
        {
            var checkedDevices = ImageIndexEnum.NONE;
            var openedDevices = ImageIndexEnum.ActionON;
            var delayOpenedDevices = ImageIndexEnum.NONE;
            var openedReverseDevices = ImageIndexEnum.NONE;
            var closedDevices = ImageIndexEnum.ActionOFF;
            var delayClosedDevices = ImageIndexEnum.NONE;
            var openedUpperSeats = ImageIndexEnum.ActionWashUpperSeats;
            var openedLowerSeats = ImageIndexEnum.ActionWashLowerSeats;
            var requiredFB = ImageIndexEnum.ActionSignals;
            var devices = ImageIndexEnum.ActionWash;
            var pairsDIDO = ImageIndexEnum.ActionDIDOPairs;
            var pairsInvertedDIDO = pairsDIDO;
            var pairsAIAO = ImageIndexEnum.ActionDIDOPairs;
            var toStepIfDevicesInSpecificState = ImageIndexEnum.NONE;
            var enableStepBySignal = ImageIndexEnum.NONE;
            var toStateIfDevicesInSpecificState = ImageIndexEnum.NONE;

            object[] notMainStepImageIndexes = new object[]
            {
                false,
                new List<ImageIndexEnum>()
                {
                    checkedDevices,
                    openedDevices,
                    delayOpenedDevices,
                    openedReverseDevices,
                    closedDevices,
                    delayClosedDevices,
                    openedUpperSeats,
                    openedLowerSeats,
                    requiredFB,
                    devices,
                    pairsDIDO,
                    pairsInvertedDIDO,
                    pairsAIAO,
                    enableStepBySignal,
                    toStepIfDevicesInSpecificState
                }
            };

            object[] mainStepImageIndexes = new object[]
            {
                true,
                new List<ImageIndexEnum>()
                {
                    checkedDevices,
                    openedDevices,
                    delayOpenedDevices,
                    openedReverseDevices,
                    closedDevices,
                    delayClosedDevices,
                    openedUpperSeats,
                    openedLowerSeats,
                    requiredFB,
                    devices,
                    pairsDIDO,
                    pairsInvertedDIDO,
                    pairsAIAO,
                    enableStepBySignal,
                    toStateIfDevicesInSpecificState,
                }
            };

            return new object[]
            {
                mainStepImageIndexes,
                notMainStepImageIndexes
            };
        }

        [TestCaseSource(nameof(ActionsTypesSequenceTest))]
        public void Constructor_NewStep_ReturnsCorrectTypesSequence(
            bool isMainStep, Type[] expectedTypes)
        {
            var step = new Step(string.Empty, null, RUN, isMainStep);

            var actualTypes = new List<Type>();
            foreach (var action in step.GetActions)
            {
                actualTypes.Add(action.GetType());
            }

            Assert.AreEqual(expectedTypes, actualTypes.ToArray());
        }

        public static object[] ActionsTypesSequenceTest()
        {
            Type action = typeof(TechObject.Action);
            Type actionGroup = typeof(ActionGroup);
            Type actionGroupWash = typeof(ActionGroupWash);
            Type actionToStepByCondition = typeof(ActionGroupCustom);
            Type actionGroupCustom = typeof(ActionGroupCustom);

            Type checkedDevices = action;
            Type openedDevices = action;
            Type openedReverseDevices = action;
            Type closedDevices = action;
            Type openedUpperSeats = actionGroup;
            Type openedLowerSeats = actionGroup;
            Type requiredFB = action;
            Type devices = actionGroupWash;
            Type pairsDIDO = actionGroup;
            Type pairsInvertedDIDO = actionGroup;
            Type pairsAIAO = actionGroup;
            Type enableStepBySignal = actionGroupCustom;

            var mainStepSequence = new Type[]
            {
                checkedDevices,
                openedDevices,
                actionGroupCustom,
                openedReverseDevices,
                closedDevices,
                actionGroupCustom,
                openedUpperSeats,
                openedLowerSeats,
                requiredFB,
                devices,
                pairsDIDO,
                pairsInvertedDIDO,
                pairsAIAO,
                enableStepBySignal,
                actionGroupCustom,
            };

            var notMainStepSequence = new Type[]
            {
                checkedDevices,
                openedDevices,
                actionGroupCustom,
                openedReverseDevices,
                closedDevices,
                actionGroupCustom,
                openedUpperSeats,
                openedLowerSeats,
                requiredFB,
                devices,
                pairsDIDO,
                pairsInvertedDIDO,
                pairsAIAO,
                enableStepBySignal,
                actionToStepByCondition
            };

            return new object[]
            {
                new object[] { true, mainStepSequence },
                new object[] { false, notMainStepSequence },
            };
        }

        [Test]
        public void UpdateOnGenericTechObject()
        {
            var step = new Step("Шаг", GetN => 1, null);
            var genericStep = new Step("Шаг 1", GetN => 1, null);

            var genericActionMock = new Mock<IAction>();
            genericActionMock.Setup(a => a.Empty).Returns(false);

            var deviceManagerMock = new Mock<IDeviceManager>();

            genericStep.GetActions[0] = genericActionMock.Object;
            
            genericStep.SetPar(2, 3);


            var actionMock = new Mock<IAction>();
            actionMock.Setup(a => a.UpdateOnGenericTechObject(It.IsAny<IAction>()))
                .Callback<IAction>((action) => Assert.AreSame(genericActionMock.Object, action));
            actionMock.Setup(a => a.UpdateOnGenericTechObject(null))
                .Callback<IAction>((action) => Assert.IsNull(action));

            step.GetActions[0] = actionMock.Object;

            Assert.Multiple(() =>
            {
                step.UpdateOnGenericTechObject(genericStep);

                step.GetActions.RemoveRange(1, step.GetActions.Count - 1);

                step.UpdateOnGenericTechObject(null);

                Assert.AreEqual("2", step.TimeParam);
                Assert.AreEqual("3", step.NextStepN);
            });
        }
    }
}
