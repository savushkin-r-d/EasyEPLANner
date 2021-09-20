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
            var result = base.SetNewValue(newValue);
            SetUpParametersVisibility();
            return result;
        }

        public override bool NeedDisable
        {
            get
            {
                return false;
            }
            set 
            {
                // Для этого параметра не предусмотрена установка значения
            }
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
