using EasyEPlanner.PxcIolinkConfiguration.Models;
using Editor;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TechObject;
using TechObject.ActionProcessingStrategy;

namespace TechObjectTests
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
            action.DevicesIndex.Add(addingValue);

            Assert.AreEqual(expectedCountAfterAdd, action.DevicesIndex.Count);

            action.DevicesIndex.Remove(addingValue);

            Assert.AreEqual(expectedCountAfterDel, action.DevicesIndex.Count);
        }

        [Test]
        public void Clear_ActionWithDevicesId_CleadDeviceIndex()
        {
            const int expectedDevsCount = 0;
            var newDevs = new List<int> { 8, 6, 4, 3, 2, 9 };
            var action = new Action(string.Empty, null, string.Empty);
            action.DevicesIndex = newDevs;

            action.Clear();

            Assert.AreEqual(expectedDevsCount, action.DevicesIndex.Count);
        }

        [Test]
        public void ActionProcessorStrategy_NewActionMockStrategy_SetNewStrategy()
        {
            var action = new Action(string.Empty, null, string.Empty);
            var strategyMock = new Mock<IDeviceProcessingStrategy>();
            strategyMock.SetupProperty(x => x.Action);

            action.SetDeviceProcessingStrategy(strategyMock.Object);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(strategyMock.Object.GetHashCode(),
                    action.GetDeviceProcessingStrategy().GetHashCode());
                Assert.AreEqual(action, strategyMock.Object.Action);
            });
        }

        [Test]
        public void ActionProcessorStrategy_NewActionNullStrategy_SetDefaultStrategy()
        {
            var action = new Action(string.Empty, null, string.Empty);

            action.SetDeviceProcessingStrategy(null);

            IDeviceProcessingStrategy strategy = action
                .GetDeviceProcessingStrategy();
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
                action.DevicesIndex.Add(devId);
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
                action.DevicesIndex.Add(devId);
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
            var deviceManagerMock = new Mock<EplanDevice.IDeviceManager>();
            deviceManagerMock.Setup(x => x.GetDeviceByIndex(It.IsAny<int>()))
                .Returns(new Mock<EplanDevice.IDevice>().Object);

            typeof(Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManagerMock.Object);

            var action = new Action(string.Empty, null, string.Empty, null,
                null, null);
            action.DrawStyle = drawStyle;
            action.DevicesIndex.AddRange(devIds);

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
            EplanDevice.DeviceType[] expectedTypes,
            EplanDevice.DeviceSubType[] expectedDeviceSubTypes)
        {
            var action = new Action(string.Empty, null, string.Empty,
                expectedTypes, expectedDeviceSubTypes);

            action.GetDisplayObjects(out EplanDevice.DeviceType[] actualDevTypes,
                out EplanDevice.DeviceSubType[] actualDevSubTypes,
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
                    new EplanDevice.DeviceType[]
                    {
                        EplanDevice.DeviceType.DI,
                        EplanDevice.DeviceType.DO
                    },
                    new EplanDevice.DeviceSubType[]
                    {
                        EplanDevice.DeviceSubType.DI,
                        EplanDevice.DeviceSubType.DI_VIRT,
                        EplanDevice.DeviceSubType.DO,
                        EplanDevice.DeviceSubType.DO_VIRT,
                    }
                },
                new object[]
                {
                    new EplanDevice.DeviceType[]
                    {
                        EplanDevice.DeviceType.V,
                        EplanDevice.DeviceType.VC
                    },
                    null
                },
                new object[]
                {
                    null,
                    new EplanDevice.DeviceSubType[]
                    {
                        EplanDevice.DeviceSubType.NONE
                    }
                },
            };
        }

        [TestCase(new int[] { 2, 5, 3 })]
        [TestCase(new int[0])]
        public void EditText_NewAction_ReturnsCorrectEditTextArr(int[] devIds)
        {
            const string devName = "Name";
            var deviceMock = new Mock<EplanDevice.IDevice>();
            deviceMock.SetupGet(x => x.Name)
            .Returns(devName);
            var deviceManagerMock = new Mock<EplanDevice.IDeviceManager>();
            deviceManagerMock.Setup(x => x.GetDeviceByIndex(It.IsAny<int>()))
                .Returns(deviceMock.Object);
            typeof(Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManagerMock.Object);

            var action = new Action(string.Empty, null, string.Empty, null,
                null, null);
            action.DevicesIndex.AddRange(devIds);
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
            var deviceMock = new Mock<EplanDevice.IDevice>();
            deviceMock.SetupGet(x => x.Name)
            .Returns(devName);
            var deviceManagerMock = new Mock<EplanDevice.IDeviceManager>();
            deviceManagerMock.Setup(x => x.GetDeviceByIndex(It.IsAny<int>()))
                .Returns(deviceMock.Object);
            typeof(Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManagerMock.Object);

            var action = new Action(string.Empty, null, string.Empty, null,
                null, null);
            action.DevicesIndex.AddRange(devIds);
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
            var strategyMock = new Mock<IDeviceProcessingStrategy>();
            strategyMock.Setup(x => x.ProcessDevices(It.IsAny<string>(),
                It.IsAny<EplanDevice.IDeviceManager>()))
                .Returns(Enumerable.Range(1, expectedDevsCount).ToList());

            var action = new Action(string.Empty, null, string.Empty,
                null, null, strategyMock.Object);

            bool result = action.SetNewValue(newDevs);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedResult, result);
                Assert.AreEqual(expectedDevsCount, action.DevicesIndex.Count);
            });
        }

        [TestCase(new int[0], 0)]
        [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4)]
        [Description("If devId mod 2 == 0 -> Valid device, else - invalid (skip)")]
        public void AddDev_NewAction_AddOrSkipDev(int[] devIds,
            int expectedDevCount)
        {
            var validDevMock = new Mock<EplanDevice.IDevice>();
            validDevMock.SetupGet(x => x.Description).Returns("Name");
            var invalidDevMock = new Mock<EplanDevice.IDevice>();
            invalidDevMock.SetupGet(x => x.Description)
                .Returns(StaticHelper.CommonConst.Cap);
            var deviceManagerMock = new Mock<EplanDevice.IDeviceManager>();
            deviceManagerMock
                .Setup(x => x.GetDeviceByIndex(It.Is<int>(y => y % 2 == 0)))
                .Returns(validDevMock.Object);
            deviceManagerMock
                .Setup(x => x.GetDeviceByIndex(It.Is<int>(y => y % 2 != 0)))
                .Returns(invalidDevMock.Object);
            typeof(Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManagerMock.Object);


            var action = new Action(string.Empty, null, string.Empty, null,
                null, null);

            foreach (var devId in devIds)
            {
                // Another arguments were skipped because they are useless.
                action.AddDev(devId, 0, string.Empty);
            }

            Assert.AreEqual(expectedDevCount, action.DevicesIndex.Count);
        }

        [Description("If devId mod 2 == 0 -> Valid device," +
            "else - device name returns cap")]
        [TestCaseSource(nameof(SaveAsLuaTableTestCaseSource))]
        public void SaveAsLuaTable_NewAction_ReturnsCodeTextToSave(
            string actionName, string devNameInMock, string prefix,
            int[] devIds, string luaName, string expectedCode)
        {
            var validDevMock = new Mock<EplanDevice.IDevice>();
            validDevMock.SetupGet(x => x.Name).Returns(devNameInMock);
            var invalidDevMock = new Mock<EplanDevice.IDevice>();
            invalidDevMock.SetupGet(x => x.Name)
                .Returns(StaticHelper.CommonConst.Cap);
            var deviceManagerMock = new Mock<EplanDevice.IDeviceManager>();
            deviceManagerMock
                .Setup(x => x.GetDeviceByIndex(It.Is<int>(y => y % 2 == 0)))
                .Returns(validDevMock.Object);
            deviceManagerMock
                .Setup(x => x.GetDeviceByIndex(It.Is<int>(y => y % 2 != 0)))
                .Returns(invalidDevMock.Object);
            typeof(Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManagerMock.Object);

            var action = new Action(actionName, null, luaName, null, null,
                null);
            action.DevicesIndex.AddRange(devIds);

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
                $"{prefix} --{name}\n{prefix}\t{{\n" +
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
            EplanDevice.IDeviceManager deviceManager = DeviceManagerMock.DeviceManager;
            typeof(Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManager);

            var action = new Action(string.Empty, null, string.Empty, null,
                null, null);
            action.DevicesIndex.AddRange(devIds);

            action.ModifyDevNames(newTechObjectName, newTechObjectNumber,
                oldTechObjectName, oldTechObjNumber);

            Assert.AreEqual(expectedDevIds, action.DevicesIndex);
        }

        [TestCase(new int[] { 1, 2, 3, 4 }, new int[] { 5, 6, 3, 4 }, 2, 1, "TANK")]
        [TestCase(new int[] { 5, 4, 6, 3 }, new int[] { 1, 4, 2, 3 }, 2, 1, "TANK")]
        [TestCase(new int[] { 1, 2, 3, 4, 8 }, new int[] { 5, 6, 3, 4, 7 }, 2, -1, "TANK")]
        [TestCase(new int[] { 8 }, new int[] { 8 }, 2, 1, "TANK")]
        public void ModifyDevNames(int[] devIds, int[] expectedDevIds,
            int newTechObjectN, int oldTechObjectN, string techObjectName)
        {
            EplanDevice.IDeviceManager deviceManager = DeviceManagerMock.DeviceManager;
            typeof(Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManager);

            var action = new Action(string.Empty, null, string.Empty, null,
                null, null);
            action.DevicesIndex.AddRange(devIds);

            action.ModifyDevNames(newTechObjectN, oldTechObjectN,
                techObjectName);

            Assert.AreEqual(expectedDevIds, action.DevicesIndex);
        }

        [TestCase(new int[] { 1, 2, 3, 4 }, new int[] { 5, 6, 3, 4 }, 2, "TANK")]
        [TestCase(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 }, 3, "TANK")]
        public void ModifyDevNames_CheckGenericUpdating(int[] devIds,
            int[] expectedDevIds, int newObjID, string techObjectName)
        {
            EplanDevice.IDeviceManager deviceManager = DeviceManagerMock.DeviceManager;
            typeof(Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManager);

            var action = new Action(string.Empty, null, string.Empty, null,
                null, null);
            action.DevicesIndex.AddRange(devIds);

            action.ModifyDevNames(newObjID, -1, techObjectName);

            CollectionAssert.AreEqual(expectedDevIds, action.DevicesIndex);
        }

        [TestCase(new int[] { 1, 3, 5, 7, 9 })]
        [TestCase(new int[0])]
        public void Clone_NewAction_ReturnsCopy(int[] devIds)
        {
            var action = new Action(string.Empty, null, string.Empty);
            action.DevicesIndex.AddRange(devIds);

            int actionStrategyHashCode = action.GetDeviceProcessingStrategy()
                .GetHashCode();

            var cloned = action.Clone();
            int clonedStrategyHashCode = cloned.GetDeviceProcessingStrategy()
                .GetHashCode();

            Assert.Multiple(() =>
            {
                Assert.AreNotEqual(action.GetHashCode(), cloned.GetHashCode());
                Assert.AreEqual(action.DevicesIndex.Count,
                    cloned.DevicesIndex.Count);
                Assert.AreEqual(actionStrategyHashCode,
                    clonedStrategyHashCode);
                for (int i = 0; i < action.DevicesIndex.Count; i++)
                {
                    Assert.AreEqual(action.DevicesIndex[i],
                        cloned.DevicesIndex[i]);
                }
            });
        }

        [Test]
        public void Constructor_NewAction_CheckDefaultDrawStyle()
        {
            var action = new Action(string.Empty, null, string.Empty);

            Assert.AreEqual(DrawInfo.Style.GREEN_BOX, action.DrawStyle);
        }

        [TestCaseSource(nameof(DeviceNamesTestCaseSource))]
        public void DeviceNames_ActionWithDevs_ReturnsCorrectNamesList(
            List<string> expectedNames, List<int> devIdsToSet)
        {
            EplanDevice.IDeviceManager deviceManager = DeviceManagerMock.DeviceManager;
            typeof(Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManager);

            var action = new Action(string.Empty, null, string.Empty, null,
                null, null);
            action.DevicesIndex.AddRange(devIdsToSet);

            List<string> actualNames = action.DevicesNames;

            Assert.AreEqual(expectedNames, actualNames);
        }

        private static object[] DeviceNamesTestCaseSource()
        {
            var randomFirstCase = new object[]
            {
                new List<string>()
                { 
                    "KOAG1V1", 
                    "KOAG1M2", 
                    "TANK2V1", 
                    "TANK1LS1" 
                },
                new List<int>() { 3, 4, 5, 9 }
            };

            var randomSecondCase = new object[]
            {
                new List<string>()
                {
                    "TANK1V2",
                    "TANK2V2",
                    "TANK2V3",
                    "TANK3V3",
                    "TANK1LS1",
                    "TANK1HL1"
                },
                new List<int>() { 2, 6, 7, 8, 9, 20 }
            };

            return new object[]
            {
                randomFirstCase,
                randomSecondCase
            };
        }

        [Test]
        public void HasSubActions_NewAction_ReturnsFalse()
        {
            var action = new Action(string.Empty, null, string.Empty);

            bool actualHasSubActions = action.HasSubActions;

            Assert.IsFalse(actualHasSubActions);
        }

        [Test]
        public void SubActions_NewAction_ReturnsNull()
        {
            var action = new Action(string.Empty, null, string.Empty);

            List<IAction> actualSubActions = action.SubActions;

            Assert.IsNull(actualSubActions);
        }

        [Test]
        public void SetGenericDevices()
        {
            var deviceManager = DeviceManagerMock.DeviceManager;
            typeof(Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManager);

            var action = new Action("Устройства", null, "devs", null, null, null);

            Assert.Multiple(() =>
            {
                action.SetNewValue("TANK1V1 TANK1V2 KOAG1V1");
                CollectionAssert.AreEqual(new List<int>() { 1, 2, 3 },
                    action.DevicesIndex);
                CollectionAssert.AreEqual(new List<int>() { },
                    action.GenericDevicesIndexAfterExclude);

                action.SetGenericDevices(new List<int>() { 2, 3 });
                CollectionAssert.AreEqual(new List<int>() { 1, },
                    action.DevicesIndex);
                CollectionAssert.AreEqual(new List<int>() { 2, 3 },
                    action.GenericDevicesIndexAfterExclude);

                action.SetNewValue("TANK1V1 TANK1V2");
                CollectionAssert.AreEqual(new List<int>() { 1, },
                    action.DevicesIndex);
                CollectionAssert.AreEqual(new List<int>() { 2, },
                    action.GenericDevicesIndexAfterExclude);
            });            
        }

        [Test]
        public void InsertCopy()
        {
            var deviceManager = DeviceManagerMock.DeviceManager;
            typeof(Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManager);

            var action1 = new Action("Устройства", null, "devs", null, null, null);
            var action2 = new Action("Устройства", null, "_devs_", null, null, null);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(action2.IsInsertableCopy);

                ITreeViewItem result = null;
                action1.SetNewValue("TANK1V1 TANK1V2 KOAG1V1");
                result = action2.InsertCopy(action1);

                Assert.AreSame(action2, result);
                CollectionAssert.AreEqual(new List<int>() { 1, 2, 3 }, action2.DevicesIndex);

                result = action2.InsertCopy(0);
                Assert.IsNull(result);
            });
        }

        [Test]
        public void InsertCopy_GroupableAction()
        {
            var deviceManager = DeviceManagerMock.DeviceManager;
            typeof(Action).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManager);

            var step1 = new Step("Шаг1", GetN => 1, null);
            var step2 = new Step("Шаг2", GetN => 2, null);

            var techObject = new TechObject.TechObject("", GetN => 1, 1, 2, "", -1, "", "", null);
            techObject.GetParamsManager().Float.Insert();
            techObject.GetParamsManager().Float.Insert();
            step1.AddParent(techObject);
            step2.AddParent(techObject);

            var action1 = step1.Items[2] as GroupableAction;
            var action2 = step2.Items[2] as GroupableAction;

            action1.AddParent(step1);
            action2.AddParent(step2);

            (action1.SubActions[0].SubActions[0] as Action)?.SetNewValue("TANK1V1 TANK1V2 KOAG1V1");
            (action1.SubActions[0] as GroupableAction).Parameters[0].SetNewValue("2");

            Assert.Multiple(() =>
            {
                ITreeViewItem result = null;

                result = action2.InsertCopy(action1);

                Assert.AreNotSame(action2, result);
                CollectionAssert.AreEqual(new List<int>() { 1, 2, 3 }, 
                    (result as IAction).SubActions[0].SubActions[0].DevicesIndex);
                Assert.AreEqual("2", ((result as IAction).SubActions[0] as GroupableAction).Parameters[0].Value);

                result = action1.InsertCopy(action1.SubActions[0]);

                CollectionAssert.AreEqual(new List<int>() { 1, 2, 3 },
                    (result as IAction).SubActions[0].DevicesIndex);
                Assert.AreEqual("2", (result as GroupableAction).Parameters[0].Value);

                result = action2.InsertCopy(0);
                Assert.IsNull(result);
            });
        }
    }

    class DefaultActionProcessingStrategyTest
    {
        [TestCaseSource(nameof(ProcessDevicesTestCaseSource))]
        public void ProcessDevices_DataFromTestCaseSource_ReturnsDevsIdsList(
            string devicesStr, EplanDevice.DeviceType[] allowedDevTypes,
            EplanDevice.DeviceSubType[] allowedDevSubTypes,
            IList<int> expectedDevsIds)
        {
            IAction action = ActionMock.GetAction(allowedDevTypes,
                allowedDevSubTypes, new List<int>());
            var strategy = new DefaultActionProcessorStrategy();
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
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.V
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF,
                },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK1V2,
                    (int)DeviceManagerMock.Devices.TANK2V3,
                    (int)DeviceManagerMock.Devices.KOAG1V1
                }
            };

            var allowValeMixproofIOLinkAndGetEmptyList = new object[]
            {
                "KOAG1M2 TANK1LS1 TANK2LS2 TANK3VC1",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.V
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF,
                },
                new List<int> { }
            };

            var allowVCAndLSIOLinkMin = new object[]
            {
                "TANK1V1 TANK3VC1 KOAG1M2 TANK2LS2 TANK1LS1",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.VC,
                    EplanDevice.DeviceType.LS
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.LS_IOLINK_MIN,
                    EplanDevice.DeviceSubType.NONE
                },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK3VC1,
                    (int)DeviceManagerMock.Devices.TANK1LS1
                }
            };

            var discardVCWhenAllowedInDevType = new object[]
            {
                "TANK3VC1",
                new EplanDevice.DeviceType[] { EplanDevice.DeviceType.VC },
                new EplanDevice.DeviceSubType[] { },
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
            string devicesStr, EplanDevice.DeviceType[] allowedDevTypes,
            EplanDevice.DeviceSubType[] allowedDevSubTypes,
            List<int> actionDevsDefaultIds, IList<int> expectedDevsIds,
            EplanDevice.DeviceType[] allowedInputTypes)
        {
            IAction action = ActionMock.GetAction(allowedDevTypes,
                allowedDevSubTypes, actionDevsDefaultIds);
            var strategy =
                new OneInManyOutActionProcessingStrategy(allowedInputTypes);
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
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.DI,
                    EplanDevice.DeviceType.DO
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.DI,
                    EplanDevice.DeviceSubType.DI_VIRT,
                    EplanDevice.DeviceSubType.DO,
                    EplanDevice.DeviceSubType.DO_VIRT
                },
                new List<int> { (int)DeviceManagerMock.Devices.TANK1DO1 },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK2DI2,
                    (int)DeviceManagerMock.Devices.TANK1DO1,
                    (int)DeviceManagerMock.Devices.TANK1DO2
                },
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.DI
                }
            };

            var correctSequenceAIAO = new object[]
            {
                "TANK1AO1 TANK1AO2 TANK2AI2",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.AI,
                    EplanDevice.DeviceType.AO
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.AI,
                    EplanDevice.DeviceSubType.AI_VIRT,
                    EplanDevice.DeviceSubType.AO,
                    EplanDevice.DeviceSubType.AO_VIRT
                },
                new List<int> { },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK2AI2,
                    (int)DeviceManagerMock.Devices.TANK1AO1,
                    (int)DeviceManagerMock.Devices.TANK1AO2
                },
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.AI
                }
            };

            var replacingAIAOCase = new object[]
            {
                "TANK1AO1 TANK2AI2 TANK1AI1",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.AI,
                    EplanDevice.DeviceType.AO
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.AI,
                    EplanDevice.DeviceSubType.AI_VIRT,
                    EplanDevice.DeviceSubType.AO,
                    EplanDevice.DeviceSubType.AO_VIRT
                },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK2AI2,
                    (int)DeviceManagerMock.Devices.TANK1AO1,
                },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK1AI1,
                    (int)DeviceManagerMock.Devices.TANK1AO1

                },
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.AI
                }
            };

            var replacingDIDOCase = new object[]
            {
                "TANK1DO1 TANK2DI2 TANK1DI1",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.DI,
                    EplanDevice.DeviceType.DO
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.DI,
                    EplanDevice.DeviceSubType.DI_VIRT,
                    EplanDevice.DeviceSubType.DO,
                    EplanDevice.DeviceSubType.DO_VIRT
                },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK2DI2,
                    (int)DeviceManagerMock.Devices.TANK1DO1,
                },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK1DI1,
                    (int)DeviceManagerMock.Devices.TANK1DO1,
                },
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.DI
                }
            };


            var useHLInSequence = new object[]
            {
                "TANK1DO1 TANK1HL1 TANK2DI2",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.DI,
                    EplanDevice.DeviceType.DO,
                    EplanDevice.DeviceType.HL,
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.DI,
                    EplanDevice.DeviceSubType.DI_VIRT,
                    EplanDevice.DeviceSubType.DO,
                    EplanDevice.DeviceSubType.DO_VIRT,
                    EplanDevice.DeviceSubType.NONE,
                },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK2DI2,
                    (int)DeviceManagerMock.Devices.TANK1DO1,
                },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK2DI2,
                    (int)DeviceManagerMock.Devices.TANK1DO1,
                    (int)DeviceManagerMock.Devices.TANK1HL1
                },
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.DI
                }
            };

            var useHLAndReplaceDIwithGS = new object[]
            {
                "TANK1DO1 TANK1HL1 TANK1GS1 TANK2DI2",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.DI,
                    EplanDevice.DeviceType.DO,
                    EplanDevice.DeviceType.HL,
                    EplanDevice.DeviceType.GS,
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.DI,
                    EplanDevice.DeviceSubType.DI_VIRT,
                    EplanDevice.DeviceSubType.DO,
                    EplanDevice.DeviceSubType.DO_VIRT,
                    EplanDevice.DeviceSubType.NONE,
                },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK2DI2,
                    (int)DeviceManagerMock.Devices.TANK1DO1,
                },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK1GS1,
                    (int)DeviceManagerMock.Devices.TANK1DO1,
                    (int)DeviceManagerMock.Devices.TANK1HL1
                },
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.DI,
                    EplanDevice.DeviceType.GS
                }
            };

            var removeAllInputDevsIfDoubleInputDevs = new object[]
            {
                "TANK1DO1 TANK1HL1 TANK1GS2 TANK2DI2",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.DI,
                    EplanDevice.DeviceType.DO,
                    EplanDevice.DeviceType.HL,
                    EplanDevice.DeviceType.GS,
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.DI,
                    EplanDevice.DeviceSubType.DI_VIRT,
                    EplanDevice.DeviceSubType.DO,
                    EplanDevice.DeviceSubType.DO_VIRT,
                    EplanDevice.DeviceSubType.NONE,
                },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK1DO1,
                },
                new List<int>
                {
                    (int)DeviceManagerMock.Devices.TANK1DO1,
                    (int)DeviceManagerMock.Devices.TANK1HL1
                },
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.DI,
                    EplanDevice.DeviceType.GS
                }
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

    static class ActionMock
    {
        public static IAction GetAction(EplanDevice.DeviceType[] allowedDevTypes,
            EplanDevice.DeviceSubType[] allowedDevSubTypes,
            List<int> expectedDevsIds)
        {
            bool displayParameters = false;
            var actionMock = new Mock<IAction>();
            actionMock.Setup(x => x.GetDisplayObjects(out allowedDevTypes,
                out allowedDevSubTypes, out displayParameters));
            actionMock.Setup(x => x.DevicesIndex).Returns(expectedDevsIds);

            return actionMock.Object;
        }
    }

    static class DeviceManagerMock
    {
        static DeviceManagerMock()
        {
            var mock = new Mock<EplanDevice.IDeviceManager>();
            SetUpMock(mock);
            deviceManager = mock.Object;
        }

        private static void SetUpMock(Mock<EplanDevice.IDeviceManager>
            devManagerMock)
        {
            EplanDevice.IDevice stubDev = MakeMockedDevice(string.Empty, 0,
                EplanDevice.DeviceType.NONE, EplanDevice.DeviceSubType.NONE, 0,
                string.Empty);

            const string TANK = "TANK";
            const string KOAG = "KOAG";

            EplanDevice.IDevice tank1V1Dev = MakeMockedDevice(TANK, 1,
                EplanDevice.DeviceType.V, EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF, 1,
                $"{TANK}1V1");
            EplanDevice.IDevice tank1V2Dev = MakeMockedDevice(TANK, 1,
                EplanDevice.DeviceType.V, EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF, 2,
                $"{TANK}1V2");
            EplanDevice.IDevice koag1V1Dev = MakeMockedDevice(KOAG, 1,
                EplanDevice.DeviceType.V, EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF, 1,
                $"{KOAG}1V1");
            EplanDevice.IDevice koag1M2Dev = MakeMockedDevice(KOAG, 1,
                EplanDevice.DeviceType.M, EplanDevice.DeviceSubType.M_ATV, 2,
                $"{KOAG}1M2");
            EplanDevice.IDevice tank2V1Dev = MakeMockedDevice(TANK, 2,
                EplanDevice.DeviceType.V, EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF, 1,
                $"{TANK}2V1");
            EplanDevice.IDevice tank2V2Dev = MakeMockedDevice(TANK, 2,
                EplanDevice.DeviceType.V, EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF, 2,
                $"{TANK}2V2");
            EplanDevice.IDevice tank2V3Dev = MakeMockedDevice(TANK, 2,
                EplanDevice.DeviceType.V, EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF, 3,
                $"{TANK}2V3");
            EplanDevice.IDevice tank3V3Dev = MakeMockedDevice(TANK, 3,
                EplanDevice.DeviceType.V, EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF, 3,
                $"{TANK}3V3");
            EplanDevice.IDevice tank1LS1Dev = MakeMockedDevice(TANK, 1,
                EplanDevice.DeviceType.LS, EplanDevice.DeviceSubType.LS_IOLINK_MIN, 1,
                $"{TANK}1LS1");
            EplanDevice.IDevice tank2LS2Dev = MakeMockedDevice(TANK, 2,
                EplanDevice.DeviceType.LS, EplanDevice.DeviceSubType.LS_IOLINK_MAX, 2,
                $"{TANK}1LS2");
            EplanDevice.IDevice tank3VC1Dev = MakeMockedDevice(TANK, 3,
                EplanDevice.DeviceType.VC, EplanDevice.DeviceSubType.NONE, 1,
                $"{TANK}3VC1");
            EplanDevice.IDevice tank1DI1Dev = MakeMockedDevice(TANK, 1,
                EplanDevice.DeviceType.DI, EplanDevice.DeviceSubType.DI, 1,
                $"{TANK}1DI1");
            EplanDevice.IDevice tank2DI2Dev = MakeMockedDevice(TANK, 2,
                EplanDevice.DeviceType.DI, EplanDevice.DeviceSubType.DI_VIRT, 2,
                $"{TANK}2DI2");
            EplanDevice.IDevice tank1DO1Dev = MakeMockedDevice(TANK, 1,
                EplanDevice.DeviceType.DO, EplanDevice.DeviceSubType.DO, 1,
                $"{TANK}1DO1");
            EplanDevice.IDevice tank1DO2Dev = MakeMockedDevice(TANK, 1,
                EplanDevice.DeviceType.DO, EplanDevice.DeviceSubType.DO_VIRT, 2,
                $"{TANK}1DO2");
            EplanDevice.IDevice tank1AI1Dev = MakeMockedDevice(TANK, 1,
                EplanDevice.DeviceType.AI, EplanDevice.DeviceSubType.AI, 1,
                $"{TANK}1AI1");
            EplanDevice.IDevice tank2AI2Dev = MakeMockedDevice(TANK, 2,
                EplanDevice.DeviceType.AI, EplanDevice.DeviceSubType.AI_VIRT, 2,
                $"{TANK}2AI2");
            EplanDevice.IDevice tank1AO1Dev = MakeMockedDevice(TANK, 1,
                EplanDevice.DeviceType.AO, EplanDevice.DeviceSubType.AO, 1,
                $"{TANK}1AO1");
            EplanDevice.IDevice tank1AO2Dev = MakeMockedDevice(TANK, 1,
                EplanDevice.DeviceType.AO, EplanDevice.DeviceSubType.AO_VIRT, 2,
                $"{TANK}1AO2");
            EplanDevice.IDevice tank1HL1Dev = MakeMockedDevice(TANK, 1,
                EplanDevice.DeviceType.HL, EplanDevice.DeviceSubType.NONE, 1,
                $"{TANK}1HL1");
            EplanDevice.IDevice tank1HL2Dev = MakeMockedDevice(TANK, 1,
                EplanDevice.DeviceType.HL, EplanDevice.DeviceSubType.NONE, 2,
                $"{TANK}1HL2");
            EplanDevice.IDevice tank1GS1Dev = MakeMockedDevice(TANK, 1,
                EplanDevice.DeviceType.GS, EplanDevice.DeviceSubType.NONE, 1,
                $"{TANK}1GS1");
            EplanDevice.IDevice tank1GS2Dev = MakeMockedDevice(TANK, 1,
                EplanDevice.DeviceType.GS, EplanDevice.DeviceSubType.NONE, 2,
                $"{TANK}1GS2");

            var devicesDescription = new[]
            {
                new { Id = 1, Dev = tank1V1Dev },
                new { Id = 2, Dev = tank1V2Dev },
                new { Id = 3, Dev = koag1V1Dev },
                new { Id = 4, Dev = koag1M2Dev },
                new { Id = 5, Dev = tank2V1Dev },
                new { Id = 6, Dev = tank2V2Dev },
                new { Id = 7, Dev = tank2V3Dev },
                new { Id = 8, Dev = tank3V3Dev },
                new { Id = 9, Dev = tank1LS1Dev },
                new { Id = 10, Dev = tank2LS2Dev },
                new { Id = 11, Dev = tank3VC1Dev },
                new { Id = 12, Dev = tank1DI1Dev },
                new { Id = 13, Dev = tank2DI2Dev },
                new { Id = 14, Dev = tank1DO1Dev },
                new { Id = 15, Dev = tank1DO2Dev },
                new { Id = 16, Dev = tank1AI1Dev },
                new { Id = 17, Dev = tank2AI2Dev },
                new { Id = 18, Dev = tank1AO1Dev },
                new { Id = 19, Dev = tank1AO2Dev },
                new { Id = 20, Dev = tank1HL1Dev },
                new { Id = 21, Dev = tank1HL2Dev },
                new { Id = 22, Dev = tank1GS1Dev },
                new { Id = 23, Dev = tank1GS2Dev },
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
                devManagerMock.Setup(x => x.GetDeviceIndex(devDescr.Dev.Name))
                    .Returns(devDescr.Id);
                devManagerMock.Setup(x => x.GetDeviceByEplanName(
                    devDescr.Dev.Name)).Returns(devDescr.Dev);
            }
        }

        public enum Devices
        {
            TANK1V1 = 1,
            TANK1V2 = 2,
            KOAG1V1 = 3,
            KOAG1M2 = 4,
            TANK2V1 = 5,
            TANK2V2 = 6,
            TANK2V3 = 7,
            TANK3V3 = 8,
            TANK1LS1 = 9,
            TANK2LS2 = 10,
            TANK3VC1 = 11,
            TANK1DI1 = 12,
            TANK2DI2 = 13,
            TANK1DO1 = 14,
            TANK1DO2 = 15,
            TANK1AI1 = 16,
            TANK2AI2 = 17,
            TANK1AO1 = 18,
            TANK1AO2 = 19,
            TANK1HL1 = 20,
            TANK1HL2 = 21,
            TANK1GS1 = 22,
            TANK1GS2 = 23,
        }

        private static EplanDevice.IDevice MakeMockedDevice(string objName,
            int objNum, EplanDevice.DeviceType devType,
            EplanDevice.DeviceSubType deviceSubType, int devNumber, string devName)
        {
            var devMock = new Mock<EplanDevice.IDevice>();
            devMock.SetupGet(x => x.DeviceDesignation).Returns($"{devType}{devNumber}");
            devMock.SetupGet(x => x.ObjectName).Returns(objName);
            devMock.SetupGet(x => x.ObjectNumber).Returns(objNum);
            devMock.SetupGet(x => x.DeviceType).Returns(devType);
            devMock.SetupGet(x => x.DeviceNumber).Returns(devNumber);
            devMock.SetupGet(x => x.DeviceSubType).Returns(deviceSubType);
            devMock.SetupGet(x => x.Name).Returns(devName);
            return devMock.Object;
        }

        public static EplanDevice.IDeviceManager DeviceManager
            => deviceManager;

        private static EplanDevice.IDeviceManager deviceManager;
    }

}
