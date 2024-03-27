using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TechObject;


namespace EasyEplannerTests
{
    public class ParamsManagerTest
    {

        [TestCase(ParamsManager.ParFloatLuaName, 1.2, "1.2")]
        [TestCase(ParamsManager.ParFloatRuntimeLuaName, 1.5, "0")]
        [TestCase(ParamsManager.ParUintLuaName, 1, "1")]
        [TestCase(ParamsManager.ParUintRuntimeLuaName, 4, "0")]
        [TestCase(ParamsManager.ParFloatLuaName, "not number", "0")]
        public void AddParamTest(string group, object valueObj, string expectedValue)
        {
            var paramsManager = new ParamsManager();

            var param = paramsManager.AddParam(group, "par1", valueObj, "с.", "P_PAR1");

            Assert.IsNotNull(param);
            Assert.AreEqual(expectedValue, param.GetValue());
        }
    }
}
