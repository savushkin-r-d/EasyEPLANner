using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;
using NUnit.Framework;
using Moq;
using Editor;

namespace TechObjectTests
{
    public class ActionCustomTest
    {
        [Test]
        public void UpdateOnGenericTechObject()
        {
            var actionCustom = new ActionCustom("действие", null, "custom");
            var genericActionCustom = new ActionCustom("действие", null, "custom");

            var actionWashMock = new Mock<IAction>();
            actionWashMock.Setup(a => a.UpdateOnGenericTechObject(It.IsAny<IAction>()))
                .Callback<IAction>((sa) => Assert.AreSame(genericActionCustom.SubActions[0], sa));

            var activeParameterMock = new Mock<ActiveParameter>("param", "param", "", null);
            activeParameterMock.Setup(p => p.SetNewValue(It.IsAny<string>()))
                .Callback<string>((s) => Assert.AreEqual(genericActionCustom.Parameters[0].Value, s));

            actionCustom.CreateAction(new TechObject.Action("Устройства", null, "devs"));
            genericActionCustom.CreateAction(new TechObject.Action("Устройства", null, "devs"));

            actionCustom.CreateParameter(new ActiveParameter("param", "param"));
            genericActionCustom.CreateParameter(new ActiveParameter("param", "param"));

            actionCustom.SubActions[0] = actionWashMock.Object;
            actionCustom.Parameters[0] = activeParameterMock.Object;

            // null object
            actionCustom.UpdateOnGenericTechObject(null);
            // wrong type object
            actionCustom.UpdateOnGenericTechObject(new TechObject.Action("action", null, "action"));

            actionCustom.UpdateOnGenericTechObject(genericActionCustom);
        }
    }
}
