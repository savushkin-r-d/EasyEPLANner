using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый float параметр
    /// </summary>
    public interface IBaseFloatParameter
    {
        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        double DefaultValue { get; }
        
        /// <summary>
        /// Lua-имя
        /// </summary>
        string LuaName { get; }

        /// <summary>
        /// Единицы измерения
        /// </summary>
        string Meter { get; }

        /// <summary>
        /// Название
        /// </summary>
        string Name { get; }
    }

    public class BaseFloatParameter : IBaseFloatParameter
    {
        public BaseFloatParameter(string luaName, string name, double defaultValue, string meter) 
        {
            LuaName = luaName;
            Name = name;
            DefaultValue = defaultValue;
            Meter = meter;
        }

        public double DefaultValue { get; private set; }
        
        public string LuaName { get; private set; }
        
        public string Meter { get; private set; }

        public string Name { get; private set; }
    }
}
