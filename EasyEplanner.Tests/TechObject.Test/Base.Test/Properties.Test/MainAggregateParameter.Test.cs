using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;


namespace TechObject.Tests
{
    class MainAggregateParameterTest
    {
        [Test]
        public void NeedDisable_ReturnFalse()
        {
            var parameter = new MainAggregateParameter("LuaName", "name", "val");
            Assert.IsFalse(parameter.NeedDisable);
        }

        [Test]
        public void Filter_ParentMatches_ShowsChildParameter()
        {
            var parent = new MainAggregateParameter("MAIN",
                "Использовать агрегат", "true");
            var child = new ActiveParameter("PARAMETER", "Дочерний параметр");
            child.Parent = parent;
            parent.Parameters.Add(child);

            var result = parent.Filter("агрегат", hideEmptyItems: false);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result);
                Assert.IsTrue(parent.Filtred);
                Assert.IsTrue(parent.ThisOrParentsContains);
                Assert.IsTrue(child.Filtred);
                Assert.IsTrue(child.ThisOrParentsContains);
            });
        }


        [Test]
        public void Autocomplete()
        {
            var tankBTO = new BaseTechObject()
            {
                EplanName = "TANK",
                Name = "Танк",
                BaseOperations = 
                {
                    new BaseOperation("Наполнение", "FILL", new List<BaseParameter>(), new Dictionary<string, List<BaseStep>>())
                    {       
                        DefaultPosition = 1,
                    }
                }
            };

            var tank = new TechObject("", getN => 1, 1, 2, "", -1, "", "", tankBTO);
            tank.SetUpFromBaseTechObject();

            var tankBaseOperation = tank.ModesManager.Modes[0].BaseOperation;


            var aggregateBTO = new BaseTechObject()
            {

            };

            var mainAggregateParameter = aggregateBTO.AddMainAggregateParameter("MAIN", "использовать узел", "false");
            var activeAggregateParameter = aggregateBTO.AddActiveAggregateParameter("PARAMETER", "Параметр");

            activeAggregateParameter.SetFloatParameter("PARAMETER", "Параметр", 0, "шт");

            var parameters = new List<BaseParameter>();
            parameters.AddRange(aggregateBTO.AggregateParameters);
            parameters.Add(aggregateBTO.MainAggregateParameter);
            tankBaseOperation.AddProperties(parameters, aggregateBTO);

            var main = tankBaseOperation.Properties.Find(p => p.LuaName == "MAIN");
            main.SetNewValue("true");
            (main as IAutocompletable)?.Autocomplete();

            Assert.Multiple(() =>
            {
                var activeAP = tankBaseOperation.Properties.Find(p => p.LuaName == "PARAMETER");

                Assert.AreEqual("FILL_PARAMETER", activeAP.Value);

                var param = tank.GetParamsManager().Float.GetParam(0);

                Assert.AreEqual("FILL_PARAMETER", param.GetNameLua());
                Assert.AreEqual("Наполнение. Параметр", param.GetName());
                Assert.AreEqual("1", param.Operations);
            });
        }
    }
}
