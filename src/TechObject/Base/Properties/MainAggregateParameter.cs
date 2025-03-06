using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    public class MainAggregateParameter : ActiveBoolParameter
    {
        public MainAggregateParameter(string luaName, string name,
            string defaultValue, List<DisplayObject> displayObjects = null)
            : base(luaName, name, defaultValue, displayObjects) { }

        public override BaseParameter Clone()
        {
            var newProperty = new MainAggregateParameter(LuaName, 
                Name, DefaultValue, DisplayObjects);
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

            if (succes && Value is "true")
                Autocomplete();

            return succes;
        }

        public override bool NeedDisable => needDisable;

        /// <summary>
        /// Установка видимости параметров агрегата-объекта-владельца в аппарате
        /// </summary>
        private void SetUpParametersVisibility()
        {
            var properties = (Parent as BaseOperation).Properties;
            var aggregateParameters = (Owner as BaseTechObject).AggregateParameters;

            foreach (var parameter in aggregateParameters)
            {
                var foundProperty = properties
                    .FirstOrDefault(x => x.LuaName == parameter.LuaName && x.Owner == Owner);
                if (foundProperty != null)
                {
                    foundProperty.NeedDisable = Value is "false";
                }
            }
        }

        /// <summary>
        /// Автоматическое заполнение связанных параметров агрегата.
        /// Добавление float-параметра к операции с автоматической привязкой.
        /// </summary>
        private void Autocomplete()
        {
            foreach (var baseParameter in (Owner as BaseTechObject).AggregateParameters)
            {
                var aggregateParameter = (Parent as BaseOperation).Properties
                    .FirstOrDefault(x => x.LuaName == baseParameter.LuaName && x.Owner == Owner);

                if (aggregateParameter is IActiveAggregateParameter activeAggregateParameter &&
                    aggregateParameter.Value == string.Empty)
                {
                    var baseFloatParameter = activeAggregateParameter.Parameter;
                    var baseOperation = aggregateParameter.Parent as BaseOperation;

                    var paramLuaName = $"{baseOperation.LuaName}_{baseFloatParameter.LuaName}";
                    var paramName = $"{baseOperation.Name}. {baseFloatParameter.Name}";

                    aggregateParameter.SetValue(paramLuaName);

                    var paramsManager = baseOperation.Owner.Owner.Owner.GetParamsManager();

                    if (paramsManager.Float.HaveSameLuaName(paramLuaName))
                        continue;

                    var param = paramsManager.AddFloatParam(paramName,
                        baseFloatParameter.DefaultValue, baseFloatParameter.Meter, paramLuaName);
                    param.SetOperationN(baseOperation.Owner.GetModeNumber());
                }
            }
        }


        private readonly bool needDisable = false;
    }
}
