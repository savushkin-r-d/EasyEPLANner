using Editor;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TechObject;
using TechObject.ActionProcessingStrategy;

namespace Tests.TechObject
{
    class ActionTest
    {
        // AddParam virtual method shouldn't test because the method do nothing.
        // Synch method delegates work to another entity.

        [TestCase("Name")]
        [TestCase("Имя")]
        [TestCase("")]
        public void Name_NewAction_ReturnsActionName(string expectedName)
        {
            var action = new Action(expectedName, null, string.Empty);

            Assert.AreEqual(expectedName, action.Name);
        }

        [TestCase("LuaName")]
        [TestCase("Name")]
        [TestCase("Имя")]
        [TestCase("")]
        public void LuaName_NewAction_ReturnsActionLuaName(
            string expectedLuaName)
        {
            var action = new Action(string.Empty, null, expectedLuaName);

            Assert.AreEqual(expectedLuaName, action.LuaName);
        }

        [TestCase(ImageIndexEnum.ActionON, "opened_devices")]
        [TestCase(ImageIndexEnum.ActionOFF, "closed_devices")]
        [TestCase(ImageIndexEnum.ActionSignals, "required_FB")]
        [TestCase(ImageIndexEnum.NONE, "opened_reverse_devices")]
        [TestCase(ImageIndexEnum.NONE, "")]
        public void ImageIndex_NewAction_ReturnsImageIndexEnum(
            ImageIndexEnum expectedImageIndex, string luaName)
        {
            var action = new Action(string.Empty, null, luaName);
            action.ImageIndex = expectedImageIndex;

            Assert.AreEqual(expectedImageIndex, action.ImageIndex);
        }

        [TestCase(DrawInfo.Style.GREEN_BOX)]
        [TestCase(DrawInfo.Style.GREEN_LOWER_BOX)]
        [TestCase(DrawInfo.Style.GREEN_RED_BOX)]
        [TestCase(DrawInfo.Style.GREEN_UPPER_BOX)]
        [TestCase(DrawInfo.Style.NO_DRAW)]
        [TestCase(DrawInfo.Style.RED_BOX)]
        public void DrawStyle_NewAction_GetSetNewDrawStyle(DrawInfo.Style style)
        {
            var action = new Action(string.Empty, null, string.Empty);

            action.DrawStyle = style;

            Assert.AreEqual(style, action.DrawStyle);
        }

        [Test]
        public void IsDrawOnEplanPage_NewAction_ReturnsTrue()
        {
            var action = new Action(string.Empty, null, string.Empty);

            Assert.IsTrue(action.IsDrawOnEplanPage);
        }

        [Test]
        public void IsUseDevList_NewAction_ReturnsTrue()
        {
            var action = new Action(string.Empty, null, string.Empty);

            Assert.IsTrue(action.IsUseDevList);
        }

        [Test]
        public void IsDeleteable_NewAction_ReturnsTrue()
        {
            var action = new Action(string.Empty, null, string.Empty);

            Assert.IsTrue(action.IsDeletable);
        }

        [TestCase(new int[] { -1, 1 })]
        public void EditablePart_NewAction_ReturnsExpectedArr(int[] expectedArr)
        {
            var action = new Action(string.Empty, null, string.Empty);

            Assert.AreEqual(expectedArr, action.EditablePart);
        }

        [Test]
        public void IsEditable_NewAction_ReturnsTrue()
        {
            var action = new Action(string.Empty, null, string.Empty);

            Assert.IsTrue(action.IsEditable);
        }

        [Test]
        public void DeviceIndex_NewAction_AddNeRemoveValues()
        {
            const int expectedCountAfterAdd = 1;
            const int expectedCountAfterDel = 0;
            var action = new Action(string.Empty, null, string.Empty);

            int addingValue = new System.Random().Next();
            action.DeviceIndex.Add(addingValue);

            Assert.AreEqual(expectedCountAfterAdd, action.DeviceIndex.Count);

            action.DeviceIndex.Remove(addingValue);

            Assert.AreEqual(expectedCountAfterDel, action.DeviceIndex.Count);
        }

        [Test]
        public void Clear_ActionWithDevicesId_CleadDeviceIndex()
        {
            const int expectedDevsCount = 0;
            var newDevs = new List<int> { 8, 6, 4, 3, 2, 9 };
            var action = new Action(string.Empty, null, string.Empty);
            action.DeviceIndex = newDevs;

            action.Clear();

            Assert.AreEqual(expectedDevsCount, action.DeviceIndex.Count);
        }

        [Test]
        public void ActionProcessorStrategy_NewActionMockStrategy_SetNewStrategy()
        {
            var action = new Action(string.Empty, null, string.Empty);
            var strategyMock = new Mock<IActionProcessorStrategy>();
            strategyMock.SetupProperty(x => x.Action);

            action.ActionProcessorStrategy = strategyMock.Object;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(strategyMock.Object.GetHashCode(),
                    action.ActionProcessorStrategy.GetHashCode());
                Assert.AreEqual(action, strategyMock.Object.Action);
            });
        }

        [Test]
        public void ActionProcessorStrategy_NewActionNullStrategy_SetDefaultStrategy()
        {
            var action = new Action(string.Empty, null, string.Empty);

            action.ActionProcessorStrategy = null;

            IActionProcessorStrategy strategy = action
                .ActionProcessorStrategy;
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(strategy);
                Assert.IsTrue(strategy is DefaultActionProcessorStrategy);
            });
        }

        [TestCaseSource(nameof(EmptyPropertyTestCaseSource))]
        public void Empty_NewActionWithOrNoDevs_ReturnsTrueOrFalse(
            List<int> devicesIds, bool expectedResult)
        {
            var action = new Action(string.Empty, null, string.Empty);

            foreach (var devId in devicesIds)
            {
                action.DeviceIndex.Add(devId);
            }

            Assert.AreEqual(action.Empty, expectedResult);
        }

        private static object[] EmptyPropertyTestCaseSource()
        {
            return new object[]
            {
                new object[] { new List<int> { 1, 2, 3 }, false },
                new object[] { new List<int> { }, true },
            };
        }

        [TestCaseSource(nameof(IsFilledPropertyTestCaseSource))]
        public void IsFilled_NewACtionWithOrNoDevs_ReturnsTrueOrFalse(
            List<int> devicesIds, bool expectedResult)
        {
            var action = new Action(string.Empty, null, string.Empty);

            foreach (var devId in devicesIds)
            {
                action.DeviceIndex.Add(devId);
            }

            Assert.AreEqual(action.IsFilled, expectedResult);
        }

        private static object[] IsFilledPropertyTestCaseSource()
        {
            return new object[]
            {
                new object[] { new List<int> { 1, 2, 3 }, true },
                new object[] { new List<int> { }, false },
            };
        }

        [TestCaseSource(nameof(GetObjectToDrawOnEplanPageTestCaseSource))]
        public void GetObjectToDrawOnEplanPage_NewAction_ReturnsDrawInfoList(
            int expectedCount, DrawInfo.Style drawStyle, List<int> devIds)
        {
            var deviceManagerMock = new Mock<Device.IDeviceManager>();
            deviceManagerMock.Setup(x => x.GetDeviceByIndex(It.IsAny<int>()))
                .Returns(new Mock<Device.IDevice>().Object);
            var action = new Action(string.Empty, null, string.Empty, null,
                null, null, deviceManagerMock.Object);
            action.DrawStyle = drawStyle;
            action.DeviceIndex.AddRange(devIds);

            List<DrawInfo> drawObjs = action.GetObjectToDrawOnEplanPage();

            Assert.Multiple(() =>
            {
                foreach (var drawObj in drawObjs)
                {
                    Assert.AreEqual(action.DrawStyle, drawObj.DrawingStyle);
                }

                Assert.AreEqual(expectedCount, drawObjs.Count);
            });
        }

        private static object[] GetObjectToDrawOnEplanPageTestCaseSource()
        {
            return new object[]
            {
                new object[]
                {
                    0,
                    DrawInfo.Style.GREEN_BOX,
                    new List<int>()
                },
                new object[]
                {
                    5,
                    DrawInfo.Style.GREEN_LOWER_BOX,
                    new List<int>() { 8, 6, 4, 2, 7 }
                },
                new object[]
                {
                    3,
                    DrawInfo.Style.GREEN_RED_BOX,
                    new List<int>() { 3, 6, 9 }
                },
                new object[]
                {
                    2,
                    DrawInfo.Style.GREEN_UPPER_BOX,
                    new List<int>() { 8, 3 }
                },
                new object[]
                {
                    4,
                    DrawInfo.Style.NO_DRAW,
                    new List<int>() { 4, 66, 33, 22 }
                },
                new object[]
                {
                    0,
                    DrawInfo.Style.RED_BOX,
                    new List<int>()
                },
            };
        }

        [TestCaseSource(nameof(GetDisplayObjectsTestCaseSource))]
        public void GetDisplayObjects_NewAction_ReturnsExpectedValues(
            Device.DeviceType[] expectedTypes,
            Device.DeviceSubType[] expectedDeviceSubTypes)
        {
            var action = new Action(string.Empty, null, string.Empty,
                expectedTypes, expectedDeviceSubTypes);

            action.GetDisplayObjects(out Device.DeviceType[] actualDevTypes,
                out Device.DeviceSubType[] actualDevSubTypes,
                out bool displayParameters);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedTypes, actualDevTypes);
                Assert.AreEqual(expectedDeviceSubTypes, actualDevSubTypes);
                Assert.IsFalse(displayParameters);
            });
        }

        private static object[] GetDisplayObjectsTestCaseSource()
        {
            return new object[]
            {
                new object[] { null, null },
                new object[]
                {
                    new Device.DeviceType[]
                    {
                        Device.DeviceType.DI,
                        Device.DeviceType.DO
                    },
                    new Device.DeviceSubType[]
                    {
                        Device.DeviceSubType.DI,
                        Device.DeviceSubType.DI_VIRT,
                        Device.DeviceSubType.DO,
                        Device.DeviceSubType.DO_VIRT,
                    }
                },
                new object[]
                {
                    new Device.DeviceType[]
                    {
                        Device.DeviceType.V,
                        Device.DeviceType.VC
                    },
                    null
                },
                new object[]
                {
                    null,
                    new Device.DeviceSubType[]
                    {
                        Device.DeviceSubType.NONE
                    }
                },
            };
        }

        [TestCase(new int[] { 2, 5, 3 })]
        [TestCase(new int[0])]
        public void EditText_NewAction_ReturnsCorrectEditTextArr(int[] devIds)
        {
            const string devName = "Name";
            var deviceMock = new Mock<Device.IDevice>();
            deviceMock.SetupGet(x => x.Name)
            .Returns(devName);
            var deviceManagerMock = new Mock<Device.IDeviceManager>();
            deviceManagerMock.Setup(x => x.GetDeviceByIndex(It.IsAny<int>()))
                .Returns(deviceMock.Object);
            var action = new Action(string.Empty, null, string.Empty, null,
                null, null, deviceManagerMock.Object);
            action.DeviceIndex.AddRange(devIds);
            var preExpectedText = new string[devIds.Length];
            for (int i = 0; i < preExpectedText.Length; i++)
            {
                preExpectedText[i] = devName;
            }
            var expectedEditText = new string[]
            {
                string.Empty, string.Join(" ", preExpectedText)
            };

            string[] actualEditText = action.EditText;

            Assert.AreEqual(expectedEditText, actualEditText);
        }

        [TestCase(new int[] { 2, 5, 3 })]
        [TestCase(new int[0])]
        public void DisplayText_NewAction_ReturnsCorrectEditTextArr(int[] devIds)
        {
            const string devName = "Name";
            var deviceMock = new Mock<Device.IDevice>();
            deviceMock.SetupGet(x => x.Name)
            .Returns(devName);
            var deviceManagerMock = new Mock<Device.IDeviceManager>();
            deviceManagerMock.Setup(x => x.GetDeviceByIndex(It.IsAny<int>()))
                .Returns(deviceMock.Object);
            var action = new Action(string.Empty, null, string.Empty, null,
                null, null, deviceManagerMock.Object);
            action.DeviceIndex.AddRange(devIds);
            var preExpectedText = new string[devIds.Length];
            for (int i = 0; i < preExpectedText.Length; i++)
            {
                preExpectedText[i] = devName;
            }
            var expectedDisplayText = new string[]
            {
                string.Empty, string.Join(" ", preExpectedText)
            };

            string[] actualDisplayText = action.DisplayText;

            Assert.AreEqual(expectedDisplayText, actualDisplayText);
        }

        [TestCase("TANK1V1 TANK2V2 TT4W ЫЫЫЫ", 2, true)]
        [TestCase("", 0, true)]
        [TestCase("##$$$ %% ABW АБВГДЕ ТАНК1", 0, false)]
        public void SetNewvalue_NewAction_ReturnsExpectedValues(
            string newDevs, int expectedDevsCount, bool expectedResult)
        {
            var strategyMock = new Mock<IActionProcessorStrategy>();
            strategyMock.Setup(x => x.ProcessDevices(It.IsAny<string>(),
                It.IsAny<Device.IDeviceManager>()))
                .Returns(Enumerable.Range(1, expectedDevsCount).ToList());

            var action = new Action(string.Empty, null, string.Empty,
                null, null, strategyMock.Object);

            bool result = action.SetNewValue(newDevs);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedResult, result);
                Assert.AreEqual(expectedDevsCount, action.DeviceIndex.Count);
            });
        }

        [TestCase(new int[0], 0)]
        [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4)]
        [Description("If devId mod 2 == 0 -> Valid device, else - invalid (skip)")]
        public void AddDev_NewAction_AddOrSkipDev(int[] devIds,
            int expectedDevCount)
        {
            var validDevMock = new Mock<Device.IDevice>();
            validDevMock.SetupGet(x => x.Description).Returns("Name");
            var invalidDevMock = new Mock<Device.IDevice>();
            invalidDevMock.SetupGet(x => x.Description)
                .Returns(StaticHelper.CommonConst.Cap);
            var deviceManagerMock = new Mock<Device.IDeviceManager>();
            deviceManagerMock
                .Setup(x => x.GetDeviceByIndex(It.Is<int>(y => y % 2 == 0)))
                .Returns(validDevMock.Object);
            deviceManagerMock
                .Setup(x => x.GetDeviceByIndex(It.Is<int>(y => y % 2 != 0)))
                .Returns(invalidDevMock.Object);
            var action = new Action(string.Empty, null, string.Empty, null,
                null, null, deviceManagerMock.Object);

            foreach (var devId in devIds)
            {
                // Another arguments were skipped because they are useless.
                action.AddDev(devId);
            }

            Assert.AreEqual(expectedDevCount, action.DeviceIndex.Count);
        }

        [Description("If devId mod 2 == 0 -> Valid device," +
            "else - device name returns cap")]
        [TestCaseSource(nameof(SaveAsLuaTableTestCaseSource))]
        public void SaveAsLuaTable_NewAction_ReturnsCodeTextToSave(
            string actionName, string devNameInMock, string prefix,
            int[] devIds, string luaName, string expectedCode)
        {
            var validDevMock = new Mock<Device.IDevice>();
            validDevMock.SetupGet(x => x.Name).Returns(devNameInMock);
            var invalidDevMock = new Mock<Device.IDevice>();
            invalidDevMock.SetupGet(x => x.Name)
                .Returns(StaticHelper.CommonConst.Cap);
            var deviceManagerMock = new Mock<Device.IDeviceManager>();
            deviceManagerMock
                .Setup(x => x.GetDeviceByIndex(It.Is<int>(y => y % 2 == 0)))
                .Returns(validDevMock.Object);
            deviceManagerMock
                .Setup(x => x.GetDeviceByIndex(It.Is<int>(y => y % 2 != 0)))
                .Returns(invalidDevMock.Object);
            var action = new Action(actionName, null, luaName, null, null,
                null, deviceManagerMock.Object);
            action.DeviceIndex.AddRange(devIds);

            var actualCode = action.SaveAsLuaTable(prefix);

            Assert.AreEqual(expectedCode, actualCode);
        }

        private static object[] SaveAsLuaTableTestCaseSource()
        {
            string prefix = "\t";
            string devName = "Name";
            string name = "Имя действия";
            string luaName = "ActionLuaName";

            var emptyCodeBecauseNoDevs = new object[]
            {
                name, devName, prefix, new int[0], string.Empty, string.Empty,
            };

            var emptyCodeBecauseInvalidDevs = new object[]
            {
                name, devName, prefix, new int[] { 1, 3, 5 }, string.Empty,
                string.Empty
            };

            var okCodeNoLua = new object[]
            {
                name, devName, prefix, new int[] { 2, 4, 6 }, string.Empty,
                $"{prefix}--{name}\n{prefix}\t{{\n" +
                $"{prefix}\t" +
                $"'{devName}', " + $"'{devName}', " + $"'{devName}'" +
                "\n" +
                $"{prefix}\t}},\n"
            };

            var okCodeWithSkippedDevs = new object[]
            {
                name, devName, prefix, new int[] { 1, 2, 3, 4, 5, 6 }, luaName,
                $"{prefix}{luaName} = " +
                $"--{name}\n{prefix}\t{{\n" +
                $"{prefix}\t" +
                $"'{devName}', " + $"'{devName}', " + $"'{devName}'" +
                "\n" +
                $"{prefix}\t}},\n"
            };

            return new object[]
            {
                emptyCodeBecauseNoDevs,
                emptyCodeBecauseInvalidDevs,
                okCodeNoLua,
                okCodeWithSkippedDevs
            };
        }

        [TestCase(new int[] { 1, 2, 4, 5, 6 },
            new int[] { 3, 4, 5, 6 }, "KOAG", "1", "TANK", "1")]
        [TestCase(new int[] { 1, 2, 3, 4 },
            new int[] { 5, 6, 3, 4 }, "TANK", "2", "TANK", "1")]
        [TestCase(new int[] { 1, 2, 3, 4 },
            new int[] { 1, 2, 5 }, "TANK", "2", "KOAG", "1")]
        public void ModifyDevNames(int[] devIds, int[] expectedDevIds,
            string newTechObjectName, int newTechObjectNumber,
            string oldTechObjectName, int oldTechObjNumber)
        {
            Device.IDeviceManager deviceManager = DeviceManagerMock
                .DeviceManager;
            var action = new Action(string.Empty, null, string.Empty, null,
                null, null, deviceManager);
            action.DeviceIndex.AddRange(devIds);

            action.ModifyDevNames(newTechObjectName, newTechObjectNumber,
                oldTechObjectName, oldTechObjNumber);

            Assert.AreEqual(expectedDevIds, action.DeviceIndex);
        }

        [TestCase(new int[] { 1, 2, 3, 4 }, new int[] { 5, 6, 3, 4 }, 2, 1, "TANK")]
        [TestCase(new int[] { 5, 4, 6, 3 }, new int[] { 1, 4, 2, 3 }, 2, 1, "TANK")]
        [TestCase(new int[] { 1, 2, 3, 4, 8 }, new int[] { 5, 6, 3, 4, 7 }, 2, -1, "TANK")]
        public void ModifyDevNames(int[] devIds, int[] expectedDevIds,
            int newTechObjectN, int oldTechObjectN, string techObjectName)
        {
            Device.IDeviceManager deviceManager = DeviceManagerMock
                .DeviceManager;
            var action = new Action(string.Empty, null, string.Empty, null,
                null, null, deviceManager);
            action.DeviceIndex.AddRange(devIds);

            action.ModifyDevNames(newTechObjectN, oldTechObjectN,
                techObjectName);

            Assert.AreEqual(expectedDevIds, action.DeviceIndex);
        }

        [TestCase(new int[] { 1, 3, 5, 7, 9 })]
        [TestCase(new int[0])]
        public void Clone_NewAction_ReturnsCopy(int[] devIds)
        {
            var action = new Action(string.Empty, null, string.Empty);
            action.DeviceIndex.AddRange(devIds);

            var cloned = action.Clone();

            int actionStrategyHashCode = action.ActionProcessorStrategy
                .GetHashCode();
            int clonedStrategyHashCode = cloned.ActionProcessorStrategy
                .GetHashCode();

            Assert.Multiple(() =>
            {
                Assert.AreNotEqual(action.GetHashCode(), cloned.GetHashCode());
                Assert.AreEqual(action.DeviceIndex.Count,
                    cloned.DeviceIndex.Count);
                Assert.AreEqual(actionStrategyHashCode,
                    clonedStrategyHashCode);
                for (int i = 0; i < action.DeviceIndex.Count; i++)
                {
                    Assert.AreEqual(action.DeviceIndex[i],
                        cloned.DeviceIndex[i]);
                }
            });
        }

        [Test]
        public void Constructor_NewAction_CheckDefaultDrawStyle()
        {
            var action = new Action(string.Empty, null, string.Empty);

            Assert.AreEqual(DrawInfo.Style.GREEN_BOX, action.DrawStyle);
        }
    }

    class DefaultActionProcessingStrategyTest
    {
        [TestCaseSource(nameof(ProcessDevicesTestCaseSource))]
        public void ProcessDevices_DataFromTestCaseSource_ReturnsDevsIdsList(
            string devicesStr, Device.DeviceType[] allowedDevTypes,
            Device.DeviceSubType[] allowedDevSubTypes,
            IList<int> expectedDevsIds)
        {
            IAction action = IActionMock.GetAction(allowedDevTypes,
                allowedDevSubTypes, new List<int>());
            var strategy = new OneInManyOutActionProcessingStrategy();
            strategy.Action = action;
            var deviceManager = DeviceManagerMock.DeviceManager;

            IList<int> actualDevsIds = strategy.ProcessDevices(devicesStr,
                deviceManager);

            Assert.AreEqual(expectedDevsIds, actualDevsIds);
        }

        private static object[] ProcessDevicesTestCaseSource()
        {
            var allowValveMixproofIOLink = new object[]
            {
                "TANK1V2 TANK2V3 KOAG1M2 KOAG1V1 TANK2LS2 TANK3VC1",
                new Device.DeviceType[]
                {
                    Device.DeviceType.V
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.V_IOLINK_MIXPROOF,
                },
                new List<int> { 2, 7, 3}
            };

            var allowValeMixproofIOLinkAndGetEmptyList = new object[]
            {
                "KOAG1M2 TANK1LS1 TANK2LS2 TANK3VC1",
                new Device.DeviceType[]
                {
                    Device.DeviceType.V
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.V_IOLINK_MIXPROOF,
                },
                new List<int> { }
            };

            var allowVCAndLSIOLinkMin = new object[]
            {
                "TANK1V1 TANK3VC1 KOAG1M2 TANK2LS2 TANK1LS1",
                new Device.DeviceType[]
                {
                    Device.DeviceType.VC,
                    Device.DeviceType.LS
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.LS_IOLINK_MIN,
                    Device.DeviceSubType.NONE
                },
                new List<int> { 9, 11 }
            };

            var discardVCWhenAllowedInDevType = new object[]
            {
                "TANK3VC1",
                new Device.DeviceType[] { Device.DeviceType.VC },
                new Device.DeviceSubType[] { },
                new List<int> { }
            };

            return new object[]
            {
                allowValveMixproofIOLink,
                allowValeMixproofIOLinkAndGetEmptyList,
                allowVCAndLSIOLinkMin,
                discardVCWhenAllowedInDevType
            };
        }
    }

    class OneInManyOutActionProcessingStrategyTest
    {
        [TestCaseSource(nameof(ProcessDevicesTestCaseSource))]
        public void ProcessDevices_DataFromTestCaseSource_ReturnsDevsIdsList(
            string devicesStr, Device.DeviceType[] allowedDevTypes,
            Device.DeviceSubType[] allowedDevSubTypes,
            List<int> actionDevsDefaultIds, IList<int> expectedDevsIds)
        {
            IAction action = IActionMock.GetAction(allowedDevTypes,
                allowedDevSubTypes, actionDevsDefaultIds);
            var strategy = new OneInManyOutActionProcessingStrategy();
            strategy.Action = action;
            var deviceManager = DeviceManagerMock.DeviceManager;

            IList<int> actualDevsIds = strategy.ProcessDevices(devicesStr,
                deviceManager);

            Assert.AreEqual(expectedDevsIds, actualDevsIds);
        }

        private static object[] ProcessDevicesTestCaseSource()
        {
            var correctSequenceDIDO = new object[]
            {
                "TANK1DO1 TANK1DO2 TANK2DI2",
                new Device.DeviceType[]
                {
                    Device.DeviceType.DI,
                    Device.DeviceType.DO
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.DI,
                    Device.DeviceSubType.DI_VIRT,
                    Device.DeviceSubType.DO,
                    Device.DeviceSubType.DO_VIRT
                },
                new List<int> { 14 },
                new List<int> { 13, 14, 15 }
            };

            var correctSequenceAIAO = new object[]
            {
                "TANK1AO1 TANK1AO2 TANK2AI2",
                new Device.DeviceType[]
                {
                    Device.DeviceType.AI,
                    Device.DeviceType.AO
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.AI,
                    Device.DeviceSubType.AI_VIRT,
                    Device.DeviceSubType.AO,
                    Device.DeviceSubType.AO_VIRT
                },
                new List<int> { },
                new List<int> { 17, 18, 19 }
            };

            var replacingAIAOCase = new object[]
            {
                "TANK1AO1 TANK2AI2 TANK1AI1",
                new Device.DeviceType[]
                {
                    Device.DeviceType.AI,
                    Device.DeviceType.AO
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.AI,
                    Device.DeviceSubType.AI_VIRT,
                    Device.DeviceSubType.AO,
                    Device.DeviceSubType.AO_VIRT
                },
                new List<int> { 17, 18 },
                new List<int> { 16, 18 }
            };

            var replacingDIDOCase = new object[]
            {
                "TANK1DO1 TANK2DI2 TANK1DI1",
                new Device.DeviceType[]
                {
                    Device.DeviceType.DI,
                    Device.DeviceType.DO
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.DI,
                    Device.DeviceSubType.DI_VIRT,
                    Device.DeviceSubType.DO,
                    Device.DeviceSubType.DO_VIRT
                },
                new List<int> { 13, 14 },
                new List<int> { 12, 14 }
            };


            var useHLInSequence = new object[]
            {
                "TANK1DO1 TANK1HL1 TANK2DI2",
                new Device.DeviceType[]
                {
                    Device.DeviceType.DI,
                    Device.DeviceType.DO,
                    Device.DeviceType.HL,
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.DI,
                    Device.DeviceSubType.DI_VIRT,
                    Device.DeviceSubType.DO,
                    Device.DeviceSubType.DO_VIRT,
                    Device.DeviceSubType.NONE,
                },
                new List<int> { 13, 14 },
                new List<int> { 13, 14, 20 }
            };

            var useHLAndReplaceDIwithGS = new object[]
            {
                "TANK1DO1 TANK1HL1 TANK1GS1 TANK2DI2",
                new Device.DeviceType[]
                {
                    Device.DeviceType.DI,
                    Device.DeviceType.DO,
                    Device.DeviceType.HL,
                    Device.DeviceType.GS,
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.DI,
                    Device.DeviceSubType.DI_VIRT,
                    Device.DeviceSubType.DO,
                    Device.DeviceSubType.DO_VIRT,
                    Device.DeviceSubType.NONE,
                },
                new List<int> { 13, 14 },
                new List<int> { 22, 14, 20 }
            };

            var removeAllInputDevsIfDoubleInputDevs = new object[]
            {
                "TANK1DO1 TANK1HL1 TANK1GS2 TANK2DI2",
                new Device.DeviceType[]
                {
                    Device.DeviceType.DI,
                    Device.DeviceType.DO,
                    Device.DeviceType.HL,
                    Device.DeviceType.GS,
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.DI,
                    Device.DeviceSubType.DI_VIRT,
                    Device.DeviceSubType.DO,
                    Device.DeviceSubType.DO_VIRT,
                    Device.DeviceSubType.NONE,
                },
                new List<int> { 14 },
                new List<int> { 14, 20 }
            };

            return new object[]
            {
                correctSequenceAIAO,
                replacingAIAOCase,
                correctSequenceDIDO,
                replacingDIDOCase,
                useHLInSequence,
                useHLAndReplaceDIwithGS,
                removeAllInputDevsIfDoubleInputDevs
            };
        }
    }

    static class IActionMock
    {
        public static IAction GetAction(Device.DeviceType[] allowedDevTypes,
            Device.DeviceSubType[] allowedDevSubTypes,
            List<int> expectedDevsIds)
        {
            bool displayParameters = false;
            var actionMock = new Mock<IAction>();
            actionMock.Setup(x => x.GetDisplayObjects(out allowedDevTypes,
                out allowedDevSubTypes, out displayParameters));
            actionMock.Setup(x => x.DeviceIndex).Returns(expectedDevsIds);

            return actionMock.Object;
        }
    }

    static class DeviceManagerMock
    {
        static DeviceManagerMock()
        {
            var mock = new Mock<Device.IDeviceManager>();
            SetUpMock(mock);
            deviceManager = mock.Object;
        }

        private static void SetUpMock(Mock<Device.IDeviceManager>
            devManagerMock)
        {
            Device.IDevice stubDev = MakeMockedDevice(string.Empty, 0,
                Device.DeviceType.NONE, 0, Device.DeviceSubType.NONE);

            const string TANK = "TANK";
            const string KOAG = "KOAG";

            Device.IDevice tank1V1Dev = MakeMockedDevice(TANK, 1,
                Device.DeviceType.V, 1, Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice tank1V2Dev = MakeMockedDevice(TANK, 1,
                Device.DeviceType.V, 2, Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice koag1V1Dev = MakeMockedDevice(KOAG, 1,
                Device.DeviceType.V, 1, Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice koag1M2Dev = MakeMockedDevice(KOAG, 1,
                Device.DeviceType.M, 2, Device.DeviceSubType.M_ATV);
            Device.IDevice tank2V1Dev = MakeMockedDevice(TANK, 2,
                Device.DeviceType.V, 1, Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice tank2V2Dev = MakeMockedDevice(TANK, 2,
                Device.DeviceType.V, 2, Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice tank2V3Dev = MakeMockedDevice(TANK, 2,
                Device.DeviceType.V, 3, Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice tank3V3Dev = MakeMockedDevice(TANK, 3,
                Device.DeviceType.V, 3, Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice tank1LS1Dev = MakeMockedDevice(TANK, 1,
                Device.DeviceType.LS, 1, Device.DeviceSubType.LS_IOLINK_MIN);
            Device.IDevice tank2LS2Dev = MakeMockedDevice(TANK, 2,
                Device.DeviceType.LS, 2, Device.DeviceSubType.LS_IOLINK_MAX);
            Device.IDevice tank3VC1Dev = MakeMockedDevice(TANK, 3,
                Device.DeviceType.VC, 1, Device.DeviceSubType.NONE);
            Device.IDevice tank1DI1Dev = MakeMockedDevice(TANK, 1,
                Device.DeviceType.DI, 1, Device.DeviceSubType.DI);
            Device.IDevice tank2DI2Dev = MakeMockedDevice(TANK, 2,
                Device.DeviceType.DI, 2, Device.DeviceSubType.DI_VIRT);
            Device.IDevice tank1DO1Dev = MakeMockedDevice(TANK, 1,
                Device.DeviceType.DO, 1, Device.DeviceSubType.DO);
            Device.IDevice tank1DO2Dev = MakeMockedDevice(TANK, 1,
                Device.DeviceType.DO, 2, Device.DeviceSubType.DO_VIRT);
            Device.IDevice tank1AI1Dev = MakeMockedDevice(TANK, 1,
                Device.DeviceType.AI, 1, Device.DeviceSubType.AI);
            Device.IDevice tank2AI2Dev = MakeMockedDevice(TANK, 2,
                Device.DeviceType.AI, 2, Device.DeviceSubType.AI_VIRT);
            Device.IDevice tank1AO1Dev = MakeMockedDevice(TANK, 1,
                Device.DeviceType.AO, 1, Device.DeviceSubType.AO);
            Device.IDevice tank1AO2Dev = MakeMockedDevice(TANK, 1,
                Device.DeviceType.AO, 2, Device.DeviceSubType.AO_VIRT);
            Device.IDevice tank1HL1Dev = MakeMockedDevice(TANK, 1,
                Device.DeviceType.HL, 1, Device.DeviceSubType.NONE);
            Device.IDevice tank1HL2Dev = MakeMockedDevice(TANK, 1,
                Device.DeviceType.HL, 2, Device.DeviceSubType.NONE);
            Device.IDevice tank1GS1Dev = MakeMockedDevice(TANK, 1,
                Device.DeviceType.GS, 1, Device.DeviceSubType.NONE);
            Device.IDevice tank1GS2Dev = MakeMockedDevice(TANK, 1,
                Device.DeviceType.GS, 2, Device.DeviceSubType.NONE);

            var devicesDescription = new[]
            {
                new { Id = 1, Name = "TANK1V1", Dev = tank1V1Dev },
                new { Id = 2, Name = "TANK1V2", Dev = tank1V2Dev },
                new { Id = 3, Name = "KOAG1V1", Dev = koag1V1Dev },
                new { Id = 4, Name = "KOAG1M2", Dev = koag1M2Dev },
                new { Id = 5, Name = "TANK2V1", Dev = tank2V1Dev },
                new { Id = 6, Name = "TANK2V2", Dev = tank2V2Dev },
                new { Id = 7, Name = "TANK2V3", Dev = tank2V3Dev },
                new { Id = 8, Name = "TANK3V3", Dev = tank3V3Dev },
                new { Id = 9, Name = "TANK1LS1", Dev = tank1LS1Dev },
                new { Id = 10, Name = "TANK2LS2", Dev = tank2LS2Dev },
                new { Id = 11, Name = "TANK3VC1", Dev = tank3VC1Dev },
                new { Id = 12, Name = "TANK1DI1", Dev = tank1DI1Dev },
                new { Id = 13, Name = "TANK2DI2", Dev = tank2DI2Dev },
                new { Id = 14, Name = "TANK1DO1", Dev = tank1DO1Dev },
                new { Id = 15, Name = "TANK1DO2", Dev = tank1DO2Dev },
                new { Id = 16, Name = "TANK1AI1", Dev = tank1AI1Dev },
                new { Id = 17, Name = "TANK2AI2", Dev = tank2AI2Dev },
                new { Id = 18, Name = "TANK1AO1", Dev = tank1AO1Dev },
                new { Id = 19, Name = "TANK1AO2", Dev = tank1AO2Dev },
                new { Id = 20, Name = "TANK1HL1", Dev = tank1HL1Dev },
                new { Id = 21, Name = "TANK1HL2", Dev = tank1HL2Dev },
                new { Id = 22, Name = "TANK1GS1", Dev = tank1GS1Dev },
                new { Id = 23, Name = "TANK1GS2", Dev = tank1GS2Dev },
            };

            devManagerMock.Setup(x => x.GetDeviceByIndex(It.IsAny<int>()))
                .Returns(stubDev);
            devManagerMock.Setup(x => x.GetDeviceIndex(It.IsAny<string>()))
                .Returns(-1);
            devManagerMock
                .Setup(x => x.GetDeviceByEplanName(It.IsAny<string>()))
                .Returns(stubDev);
            foreach (var devDescr in devicesDescription)
            {
                devManagerMock.Setup(x => x.GetDeviceByIndex(devDescr.Id))
                    .Returns(devDescr.Dev);
                devManagerMock.Setup(x => x.GetDeviceIndex(devDescr.Name))
                    .Returns(devDescr.Id);
                devManagerMock.Setup(x => x.GetDeviceByEplanName(devDescr.Name))
                    .Returns(devDescr.Dev);
            }
        }

        private static Device.IDevice MakeMockedDevice(string objName, int objNum,
            Device.DeviceType devType, int devNumber,
            Device.DeviceSubType deviceSubType)
        {
            var devMock = new Mock<Device.IDevice>();
            devMock.SetupGet(x => x.ObjectName).Returns(objName);
            devMock.SetupGet(x => x.ObjectNumber).Returns(objNum);
            devMock.SetupGet(x => x.DeviceType).Returns(devType);
            devMock.SetupGet(x => x.DeviceNumber).Returns(devNumber);
            devMock.SetupGet(x => x.DeviceSubType).Returns(deviceSubType);
            return devMock.Object;
        }

        public static Device.IDeviceManager DeviceManager
            => deviceManager;

        private static Device.IDeviceManager deviceManager;
    }
}
