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

        [Test]
        public void Autocomplete()
        {
            var baseTechObject = new BaseTechObject()
            {
                Name = "Объект",
                EplanName = "Object",
                BaseOperations = new List<BaseOperation>()
                {
                    new BaseOperation(null)
                    {
                        LuaName = "Operation_1",
                        Name = "Операция 1",
                        DefaultPosition = 1,
                        Parameters = new List<IBaseFloatParameter>()
                        {
                            new BaseFloatParameter("PAR_1", "Параметр 1", 0, "шт"),
                            new BaseFloatParameter("PAR_2", "Параметр 2", 1.4, "шт"),
                        }
                    },
                    new BaseOperation(null)
                    {
                        LuaName = "Operation_2",
                        Name = "Операция 2",
                        DefaultPosition = 2,
                        Parameters = new List<IBaseFloatParameter>()
                        {
                            new BaseFloatParameter("PAR_1", "Параметр 1", 0, "шт"),
                            new BaseFloatParameter("PAR_3", "Параметр 3", 1, "шт"),
                        }
                    },
                    new BaseOperation(null)
                    {
                        LuaName = "Operation_3",
                        Name = "Операция 3",
                        DefaultPosition = 3,
                        Parameters = new List<IBaseFloatParameter>()
                        {
                            new BaseFloatParameter("PAR_2", "Параметр 2", 0, "шт"),
                        }
                    },
                },
            };
            baseTechObject.ParamsManager.AddFloatParam("Общий параметр 1", 0, "шт", "SYS_PAR1");


            var techObject = new TechObject.TechObject("", getN => 1, 1, 2, "", -1, "", "", baseTechObject);
            techObject.SetUpFromBaseTechObject();

            var paramsManager = techObject.GetParamsManager();
            paramsManager.Autocomplete();

            Assert.Multiple(() =>
            {
                Assert.AreEqual("PAR_1", paramsManager.Float.GetParam(20).GetNameLua());
                Assert.AreEqual("PAR_2", paramsManager.Float.GetParam(21).GetNameLua());
                Assert.AreEqual("P", paramsManager.Float.GetParam(22).GetNameLua());
                Assert.AreEqual("P", paramsManager.Float.GetParam(23).GetNameLua());

                Assert.AreEqual("PAR_3", paramsManager.Float.GetParam(30).GetNameLua());
                Assert.AreEqual("P", paramsManager.Float.GetParam(31).GetNameLua());
                Assert.AreEqual("P", paramsManager.Float.GetParam(32).GetNameLua());

                Assert.AreEqual("1 2", paramsManager.Float.GetParam(20).Operations);
                Assert.AreEqual("1.4", paramsManager.Float.GetParam(21).GetValue());
                Assert.AreEqual("1", paramsManager.Float.GetParam(30).GetValue());

                Assert.IsNull(paramsManager.Float.GetParam(40));
            });
        }
    }
}
