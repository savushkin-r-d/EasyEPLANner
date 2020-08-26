using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTechObject
{
    /// <summary>
    /// Интерфейс класса TechObjectManager
    /// </summary>
    interface ITechObjectManager
    {
        /// <summary>
        /// Загрузка описания проекта из строки Lua
        /// </summary>
        /// <param name="LuaStr">Строка с описанием</param>
        /// <param name="projectName">Имя проекта</param>
        void LoadDescription(string LuaStr, string projectName);

        /// <summary>
        /// Загрузка ограничений проекта
        /// </summary>
        /// <param name="LuaStr">Описание ограничений</param>
        void LoadRestriction(string LuaStr);
    }
}
