using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;
using EplanDevice;
using Moq;
using NUnit.Framework;
using StaticHelper;
using TechObject;

namespace TechObjectTests
{
    public class ActionParameterTest
    {
        [Test]
        public void SetNewValue_and_DisplayText_DisplayParameter()
        {
            var actionParameter = new ActionParameter("action_parameter", "параметр");
            var techObject = new TechObject.TechObject("", getN => 1, 1, 2, "", -1, "", "", null);
            var param_1 = techObject.GetParamsManager().AddFloatParam("Параметр 1", 5, "сек", "TIME_PARAM");
            var param_2 = techObject.GetParamsManager().AddFloatParam("Параметр 2", 1, "шт", "QUANTITY_PARAM");

            actionParameter.Parent = techObject;

            Assert.Multiple(() =>
            {
                actionParameter.SetNewValue("1");
                _ = actionParameter.Value;
                Assert.AreEqual("1. TIME_PARAM: 5 сек", actionParameter.DisplayText[1]);
                Assert.AreEqual("1", actionParameter.EditText[1]);
                Assert.AreSame(param_1, actionParameter.Parameter);

                actionParameter.SetNewValue("2");
                _ = actionParameter.Value;
                Assert.AreEqual("2. QUANTITY_PARAM: 1 шт", actionParameter.DisplayText[1]);
                Assert.AreEqual("2", actionParameter.EditText[1]);
                Assert.AreSame(param_2, actionParameter.Parameter);

                actionParameter.SetNewValue("TIME_PARAM");
                _ = actionParameter.Value;
                Assert.AreEqual("1. TIME_PARAM: 5 сек", actionParameter.DisplayText[1]);
                Assert.AreEqual("1", actionParameter.EditText[1]);
                Assert.AreSame(param_1, actionParameter.Parameter);

                techObject.GetParamsManager().Float.MoveDown(param_1);
                _ = actionParameter.Value;
                Assert.AreEqual("2. TIME_PARAM: 5 сек", actionParameter.DisplayText[1]);
                Assert.AreEqual("2", actionParameter.EditText[1]);
                Assert.AreSame(param_1, actionParameter.Parameter);

                techObject.GetParamsManager().Float.Delete(param_1);
                _ = actionParameter.Value;
                Assert.AreEqual(CommonConst.StubForCells, actionParameter.DisplayText[1]);
                Assert.AreEqual("-1", actionParameter.EditText[1]);
                Assert.IsNull(actionParameter.Parameter);

                actionParameter.SetNewValue("-1");
                _ = actionParameter.Value;
                Assert.AreEqual(CommonConst.StubForCells, actionParameter.DisplayText[1]);
                Assert.AreEqual("-1", actionParameter.EditText[1]);
                Assert.IsNull(actionParameter.Parameter);

                actionParameter.SetNewValue("10");
                _ = actionParameter.Value;
                Assert.AreEqual("Параметр 10 не найден", actionParameter.DisplayText[1]);
                Assert.AreEqual("10", actionParameter.EditText[1]);
                Assert.IsNull(actionParameter.Parameter);
            });
        }

        [Test]
        public void ModifyDevName()
        {
            string OBJ = nameof(OBJ);

            var dev1 = new V($"{OBJ}1V1", $"+{OBJ}1-V1", "", 1, OBJ, 1, "");
            var dev2 = new V($"{OBJ}2V1", $"+{OBJ}2-V1", "", 1, OBJ, 2, "");

            var deviceManager = Mock.Of<IDeviceManager>(m =>
                m.GetDeviceByEplanName($"{OBJ}1V1") == dev1 &&
                m.GetDeviceByEplanName($"{OBJ}2V1") == dev2 &&
                m.GetDeviceIndex($"{OBJ}1V1") == 0 &&
                m.GetDeviceIndex($"{OBJ}2V1") == 1 &&
                m.GetDeviceByIndex(0) == dev1 &&
                m.GetDeviceByIndex(1) == dev2 &&
                m.GetModifiedDevice(dev1, It.IsAny<IDevModifyOptions>()) == dev2);

            var actionParameter = new ActionParameter("action_parameter", "параметр");
            
            typeof(BaseParameter).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManager);

            actionParameter.SetNewValue($"{OBJ}1V1");
            actionParameter.ModifyDevNames(null); // modify by deviceManager(Mock).GetModifiedDevice()

            Assert.AreEqual($"{OBJ}2V1", actionParameter.Value);

            typeof(BaseParameter).GetField("deviceManager",
               System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
               .SetValue(null, DeviceManager.GetInstance());
        }

        [Test]
        public void IsDrawToEplanPage_True()
        {
            Assert.IsTrue(new ActionParameter("", "").IsDrawOnEplanPage);
        }

        [Test]
        public void IsDrawToEplanPage_False() 
        {
            var actionParameter = new ActionParameter("ap", "ap");

            var dev = new V("OBJ1V1", "+OBJ1-V1", "", 1, "OBJ", 1, "");

            var deviceManager = Mock.Of<IDeviceManager>(m =>
                m.GetDeviceByEplanName("OBJ1V1") == dev &&
                m.GetDeviceIndex("OBJ1V1") == 1 &&
                m.GetDeviceByIndex(1) == dev);

            typeof(BaseParameter).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, deviceManager);


            Assert.Multiple(() =>
            {
                actionParameter.SetNewValue("OBJ1V1");
                var res = actionParameter.GetObjectToDrawOnEplanPage();
                Assert.AreEqual(dev, res.FirstOrDefault().DrawingDevice);
                Assert.AreEqual(DrawInfo.Style.GREEN_BOX, res.FirstOrDefault().DrawingStyle);

                actionParameter.SetNewValue("");
                res = actionParameter.GetObjectToDrawOnEplanPage();
                CollectionAssert.IsEmpty(res);
            });

            typeof(BaseParameter).GetField("deviceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, DeviceManager.GetInstance());
        }
    }
}
