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

            string baseObjectLuaName = FindBaseObjectLuaNameInGeneralSymbol();
            if (!string.IsNullOrEmpty(baseObjectLuaName))
            {
                InsertNewObject(baseObjectLuaName);
            }
        }

        private string FindBaseObjectLuaNameInGeneralSymbol()
        {
            if (InsertedItems?.Length <= 0)
            {
                return string.Empty;
            }

            var functionsInStorableObjects = InsertedItems
                .Where(x => x is Function func &&
                func.IsMainFunction == true &&
                func.Category == Function.Enums.Category.FunctionalFunction);
            if(functionsInStorableObjects?.Count() < 0)
            {
                return string.Empty;
            }

            var castedFunctions = functionsInStorableObjects.Cast<Function>();
            foreach (var function in castedFunctions)
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
            System.Windows.Forms.MessageBox.Show("Tylko jedno w glowie mam");
        }

        const string DefinedMacrosName = "Macros definition";
    }
}
