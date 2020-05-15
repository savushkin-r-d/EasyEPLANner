using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Все параметры технологического объекта.
    /// </summary>
    public class ParamsManager : Editor.TreeViewItem
    {
        public ParamsManager()
        {
            parFLoat = new Params("Параметры float", "par_float", 
                "init_params_float", false, true);
            parUInt = new Params("Параметры uint", "par_uint", 
                "init_params_uint", false);
            parFLoatRunTime = new Params("Рабочие параметры float", 
                "rt_par_float", "init_rt_params_float", true);
            parUIntRunTime = new Params("Рабочие параметры uint", 
                "rt_par_uint", "init_rt_params_uint", true);

            parFLoat.Parent = this;
            parUInt.Parent = this;
            parFLoatRunTime.Parent = this;
            parUIntRunTime.Parent = this;

            items = new List<Editor.ITreeViewItem>();
            items.Add(parFLoat);
            items.Add(parUInt);
            items.Add(parFLoatRunTime);
            items.Add(parUIntRunTime);
        }

        /// <summary>
        /// Добавление параметра.
        /// </summary>
        /// <param name="group">Группа.</param>
        /// <param name="name">Имя.</param>
        /// <param name="value">Значение.</param>
        /// <param name="meter">Размерность.</param>
        /// <param name="nameLua">Имя в Lua.</param>
        public Param AddParam(string group, string name, float value, 
            string meter, string nameLua = "")
        {
            Param res = null;
            switch (group)
            {
                case "par_float":
                    res = parFLoat.AddParam(
                        new Param(parFLoat.GetIdx, name, false, value, meter, 
                        nameLua, true));
                    break;

                case "par_uint":
                    res = parUInt.AddParam(
                        new Param(parUInt.GetIdx, name, false, value, meter, 
                        nameLua));
                    break;

                case "rt_par_float":
                    res = parFLoatRunTime.AddParam(
                        new Param(parFLoatRunTime.GetIdx, name, true, value, 
                        meter, nameLua));
                    break;

                case "rt_par_uint":
                    res = parUIntRunTime.AddParam(
                        new Param(parUIntRunTime.GetIdx, name, true, value, 
                        meter, nameLua));
                    break;
            }

            return res;
        }

        /// <summary>
        /// Получение параметра.
        /// </summary>
        /// <param name="nameLua">Имя в Lua.</param>
        public Param GetFParam(string nameLua)
        {
            return parFLoat.GetParam(nameLua);
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = "";

            res += parFLoat.SaveAsLuaTable(prefix);
            res += parUInt.SaveAsLuaTable(prefix);
            res += parFLoatRunTime.SaveAsLuaTable(prefix);
            res += parUIntRunTime.SaveAsLuaTable(prefix);

            return res;
        }

        public ParamsManager Clone()
        {
            ParamsManager clone = (ParamsManager)MemberwiseClone();

            clone.parFLoat = parFLoat.Clone(clone);
            clone.parFLoatRunTime = parFLoatRunTime.Clone(clone);
            clone.parUInt = parUInt.Clone(clone);
            clone.parUIntRunTime = parUIntRunTime.Clone(clone);

            clone.items = new List<Editor.ITreeViewItem>();
            clone.items.Add(clone.parFLoat);
            clone.items.Add(clone.parUInt);
            clone.items.Add(clone.parFLoatRunTime);
            clone.items.Add(clone.parUIntRunTime);

            return clone;
        }

        /// <summary>
        /// Получить Float параметры объекта.
        /// </summary>
        public Params Float
        {
            get
            {
                return parFLoat;
            }
        }

        /// <summary>
        /// Проверить менеджер параметров объекта.
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <returns>Ошибки</returns>
        public string Check(string objName)
        {
            var errors = "";
            errors += Float.Check(objName);
            return errors;
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                return new string[] { "Параметры", "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        override public bool IsReplaceable
        {
            get
            {
                return true;
            }
        }

        override public bool Delete(object child)
        {
            Params params_ = child as Params;
            if (params_ != null)
            {
                params_.Clear();
                return true;
            }

            return false;
        }

        override public Editor.ITreeViewItem Replace(object child,
            object copyObject)
        {
            Params pars = child as Params;
            if (copyObject is Params && pars != null)
            {
                pars.Clear();
                Params copyPars = copyObject as Params;
                bool isRunTimeParams = copyPars.IsRuntimeParameter();
                foreach (Param par in copyPars.Items)
                {
                    pars.InsertCopy(par);
                }

                return pars;
            }

            return null;
        }
        #endregion

        private Params parFLoat;
        private Params parUInt;
        private Params parFLoatRunTime;
        private Params parUIntRunTime;

        private List<Editor.ITreeViewItem> items;
    }
}
