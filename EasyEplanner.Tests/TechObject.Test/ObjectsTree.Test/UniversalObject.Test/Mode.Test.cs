using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TechObject;

namespace EasyEplannerTests.TechObjectTest.ObjectsTreeTest.UniversalObjectTest
{
    public class ModeTest
    {
        [Test]
        public void SaveAsLuaTable_CheckResultString()
        {
            Mode mode = new Mode("TestMode", GetN => 1, null);
            mode.AddStep((int)State.StateType.RUN, "step run 1", "step run lua 1");
            mode.AddStep((int)State.StateType.IDLE, "step idle 1", "step idle lua 1");

            var prefix = "\t";
            var expectedResultStrBuilder = new StringBuilder()
                .Append($"{prefix}{{\n")
                .Append($"{prefix}name = 'TestMode',\n")
                .Append($"{prefix}states =\n")
                .Append($"{prefix}\t{{\n")
                .Append($"{prefix}\t[ 0 ] =\n")
                .Append($"{prefix}\t\t{{\n")
                .Append($"{prefix}\t\t--'Простой'\n")
                .Append($"{prefix}\t\tsteps =\n")
                .Append($"{prefix}\t\t\t{{\n")
                .Append($"{prefix}\t\t\t[ 1 ] =\n")
                .Append($"{prefix}\t\t\t\t{{\n")
                .Append($"{prefix}\t\t\t\tname = 'step idle 1',\n")
                .Append($"{prefix}\t\t\t\ttime_param_n = -1,\n")
                .Append($"{prefix}\t\t\t\tnext_step_n = -1,\n")
                .Append($"{prefix}\t\t\t\t}},\n")
                .Append($"{prefix}\t\t\t}},\n")
                .Append($"{prefix}\t\t}},\n")
                .Append($"{prefix}\t[ 1 ] =\n")
                .Append($"{prefix}\t\t{{\n")
                .Append($"{prefix}\t\t--'Выполнение'\n")
                .Append($"{prefix}\t\tsteps =\n")
                .Append($"{prefix}\t\t\t{{\n")
                .Append($"{prefix}\t\t\t[ 1 ] =\n")
                .Append($"{prefix}\t\t\t\t{{\n")
                .Append($"{prefix}\t\t\t\tname = 'step run 1',\n")
                .Append($"{prefix}\t\t\t\ttime_param_n = -1,\n")
                .Append($"{prefix}\t\t\t\tnext_step_n = -1,\n")
                .Append($"{prefix}\t\t\t\t}},\n")
                .Append($"{prefix}\t\t\t}},\n")
                .Append($"{prefix}\t\t}},\n")
                .Append($"{prefix}\t}},\n")
                .Append($"{prefix}}},\n");
            
            var actualResult = mode.SaveAsLuaTable(prefix);


            Assert.AreEqual(expectedResultStrBuilder.ToString(), actualResult);
        }

        [Test]
        public void AddStep_CheckStepsCount()
        {
            Mode mode = new Mode("TestMode", GetN => 1, null);
            mode.AddStep((int)State.StateType.RUN, "step run 1", "step run lua 1");
            mode.AddStep((int)State.StateType.STARTING, "step starting 1", "step starting lua 1");

            Assert.Multiple(() =>
            {
                Assert.IsTrue(mode.States.FirstOrDefault(state => state.Type == State.StateType.RUN).Steps.Count > 1);
                Assert.IsFalse(mode.States.FirstOrDefault(state => state.Type == State.StateType.IDLE).Steps.Count > 1);
                Assert.IsFalse(mode.States.FirstOrDefault(state => state.Type == State.StateType.STOP).Steps.Count > 1);
                Assert.IsTrue(mode.States.FirstOrDefault(state => state.Type == State.StateType.STARTING).Steps.Count > 1);
            });

        }

        [Test]
        public void IndexerGetState_CheckStateTypeAndStepsCount()
        {
            Mode mode = new Mode("TestMode", GetN => 1, null);
            mode.AddStep((int)State.StateType.RUN, "step run 1", "step run lua 1");
            mode.AddStep((int)State.StateType.STARTING, "step starting 1", "step starting lua 1");

            State idle = mode[0];
            State run = mode[1];
            State starting = mode[10];
            State pausing = mode[11];

            Assert.Multiple(() =>
            {
                Assert.IsTrue(idle.Type == State.StateType.IDLE);
                Assert.IsTrue(run.Type == State.StateType.RUN);
                Assert.IsTrue(starting.Type == State.StateType.STARTING);
                Assert.IsTrue(pausing.Type == State.StateType.PAUSING);

                Assert.IsFalse(idle.Steps.Count > 1);
                Assert.IsTrue(run.Steps.Count > 1);
                Assert.IsTrue(starting.Steps.Count > 1);
                Assert.IsFalse(pausing.Steps.Count > 1);
            });
        }
    }
}
