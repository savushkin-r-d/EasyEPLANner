using LuaInterface;

namespace StaticHelper
{
    public static class LuaHelper
    {
        /// <summary>
        /// Конвертация Lua-таблицы в массив свойств, новый редактор
        /// </summary>
        /// <param name="table">Lua таблица</param>
        /// <returns></returns>
        public static Editor.ObjectProperty[] ConvertLuaTableToCArray(
            LuaTable table)
        {
            var Keys = new string[table.Values.Count];
            var Values = new string[table.Values.Count];
            var res = new Editor.ObjectProperty[Keys.Length];

            table.Values.CopyTo(Values, 0);
            table.Keys.CopyTo(Keys, 0);

            for (int i = 0; i < Keys.Length; i++)
            {
                res[i] = new Editor.ObjectProperty(Keys[i], Values[i]);
            }

            return res;
        }

        /// <summary>
        /// Конвертация Lua-таблицы в строку с разделителем - пробел.
        /// </summary>
        /// <param name="table">Lua таблица</param>
        /// <returns></returns>
        public static string ConvertLuaTableToString(object table)
        {
            var luaTable = table as LuaTable;
            var values = new object[luaTable.Values.Count];
            luaTable.Values.CopyTo(values, 0);

            var result = string.Empty;
            foreach (object obj in values)
            {
                result += obj.ToString() + " ";
            }
            
            return result.Trim();
        }
    }
}
