using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;

namespace TechObject
{
    public class ParamsManager : TreeViewItem
    {
        /// <summary>
        /// Все параметры технологического объекта.
        /// </summary>
        public ParamsManager()
        {
            items = new List<ITreeViewItem>();

            parFLoat = new Params("Параметры float", "par_float", false,
                "S_PAR_F", true);
            parFLoat.Parent = this;
            items.Add(parFLoat);
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
                    res = AddFloatParam(name, value, meter, nameLua);
                    break;

                case "rt_par_float":
                    res = AddFloatRuntimeParam(name, value, meter, nameLua);
                    break;

                case "par_uint":
                    res = AddUIntParam(name, value, meter, nameLua);
                    break;

                case "rt_par_uint":
                    res = AddUIntRuntimeParam(name, value, meter, nameLua);
                    break;
            }

            return res;
        }

        /// <summary>
        /// Добавить Float параметр.
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="value">Значение</param>
        /// <param name="meter">Ед. измерения</param>
        /// <param name="nameLua">Имя Lua</param>
        /// <returns></returns>
        private Param AddFloatParam(string name, double value, string meter,
            string nameLua) 
        {
            Param res = parFLoat.AddParam(new Param(parFLoat.GetIdx, name,
                false, value, meter, nameLua, true));
            return res;
        }

        /// <summary>
        /// Добавить рабочий Float параметр.
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="value">Значение</param>
        /// <param name="meter">Ед. измерения</param>
        /// <param name="nameLua">Имя Lua</param>
        /// <returns></returns>
        private Param AddFloatRuntimeParam(string name, double value,
            string meter, string nameLua) 
        {
            if (parFLoatRunTime == null)
            {
                parFLoatRunTime = new Params("Рабочие параметры float",
                    "rt_par_float", true, "RT_PAR_F");
                parFLoatRunTime.Parent = this;
                items.Add(parFLoatRunTime);
            }

            Param res = parFLoatRunTime.AddParam(
                new Param(parFLoatRunTime.GetIdx, name, true, value, meter,
                nameLua));
            return res;
        }

        /// <summary>
        /// Добавить UInt параметр.
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="value">Значение</param>
        /// <param name="meter">Ед. измерения</param>
        /// <param name="nameLua">Имя Lua</param>
        /// <returns></returns>
        private Param AddUIntParam(string name, double value, string meter,
            string nameLua) 
        {
            if (parUInt == null)
            {
                parUInt = new Params("Параметры uint", "par_uint", false,
                    "S_PAR_UI");
                parUInt.Parent = this;
                items.Add(parUInt);
            }

            Param res = parUInt.AddParam(new Param(parUInt.GetIdx, name,
                false, value, meter, nameLua));
            return res;
        }

        /// <summary>
        /// Добавить рабочий UInt параметр.
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="value">Значение</param>
        /// <param name="meter">Ед. измерения</param>
        /// <param name="nameLua">Имя Lua</param>
        /// <returns></returns>
        private Param AddUIntRuntimeParam(string name, double value,
            string meter, string nameLua) 
        {
            if (parUIntRunTime == null)
            {
                parUIntRunTime = new Params("Рабочие параметры uint",
                    "rt_par_uint", false, "RT_PAR_UI");
                parUIntRunTime.Parent = this;
                items.Add(parUIntRunTime);
            }

            Param res = parUIntRunTime.AddParam(
                new Param(parUIntRunTime.GetIdx, name, true, value,
                meter, nameLua));
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
        /// Получение параметра.
        /// </summary>
        /// <param name="paramIndex">Индекс параметра</param>
        /// <returns></returns>
        public Param GetParam(int paramIndex)
        {
            return parFLoat.GetParam(paramIndex);
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = "";

            foreach(Params paramsGroups in Items)
            {
                res += paramsGroups.SaveAsLuaTable(prefix);
            }

            return res;
        }

        public ParamsManager Clone()
        {
            ParamsManager clone = (ParamsManager)MemberwiseClone();
            clone.items = new List<ITreeViewItem>();

            clone.parFLoat = parFLoat.Clone();
            clone.items.Add(clone.parFLoat);

            if (parFLoatRunTime != null)
            {
                clone.parFLoatRunTime = parFLoatRunTime.Clone();
                clone.items.Add(clone.parFLoatRunTime);
            }

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

        public override bool ShowWarningBeforeDelete
        {
            get
            {
                return true;
            }
        }

        public override ImageIndexEnum ImageIndex
        {
            get
            {
                return ImageIndexEnum.ParamsManager;
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
        private Params parUInt;
        private Params parUIntRunTime;

        private List<ITreeViewItem> items;
    }
}