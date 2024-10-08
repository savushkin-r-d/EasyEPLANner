﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Editor;
using Moq;
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

        /// <summary>
        /// Тестированиме установки базовой операции с диалоговым окном сброса доп.свойств.
        /// </summary>
        /// <param name="oldValue">Старое значение базвой операции</param>
        /// <param name="newValue">Новое значение базовой операции</param>
        /// <param name="dialogResult">Результат диалогового окна сюроса доп.свойств</param>
        /// <param name="baseOperationChanged">Базовая операция изменена</param>
        /// <param name="extraPropertiesCloned">Значения доп.свойств операции клонировано
        /// (доп.свойства не сброшены)</param>
        /// <param name="expectedResult">Результат работы метода установки нового значения</param>
        [TestCase("", "BO1", null, true, false, true)]
        [TestCase("BO1", "", null, true, false, true)]
        [TestCase("BO1", "BO1", null, false, false, false)]
        [TestCase("BO1", "BO2", DialogResult.Cancel, false, false, false)]
        [TestCase("BO1", "BO2", DialogResult.Yes, true, false, true)]
        [TestCase("BO1", "BO2", DialogResult.No, true, true, true)]
        public void SetNewValue_CheckChangeOperation(string oldValue, string newValue, DialogResult dialogResult,
            bool baseOperationChanged, bool extraPropertiesCloned , bool expectedResult)
        {
            var TechObjectEditorMock = new Mock<Editor.IEditor>();
            TechObjectEditorMock.Setup(obj => obj.DialogResetExtraProperties()).Returns(dialogResult);
            TechObjectEditorMock.Setup(obj => obj.Editable).Returns(true);
            Mode.TechObjectEditor = TechObjectEditorMock.Object;


            var baseOperationMock = new Mock<IBaseOperation>();
            var extraProperties = new List<BaseParameter>() { new ActiveBoolParameter(string.Empty, string.Empty, "true") };

            baseOperationMock.Setup(obj => obj.Name).Returns(oldValue);
            baseOperationMock.Setup(obj => obj.Properties).Returns(extraProperties);

            var operation = new Mode("операция", getN => 1, new ModesManager(null), baseOperationMock.Object);

            Assert.Multiple(() =>
            {
                string settedBaseOperationName = null;
                List<BaseParameter> clonedExtraProperties = null;


                baseOperationMock.Setup(obj => obj.Init(It.IsAny<string>(), It.IsAny<Mode>()))
                    .Callback<string, Mode>((baseOp, op) => settedBaseOperationName = baseOp);

                baseOperationMock.Setup(obj => obj.SetExtraProperties(It.IsAny<List<BaseParameter>>()))
                    .Callback<List<BaseParameter>>((clone) => clonedExtraProperties = clone);

                bool result = operation.SetNewValue(newValue, true);

                Assert.AreEqual(expectedResult, result);
                Assert.AreEqual(baseOperationChanged, 
                    settedBaseOperationName != null &&
                    settedBaseOperationName != oldValue);
                Assert.AreEqual(extraPropertiesCloned,
                    clonedExtraProperties != null &&
                    clonedExtraProperties != extraProperties &&
                    clonedExtraProperties.SequenceEqual(extraProperties));
            });  
        }

        [Test]
        public void UpdateOnGenericTechObject()
        {
            bool stateUpdateMethodCalled = false;

            var mode = new Mode("operation", GetN => 1, null);
            var genericMode = new Mode("operation", GetN => 1, null);

            var stateMock = new Mock<State>(State.StateType.IDLE, mode, false);
            stateMock.Setup(s => s.UpdateOnGenericTechObject(It.IsAny<ITreeViewItem>()))
                .Callback<ITreeViewItem>(t =>
                {
                    Assert.AreSame(genericMode[0], t);
                    stateUpdateMethodCalled = true;
                });

            mode.States[0] = stateMock.Object;

            Assert.Multiple(() =>
            {
                mode.UpdateOnGenericTechObject(genericMode);
                Assert.IsTrue(stateUpdateMethodCalled);
            });
            
        }

        [Test]
        public void UpdateOnGenericTechObject_NullValue()
        {
            bool stateUpdateMethodCalled = false;

            var mode = new Mode("operation", GetN => 1, null);
            var stateMock = new Mock<State>(State.StateType.IDLE, mode, false);
            stateMock.Setup(s => s.UpdateOnGenericTechObject(null))
                .Callback<ITreeViewItem>(t =>
                {
                    stateUpdateMethodCalled = true;
                });

            mode.States[0] = stateMock.Object;

            mode.UpdateOnGenericTechObject(null);
            Assert.IsTrue(stateUpdateMethodCalled);
        }

        [Test]
        public void GetBaseObjectList()
        {

            var baseTechObject = new BaseTechObject()
            {
                Name = "base_tech_object",
            };
            baseTechObject.AddBaseOperation("operation_1", "операция 1", 0);
            baseTechObject.AddBaseOperation("operation_2", "операция 2", 0);
            baseTechObject.AddBaseOperation("operation_3", "операция 3", 0);

            var techObject = new TechObject.TechObject("", getN => 1, 1, 1, "", -1, "", "", baseTechObject);
            var modesManager = new ModesManager(techObject);

            var mode = modesManager.AddMode("", "");

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(new List<string>() { "", "операция 1", "операция 2", "операция 3" }, mode.BaseObjectsList);
                
                modesManager.AddMode("операция 2", "operation_2");
                CollectionAssert.AreEqual(new List<string>() { "", "операция 1", "операция 3" }, mode.BaseObjectsList);
            });
        }
    }
}
