using EasyEPlanner;
using Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LuaInterface;
using System.Windows.Forms;
using TechObject;
using System.Text.RegularExpressions;

namespace Recipe
{
    public interface IRecipesManager
    {
        /// <summary>
        /// Загрузка ограничений проекта
        /// </summary>
        /// <param name="LuaStr">Описание ограничений</param>
        void LoadRecipes(string LuaStr);

        /// <summary>
        /// Сохранить описание проекта
        /// </summary>
        /// <param name="prefixStr">Отступ</param>
        /// <returns></returns>
        string SaveAsLuaTable(string prefixStr);

    }

}
