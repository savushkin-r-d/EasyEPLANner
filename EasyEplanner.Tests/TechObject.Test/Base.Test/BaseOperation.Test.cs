﻿using System;
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
            Dictionary<string, List<BaseStep>> expectedStates,
            BaseOperation customizableOper)
        {
            foreach (var state in addingStates.Keys)
            {
                foreach (var step in addingStates[state])
                {
                    customizableOper.AddStep(state, step.LuaName, step.Name,
                        step.DefaultPosition);
                }
            }

            Assert.Multiple(() =>
            {
                foreach (var state in customizableOper.States.Keys)
                {
                    Assert.True(expectedStates.ContainsKey(state));
                    foreach (var step in customizableOper.States[state])
                    {
                        Assert.True(expectedStates[state]
                            .Where(x => x.Name == step.Name &&
                            x.LuaName == step.LuaName &&
                            x.DefaultPosition == step.DefaultPosition &&
                            x.Owner == step.Owner)
                            .Count() == 1);
                    }
                }
            });
        }

        private static object[] AddStepTestCaseSource()
        {
            var noStatesAndSteps = new Dictionary<string, List<BaseStep>>();
            var oper = new BaseOperation(null);

            var step1 = new BaseStep("Шаг 1", "Step Lua 1", 1, oper);
            var step2 = new BaseStep("Шаг 2", "Step Lua 2", 2, oper);
            var step3 = new BaseStep("Шаг 3", "Step Lua 3", 3, oper);
            var step4 = new BaseStep("Шаг 4", "Step Lua 4", 4, oper);
            var step5 = new BaseStep("Шаг 5", "Step Lua 5", 5, oper);
            var step6 = new BaseStep("Шаг 6", "Step Lua 6", 6, oper);

            var runStep1 = new BaseStep("Шаг 1", "FirstStepLua", 1, oper);
            var runStep3 = new BaseStep("Шаг 3", "ThirdLuaStep", 3, oper);
            var stopStep1 = new BaseStep("Первый шаг", "Step1Lua", 0, oper);
            var stopStep2 = new BaseStep("Второй шаг", "StepLua2", 0, oper);
            var emptyStep = new BaseStep(string.Empty, string.Empty, 0, oper);

            var StatesWithSteps = new Dictionary<string, List<BaseStep>>()
            {
                {
                    State.StateType.IDLE.ToString(),
                    new List<BaseStep> { step1, step2 }
                },
                {
                    State.StateType.RUN.ToString(),
                    new List<BaseStep> { runStep1, runStep3 }
                },
                {
                    State.StateType.PAUSE.ToString(),
                    new List<BaseStep> { step3, step4 }
                },
                {
                    State.StateType.STOP.ToString(),
                    new List<BaseStep> { stopStep1, stopStep2 }
                },
                {
                    State.StateType.STARTING.ToString(),
                    new List<BaseStep> { step1, step3 }
                },
                {
                    State.StateType.PAUSING.ToString(),
                    new List<BaseStep> { step5, step6 }
                },
                {
                    State.StateType.UNPAUSING.ToString(),
                    new List<BaseStep> { step6, step1 }
                },
                {
                    State.StateType.STOPPING.ToString(),
                    new List<BaseStep> { step3, step6 }
                },
            };

            var expectedStatesWithSteps = new Dictionary<string, List<BaseStep>>()
            {
                {
                    State.StateType.IDLE.ToString(),
                    new List<BaseStep> { emptyStep, step1, step2 }
                },
                {
                    State.StateType.RUN.ToString(),
                    new List<BaseStep> { emptyStep, runStep1, runStep3 }
                },
                {
                    State.StateType.PAUSE.ToString(),
                    new List<BaseStep> { emptyStep, step3, step4 }
                },
                {
                    State.StateType.STOP.ToString(),
                    new List<BaseStep> { emptyStep, stopStep1, stopStep2 }
                },
                {
                    State.StateType.STARTING.ToString(),
                    new List<BaseStep> { emptyStep, step1, step3 }
                },
                {
                    State.StateType.PAUSING.ToString(),
                    new List<BaseStep> { emptyStep, step5, step6 }
                },
                {
                    State.StateType.UNPAUSING.ToString(),
                    new List<BaseStep> { emptyStep, step6, step1 }
                },
                {
                    State.StateType.STOPPING.ToString(),
                    new List<BaseStep> { emptyStep, step3, step6 }
                },
            };

            return new object[]
            {
                new object[]
                {
                    noStatesAndSteps,
                    noStatesAndSteps,
                    oper
                },
                new object[]
                {
                    StatesWithSteps,
                    expectedStatesWithSteps,
                    oper
                }
            };
        }

        [TestCaseSource(nameof(GetStateBaseStepsTestCaseSource))]
        public void GetStateBaseSteps_StateFromSource_ReturnsStepsOrEmptyList(
            Dictionary<string, List<BaseStep>> addingStates,
            State.StateType selectedState, List<BaseStep> expectedSteps,
            BaseOperation customizableOper)
        {
            FillBaseOperationStatesWithReset(customizableOper, addingStates);

            List<BaseStep> actualSteps = customizableOper
                .GetStateBaseSteps(selectedState);

            Assert.Multiple(() =>
            {
                for (int stepId = 0; stepId < actualSteps.Count; stepId++)
                {
                    BaseStep expectedStep = expectedSteps[stepId];
                    BaseStep actualStep = actualSteps[stepId];

                    bool areEqual =
                        expectedStep.DefaultPosition ==
                        actualStep.DefaultPosition &&
                        expectedStep.LuaName == actualStep.LuaName &&
                        expectedStep.Name == actualStep.Name &&
                        expectedStep.Owner == actualStep.Owner;

                    Assert.IsTrue(areEqual);
                }
            });

        }

        private static object[] GetStateBaseStepsTestCaseSource()
        {
            var oper = new BaseOperation(null);


            var step1 = new BaseStep("Шаг 1", "Step Lua 1", 1, oper);
            var step2 = new BaseStep("Шаг 2", "Step Lua 2", 2, oper);
            var step3 = new BaseStep("Шаг 3", "Step Lua 3", 3, oper);
            var step4 = new BaseStep("Шаг 4", "Step Lua 4", 4, oper);
            var step5 = new BaseStep("Шаг 5", "Step Lua 5", 5, oper);
            var step6 = new BaseStep("Шаг 6", "Step Lua 6", 6, oper);

            var runStep1 = new BaseStep("fRun", "RunFirSt", 1, oper);
            var runStep2 = new BaseStep("sRun", "RunSecSt", 2, oper);
            var stopStep1 = new BaseStep("fSt", "StopFirSt", 0, oper);
            var pauseStep1 = new BaseStep("fP", "PauseFirst", 1, oper);
            var pauseStep2 = new BaseStep("sP", "PauseSecond", 0, oper);

            var addingStates = new Dictionary<string, List<BaseStep>>
            {
                {
                    State.StateType.IDLE.ToString(),
                    new List<BaseStep> { step1, step2 }
                },
                {
                    State.StateType.RUN.ToString(),
                    new List<BaseStep> { runStep1, runStep2 }
                },
                {
                    State.StateType.PAUSE.ToString(),
                    new List<BaseStep> { pauseStep1, pauseStep2 }
                },
                {
                    State.StateType.STOP.ToString(),
                    new List<BaseStep> { stopStep1 }
                },
                {
                    State.StateType.STARTING.ToString(),
                    new List<BaseStep> { step1, step3 }
                },
                {
                    State.StateType.PAUSING.ToString(),
                    new List<BaseStep> { step5, step6 }
                },
                {
                    State.StateType.UNPAUSING.ToString(),
                    new List<BaseStep> { step6, step1 }
                },
                {
                    State.StateType.STOPPING.ToString(),
                    new List<BaseStep> { step3, step6 }
                },
            };

            var emptyStep = new BaseStep(string.Empty, string.Empty, 0, oper);

            var returnsIdleSteps = new object[]
            {
                addingStates,
                State.StateType.IDLE,
                new List<BaseStep> { emptyStep, step1, step2 },
                oper
            };

            var returnsRunSteps = new object[]
            {
                addingStates,
                State.StateType.RUN,
                new List<BaseStep> { emptyStep, runStep1, runStep2 },
                oper
            };

            var returnsStopSteps = new object[]
            {
                addingStates,
                State.StateType.STOP,
                new List<BaseStep> { emptyStep, stopStep1 },
                oper
            };

            var returnsPauseSteps = new object[]
            {
                addingStates,
                State.StateType.PAUSE,
                new List<BaseStep> { emptyStep, pauseStep1, pauseStep2 },
                oper
            };

            var returnsStartingSteps = new object[]
            {
                addingStates,
                State.StateType.STARTING,
                new List<BaseStep> { emptyStep, step1, step3 },
                oper
            };
            var returnsPausingSteps = new object[]
            {
                addingStates,
                State.StateType.PAUSING,
                new List<BaseStep> { emptyStep, step5, step6 },
                oper
            };
            var returnsUnpausingSteps = new object[]
            {
                addingStates,
                State.StateType.UNPAUSING,
                new List<BaseStep> { emptyStep, step6, step1 },
                oper
            };
            var returnsStoppingSteps = new object[]
            {
                addingStates,
                State.StateType.STOPPING,
                new List<BaseStep> { emptyStep, step3, step6 },
                oper
            };


            var returnsEmptyListWrongSelectedType = new object[]
            {
                addingStates,
                (State.StateType)100,
                new List<BaseStep>(),
                oper
            };

            var returnsEmptyListNoStates = new object[]
            {
                new Dictionary<string, List<BaseStep>>(),
                State.StateType.PAUSE,
                new List<BaseStep>(),
                oper
            };

            return new object[]
            {
                returnsIdleSteps,
                returnsRunSteps,
                returnsPauseSteps,
                returnsStopSteps,
                returnsStartingSteps,
                returnsPausingSteps, 
                returnsUnpausingSteps,
                returnsStoppingSteps,
                returnsEmptyListNoStates,
                returnsEmptyListWrongSelectedType
            };
        }

        [TestCaseSource(nameof(GetStateStepsNamesTestCaseSource))]
        public void GetStateStepsNames_StateFromSource_ReturnsNamesOrEmptyList(
            BaseOperation customizableOper, State.StateType selectedType,
            Dictionary<string, List<BaseStep>> addingStates,
            List<string> expectedNames)
        {
            FillBaseOperationStatesWithReset(customizableOper, addingStates);

            List<string> actualNames = customizableOper
                .GetStateStepsNames(selectedType);

            Assert.AreEqual(expectedNames, actualNames);
        }

        private static object[] GetStateStepsNamesTestCaseSource()
        {
            string emptyStepName = string.Empty;

            var oper = new BaseOperation(null);


            string step1Name = "Шаг 1";
            string step2Name = "Шаг 2";
            string step3Name = "Шаг 3";
            string step4Name = "Шаг 4";
            string step5Name = "Шаг 5";
            string step6Name = "Шаг 6";
            string runStep1Name = "ВыпШаг1";
            string runStep2name = "RunШаг2";
            string stopStep1Name = "StName2";
            string pauseStep1Name = "ПаузаName1";
            string pauseStep2Name = "PauseШаг2";

            var step1 = new BaseStep(step1Name, "Step Lua 1", 1, oper);
            var step2 = new BaseStep(step2Name, "Step Lua 2", 2, oper);
            var step3 = new BaseStep(step3Name, "Step Lua 3", 3, oper);
            var step4 = new BaseStep(step4Name, "Step Lua 4", 4, oper);
            var step5 = new BaseStep(step5Name, "Step Lua 5", 5, oper);
            var step6 = new BaseStep(step6Name, "Step Lua 6", 6, oper);
            var runStep1 = new BaseStep(runStep1Name, "Run1Lua", 1, oper);
            var runStep2 = new BaseStep(runStep2name, "Run2Lua", 2, oper);
            var stopStep1 = new BaseStep(stopStep1Name, "Stop1Lua", 1, oper);
            var pauseStep1 = new BaseStep(pauseStep1Name, "Pause1Lua", 1, oper);
            var pauseStep2 = new BaseStep(pauseStep2Name, "Pause2Lua", 2, oper);

            var addingStates = new Dictionary<string, List<BaseStep>>
            {
                {
                    State.StateType.IDLE.ToString(),
                    new List<BaseStep> { step1, step2 }
                },
                {
                    State.StateType.RUN.ToString(),
                    new List<BaseStep> { runStep1, runStep2 }
                },
                {
                    State.StateType.PAUSE.ToString(),
                    new List<BaseStep> { pauseStep1, pauseStep2 }
                },
                {
                    State.StateType.STOP.ToString(),
                    new List<BaseStep> { stopStep1 }
                },
                {
                    State.StateType.STARTING.ToString(),
                    new List<BaseStep> { step1, step3 }
                },
                {
                    State.StateType.PAUSING.ToString(),
                    new List<BaseStep> { step5, step6 }
                },
                {
                    State.StateType.UNPAUSING.ToString(),
                    new List<BaseStep> { step6, step1 }
                },
                {
                    State.StateType.STOPPING.ToString(),
                    new List<BaseStep> { step3, step6 }
                },
            };

            var idleStepsNames = new object[]
            {
                oper,
                State.StateType.IDLE,
                addingStates,
                new List<string> { emptyStepName, step1Name, step2Name },
            };

            var runStepsNames = new object[]
            {
                oper,
                State.StateType.RUN,
                addingStates,
                new List<string> { emptyStepName, runStep1Name, runStep2name}
            };

            var stopStepsNames = new object[]
            {
                oper,
                State.StateType.STOP,
                addingStates,
                new List<string> { emptyStepName, stopStep1Name },
            };

            var pauseStepsNames = new object[]
            {
                oper,
                State.StateType.PAUSE,
                addingStates,
                new List<string> { emptyStepName, pauseStep1Name,
                    pauseStep2Name }
            };

            var startingStepsNames = new object[]
            {
                oper,
                State.StateType.STARTING,
                addingStates,
                new List<string> { emptyStepName, step1Name, step3Name },
            };

            var pausingStepsNames = new object[]
            {
                oper,
                State.StateType.PAUSING,
                addingStates,
                new List<string> { emptyStepName, step5Name, step6Name },
            };

            var unpausingStepsNames = new object[]
            {
                oper,
                State.StateType.UNPAUSING,
                addingStates,
                new List<string> { emptyStepName, step6Name, step1Name },
            };

            var stoppingStepsNames = new object[]
            {
                oper,
                State.StateType.STOPPING,
                addingStates,
                new List<string> { emptyStepName, step3Name, step6Name },
            };

            var emptyStepsWrongStateType = new object[]
            {
                oper,
                (State.StateType)100,
                addingStates,
                new List<string>()
            };

            var emptyStepsNoStates = new object[]
            {
                oper,
                State.StateType.RUN,
                new Dictionary<string, List<BaseStep>>(),
                new List<string>()
            };

            return new object[]
            {
                idleStepsNames,
                runStepsNames,
                stopStepsNames,
                pauseStepsNames,
                startingStepsNames,
                pausingStepsNames,
                unpausingStepsNames,
                stoppingStepsNames,
                emptyStepsWrongStateType,
                emptyStepsNoStates
            };
        }

        [Test]
        public void Clone_StateUnderTestSource_ReturnsCopy()
        {
            string name = "Операция 1";
            string luaName = "LuaName 1";
            int defaultPosition = 1;
            var operation = new BaseOperation(null)
            {
                Name = name,
                LuaName = luaName,
                DefaultPosition = defaultPosition
            };

            var runStep1 = new BaseStep("fRun", "RunFirSt", 1, operation);
            var runStep2 = new BaseStep("sRun", "RunSecSt", 2, operation);
            var stopStep1 = new BaseStep("fSt", "StopFirSt", 0, operation);
            var pauseStep1 = new BaseStep("fP", "PauseFirst", 1, operation);
            var pauseStep2 = new BaseStep("sP", "PauseSecond", 0, operation);

            var addingStates = new Dictionary<string, List<BaseStep>>
            {
                {
                    State.StateType.RUN.ToString(),
                    new List<BaseStep> { runStep1, runStep2 }
                },
                {
                    State.StateType.PAUSE.ToString(),
                    new List<BaseStep> { pauseStep1, pauseStep2 }
                },
                {
                    State.StateType.STOP.ToString(),
                    new List<BaseStep> { stopStep1 }
                }
            };

            FillBaseOperationStatesWithReset(operation, addingStates);

            operation.AddActiveParameter("p1", "пар1", string.Empty);
            operation.AddActiveParameter("p2", "пар2", string.Empty);

            var baseTechObj = new BaseTechObject()
            {
                S88Level = 1
            };
            var objParams = new List<BaseParameter>()
                {
                    new ActiveParameter("op1", "обПар1"),
                    new ActiveParameter("op2", "обПар2"),
                };
            operation.AddProperties(objParams, baseTechObj);

            BaseOperation cloned = operation.Clone();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(operation.DefaultPosition,
                    cloned.DefaultPosition);
                Assert.AreEqual(operation.Name, cloned.Name);
                Assert.AreEqual(operation.LuaName, cloned.LuaName);
                Assert.IsNull(cloned.Owner);

                foreach (var property in operation.Properties)
                {
                    bool hasTheSameProperty = cloned.Properties
                        .Where(x => x.Name == property.Name &&
                        x.LuaName == property.LuaName &&
                        (x.Owner != property.Owner || x.Owner == baseTechObj))
                        .Count() == 1;
                    Assert.IsTrue(hasTheSameProperty);
                }

                foreach (var state in operation.States.Keys)
                {
                    Assert.True(cloned.States.ContainsKey(state));
                    foreach (var step in operation.States[state])
                    {
                        Assert.True(cloned.States[state]
                            .Where(x => x.Name == step.Name &&
                            x.LuaName == step.LuaName &&
                            x.DefaultPosition == step.DefaultPosition &&
                            x.Owner != step.Owner)
                            .Count() == 1);
                    }
                }
            });
        }

        private void FillBaseOperationStatesWithReset(BaseOperation oper,
            Dictionary<string, List<BaseStep>> addingStates)
        {
            oper.States.Clear();
            foreach (var state in addingStates.Keys)
            {
                foreach (var step in addingStates[state])
                {
                    oper.AddStep(state, step.LuaName, step.Name,
                        step.DefaultPosition);
                }
            }
        }
    }
}
