using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Активный параметр агрегата
    /// </summary>
    public interface IActiveAggregateParameter
    {
        /// <summary>
        /// Параметр, создаваемый по-умолчанию
        /// </summary>
        IBaseFloatParameter Parameter { get; }

        /// <summary>
        /// Установить параметр
        /// </summary>
        /// <param name="luaName">Lua-название</param>
        /// <param name="name">Название</param>
        /// <param name="defaultValue">Значение по умолчанию/param>
        /// <param name="meter">Единицы измерения</param>
        void SetFloatParameter(string luaName, string name, double defaultValue, string meter);
    }

    public class ActiveAggregateParameter : ActiveParameter, IActiveAggregateParameter
    {
        private IBaseFloatParameter parameter;

        public ActiveAggregateParameter(string luaName, string name,
            string defaultValue = "", List<DisplayObject> displayObjects = null) 
            : base(luaName, name, defaultValue, displayObjects)
        {

        }

        public void SetFloatParameter(string luaName, string name, double defaultValue, string meter)
        {
            parameter = new BaseFloatParameter(luaName, name, defaultValue, meter);
        }

        public override BaseParameter Clone()
        {
            var clone = new ActiveAggregateParameter(LuaName, Name,
                DefaultValue, DisplayObjects)
            {
                NeedDisable = NeedDisable,
                OneValueOnly = OneValueOnly,
                Parameter = parameter
            };
            clone.SetNewValue(Value);

            return clone;
        }

        public IBaseFloatParameter Parameter
        {
            get => parameter;
            private set => parameter = value;
        }
    }
}
