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

        /// <summary>
        /// Сохранить описание проекта
        /// </summary>
        /// <param name="prefixStr">Отступ</param>
        /// <returns></returns>
        string SaveAsLuaTable(string prefixStr);

        /// <summary>
        /// Сохранить ограничения проекта
        /// </summary>
        /// <param name="prefixStr">Отступ</param>
        /// <returns></returns>
        string SaveRestrictionAsLua(string prefixStr);

        /// <summary>
        /// Глобальный список объектов
        /// </summary>
        List<TechObject> Objects { get; }

        /// <summary>
        /// Получить объект по его глобальному номеру
        /// </summary>
        /// <param name="globalNum">Глобальный номер</param>
        /// <returns></returns>
        TechObject GetTObject(int globalNum);

        /// <summary>
        /// Получить глобальный номер объекта
        /// </summary>
        /// <param name="techObject">Объект</param>
        /// <returns></returns>
        int GetTechObjectN(object techObject);
    }
}
