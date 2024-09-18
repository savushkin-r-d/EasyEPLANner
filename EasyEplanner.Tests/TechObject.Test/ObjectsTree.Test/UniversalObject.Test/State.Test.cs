using System.Linq;
using TechObject;
using NUnit.Framework;
using Moq;
using Editor;
using System;
using System.Collections.Generic;

namespace EasyEplanner.Tests
{
    class StateTest
    {
        [TestCase(false, new string[] { "st1", "st2", "st3" }, 4)]
        [TestCase(true, new string[] { "st1", "st2", "st3" }, 4)]
        public void AddStep_EmptyState_AddNewStepsFirstMain(bool needMainStep,
            string[] stepNames, int expectedStepsCount)
        {
            var state = new State(State.StateType.STOP, null, needMainStep);

            foreach(var stepName in stepNames)
            {
                state.AddStep(stepName, string.Empty);
            }

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedStepsCount, state.Steps.Count);
                Assert.IsTrue(state.Steps.First().GetStepName()
                    .Equals(Step.MainStepName));
                foreach(var stepName in stepNames)
                {
                    Assert.IsTrue(state.Steps
                        .Any(x => x.GetStepName() == stepName));
                }
            });
        }

        [Test]
        public void UpdateOnGenericTechObject()
        {
            bool setNewValueMethodCalled = false;
            bool setNewBaseStepMethodCalled = false;

            string baseStep = "BaseStep";

            var mode = new Mode("operation", GetN => 1, null,
                new BaseOperation("baseOperation", "baseOperation", new List<BaseParameter>() { },
                new Dictionary<string, List<BaseStep>>()
                {
                    { "IDLE", new List<BaseStep>() { new BaseStep(baseStep, baseStep) } },
                }));
           

            var state = new State(State.StateType.IDLE, mode);
            var genericState = new State(State.StateType.IDLE, mode);

            genericState.AddStep("Шаг 1", "Step 1");
            genericState.AddStep("Шаг 2", "Step 2");

            genericState.Steps[1].SetNewValue(baseStep, true);

            var stepMock = new Mock<Step>("Шаг 1", new GetN((getN) => 1), state, false);
            stepMock.Setup(x => x.SetNewValue(It.IsAny<string>()))
                .Callback<string>((newValue) =>
                {
                    Assert.AreEqual("Шаг 1", newValue);
                    setNewValueMethodCalled = true;
                });
            stepMock.Setup(x => x.SetNewValue(It.IsAny<string>(), true))
                .Callback<string, bool>((newValue, _) =>
                {
                    Assert.AreEqual(baseStep, newValue);
                    setNewBaseStepMethodCalled = true;
                });


            state.AddStep("Шаг 1", "Step 1");

            state.Steps[1] = stepMock.Object;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, state.Steps.Count);
                state.UpdateOnGenericTechObject(genericState);
                Assert.AreEqual(3, state.Steps.Count);
                Assert.IsTrue(setNewValueMethodCalled);
                Assert.IsTrue(setNewBaseStepMethodCalled);
            });
        }

        [Test]
        public void UpdateOnGenericTechObject_NullValue()
        {
            bool stepUpdateNullCalled = false;

            var state = new State(State.StateType.IDLE, null);
            state.AddStep("", "");

            var stepMock = new Mock<Step>("Шаг 1", new GetN((getN) => 1), state, false);
            stepMock.Setup(s => s.UpdateOnGenericTechObject(null))
                .Callback<ITreeViewItem>((_) => { stepUpdateNullCalled = true; });

            state.Steps[1] = stepMock.Object;

            state.UpdateOnGenericTechObject(null);

            Assert.IsTrue(stepUpdateNullCalled);
        }

        [Test]
        public void Move()
        {
            var state = new State(State.StateType.IDLE, null, true);
            var step1 = state.Insert();
            var step2 = state.Insert();

            Assert.Multiple(() =>
            {
                Assert.AreSame(step2, state.MoveUp(step2));
                Assert.IsNull(state.MoveUp(step2));
                Assert.IsNull(state.MoveUp(0));

                Assert.AreSame(step2, state.MoveDown(step2));
                Assert.IsNull(state.MoveDown(step2));
                Assert.IsNull(state.MoveDown(0));
            });
        }

        [Test]
        public void InsertCopyAndReplace()
        {
            var operation = new Mode("Операция", getN => 1, null,
                new BaseOperation(
                    "операция", "operation",
                    new List<BaseParameter>() { },
                    new Dictionary<string, List<BaseStep>>() 
                    {
                        { "RUN", new List<BaseStep>() { new BaseStep("", ""), new BaseStep("шаг_1", "step_1"), new BaseStep("шаг_2", "step_2") } },
                    }
                )
            );

            var state = new State(State.StateType.RUN, operation, true);

            var copied_step_1 = new Step("шаг_1", getN => 1, state);
            copied_step_1.SetNewValue("step_1", false);
            var copied_step_2 = new Step("шаг_2", getN => 1, state);
            copied_step_2.SetNewValue("step_2", false);
            var copied_step_no_base = new Step("шаг", getN => 1, state);

            Assert.Multiple(() =>
            {
                // Вставка базового шага 1
                var insertedStep_1 = state.InsertCopy(copied_step_1) as Step;
                Assert.IsNotNull(insertedStep_1);
                Assert.AreNotSame(copied_step_1, insertedStep_1);
                Assert.AreEqual(copied_step_1.GetBaseStepLuaName(), insertedStep_1.GetBaseStepLuaName());

                // Повторная вставка базового шага 1 (canceled)
                var insertedStep_null = state.InsertCopy(copied_step_1) as Step;
                Assert.IsNull(insertedStep_null);

                // no step
                insertedStep_null = state.InsertCopy(null) as Step;
                Assert.IsNull(insertedStep_null);

                // Вставка не базового шага
                var insertedStep_no_base = state.InsertCopy(copied_step_no_base) as Step;
                Assert.IsNotNull(insertedStep_no_base);
                Assert.AreNotSame(copied_step_no_base, insertedStep_no_base);
                Assert.AreEqual(copied_step_no_base.GetBaseStepLuaName(), insertedStep_no_base.GetBaseStepLuaName());

                // Замена не базового шага базовым шагом 2
                var replacedStep_2 = state.Replace(insertedStep_no_base, copied_step_2) as Step;
                Assert.IsNotNull(replacedStep_2);
                Assert.AreNotSame(copied_step_2, replacedStep_2);
                Assert.AreEqual(copied_step_2.GetBaseStepLuaName(), replacedStep_2.GetBaseStepLuaName());

                // Замена предыдущего шага на уже имеющийся шаг 1 (canceled)
                var replacedStep_null = state.Replace(replacedStep_2, copied_step_1) as Step;
                Assert.IsNull(replacedStep_null);

                // no step
                replacedStep_null = state.Replace(null, null) as Step;
                Assert.IsNull(replacedStep_null);
            });
        }
    }
}
