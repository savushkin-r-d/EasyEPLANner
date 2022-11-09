using EasyEPlanner.Extensions;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.EServices.Ged;
using System.Linq;
using TechObject;

namespace EasyEPlanner
{
    [Interaction(Name = "XMIaInsertMacro", NameOfBaseInteraction = "XMIaInsertMacro", Ordinal = 51, Prio = 21)]
    public class InsertMacroInteraction : InsertInteraction
    {
        public override RequestCode OnStart(InteractionContext oContext)
        {
            return base.OnStart(oContext);
        }

        public override void OnSuccess(InteractionContext result)
        {
            base.OnSuccess(result);

            string baseObjectLuaName = FindBaseObjectLuaNameInMainSymbol();
            if (!string.IsNullOrEmpty(baseObjectLuaName))
            {
                InsertNewObject(baseObjectLuaName);
            }
        }

        private string FindBaseObjectLuaNameInMainSymbol()
        {
            if (InsertedItems?.Length <= 0)
            {
                return string.Empty;
            }

            var functionsInStorableObjects = InsertedItems
                .Where(x => x is Function func &&
                func.IsMainFunction == true &&
                func.Category == Function.Enums.Category.FunctionalFunction);
            if(functionsInStorableObjects?.Count() == 0)
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
                    return function.GetFunctionalText();
                }
            }

            return string.Empty;
        }

        private void InsertNewObject(string baseObjectLuaName)
        {
            ITechObjectManager techObjectManager = TechObjectManager
                .GetInstance();
            techObjectManager.InsertBaseObject(baseObjectLuaName);
        }

        const string DefinedMacrosName = "Macros definition";
    }
}
