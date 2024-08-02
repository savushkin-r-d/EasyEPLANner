using NUnit.Framework;
using System.Collections.Generic;
using TechObject;
using Moq;
using System.Reflection;

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
                List<DisplayObject> displayObjects = null) 
                : base(luaName, name, defaultValue, displayObjects) { }

            public override BaseParameter Clone()
            {
                return new BaseParameterImplementation(LuaName, Name,
                    DefaultValue, DisplayObjects);
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            typeof(BaseParameter).GetField("deviceManager",
                BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, GetMoqForSetValuesAndDisplayTextTest());
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
                Assert.IsTrue(obj.IsEmpty);
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

            foreach (var displayObject in actualEnumNames)
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
            EplanDevice.DeviceType[] expectedDevTypes,
            EplanDevice.DeviceSubType[] expectedDevSubTypes,
            bool expectedDisplayParameters)
        {
            var newObj = new BaseParameterImplementation(stub, stub, stub,
                displayObjects);

            newObj.GetDisplayObjects(out EplanDevice.DeviceType[] devTypes,
                out EplanDevice.DeviceSubType[] devSubTypes,
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
                    new EplanDevice.DeviceType[]
                    {
                        EplanDevice.DeviceType.AI,
                        EplanDevice.DeviceType.AO,
                        EplanDevice.DeviceType.DI,
                        EplanDevice.DeviceType.DO
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
                    new EplanDevice.DeviceType[]
                    {
                        EplanDevice.DeviceType.AI,
                        EplanDevice.DeviceType.AO,
                        EplanDevice.DeviceType.DI,
                        EplanDevice.DeviceType.DO
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
                    new EplanDevice.DeviceType[0],
                    null,
                    true,
                },
                new object[]
                {
                    null,
                    new EplanDevice.DeviceType[0],
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

            var parameter = new BaseParameterImplementation(luaName, name, stub, null);

            parameter.SetNewValue(setValue);

            Assert.AreEqual(expectedValue, parameter.Value);
        }

        [TestCaseSource(nameof(SetValueAndSetNewValueTestCaseSource))]
        public void SetValue_NewObject_ProcessValueReturnsCorrect(
            string setValue, string expectedValue)
        {
            string name = "Name";
            string luaName = "LuaName";
            var parameter = new BaseParameterImplementation(luaName, name, stub, null);

            parameter.SetValue(setValue);

            Assert.AreEqual(expectedValue, parameter.Value);
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

        [TestCase("1", "1")]
        [TestCase("200", "200")]
        [TestCase("-300", "-300")]
        [TestCase("EDCBSSWE", "EDCBSSWE")]
        [TestCase("", "")] // Default value - ""
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
            var parameter = new BaseParameterImplementation(luaName, name, stub, null);
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
        // Can be tested only for Bool parameters (this one is not bool)
        //[TestCase(true, BaseParameter.ValueType.Boolean)]
        public void CurrentValueType_NewObjectSetNewValue(string setValue,
            BaseParameter.ValueType expectedValueType)
        {
            string name = "Name";
            string luaName = "LuaName";
            var parameter = new BaseParameterImplementation(luaName, name, string.Empty, null);

            parameter.SetNewValue(setValue);

            Assert.AreEqual(expectedValueType, parameter.CurrentValueType);
        }

        [TestCase("NORM1DEV1", "\t\t", "\t\tLuaName = prg.control_modules.NORM1DEV1")]
        [TestCase("NORM1DEV1 NORM1DEV2", "\t\t", "\t\tLuaName = { prg.control_modules.NORM1DEV1, prg.control_modules.NORM1DEV2 }")]
        [TestCase("1", "\t", "\tLuaName = prg.0.operations.BASEOPERATIONLUANAME1")]
        [TestCase("2", "\t", "\tLuaName = prg.0.operations.BASEOPERATIONLUANAME2")]
        [TestCase("other", "", "LuaName = other")]
        public void SaveToPrgLua_CheckBaseTechObjectOwner(string value, string prefix, string expected)
        {
            var parameter = new BaseParameterImplementation("LuaName", "Name", stub, null);
            SetUpParameterBaseTechObjectOwner(parameter);

            parameter.SetNewValue(value);
            var res = parameter.SaveToPrgLua(prefix);

            Assert.AreEqual(expected, res);
        }

        private void SetUpParameterBaseTechObjectOwner(BaseParameter parameter)
        {
            parameter.Owner = new BaseTechObject(new TechObject
               .TechObject("techObjectName", getN => 0, 0,
                   0, "techObjectEplanName", 0, "techObjectNameBC",
                   "attachedObjects", null));
            var modesManager = (parameter.Owner as BaseTechObject)
                .Owner.ModesManager;
            var modes = new List<Mode>
            {
                new Mode("modeName_1", getN => 1, modesManager),
                new Mode("modeName_2", getN => 2, modesManager),
            };

            modes[0].BaseOperation.Name = "baseOperationName1";
            modes[0].BaseOperation.LuaName = "baseOperationLuaName1";

            modes[1].BaseOperation.Name = "baseOperationName2";
            modes[1].BaseOperation.LuaName = "baseOperationLuaName2";

            parameter.Parent = new BaseOperation(modes[0]);

            modesManager.Modes.AddRange(modes);
        }

        [TestCase("parameter1", "\t", "\tLuaName = prg.techobject1.PAR_FLOAT.parameter1")]
        [TestCase("parameter2", "\t", "\tLuaName = prg.techobject1.PAR_FLOAT.parameter2")]
        [TestCase("other", "", "")]
        public void SaveToPrgLua_CheckBaseOperationOwner(string value, string prefix,
            string expected)
        {
            var parameter = new ActiveParameter("LuaName", "Name", "", new List<BaseParameter.DisplayObject>() { BaseParameter.DisplayObject.Parameters });
            SetUpParameterBaseOperationOwner(parameter);

            parameter.SetNewValue(value);
            var res = parameter.SaveToPrgLua(prefix);
            Assert.AreEqual(expected, res);
        }

        private void SetUpParameterBaseOperationOwner(BaseParameter parameter)
        {
            var techObject = new TechObject.TechObject("techObjectName",
                getN => 1, 1, 1, "techObjectEplanName", 1, "techObjectNameBC",
                "attachedObjects", null);
            techObject.NameEplanForFile = "TechObject";
            techObject.GetParamsManager().AddFloatParam("параметр 1", 1, "unit", "parameter1");
            techObject.GetParamsManager().AddFloatParam("параметр 2", 5, "unit", "parameter2");

            var operation = new Mode("operation1", getN => 1, new ModesManager(techObject));
            
            var baseOperation = new BaseOperation(operation);

            parameter.Owner = baseOperation;
        }

        [TestCase("true", "\t", "\tLuaName = true")]
        [TestCase("false", "\t\t", "\t\tLuaName = false")]
        public void SaveToPrgLua_CheckActiveBoolParameter(string value, string prefix, string expected)
        {
            var parameter = new ActiveBoolParameter("LuaName", "Name", "false", null);

            parameter.SetNewValue(value);
            var res = parameter.SaveToPrgLua(prefix);

            Assert.AreEqual(expected, res);
        }

        [TestCase("NORM1DEV2", "NORM", 2, "NORM2DEV2")]
        [TestCase("NORM1DEV2", "OTHER", 1, "OTHER1DEV2")]
        public void ModifyDevNames(
            string source,
            string newName, int newNumber,
            string expected)
        {
            var parameter = new ActiveParameter("LuaName", "Name");
            parameter.SetNewValue(source);

            var options = Mock.Of<IDevModifyOptions>(o => 
                o.NewTechObjectName == newName &&
                o.NewTechObjectNumber == newNumber);

            parameter.ModifyDevNames(options);

            Assert.AreEqual(expected, parameter.Value);
        }

        private EplanDevice.IDeviceManager GetMoqForSetValuesAndDisplayTextTest()
        {
            string stubDev1Name = "STUB1DEV1";
            string stubDev2Name = "STUB1DEV2";
            string norm_1_dev_1_name = "NORM1DEV1";
            string norm_1_dev_2_name = "NORM1DEV2";
            string norm_2_dev_2_name = "NORM2DEV2";
            string other_1_dev_2_name = "OTHER1DEV2";


            int norm_1_dev_1_index = 1;
            int stubDev1Index = 2;
            int norm_1_dev_2_index = 3;
            int stubDev2Index = 4;
            int norm_2_dev_2_index = 5;
            int other_1_dev_2_index = 6;

            var stubDevice1 = Mock.Of<EplanDevice.IDevice>(
                dev => dev.Name == stubDev1Name &&
                dev.Description == StaticHelper.CommonConst.Cap);
            var stubDevice2 = Mock.Of<EplanDevice.IDevice>(
                dev => dev.Name == stubDev2Name &&
                dev.Description == StaticHelper.CommonConst.Cap);

            var norm_1_dev_1 = Mock.Of<EplanDevice.IDevice>(
                dev => dev.Name == norm_1_dev_1_name &&
                dev.Description == "Description 1");
            var norm_1_dev_2 = Mock.Of<EplanDevice.IDevice>(
                dev => dev.Name == norm_1_dev_2_name &&
                dev.Description == "Description 2");

            var norm_2_dev_2 = Mock.Of<EplanDevice.IDevice>(
                dev => dev.Name == norm_2_dev_2_name &&
                dev.Description == "Description");

            var other_1_dev_2 = Mock.Of<EplanDevice.IDevice>(
                dev => dev.Name == other_1_dev_2_name &&
                dev.Description == "Description");

            var deviceManagerMock = Mock.Of<EplanDevice.IDeviceManager>(
                d => d.GetDeviceByEplanName(stubDev1Name) == stubDevice1 &&
                d.GetDeviceByEplanName(It.IsAny<string>()) == stubDevice1 &&
                d.GetDeviceByEplanName(stubDev2Name) == stubDevice2 &&
                d.GetDeviceByEplanName(norm_1_dev_1_name) == norm_1_dev_1 &&
                d.GetDeviceByEplanName(norm_1_dev_2_name) == norm_1_dev_2 &&
                d.GetDeviceByEplanName(norm_2_dev_2_name) == norm_2_dev_2 &&
                d.GetDeviceByEplanName(other_1_dev_2_name) == other_1_dev_2 &&
                d.GetDeviceByIndex(It.IsAny<int>()) == stubDevice1 &&
                d.GetDeviceByIndex(norm_1_dev_1_index) == norm_1_dev_1 &&
                d.GetDeviceByIndex(norm_1_dev_2_index) == norm_1_dev_2 &&
                d.GetDeviceByIndex(stubDev1Index) == stubDevice1 &&
                d.GetDeviceByIndex(stubDev2Index) == stubDevice2 &&
                d.GetDeviceByIndex(norm_2_dev_2_index) == norm_2_dev_2 &&
                d.GetDeviceByIndex(other_1_dev_2_index) == other_1_dev_2 &&
                d.GetDeviceIndex(It.IsAny<string>()) == -1 &&
                d.GetDeviceIndex(norm_1_dev_1_name) == norm_1_dev_1_index &&
                d.GetDeviceIndex(norm_1_dev_2_name) == norm_1_dev_2_index &&
                d.GetDeviceIndex(stubDev1Name) == stubDev1Index &&
                d.GetDeviceIndex(stubDev2Name) == stubDev2Index &&
                d.GetDeviceIndex(norm_2_dev_2_name) == norm_2_dev_2_index &&
                d.GetDeviceIndex(other_1_dev_2_name) == other_1_dev_2_index &&
                d.GetModifiedDevice(norm_1_dev_2, It.Is<IDevModifyOptions>(o => o.NewTechObjectNumber == 2)) == norm_2_dev_2 &&
                d.GetModifiedDevice(norm_1_dev_2, It.Is<IDevModifyOptions>(o => o.NewTechObjectName == "OTHER")) == other_1_dev_2);

            return deviceManagerMock;
        }

        string stub = string.Empty;
    }
}
