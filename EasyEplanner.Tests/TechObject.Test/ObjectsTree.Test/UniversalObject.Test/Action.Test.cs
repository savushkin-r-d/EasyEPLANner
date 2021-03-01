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
            var action = new Action(expectedName, null);

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

        [TestCase(ImageIndexEnum.ActionON, Action.OpenDevices)]
        [TestCase(ImageIndexEnum.ActionOFF, Action.CloseDevices)]
        [TestCase(ImageIndexEnum.ActionSignals, Action.RequiredFB)]
        [TestCase(ImageIndexEnum.NONE, Action.OpenReverseDevices)]
        [TestCase(ImageIndexEnum.NONE, "")]
        public void ImageIndex_NewAction_ReturnsImageIndexEnum(
            ImageIndexEnum expectedImageIndex, string luaName)
        {
            var action = new Action(string.Empty, null, luaName);

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
            var action = new Action(string.Empty, null);

            action.DrawStyle = style;

            Assert.AreEqual(style, action.DrawStyle);
        }

        [Test]
        public void IsDrawOnEplanPage_NewAction_ReturnsTrue()
        {
            var action = new Action(string.Empty, null);

            Assert.IsTrue(action.IsDrawOnEplanPage);
        }

        [Test]
        public void IsUseDevList_NewAction_ReturnsTrue()
        {
            var action = new Action(string.Empty, null);

            Assert.IsTrue(action.IsUseDevList);
        }

        [Test]
        public void IsDeleteable_NewAction_ReturnsTrue()
        {
            var action = new Action(string.Empty, null);

            Assert.IsTrue(action.IsDeletable);
        }

        [TestCase(new int[] { -1, 1 })]
        public void EditablePart_NewAction_ReturnsExpectedArr(int[] expectedArr)
        {
            var action = new Action(string.Empty, null);

            Assert.AreEqual(expectedArr, action.EditablePart);
        }

        [Test]
        public void IsEditable_NewAction_ReturnsTrue()
        {
            var action = new Action(string.Empty, null);

            Assert.IsTrue(action.IsEditable);
        }

        [Test]
        public void DeviceIndex_NewAction_AddNeRemoveValues()
        {
            const int expectedCountAfterAdd = 1;
            const int expectedCountAfterDel = 0;
            var action = new Action(string.Empty, null);

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
            var action = new Action(string.Empty, null);
            action.DeviceIndex = newDevs;

            action.Clear();

            Assert.AreEqual(expectedDevsCount, action.DeviceIndex.Count);
        }

        [Test]
        public void SetActionStrategy_NewActionMockStrategy_SetNewStrategy()
        {
            var action = new Action(string.Empty, null);
            var strategyMock = new Mock<IActionProcessorStrategy>();
            strategyMock.SetupProperty(x => x.Action);

            action.SetActionProcessingStrategy(strategyMock.Object);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(strategyMock.Object.GetHashCode(),
                    action.GetActionProcessingStrategy().GetHashCode());
                Assert.AreEqual(action, strategyMock.Object.Action);
            });
        }

        [Test]
        public void SetActionStrategy_NewActionNullStrategy_SetDefaultStrategy()
        {
            var action = new Action(string.Empty, null);

            action.SetActionProcessingStrategy(null);

            IActionProcessorStrategy strategy = action
                .GetActionProcessingStrategy();
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
            var action = new Action(string.Empty, null);

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
            var action = new Action(string.Empty, null);

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
                .Setup(x => x.GetDeviceByIndex(It.Is<int>(x => x % 2 == 0)))
                .Returns(validDevMock.Object);
            deviceManagerMock
                .Setup(x => x.GetDeviceByIndex(It.Is<int>(x => x % 2 != 0)))
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
                .Setup(x => x.GetDeviceByIndex(It.Is<int>(x => x % 2 == 0)))
                .Returns(validDevMock.Object);
            deviceManagerMock
                .Setup(x => x.GetDeviceByIndex(It.Is<int>(x => x % 2 != 0)))
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
            var action = new Action(string.Empty, null);
            action.DeviceIndex.AddRange(devIds);

            var cloned = action.Clone();

            int actionStrategyHashCode = action.GetActionProcessingStrategy()
                .GetHashCode();
            int clonedStrategyHashCode = cloned.GetActionProcessingStrategy()
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
            var action = new Action(string.Empty, null);

            Assert.AreEqual(DrawInfo.Style.GREEN_BOX, action.DrawStyle);
        }
    }

    class DefaultActionProcessingStrategyTest
    {
        [TestCaseSource(nameof(ProcessDevicesTestCaseSource))]
        public void ProcessDevices_DataFromTestCaseSource_ReturnsDevsIdsList()
        {
            //TODO: test methods
        }

        private object[] ProcessDevicesTestCaseSource()
        {
            return new object[0];
        }
    }

    class OneInManyOutActionProcessingStrategyTest
    {
        [TestCaseSource(nameof(ProcessDevicesTestCaseSource))]
        public void ProcessDevices_DataFromTestCaseSource_ReturnsDevsIdsList()
        {
            //TODO: test methods
        }

        private object[] ProcessDevicesTestCaseSource()
        {
            return new object[0];
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
            //STUB
            var stubDevMock = new Mock<Device.IDevice>();
            stubDevMock.SetupGet(x => x.DeviceType)
                .Returns(Device.DeviceType.NONE);
            stubDevMock.SetupGet(x => x.DeviceSubType)
                .Returns(Device.DeviceSubType.NONE);
            Device.IDevice stubDev = stubDevMock.Object;

            const string TANK = "TANK";

            //TANK1V1
            var dev1Mock = new Mock<Device.IDevice>();
            dev1Mock.SetupGet(x => x.ObjectName).Returns(TANK);
            dev1Mock.SetupGet(x => x.ObjectNumber).Returns(1);
            dev1Mock.SetupGet(x => x.DeviceType).Returns(Device.DeviceType.V);
            dev1Mock.SetupGet(x => x.DeviceNumber).Returns(1);
            dev1Mock.SetupGet(x => x.DeviceSubType)
                .Returns(Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice dev1 = dev1Mock.Object;

            //TANK1V2
            var dev2Mock = new Mock<Device.IDevice>();
            dev2Mock.SetupGet(x => x.ObjectName).Returns(TANK);
            dev2Mock.SetupGet(x => x.ObjectNumber).Returns(1);
            dev2Mock.SetupGet(x => x.DeviceType).Returns(Device.DeviceType.V);
            dev2Mock.SetupGet(x => x.DeviceNumber).Returns(2);
            dev2Mock.SetupGet(x => x.DeviceSubType)
                .Returns(Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice dev2 = dev2Mock.Object;

            //KOAG1V1
            var dev3Mock = new Mock<Device.IDevice>();
            dev3Mock.SetupGet(x => x.ObjectName).Returns("KOAG");
            dev3Mock.SetupGet(x => x.ObjectNumber).Returns(1);
            dev3Mock.SetupGet(x => x.DeviceType).Returns(Device.DeviceType.V);
            dev3Mock.SetupGet(x => x.DeviceNumber).Returns(1);
            dev3Mock.SetupGet(x => x.DeviceSubType)
                .Returns(Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice dev3 = dev3Mock.Object;

            //KOAG1M2
            var dev4Mock = new Mock<Device.IDevice>();
            dev4Mock.SetupGet(x => x.ObjectName).Returns("KOAG");
            dev4Mock.SetupGet(x => x.ObjectNumber).Returns(1);
            dev4Mock.SetupGet(x => x.DeviceType).Returns(Device.DeviceType.M);
            dev4Mock.SetupGet(x => x.DeviceNumber).Returns(2);
            dev4Mock.SetupGet(x => x.DeviceSubType)
                .Returns(Device.DeviceSubType.M_ATV);
            Device.IDevice dev4 = dev4Mock.Object;

            //TANK2V1
            var dev5Mock = new Mock<Device.IDevice>();
            dev5Mock.SetupGet(x => x.ObjectName).Returns(TANK);
            dev5Mock.SetupGet(x => x.ObjectNumber).Returns(2);
            dev5Mock.SetupGet(x => x.DeviceType).Returns(Device.DeviceType.V);
            dev5Mock.SetupGet(x => x.DeviceNumber).Returns(1);
            dev5Mock.SetupGet(x => x.DeviceSubType)
                .Returns(Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice dev5 = dev5Mock.Object;

            //TANK2V2
            var dev6Mock = new Mock<Device.IDevice>();
            dev6Mock.SetupGet(x => x.ObjectName).Returns(TANK);
            dev6Mock.SetupGet(x => x.ObjectNumber).Returns(2);
            dev6Mock.SetupGet(x => x.DeviceType).Returns(Device.DeviceType.V);
            dev6Mock.SetupGet(x => x.DeviceNumber).Returns(2);
            dev6Mock.SetupGet(x => x.DeviceSubType)
                .Returns(Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice dev6 = dev6Mock.Object;

            //TANK2V3
            var dev7Mock = new Mock<Device.IDevice>();
            dev7Mock.SetupGet(x => x.ObjectName).Returns(TANK);
            dev7Mock.SetupGet(x => x.ObjectNumber).Returns(2);
            dev7Mock.SetupGet(x => x.DeviceType).Returns(Device.DeviceType.V);
            dev7Mock.SetupGet(x => x.DeviceNumber).Returns(3);
            dev7Mock.SetupGet(x => x.DeviceSubType)
                .Returns(Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice dev7 = dev7Mock.Object;

            //TANK3V3
            var dev8Mock = new Mock<Device.IDevice>();
            dev8Mock.SetupGet(x => x.ObjectName).Returns(TANK);
            dev8Mock.SetupGet(x => x.ObjectNumber).Returns(3);
            dev8Mock.SetupGet(x => x.DeviceType).Returns(Device.DeviceType.V);
            dev8Mock.SetupGet(x => x.DeviceNumber).Returns(3);
            dev8Mock.SetupGet(x => x.DeviceSubType)
                .Returns(Device.DeviceSubType.V_IOLINK_MIXPROOF);
            Device.IDevice dev8 = dev8Mock.Object;

            //TANK1LS1
            var dev9Mock = new Mock<Device.IDevice>();
            dev9Mock.SetupGet(x => x.ObjectName).Returns(TANK);
            dev9Mock.SetupGet(x => x.ObjectNumber).Returns(1);
            dev9Mock.SetupGet(x => x.DeviceType).Returns(Device.DeviceType.LS);
            dev9Mock.SetupGet(x => x.DeviceNumber).Returns(1);
            dev9Mock.SetupGet(x => x.DeviceSubType)
                .Returns(Device.DeviceSubType.LS_IOLINK_MIN);
            Device.IDevice dev9 = dev9Mock.Object;

            //TANK1LS2
            var dev10Mock = new Mock<Device.IDevice>();
            dev10Mock.SetupGet(x => x.ObjectName).Returns(TANK);
            dev10Mock.SetupGet(x => x.ObjectNumber).Returns(2);
            dev10Mock.SetupGet(x => x.DeviceType).Returns(Device.DeviceType.LS);
            dev10Mock.SetupGet(x => x.DeviceNumber).Returns(2);
            dev10Mock.SetupGet(x => x.DeviceSubType)
                .Returns(Device.DeviceSubType.LS_IOLINK_MAX);
            Device.IDevice dev10 = dev10Mock.Object;

            //TANK1VC1
            var dev11Mock = new Mock<Device.IDevice>();
            dev11Mock.SetupGet(x => x.ObjectName).Returns(TANK);
            dev11Mock.SetupGet(x => x.ObjectNumber).Returns(3);
            dev11Mock.SetupGet(x => x.DeviceType).Returns(Device.DeviceType.VC);
            dev11Mock.SetupGet(x => x.DeviceNumber).Returns(1);
            dev11Mock.SetupGet(x => x.DeviceSubType)
                .Returns(Device.DeviceSubType.NONE);
            Device.IDevice dev11 = dev11Mock.Object;

            devManagerMock.Setup(x => x.GetDeviceByIndex(It.IsAny<int>()))
                .Returns(stubDev);
            devManagerMock.Setup(x => x.GetDeviceByIndex(1)).Returns(dev1);
            devManagerMock.Setup(x => x.GetDeviceByIndex(2)).Returns(dev2);
            devManagerMock.Setup(x => x.GetDeviceByIndex(3)).Returns(dev3);
            devManagerMock.Setup(x => x.GetDeviceByIndex(4)).Returns(dev4);
            devManagerMock.Setup(x => x.GetDeviceByIndex(5)).Returns(dev5);
            devManagerMock.Setup(x => x.GetDeviceByIndex(6)).Returns(dev6);
            devManagerMock.Setup(x => x.GetDeviceByIndex(7)).Returns(dev7);
            devManagerMock.Setup(x => x.GetDeviceByIndex(8)).Returns(dev8);
            devManagerMock.Setup(x => x.GetDeviceByIndex(9)).Returns(dev9);
            devManagerMock.Setup(x => x.GetDeviceByIndex(10)).Returns(dev10);
            devManagerMock.Setup(x => x.GetDeviceByIndex(11)).Returns(dev11);

            const string TANK1V1 = "TANK1V1";
            const string TANK1V2 = "TANK1V2";
            const string KOAG1V1 = "KOAG1V1";
            const string KOAG1M2 = "KOAG1M2";
            const string TANK2V1 = "TANK2V1";
            const string TANK2V2 = "TANK2V2";
            const string TANK2V3 = "TANK2V3";
            const string TANK3V3 = "TANK3V3";
            const string TANK1LS1 = "TANK1LS1";
            const string TANK2LS2 = "TANK2LS2";
            const string TANK3VC1 = "TANK3VC1";

            devManagerMock.Setup(x => x.GetDeviceIndex(It.IsAny<string>()))
                .Returns(-1);
            devManagerMock.Setup(x => x.GetDeviceIndex(TANK1V1)).Returns(1);
            devManagerMock.Setup(x => x.GetDeviceIndex(TANK1V2)).Returns(2);
            devManagerMock.Setup(x => x.GetDeviceIndex(KOAG1V1)).Returns(3);
            devManagerMock.Setup(x => x.GetDeviceIndex(KOAG1M2)).Returns(4);
            devManagerMock.Setup(x => x.GetDeviceIndex(TANK2V1)).Returns(5);
            devManagerMock.Setup(x => x.GetDeviceIndex(TANK2V2)).Returns(6);
            devManagerMock.Setup(x => x.GetDeviceIndex(TANK2V3)).Returns(7);
            devManagerMock.Setup(x => x.GetDeviceIndex(TANK3V3)).Returns(8);
            devManagerMock.Setup(x => x.GetDeviceIndex(TANK1LS1)).Returns(9);
            devManagerMock.Setup(x => x.GetDeviceIndex(TANK2LS2)).Returns(10);
            devManagerMock.Setup(x => x.GetDeviceIndex(TANK3VC1)).Returns(11);

            devManagerMock
                .Setup(x => x.GetDeviceByEplanName(It.IsAny<string>()))
                .Returns(stubDev);
            devManagerMock.Setup(x => x.GetDeviceByEplanName(TANK1V1))
                .Returns(dev1);
            devManagerMock.Setup(x => x.GetDeviceByEplanName(TANK1V2))
                .Returns(dev2);
            devManagerMock.Setup(x => x.GetDeviceByEplanName(KOAG1V1))
                .Returns(dev3);
            devManagerMock.Setup(x => x.GetDeviceByEplanName(KOAG1M2))
                .Returns(dev4);
            devManagerMock.Setup(x => x.GetDeviceByEplanName(TANK2V1))
                .Returns(dev5);
            devManagerMock.Setup(x => x.GetDeviceByEplanName(TANK2V2))
                .Returns(dev6);
            devManagerMock.Setup(x => x.GetDeviceByEplanName(TANK2V3))
                .Returns(dev7);
            devManagerMock.Setup(x => x.GetDeviceByEplanName(TANK3V3))
                .Returns(dev8);
            devManagerMock.Setup(x => x.GetDeviceByEplanName(TANK1LS1))
                .Returns(dev9);
            devManagerMock.Setup(x => x.GetDeviceByEplanName(TANK2LS2))
                .Returns(dev10);
            devManagerMock.Setup(x => x.GetDeviceByEplanName(TANK3VC1))
                .Returns(dev11);
        }

        public static Device.IDeviceManager DeviceManager
            => deviceManager;

        private static Device.IDeviceManager deviceManager;
    }
}
