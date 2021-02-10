using NUnit.Framework;
using System.Collections.Generic;
using TechObject;
using Moq;

namespace EasyEplanner.Tests
{
    public class BaseParameterTest
    {
        /// <summary>
        /// Реализация абстрактного класса BaseParameter для его тестирования
        /// </summary>
        class BaseParameterImplementation : BaseParameter
        {
            public BaseParameterImplementation(string luaName, string name,
                string defaultValue = "",
                List<DisplayObject> displayObjects = null,
                Device.IDeviceManager deviceManager = null) : base(luaName,
                    name, defaultValue, displayObjects, deviceManager) { }

            public override BaseParameter Clone()
            {
                return new BaseParameterImplementation(LuaName, Name,
                    DefaultValue, DisplayObjects, deviceManager);
            }
        }

        [Test]
        public void DisplayObjectsFromNewObj_EmptyObject_ReturnsNoDisplayObjects()
        {
            var nonDisplayObjects = new List<BaseParameter.DisplayObject>
            {
                BaseParameter.DisplayObject.None
            };

            var newObj = new BaseParameterImplementation(stub, stub);

            Assert.AreEqual(nonDisplayObjects, newObj.DisplayObjects);
        }

        [Test]
        public void DisplayObjectsFromNewObj_EmptyObject_ReturnsAllDisplayObjects()
        {
            var allDisplayObjects = new List<BaseParameter.DisplayObject>
            {
                BaseParameter.DisplayObject.Signals,
                BaseParameter.DisplayObject.Parameters,
            };

            var newObj = new BaseParameterImplementation(stub, stub, stub,
                allDisplayObjects);

            Assert.AreEqual(allDisplayObjects, newObj.DisplayObjects);
        }

        [Test]
        public void CreateObject_NoObject_ReturnsObject()
        {
            var obj = new BaseParameterImplementation(stub, stub);
            string[] expectedEditText = new string[] { obj.LuaName, obj.Value };

            Assert.Multiple(() =>
            {
                Assert.AreEqual(obj.LuaName, stub);
                Assert.AreEqual(obj.Name, stub);
                Assert.IsTrue(obj.IsUseDevList);
                Assert.IsFalse(obj.IsEmpty);
                Assert.IsNull(obj.Owner);
                Assert.AreEqual(expectedEditText, obj.EditText);
                Assert.AreEqual(obj.CurrentValueType,
                    BaseParameter.ValueType.None);
            });
        }

        [Test]
        public void Clone_EmptyObject_ReturnsTheSameButWithAnotherHash()
        {
            string name = "Name";
            string luaName = "LuaName";
            string defaultValue = "defaultValue";
            var displayObjects = new List<BaseParameter.DisplayObject>
            {
                BaseParameter.DisplayObject.Parameters
            };
            var obj = new BaseParameterImplementation(luaName, name,
                defaultValue, displayObjects);

            var cloned = obj.Clone();

            Assert.Multiple(() =>
            {
                Assert.AreNotEqual(cloned.GetHashCode(), obj.GetHashCode());
                Assert.AreEqual(cloned.Name, obj.Name);
                Assert.AreEqual(cloned.LuaName, obj.LuaName);
                Assert.AreEqual(cloned.DefaultValue, obj.DefaultValue);
                Assert.AreEqual(cloned.DisplayObjects, obj.DisplayObjects);
                Assert.AreEqual(cloned.CurrentValueType, obj.CurrentValueType);
            });
        }

        [TestCaseSource(nameof(AddDisplayObjectCaseSource))]
        public void AddDisplayObject_EmptyParameter_AddArgumentOrReplac(
            List<string> actualEnumNames,
            List<BaseParameter.DisplayObject> expected)
        {
            var newObject = new BaseParameterImplementation(stub, stub);

            foreach(var displayObject in actualEnumNames)
            {
                newObject.AddDisplayObject(displayObject);
            }

            Assert.AreEqual(expected, newObject.DisplayObjects);
        }

        private static object[] AddDisplayObjectCaseSource()
        {
            return new object[]
            {
                new object[]
                {
                    new List<string>
                    {
                        "none"
                    },
                    new List<BaseParameter.DisplayObject>
                    {
                        BaseParameter.DisplayObject.None,
                    },
                },
                new object[]
                {
                    new List<string>
                    {
                        "signals"
                    },
                    new List<BaseParameter.DisplayObject>
                    {
                        BaseParameter.DisplayObject.Signals,
                    },
                },
                new object[]
                {
                    new List<string>
                    {
                        "parameters"
                    },
                    new List<BaseParameter.DisplayObject>
                    {
                        BaseParameter.DisplayObject.Parameters,
                    },
                },
                new object[]
                {
                    new List<string>
                    {
                        "parameters",
                        "signals"
                    },
                    new List<BaseParameter.DisplayObject>
                    {
                        BaseParameter.DisplayObject.Parameters,
                        BaseParameter.DisplayObject.Signals,
                    },
                },
            };
        }

        [TestCaseSource(nameof(GetDisplayObjectsCaseSource))]
        public void GetDisplayObjects_NewObject_ReturnsExpectedValues(
            List<BaseParameter.DisplayObject> displayObjects,
            Device.DeviceType[] expectedDevTypes,
            Device.DeviceSubType[] expectedDevSubTypes,
            bool expectedDisplayParameters)
        {
            var newObj = new BaseParameterImplementation(stub, stub, stub,
                displayObjects);

            newObj.GetDisplayObjects(out Device.DeviceType[] devTypes,
                out Device.DeviceSubType[] devSubTypes,
                out bool displayParameters);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedDevTypes, devTypes);
                Assert.AreEqual(expectedDevSubTypes, devSubTypes);
                Assert.AreEqual(expectedDisplayParameters, displayParameters);
            });
        }
        
        private static object[] GetDisplayObjectsCaseSource()
        {
            return new object[]
            {
                new object[]
                {
                    new List<BaseParameter.DisplayObject>
                    {
                        BaseParameter.DisplayObject.Signals,
                        BaseParameter.DisplayObject.Parameters
                    },
                    new Device.DeviceType[]
                    {
                        Device.DeviceType.AI,
                        Device.DeviceType.AO,
                        Device.DeviceType.DI,
                        Device.DeviceType.DO
                    },
                    null,
                    true,
                },
                new object[]
                {
                    new List<BaseParameter.DisplayObject>
                    {
                        BaseParameter.DisplayObject.Signals,
                    },
                    new Device.DeviceType[]
                    {
                        Device.DeviceType.AI,
                        Device.DeviceType.AO,
                        Device.DeviceType.DI,
                        Device.DeviceType.DO
                    },
                    null,
                    false,
                },
                new object[]
                {
                    new List<BaseParameter.DisplayObject>
                    {
                        BaseParameter.DisplayObject.Parameters
                    },
                    new Device.DeviceType[0],
                    null,
                    true,
                },
                new object[]
                {
                    null,
                    new Device.DeviceType[0],
                    null,
                    false,
                },
            };
        }

        [TestCaseSource(nameof(SetValueAndSetNewValueTestCaseSource))]
        public void SetNewValue_NewObject_ProcessValueReturnsCorrect(
            string setValue, string expectedValue)
        {
            string name = "Name";
            string luaName = "LuaName";
            Device.IDeviceManager deviceManager =
                GetMoqForSetValuesAndDisplayTextTest();
            var parameter = new BaseParameterImplementation(luaName, name,
                stub, null, deviceManager);

            parameter.SetNewValue(setValue);

            Assert.AreEqual(expectedValue, parameter.Value);
        }

        [TestCaseSource(nameof(SetValueAndSetNewValueTestCaseSource))]
        public void SetValue_NewObject_ProcessValueReturnsCorrect(
            string setValue, string expectedValue)
        {
            string name = "Name";
            string luaName = "LuaName";
            Device.IDeviceManager deviceManager =
                GetMoqForSetValuesAndDisplayTextTest();
            var parameter = new BaseParameterImplementation(luaName, name,
                stub, null, deviceManager);

            parameter.SetValue(setValue);

            Assert.AreEqual(expectedValue, parameter.Value);
        }

        [TestCase("1", "1")]
        [TestCase("200", "200")]
        [TestCase("-300", "-300")]
        [TestCase("EDCBSSWE", "EDCBSSWE")]
        [TestCase("", "Нет")] // Default value - ""
        [TestCase("0", "0")]
        [TestCase("Два один четыре", "Два один четыре")]
        [TestCase("4.5", "4.5")]
        [TestCase("Нет", "Нет")]
        [TestCase("NORM1DEV2 NORM1DEV1", "NORM1DEV2 NORM1DEV1")]
        [TestCase("NORM1DEV1 NORM1DEV1", "NORM1DEV1")]
        public void DisplayText_NewObjectSetNewvalue_ReturnsCorrectDisplayText(
            string setValue, string expectedParameterValue)
        {
            string name = "Name";
            string luaName = "LuaName";
            Device.IDeviceManager deviceManager =
                GetMoqForSetValuesAndDisplayTextTest();
            var parameter = new BaseParameterImplementation(luaName, name,
                stub, null, deviceManager);
            parameter.SetNewValue(setValue);

            var expectedDisplayText = new string[]
            {
                name,
                expectedParameterValue
            };
            Assert.AreEqual(expectedDisplayText, parameter.DisplayText);
        }

        [TestCase("1", BaseParameter.ValueType.Number)]
        [TestCase("200", BaseParameter.ValueType.Number)]
        [TestCase("-300", BaseParameter.ValueType.Number)]
        [TestCase("EDCBSSWE", BaseParameter.ValueType.Other)]
        [TestCase("", BaseParameter.ValueType.None)]
        [TestCase("0", BaseParameter.ValueType.Number)]
        [TestCase("Два один четыре", BaseParameter.ValueType.Other)]
        [TestCase("4.5", BaseParameter.ValueType.Other)]
        [TestCase("Нет", BaseParameter.ValueType.Stub)]
        [TestCase("NORM1DEV2", BaseParameter.ValueType.Device)]
        [TestCase("STUB1DEV1 NORM1DEV1", BaseParameter.ValueType.Other)]
        [TestCase("NORM1DEV2 NORM1DEV1", BaseParameter.ValueType.ManyDevices)]
        [TestCase("NORM1DEV1 NORM1DEV1", BaseParameter.ValueType.ManyDevices)]
        [TestCase("STUB1DEV1 STUB1DEV2", BaseParameter.ValueType.Other)]
        [TestCase("STUB1DEV1", BaseParameter.ValueType.Other)]
        // Can't be tested yet, have to make DI
        //[TestCase("Param_Name", BaseParameter.ValueType.Parameter)]
        public void CurrentValueType_NewObjectSetNewValue(string setValue,
            BaseParameter.ValueType expectedValueType)
        {
            string name = "Name";
            string luaName = "LuaName";
            Device.IDeviceManager deviceManager =
                GetMoqForSetValuesAndDisplayTextTest();
            var parameter = new BaseParameterImplementation(luaName, name,
                string.Empty, null, deviceManager);

            parameter.SetNewValue(setValue);

            Assert.AreEqual(expectedValueType, parameter.CurrentValueType);
        }

        private static object[] SetValueAndSetNewValueTestCaseSource()
        {
            return new object[]
            {
                new object[] { "1", "1" },
                new object[] { "200", "200" },
                new object[] { "-300", "-300" },
                new object[] { "EDCBSSWE", "EDCBSSWE" },
                new object[] { "", "" },
                new object[] { "0", "0" },
                new object[] { "Два один четыре", "Два один четыре" },
                new object[] { "4.5", "4.5" },
                new object[] { "Нет", "Нет" },
                new object[] { "NORM1DEV2 NORM1DEV1", "NORM1DEV2 NORM1DEV1" },
                new object[] { "NORM1DEV1 NORM1DEV1", "NORM1DEV1" },
            };
        }

        private Device.IDeviceManager GetMoqForSetValuesAndDisplayTextTest()
        {
            string stubDev1Name = "STUB1DEV1";
            string stubDev2Name = "STUB1DEV2";
            string normDev1Name = "NORM1DEV1";
            string normDev2Name = "NORM1DEV2";

            int normDev1Index = 1;
            int stubDev1Index = 2;
            int normDev2Index = 3;
            int stubDev2Index = 4;

            var stubDevice1 = Mock.Of<Device.IDevice>(
                dev => dev.Name == stubDev1Name &&
                dev.Description == StaticHelper.CommonConst.Cap);
            var stubDevice2 = Mock.Of<Device.IDevice>(
                dev => dev.Name == stubDev2Name &&
                dev.Description == StaticHelper.CommonConst.Cap);

            var okDevice1 = Mock.Of<Device.IDevice>(
                dev => dev.Name == normDev1Name &&
                dev.Description == "Description 1");
            var okDevice2 = Mock.Of<Device.IDevice>(
                dev => dev.Name == normDev2Name &&
                dev.Description == "Description 2");

            var deviceManagerMock = Mock.Of<Device.IDeviceManager>(
                d => d.GetDeviceByEplanName(stubDev1Name) == stubDevice1 &&
                d.GetDeviceByEplanName(It.IsAny<string>()) == stubDevice1 &&
                d.GetDeviceByEplanName(stubDev2Name) == stubDevice2 &&
                d.GetDeviceByEplanName(normDev1Name) == okDevice1 &&
                d.GetDeviceByEplanName(normDev2Name) == okDevice2 &&
                d.GetDeviceByIndex(It.IsAny<int>()) == stubDevice1 &&
                d.GetDeviceByIndex(normDev1Index) == okDevice1 &&
                d.GetDeviceByIndex(normDev2Index) == okDevice2 &&
                d.GetDeviceByIndex(stubDev1Index) == stubDevice1 &&
                d.GetDeviceByIndex(stubDev2Index) == stubDevice2 &&
                d.GetDeviceIndex(It.IsAny<string>()) == -1 &&
                d.GetDeviceIndex(normDev1Name) == normDev1Index &&
                d.GetDeviceIndex(normDev2Name) == normDev2Index &&
                d.GetDeviceIndex(stubDev1Name) == stubDev1Index &&
                d.GetDeviceIndex(stubDev2Name) == stubDev2Index);

            return deviceManagerMock;
        }

        string stub = string.Empty;
    }
}
