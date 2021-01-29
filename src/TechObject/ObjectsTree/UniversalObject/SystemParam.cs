using System.Collections.Generic;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Отдельный параметр технологического объекта.
    /// </summary>
    public class SystemParam : TreeViewItem
    {
        public SystemParam(GetN getN, string name, double value = 0,
            string meter = "шт", string nameLua = "")
        {
            this.name = name;
            this.getN = getN;

            bool editable = false;

            this.value = new ParamProperty("Значение", value);
            this.meter = new ParamProperty("Размерность", meter,
                string.Empty, editable);
            this.nameLua = new ParamProperty("Lua имя",
                nameLua, string.Empty, editable);

            items = new List<ITreeViewItem>();
            items.Add(this.value);
            items.Add(this.meter);
            items.Add(this.nameLua);
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + $"{nameLua.Value} =\n";
            res += prefix + "\t{\n";
            res += prefix + "\tvalue = " + value.Value + ",\n";
            res += prefix + "\t},\n";
            return res;
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = "";
                res = $"{getN(this)}. {name} - {value.Value} " +
                    $"{meter.Value}.";

                return new string[] { res, "" };
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }
        #endregion

        public string Name => name;

        public string LuaName => nameLua.Value;

        public string Meter => meter.Value;

        public ParamProperty Value => value;

        private GetN getN;

        private string name;
        private List<ITreeViewItem> items;    ///Данные для редактирования.
        private ParamProperty nameLua;        ///Имя в Lua.
        private ParamProperty value;          ///Значение.
        private ParamProperty meter;          ///Размерность.
    }
}
