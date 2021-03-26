using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TechObject;
using Moq;
using System.Linq;

namespace EasyEplanner.Tests
{
    class BaseOperationTest
    {
        [Test]
        public void ConstructorWithNullMode_NewInstance_TrueIfDefaultValues()
        {
            var baseOperation = new BaseOperation(null);

            Assert.Multiple(() =>
            {
                Assert.IsEmpty(baseOperation.Name);
                Assert.IsEmpty(baseOperation.LuaName);
                Assert.IsNull(baseOperation.Owner);
                Assert.AreEqual(new List<BaseParameter>(), 
                    baseOperation.Properties);
                Assert.AreEqual(new Dictionary<string, List<BaseStep>>(),
                    baseOperation.States);
            });
        }

        [Test]
        public void ConctructorWith4Arguments_NewInstance_TrueIfDefaultValues()
        {
            string name = "Имя операции";
            string luaName = "LuaNameOfOperation";
            var baseOperationProperties = new List<BaseParameter>();
            var baseStates = new Dictionary<string, List<BaseStep>>();

            var baseOperation = new BaseOperation(name, luaName,
                baseOperationProperties, baseStates);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(name, baseOperation.Name);
                Assert.AreEqual(luaName, baseOperation.LuaName);
                Assert.IsNull(baseOperation.Owner);
                Assert.AreEqual(baseOperationProperties, 
                    baseOperation.Properties);
                Assert.AreEqual(baseStates, baseOperation.States);
            });
        }

        [Test]
        public void EmptyOperation_UseMethod_ReturnsNewEmptyOperation()
        {
            var emptyOpeartion = BaseOperation.EmptyOperation();

            Assert.Multiple(() =>
            {
                Assert.IsEmpty(emptyOpeartion.Name);
                Assert.IsEmpty(emptyOpeartion.LuaName);
                Assert.IsNull(emptyOpeartion.Owner);
                Assert.AreEqual(new List<BaseParameter>(),
                    emptyOpeartion.Properties);
                Assert.AreEqual(new Dictionary<string, List<BaseStep>>(),
                    emptyOpeartion.States);
            });
        }
        
        [TestCaseSource(nameof(AddStepTestCaseSource))]
        public void AddStep_NewOperation_CorrectAddingOfSteps(
            Dictionary<string, List<BaseStep>> addingStates,
            Dictionary<string, List<BaseStep>> expectedStates)
        {
            var operation = new BaseOperation(null);

            foreach (var state in addingStates.Keys)
            {
                foreach(var step in addingStates[state])
                {
                    operation.AddStep(state, step.LuaName, step.Name,
                        step.DefaultPosition);
                }
            }

            Assert.Multiple(() =>
            {
                foreach(var state in operation.States.Keys)
                {
                    Assert.True(expectedStates.ContainsKey(state));
                    foreach(var step in operation.States[state])
                    {
                        Assert.True(expectedStates[state]
                            .Where(x => x.Name == step.Name &&
                            x.LuaName == step.LuaName &&
                            x.DefaultPosition == step.DefaultPosition)
                            .Count() > 0);
                    }
                }
            });
        }

        private static object[] AddStepTestCaseSource()
        {
            var noStatesAndSteps = new Dictionary<string, List<BaseStep>>();

            string runState = "RUN";
            string stopState = "STOP";

            var runFirstStep = new BaseStep("Шаг 1", "FirstStepLua", 1);
            var runThirdStep = new BaseStep("Шаг 3", "ThirdLuaStep", 3);
            var stopFirstStep = new BaseStep("Первый шаг", "Step1Lua");
            var stopSecondStep = new BaseStep("Второй шаг", "SecondStepLua");
            var zeroEmptyStep = new BaseStep(string.Empty, string.Empty);

            var twoStatesWithSteps = new Dictionary<string, List<BaseStep>>()
            {
                { 
                    runState,
                    new List<BaseStep> { runFirstStep, runThirdStep }
                },
                {
                    stopState,
                    new List<BaseStep> { stopFirstStep, stopSecondStep }
                },
            };

            var expectedTwoStatesWithSteps =
                new Dictionary<string, List<BaseStep>>()
            {
                {
                    runState,
                    new List<BaseStep> {  zeroEmptyStep, runFirstStep,
                        runThirdStep }
                },
                {
                    stopState,
                    new List<BaseStep>() { zeroEmptyStep, stopFirstStep,
                        stopSecondStep }
                },
            };

            return new object[]
            {
                new object[] { noStatesAndSteps,noStatesAndSteps },
                new object[] { twoStatesWithSteps, expectedTwoStatesWithSteps }
            };
        }
    }
}
