using NUnit.Framework;
using System.Collections.Generic;
using TechObject;

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
                : base(luaName, name, default, displayObjects) { }

            public override BaseParameter Clone()
            {
                return new BaseParameterImplementation(LuaName, Name,
                    DefaultValue, DisplayObjects);
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

        string stub = string.Empty;
    }
}
