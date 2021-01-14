using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.EServices.Ged;
using System.Linq;

namespace EasyEPlanner
{
    [Interaction(Name = "XMIaInsertMacro", NameOfBaseInteraction = "XMIaInsertMacro", Ordinal = 51, Prio = 21)]
    public class XMIaInsertMacroDerived : InsertInteraction
    {
        public override RequestCode OnStart(InteractionContext oContext)
        {
            return base.OnStart(oContext);
        }

        public override void OnSuccess(InteractionContext result)
        {
            base.OnSuccess(result);

            var storableFunctions = InsertedItems.Where(x => x is Function);
            var castedFunctions = storableFunctions.Cast<Function>().ToArray();
            foreach (var function in castedFunctions)
            {
                string field = function.Properties.FUNC_SUPPLEMENTARYFIELD[100].ToString(ISOCode.Language.L___, "");
                if (!string.IsNullOrEmpty(field))
                {
                    System.Windows.Forms.MessageBox.Show(field);
                    // Test implementation
                }
            }
        }
    }
}
