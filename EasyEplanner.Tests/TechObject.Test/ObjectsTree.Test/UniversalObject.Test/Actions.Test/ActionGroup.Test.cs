using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;
using NUnit.Framework;
using Moq;

namespace TechObjectTests
{
    public class ActionGroupTest
    {
        [Test]
        public void UpadateOnGenericTechObject()
        {
            var actionGroup = new ActionGroup("Устройства", null, "devices", null, null);
            var genericActionGroup = new ActionGroup("Устройства", null, "devices", null, null);

            genericActionGroup.Insert();

            var actionWashMock = new Mock<IAction>();
            actionWashMock.Setup(a => a.UpdateOnGenericTechObject(It.IsAny<IAction>()))
                .Callback<IAction>((sa) => Assert.AreSame(genericActionGroup.SubActions[0], sa));

            actionGroup.SubActions[0] = actionWashMock.Object;

            Assert.Multiple(() =>
            {
                // null object
                actionGroup.UpdateOnGenericTechObject(null);
                // wrong type object
                actionGroup.UpdateOnGenericTechObject(new TechObject.Action("action", null, "action"));

                Assert.AreEqual(1, actionGroup.SubActions.Count());

                actionGroup.UpdateOnGenericTechObject(genericActionGroup);

                Assert.AreEqual(2, actionGroup.SubActions.Count());
            });
        }
    }
}
