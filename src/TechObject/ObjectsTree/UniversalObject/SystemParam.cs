using Editor;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

            items = [this.value, this.meter];

            LuaName = nameLua ?? "";
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + $"{LuaName} =\n";
            res += prefix + "\t{\n";
            res += prefix + "\tvalue = " + value.Value + ",\n";
            res += prefix + "\t},\n";
            return res;
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText => [
            $"{getN(this)}. {name} - {value.Value} {meter.Value}.",
            LuaName];

        public override bool IsEditable => true;

        public override int[] EditablePart => [0, 1];

        public override string[] EditText => [name, LuaName];

        override public ITreeViewItem[] Items => [.. items];
        #endregion

        public string Name => name;

        public string LuaName { get; private set; }

        public string Meter => meter.Value;

        public ParamProperty Value => value;

        private GetN getN;

        private string name;
        private List<ITreeViewItem> items;    ///Данные для редактирования.
        private ParamProperty value;          ///Значение.
        private ParamProperty meter;          ///Размерность.
    }
}
