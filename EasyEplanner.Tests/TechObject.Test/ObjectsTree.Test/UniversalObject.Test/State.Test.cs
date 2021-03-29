using System.Linq;
using TechObject;
using NUnit.Framework;

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
    }
}
