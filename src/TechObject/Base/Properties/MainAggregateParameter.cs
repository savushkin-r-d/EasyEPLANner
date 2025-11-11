using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    public class MainAggregateParameter : GroupableParameters, IAutocompletable
    {
        public MainAggregateParameter(string luaName, string name,
            string defaultValue, List<DisplayObject> displayObjects = null)
            : base(luaName, name, defaultValue, displayObjects, true) { }

        public override BaseParameter Clone()
        {
            var newProperty = new MainAggregateParameter(LuaName, 
                Name, DefaultValue, DisplayObjects);

            newProperty.Parameters = [.. Parameters.Select(p => {
                var clonedParameter = p.Clone();
                clonedParameter.Owner = newProperty;
                return clonedParameter;
            })];

            newProperty.SetValue(Value);
            return newProperty;
        }

        /// <summary>
        /// Проверка параметра
        /// </summary>
        public override void Check()
        {
            SetUpParametersVisibility();
        }

        public override bool SetNewValue(string newValue)
        {
            var succes = base.SetNewValue(newValue);
            SetUpParametersVisibility();

            return succes;
        }

        public override bool NeedDisable  => needDisable;

        /// <summary>
        /// Установка видимости параметров агрегата-объекта-владельца в аппарате
        /// </summary>
        public override void SetUpParametersVisibility()
        {
            var properties = BaseOperation.Properties;
            var aggregateParameters = (Owner as BaseTechObject).AggregateParameters;

            foreach (var parameter in aggregateParameters)
            {
                var foundProperty = properties
                    .FirstOrDefault(x => x.LuaName == parameter.LuaName && x.Owner == Owner);
                if (foundProperty != null)
                {
                    foundProperty.NeedDisable = Value == "false";
                    foundProperty.Visibility = Value == "true";
                }
            }
        }

        bool IAutocompletable.CanExecute => Value == "true";

        public void Autocomplete()
        {
            if (Value == "false")
                return;

            foreach (var baseParameter in (Owner as BaseTechObject).AggregateParameters)
            {
                var aggregateParameter = (Parent as BaseOperation).Properties
                    .FirstOrDefault(x => x.LuaName == baseParameter.LuaName && x.Owner == Owner);

                if (aggregateParameter is null)
                    continue;

                var paramsManager = BaseOperation.Owner.Owner.Owner.GetParamsManager();

                if (aggregateParameter is IActiveAggregateParameter activeAggregateParameter &&
                    !paramsManager.Float.HaveSameLuaName(aggregateParameter.Value))
                {
                    var baseFloatParameter = activeAggregateParameter.Parameter;
  
                    var paramLuaName = $"{BaseOperation.LuaName}_{baseFloatParameter.LuaName}";
                    var paramName = $"{BaseOperation.Name}. {baseFloatParameter.Name}";

                    aggregateParameter.SetValue(paramLuaName);

                    if (paramsManager.Float.HaveSameLuaName(paramLuaName))
                        continue;

                    var param = paramsManager.AddFloatParam(paramName,
                        baseFloatParameter.DefaultValue, baseFloatParameter.Meter, paramLuaName);
                    param.SetOperationN(BaseOperation.Owner.GetModeNumber());
                }
            }
        }


        private readonly bool needDisable = false;
    }
}
