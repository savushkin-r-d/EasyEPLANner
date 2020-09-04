using System.Collections.Generic;
using NewEditor;

namespace NewTechObject
{
    /// <summary>
    /// Все параметры технологического объекта.
    /// </summary>
    public class ParamsManager : TreeViewItem
    {
        public ParamsManager()
        {
            parFLoat = new Params("Параметры float", "par_float", false, true);
            parFLoatRunTime = new Params("Рабочие параметры float", 
                "rt_par_float", true);

            parFLoat.Parent = this;
            parFLoatRunTime.Parent = this;

            items = new List<ITreeViewItem>();
            items.Add(parFLoat);
            items.Add(parFLoatRunTime);
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

                case "rt_par_float":
                    res = parFLoatRunTime.AddParam(
                        new Param(parFLoatRunTime.GetIdx, name, true, value,
                        meter, nameLua));
                    break;
            }

            return res;
        }

        /// <summary>
        /// Получение параметра.
        /// </summary>
        /// <param name="nameLua">Имя в Lua.</param>
        public Param GetParam(string nameLua)
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
            res += parFLoatRunTime.SaveAsLuaTable(prefix);

            return res;
        }

        public ParamsManager Clone()
        {
            ParamsManager clone = (ParamsManager)MemberwiseClone();

            clone.parFLoat = parFLoat.Clone();
            clone.parFLoatRunTime = parFLoatRunTime.Clone();

            clone.items = new List<ITreeViewItem>();
            clone.items.Add(clone.parFLoat);
            clone.items.Add(clone.parFLoatRunTime);

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

        public void Clear()
        {
            parFLoat.Clear();
            parFLoatRunTime.Clear();
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                return new string[] { "Параметры", "" };
            }
        }

        override public ITreeViewItem[] Items
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

        override public ITreeViewItem Replace(object child,
            object copyObject)
        {
            Params pars = child as Params;
            if (copyObject is Params && pars != null)
            {
                pars.Clear();
                Params copyPars = copyObject as Params;
                foreach (Param par in copyPars.Items)
                {
                    pars.InsertCopy(par);
                }

                return pars;
            }

            return null;
        }

        public override ImageIndexEnum ImageIndex
        {
            get
            {
                return ImageIndexEnum.Params;
            }
        }
        #endregion

        public override string GetLinkToHelpPage()
        {
            string ostisLink = EasyEPlanner.ProjectManager.GetInstance()
                .GetOstisHelpSystemLink();
            return ostisLink + "?sys_id=process_parameter";
        }

        private Params parFLoat;
        private Params parFLoatRunTime;

        private List<ITreeViewItem> items;
    }
}