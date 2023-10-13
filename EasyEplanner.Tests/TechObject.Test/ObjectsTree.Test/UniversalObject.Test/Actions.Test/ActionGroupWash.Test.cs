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
    public class ActionGroupWashTest
    {

        [Test]
        public void UpdateOnGenerictTechObject()
        {
            var actionGroupWash = new ActionGroupWash("Устройства", null, "devices");
            var genericActionGroupWash = new ActionGroupWash("Устройства", null, "devices");

            genericActionGroupWash.Insert();

            var actionWashMock = new Mock<IAction>();
            actionWashMock.Setup(a => a.UpdateOnGenericTechObject(It.IsAny<IAction>()))
                .Callback<IAction>((sa) => Assert.AreSame(genericActionGroupWash.SubActions[0], sa));

            actionGroupWash.SubActions[0] = actionWashMock.Object;

            Assert.Multiple(() =>
            {
                // null object
                actionGroupWash.UpdateOnGenericTechObject(null);
                // wrong type object
                actionGroupWash.UpdateOnGenericTechObject(new TechObject.Action("action", null, "action"));

                Assert.AreEqual(1, actionGroupWash.SubActions.Count());

                actionGroupWash.UpdateOnGenericTechObject(genericActionGroupWash);

                Assert.AreEqual(2, actionGroupWash.SubActions.Count());
            });
        }
    }
}
