using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Таймеры технологического объекта.
    /// </summary>
    public class TimersManager : Editor.TreeViewItem
    {
        public TimersManager() { }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            if (cnt == 0)
            {
                return "";
            }

            string res = prefix + "timers = " + cnt.ToString() + ",\n";
            return res;
        }

        /// <summary>
        /// Количество таймеров.
        /// </summary>
        public int Count
        {
            get
            {
                return cnt;
            }
            set
            {
                cnt = value;
            }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = string.Format("Таймеры ({0})", cnt);

                return new string[] { res, "" };
            }
        }

        override public bool SetNewValue(string newName)
        {
            try
            {
                cnt = Convert.ToInt32(newName);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        override public bool IsEditable
        {
            get
            {
                return true;
            }
        }

        override public int[] EditablePart
        {
            get
            {
                //Можем редактировать содержимое первой колонки.
                return new int[] { 0, -1 };
            }
        }

        override public string[] EditText
        {
            get
            {
                return new string[] { cnt.ToString(), "" };
            }
        }
        #endregion

        private int cnt; ///< Количество таймеров.
    }
}
