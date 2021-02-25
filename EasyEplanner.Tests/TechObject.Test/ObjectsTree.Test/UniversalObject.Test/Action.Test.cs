using Editor;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using TechObject;

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
    }

    class DefaultActionProcessingStrategyTest
    {
        
    }

    class OneInManyOutActionProcessingStrategyTest
    {
        
    }

    static class DeviceManagerMock
    {
        static DeviceManagerMock()
        {
            var mock = new Mock<Device.IDeviceManager>();
            SetUpMock(mock);
            deviceManager = mock.Object;
        }

        private static void SetUpMock(Mock<Device.IDeviceManager> mock)
        {
            //TODO:
            //DeviceManager mock
            //GetDeviceByEplanName
            //GetDeviceIndex
            //GetDeviceByIndex

            //Devices mocks:
            //DeviceType
            //DeviceSubTypes
            //Description
            //Name
            //DeviceNumber
            //ObjectName

            //Action mock:
            //GetDisplayObjects
        }

        public static Device.IDeviceManager DeviceManager
            => deviceManager;

        private static Device.IDeviceManager deviceManager;
    }
}
