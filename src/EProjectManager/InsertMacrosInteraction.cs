using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.EServices.Ged;
using System.Collections.Generic;
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

            string baseObjectLuaName = FindBaseObjectLuaNameInMacroses();
            if (!string.IsNullOrEmpty(baseObjectLuaName))
            {
                InsertNewObject(baseObjectLuaName);
            }
        }

        private string FindBaseObjectLuaNameInMacroses()
        {
            IEnumerable<Function> macrosFunctions = InsertedItems
                .Where(x => x is Function func &&
                func.IsMainFunction == true &&
                func.Category == Function.Enums.Category.FunctionalFunction)
                .ToArray()
                .Cast<Function>();
            foreach (var function in macrosFunctions)
            {
                string symbolDescription = function.Properties.FUNC_SYMB_DESC
                    .ToString(ISOCode.Language.L___, string.Empty);
                bool foundMacros = !string.IsNullOrEmpty(symbolDescription) &&
                    symbolDescription == DefinedMacrosName;
                if (foundMacros)
                {
                    return StaticHelper.ApiHelper.GetFunctionalText(function);
                }
            }

            return string.Empty;
        }

        private void InsertNewObject(string baseObjectLuaName)
        {
            // TODO: Insert object mechanism.
        }

        const string DefinedMacrosName = "Macros definition";
    }
}
