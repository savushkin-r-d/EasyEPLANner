using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
