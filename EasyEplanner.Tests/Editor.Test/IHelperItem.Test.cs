using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using Editor;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using TechObject;

namespace EasyEplannerTests
{
    public class TestParameter : ObjectProperty
    {
        public TestParameter(string name, object value, object defaultValue = null) 
            : base(name, value, defaultValue)
        {
        }

        public override string SystemIdentifier => "test_sys_idtf";
    }


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
            var systemParams = new SystemParams();

            var equipment = new Equipment(null);

            var testParameter = new TestParameter("par", 0);

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
                Assert.AreEqual("process_parameter", systemParams.SystemIdentifier);

                Assert.AreEqual("control_module", equipment.SystemIdentifier);

                // Исключение, так как используется ссылка на API
                Assert.Throws<FileNotFoundException>(() => state.GetLinkToHelpPage());
                Assert.Throws<FileNotFoundException>(() => testParameter.GetLinkToHelpPage());
            });
        }



    }
}
