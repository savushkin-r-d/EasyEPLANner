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
            string defaultValue) : base(luaName, name, defaultValue)
        { }

        public override BaseParameter Clone()
        {
            var newProperty = new MainAggregateParameter(this.LuaName, 
                this.Name, this.DefaultValue);
            newProperty.SetValue(this.Value);
            return newProperty;
        }

        /// <summary>
        /// Проверка параметра
        /// </summary>
        public void Check()
        {
            if (Value == "false")
            {
                SetUpParametersVisibility();
            }
        }

        public override bool SetNewValue(string newValue)
        {
            var result = base.SetNewValue(newValue);
            SetUpParametersVisibility();
            return result;
        }

        /// <summary>
        /// Установка видимости параметров агрегата-объекта-владельца в аппарате
        /// </summary>
        private void SetUpParametersVisibility()
        {
            var parentBaseOperation = Parent as BaseOperation;
            var aggregateParameters = (Owner as BaseTechObject)
                .AggregateParameters;

            foreach (var parameter in aggregateParameters)
            {
                var foundProperty = parentBaseOperation.Properties
                    .Where(x => x.LuaName == parameter.LuaName)
                    .FirstOrDefault();
                if (foundProperty != null)
                {
                    if (Value == "false")
                    {
                        foundProperty.NeedDisable = true;
                    }
                    else
                    {
                        foundProperty.NeedDisable = false;
                    }
                }
            }
        }
    }
}
