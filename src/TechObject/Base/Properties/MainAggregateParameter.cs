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
            var newProperty = new MainAggregateParameter(LuaName, 
                Name, DefaultValue);
            newProperty.SetValue(Value);

            foreach (var displayObject in DisplayObjects)
            {
                newProperty.AddDisplayObject(displayObject);
            }

            return newProperty;
        }

        /// <summary>
        /// Проверка параметра
        /// </summary>
        public void Check()
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
