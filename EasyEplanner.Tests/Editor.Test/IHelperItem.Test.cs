using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using TechObject;

namespace EasyEplannerTests
{
    public class IHelperItemTest
    {
        [Test]
        public void CheckItemsSystemIdentifier()
        {
            var processCell = new ProcessCell(TechObjectManager.GetInstance());
            var unit = new S88Object("Аппарат", TechObjectManager.GetInstance());
            var aggregate = new S88Object("Агрегат", TechObjectManager.GetInstance());

            var operations = new ModesManager(null);
            var operation = new Mode("", getN => 1, null);

            var state = new State(State.StateType.RUN, operation);
            var phase = new Step("", getN => 1, state);
            var action = new TechObject.Action("", phase, "");
            
            
            var parameters = new Params("", "", false, "");
            var parametersManager = new ParamsManager();
            var operationParams = new OperationParams();


            Assert.Multiple(() => 
            {
                Assert.AreEqual("process_cell", processCell.SystemIdentifier);
                Assert.AreEqual("unit", unit.SystemIdentifier);
                Assert.AreEqual("equipment_module", aggregate.SystemIdentifier);

                Assert.AreEqual("operation", operations.SystemIdentifier);
                
                Assert.AreEqual("state", state.SystemIdentifier);
                Assert.AreEqual("phase", phase.SystemIdentifier);
                Assert.AreEqual("process_action", action.SystemIdentifier);

                Assert.AreEqual("process_parameter", parameters.SystemIdentifier);
                Assert.AreEqual("process_parameter", parametersManager.SystemIdentifier);
                Assert.AreEqual("process_parameter", operationParams.SystemIdentifier);
            });
        }

    }
}
