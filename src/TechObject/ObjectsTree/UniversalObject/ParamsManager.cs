using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using Editor;
using Eplan.EplApi.HEServices;

namespace TechObject
{
    public class ParamsManager : TreeViewItem, IAutocompletable
    {
        /// <summary>
        /// Все параметры технологического объекта.
        /// </summary>
        public ParamsManager()
        {
            items = new List<ITreeViewItem>();

            parFloat = new Params("Параметры float", ParFloatLuaName, false,
                "S_PAR_F", true);
            parFloat.ValueChanged += sender => OnValueChanged(sender);
            parFloat.Parent = this;
            items.Add(parFloat);
        }

        /// <summary>
        /// Добавление параметра.
        /// </summary>
        /// <param name="group">Группа.</param>
        /// <param name="name">Имя.</param>
        /// <param name="value">Значение.</param>
        /// <param name="meter">Размерность.</param>
        /// <param name="nameLua">Имя в Lua.</param>
        public Param AddParam(string group, string name, object valueObj,
            string meter, string nameLua = "")
        {
            Param res = null;
            bool parsed = true;
            if (!double.TryParse(valueObj.ToString(), out var value))
            {
                parsed = false;
                value = 0;
            }

            switch (group)
            {
                case ParFloatLuaName:
                    res = AddFloatParam(name, value, meter, nameLua);
                    break;

                case ParFloatRuntimeLuaName:
                    res = AddFloatRuntimeParam(name, value, meter, nameLua);
                    break;

                case ParUintLuaName:
                    res = AddUintParam(name, value, meter, nameLua);
                    break;

                case ParUintRuntimeLuaName:
                    res = AddUintRuntimeParam(name, value, meter, nameLua);
                    break;
            }

            if (parsed is false)
                res?.ValueItem.SetNewValue(valueObj.ToString());

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
        public Param AddFloatParam(string name, double value, string meter,
            string nameLua) 
        {
            Param res = parFloat.AddParam(new Param(parFloat.GetIdx, name,
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
        public Param AddFloatRuntimeParam(string name, double value,
            string meter, string nameLua) 
        {
            if (parFloatRuntime == null)
            {
                parFloatRuntime = new Params("Рабочие параметры float",
                    ParFloatRuntimeLuaName, true, "RT_PAR_F");
                parFloatRuntime.ValueChanged += sender => OnValueChanged(sender);
                parFloatRuntime.Parent = this;
                items.Add(parFloatRuntime);
            }

            Param res = parFloatRuntime.AddParam(
                new Param(parFloatRuntime.GetIdx, name, true, value, meter,
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
        private Param AddUintParam(string name, double value, string meter,
            string nameLua) 
        {
            if (parUint == null)
            {
                parUint = new Params("Параметры uint", ParUintLuaName, false,
                    "S_PAR_UI");
                parUint.ValueChanged += sender => OnValueChanged(sender);
                parUint.Parent = this;
                items.Add(parUint);
            }

            Param res = parUint.AddParam(new Param(parUint.GetIdx, name,
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
        private Param AddUintRuntimeParam(string name, double value,
            string meter, string nameLua) 
        {
            if (parUintRuntime == null)
            {
                parUintRuntime = new Params("Рабочие параметры uint",
                    ParUintRuntimeLuaName, false, "RT_PAR_UI");
                parUintRuntime.ValueChanged += sender => OnValueChanged(sender);
                parUintRuntime.Parent = this;
                items.Add(parUintRuntime);
            }

            Param res = parUintRuntime.AddParam(
                new Param(parUintRuntime.GetIdx, name, true, value,
                meter, nameLua));
            return res;
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

            clone.parFloat = parFloat.Clone();
            clone.items.Add(clone.parFloat);

            if (parFloatRuntime != null)
            {
                clone.parFloatRuntime = parFloatRuntime.Clone();
                clone.items.Add(clone.parFloatRuntime);
            }

            if (parUint != null)
            {
                clone.parUint = parUint.Clone();
                clone.items.Add(clone.parUint);
            }

            if (parUintRuntime != null)
            {
                clone.parUintRuntime = parUintRuntime.Clone();
                clone.items.Add(clone.parUintRuntime);
            }

            foreach (var item in clone.Items)
            {
                item.ValueChanged += sender => clone.OnValueChanged(sender);
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
                return parFloat;
            }
        }

        /// <summary>
        /// Получить Float runtime параметры объекта.
        /// </summary>
        public Params FloatRuntime
        {
            get
            {
                return parFloatRuntime;
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

        override public ITreeViewItem Replace(object child, object copyObject)
        {
            if (copyObject is ParamsManager && child is ParamsManager parsManager)
            {
                Clear();
                var copyPars = copyObject as ParamsManager;
                foreach (var parGroup in copyPars.Items.OfType<Params>())
                {
                    foreach (var par in parGroup.Items.OfType<Param>())
                    {
                        Param addedParam = AddParam(parGroup.LuaName,
                            par.GetName(), float.Parse(par.GetValue()),
                            par.GetMeter(), par.GetNameLua());
                        addedParam.SetOperationN(par.GetOperationN());
                    }
                }

                // AddParent не нужен т.к он вызывается выше по стеку функций.
                return parsManager;
            }

            if (copyObject is Params && child is Params pars)
            {
                pars.Clear();
                Params copyPars = copyObject as Params;
                foreach (var par in copyPars.Items.OfType<Param>())
                {
                    pars.InsertCopy(par);
                }

                pars.AddParent(this);
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

        public void Clear()
        {
            parFloat?.Clear();
            parFloatRuntime?.Clear();
            parUint?.Clear();
            parUintRuntime?.Clear();
        }

        public override string SystemIdentifier => "process_parameter";

        public void SetUpFromBaseTechObject(ParamsManager paramsManager)
        {
            if (paramsManager.Float != null)
            {
                foreach (Param par in paramsManager.Float.Items)
                {
                    AddFloatParam(par.GetName(), double.Parse(par.GetValue()),
                        par.GetMeter(), par.GetNameLua());
                }
            }
            
            if (paramsManager.FloatRuntime != null)
            {
                foreach (Param par in paramsManager.FloatRuntime.Items)
                {
                    AddFloatRuntimeParam(par.GetName(), double.Parse(par.GetValue()),
                        par.GetMeter(), par.GetNameLua());
                }
            }
        }

        /// <summary>
        /// Обновление параметров на основе типового технологического объекта
        /// </summary>
        /// <param name="genericParamsManager"></param>
        public void UpdateOnGenericTechObject(ParamsManager genericParamsManager)
        {
            Float?.UpdateOnGenericTechObject(genericParamsManager.Float);
            FloatRuntime?.UpdateOnGenericTechObject(genericParamsManager.Float);
            parUint?.UpdateOnGenericTechObject(genericParamsManager.Float);
            parUintRuntime?.UpdateOnGenericTechObject(genericParamsManager.Float);
        }

        public override void CreateGenericByTechObjects(IEnumerable<ITreeViewItem> itemList)
        {
            var paramsManagerList = itemList.Cast<ParamsManager>().ToList();

            var parFloatList = paramsManagerList.Select(manager => manager.parFloat);
            var parFloatRuntimeList = paramsManagerList.Select(manager => manager.parFloatRuntime);
            var parUintList = paramsManagerList.Select(manager => manager.parUint);
            var parUintRuntimeList = paramsManagerList.Select(manager => manager.parUintRuntime);

            if (parFloatList.All(par => par != null))
                parFloat.CreateGenericByTechObjects(parFloatList);
            if (parFloatRuntimeList.All(par => par != null))
                parFloatRuntime.CreateGenericByTechObjects(parFloatRuntimeList);
            if (parUintList.All(par => par != null))
                parUint.CreateGenericByTechObjects(parUintList);
            if (parUintRuntimeList.All(par => par != null))
                parUintRuntime.CreateGenericByTechObjects(parUintRuntimeList);
        }

        bool IAutocompletable.CanExecute => true;

        public void Autocomplete()
        {
            Float.FillWithStubs();
            TechObject.ModesManager.Modes.ForEach(m => m.Autocomplete());
        }

        /// <summary>
        /// Заполнение параметров по операции
        /// </summary>
        /// <param name="operation"></param>
        public void AutocompleteByOperation(Mode operation)
        {
            if (operation.Owner.Owner != TechObject) return;

            var parameters = operation.BaseOperation.Parameters;

            // Если такой параметр уже добавлен, пробуем добавить в него операцию
            parameters.ForEach(p => Float.GetParam(p.LuaName)?.UseInOperation(operation.GetModeNumber()));

            // Если все параметры базовой операции уже добавлены, пропускаем
            if (parameters.Count(p => Float.HaveSameLuaName(p.LuaName)) == parameters.Count)
                return;

            foreach (var param in parameters)
            {
                if (Float.HaveSameLuaName(param.LuaName))
                    continue;

                var parameter = AddFloatParam(
                    param.Name, param.DefaultValue,
                    param.Meter, param.LuaName);
                parameter.UseInOperation(operation.GetModeNumber());
            }
        }

        public TechObject TechObject => Parent as TechObject;

        private Params parFloat;
        private Params parFloatRuntime;
        private Params parUint;
        private Params parUintRuntime;

        public const string ParFloatLuaName = "par_float";
        public const string ParFloatRuntimeLuaName = "rt_par_float";
        public const string ParUintLuaName = "par_uint";
        public const string ParUintRuntimeLuaName = "rt_par_uint";

        private List<ITreeViewItem> items;
    }
}