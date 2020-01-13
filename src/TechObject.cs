///@file TechObject.cs
///@brief Классы, реализующие минимальную функциональность, необходимую для 
///редактирования описания операций технологических объектов.
///
/// @author  Иванюк Дмитрий Сергеевич.
///
/// @par Текущая версия:
/// @$Rev: --- $.\n
/// @$Author: sedr $.\n
/// @$Date:: 2019-10-21#$.
/// 

using System.Collections.Generic;           //TList
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;
using System;
using LuaInterface;
using System.IO;

/// <summary>
/// Пространство имен технологических объектов. 
/// </summary>
namespace TechObject
{
    /// <summary>
    /// Получение номера объекта в списке. Нумерация начинается с 1.
    /// </summary>
    public delegate int GetN(object obj);

    //-------------------------------------------------------------------------
    //------------------------------------------------------------------------
    /// <summary>
    /// Отдельный параметр технологического объекта.
    /// </summary>
    public class Param : Editor.TreeViewItem
    {
        public Param(GetN getN, string name, bool isRuntime = false,
            double value = 0, string meter = "шт", string nameLua = "",
            bool isUseOperation = false)
        {
            this.isRuntime = isRuntime;

            this.name = name;
            this.getN = getN;
            if (!isRuntime)
            {
                this.value = new Editor.ObjectProperty("Значение", value);
            }
            if (isUseOperation)
            {
                this.oper = new Editor.ObjectProperty("Операция", -1);
            }

            this.meter = new Editor.ObjectProperty("Размерность", meter);
            this.nameLua = new Editor.ObjectProperty("Lua имя", nameLua);

            items = new List<Editor.ITreeViewItem>();
            if (!isRuntime)
            {
                items.Add(this.value);
            }
            items.Add(this.meter);
            if (isUseOperation)
            {
                items.Add(oper);
            }
            items.Add(this.nameLua);
        }


        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + "[ " + getN(this) + " ] =\n";
            res += prefix + "\t{\n";
            res += prefix + "\tname = \'" + name + "\',\n";
            if (!isRuntime)
            {
                res += prefix + "\tvalue = " + value.EditText[1] + ",\n";
            }
            res += prefix + "\tmeter = \'" + meter.EditText[1] + "\',\n";
            if (oper != null)
            {
                res += prefix + "\toper = " + oper.EditText[1] + ",\n";
            }
            res += prefix + "\tnameLua = \'" + nameLua.EditText[1] + "\'\n";

            res += prefix + "\t},\n";
            return res;
        }

        public void SetOperationN(int operN)
        {
            if (oper != null)
            {
                oper.SetValue(operN);
            }
        }

        public int GetOperationN()
        {
            if (oper != null)
            {
                try
                {
                    return Convert.ToInt32(oper.EditText[1]);
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            return 0;
        }
        #region Реализация ITreeViewItem

        override public string[] DisplayText
        {
            get
            {
                string res = "";
                if (!isRuntime)
                {
                    res = getN(this) + ". " + name +
                        " - " + value.EditText[1] + " " + meter.EditText[1] + ".";
                }
                else
                {
                    res = getN(this) + ". " + name +
                        ", " + meter.EditText[1] + ".";
                }

                return new string[] { res, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }

        override public bool SetNewValue(string newName)
        {
            name = newName;

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
                return new string[] { name, "" };
            }
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        override public bool IsMoveable
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

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public object Copy()
        {
            return this;
        }

        #endregion

        public string GetName()
        {
            return name;
        }

        public string GetNameLua()
        {
            if (nameLua.EditText[1] != "")
            {
                return nameLua.EditText[1];
            }

            return "P";
        }

        public string GetValue()
        {
            if (value != null)
            {
                return value.EditText[1];
            }

            return "0";
        }

        public string GetMeter()
        {
            return meter.EditText[1];
        }

        public bool IsUseOperation()
        {
            return oper != null;
        }

        private GetN getN;

        private bool isRuntime;
        public string name;
        private List<Editor.ITreeViewItem> items; ///Данные для редактирования.
        public Editor.ObjectProperty nameLua;    ///Имя в Lua.
        public Editor.ObjectProperty value;      ///Значение.
        public Editor.ObjectProperty meter;      ///Размерность.

        public Editor.ObjectProperty oper;       ///Номер связанной операции.
    }
    //-------------------------------------------------------------------------
    //------------------------------------------------------------------------
    /// <summary>
    /// Параметры технологического объекта.
    /// </summary>
    public class Params : Editor.TreeViewItem
    {

        public Params(string name, string nameLua, string initFunctionName,
            bool isRunTimeParams)
        {
            this.name = name;
            this.nameLua = nameLua;
            this.initFunctionName = initFunctionName;
            this.isRunTimeParams = isRunTimeParams;

            parameters = new List<Param>();
        }


        public Params Clone(ParamsManager owner)
        {
            Params clone = (Params)MemberwiseClone();
            clone.parameters = new List<Param>();

            foreach (Param par in parameters)
            {
                clone.InsertCopy(par);

            }

            return clone;
        }

        /// <summary>
        /// Добавление параметра в список.
        /// </summary>
        /// <param name="param">Добавляемый параметр.</param>
        public Param AddParam(Param param)
        {
            parameters.Add(param);
            return param;
        }

        public bool getIsRuntime()
        {
            return isRunTimeParams;
        }

        /// <summary>
        /// Получение индекса параметра.
        /// </summary>
        /// <param name="param">Параметр, индекс которого требуется узнать.</param>
        /// <returns>Индекс искомого параметра.</returns>
        public int GetIdx(object param)
        {
            return parameters.IndexOf(param as Param) + 1;
        }

        /// <summary>
        /// Получение параметра по его Lua-имени.
        /// </summary>
        /// <param name="nameLua">Имя в Lua.</param>
        public Param GetParam(string nameLua)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].GetNameLua() == nameLua)
                {
                    return parameters[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            if (parameters.Count == 0)
            {
                return "";
            }

            string res = prefix + nameLua + " =\n";
            res += prefix + "\t{\n";
            foreach (Param param in parameters)
            {
                res += param.SaveAsLuaTable(prefix + "\t");
            }
            res += prefix + "\t},\n";

            return res;
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = string.Format("{0} ({1})", name, parameters.Count);

                return new string[] { res, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return parameters.ToArray();
            }
        }

        override public bool IsDeletable
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
            Param removedParam = child as Param;
            if (removedParam != null)
            {
                parameters.Remove(removedParam);
                return true;
            }

            return false;
        }

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        override public bool SetNewValue(string newValue)
        {
            return false;
        }

        override public Editor.ITreeViewItem Insert()
        {
            Param newParam;

            if (parameters.Count > 0)
            {
                string newName = parameters[parameters.Count - 1].GetName();
                string newValueStr = parameters[parameters.Count - 1].GetValue();
                double newValue = Convert.ToDouble(newValueStr);
                string newMeter = parameters[parameters.Count - 1].GetMeter();
                string newNameLua = parameters[parameters.Count - 1].GetNameLua();
                bool useOperation = parameters[parameters.Count - 1].IsUseOperation();

                newParam = new Param(
                    GetIdx, newName, isRunTimeParams, newValue, newMeter, newNameLua, useOperation);
                if (useOperation)
                {
                    newParam.SetOperationN(parameters[parameters.Count - 1].GetOperationN());
                }
            }
            else
            {
                newParam = new Param(GetIdx, "Название параметра", isRunTimeParams);
            }

            parameters.Add(newParam);
            return newParam;
        }

        public void Clear()
        {
            parameters.Clear();
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        override public object Copy()
        {
            return this;
        }

        override public bool IsInsertableCopy
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem InsertCopy(object obj)
        {
            if (obj is Param)
            {
                Param newParam = obj as Param;

                string newName = newParam.GetName();
                string newValueStr = newParam.GetValue();
                double newValue = Convert.ToDouble(newValueStr);
                string newMeter = newParam.GetMeter();
                string newNameLua = newParam.GetNameLua();
                bool useOperation = newParam.IsUseOperation();

                Param newPar = new Param(
                    GetIdx, newName, isRunTimeParams, newValue, newMeter, newNameLua, useOperation);
                if (useOperation)
                {
                    newPar.SetOperationN(newParam.GetOperationN());
                }

                parameters.Add(newPar);

                return newPar;
            }
            else
            {
                return null;
            }
        }

        override public Editor.ITreeViewItem MoveDown(object child)
        {
            Param par = child as Param;

            if (par != null)
            {
                int index = parameters.IndexOf(par);
                if (index <= parameters.Count - 2)
                {
                    parameters.Remove(par);
                    parameters.Insert(index + 1, par);

                    return parameters[index];
                }
            }

            return null;
        }

        override public Editor.ITreeViewItem MoveUp(object child)
        {
            Param par = child as Param;

            if (par != null)
            {
                int index = parameters.IndexOf(par);
                if (index > 0)
                {
                    parameters.Remove(par);
                    parameters.Insert(index - 1, par);

                    return parameters[index];
                }
            }

            return null;
        }

        override public Editor.ITreeViewItem Replace(object child,
            object copyObject)
        {
            Param par = child as Param;
            if (copyObject is Param && par != null)
            {
                string newName = (copyObject as Param).GetName();
                string newValueStr = (copyObject as Param).GetValue();
                double newValue = Convert.ToDouble(newValueStr);
                string newMeter = (copyObject as Param).GetMeter();
                string newNameLua = (copyObject as Param).GetNameLua();
                bool useOperation = (copyObject as Param).IsUseOperation();

                Param newPar = new Param(GetIdx, newName, isRunTimeParams,
                    newValue, newMeter, newNameLua, useOperation);
                if (useOperation)
                {
                    newPar.SetOperationN((copyObject as Param).GetOperationN());
                }

                int index = parameters.IndexOf(par);

                parameters.Remove(par);

                parameters.Insert(index, newPar);

                //newMode.ModifyDevNames( owner.TechNumber, -1, owner.NameEplan );
                return newPar;
            }

            return null;
        }

        #endregion

        private string nameLua;
        private string initFunctionName;
        private bool isRunTimeParams;

        private string name;

        private List<Param> parameters;
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Все параметры технологического объекта.
    /// </summary>
    public class ParamsManager : Editor.TreeViewItem
    {

        public ParamsManager()
        {
            parFLoat = new Params("Параметры float", "par_float", "init_params_float", false);
            parUInt = new Params("Параметры uint", "par_uint", "init_params_uint", false);

            parFLoatRunTime = new Params("Рабочие параметры float", "rt_par_float",
                "init_rt_params_float", true);
            parUIntRunTime = new Params("Рабочие параметры uint", "rt_par_uint",
                "init_rt_params_uint", true);

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
        public Param AddParam(string group, string name,
            float value, string meter, string nameLua = "")
        {
            Param res = null;
            switch (group)
            {
                case "par_float":
                    res = parFLoat.AddParam(new Param(
                        parFLoat.GetIdx, name, false, value, meter, nameLua, true));
                    break;

                case "par_uint":
                    res = parUInt.AddParam(new Param(
                        parUInt.GetIdx, name, false, value, meter, nameLua));
                    break;

                case "rt_par_float":
                    res = parFLoatRunTime.AddParam(new Param(
                        parFLoatRunTime.GetIdx, name, true, value, meter, nameLua));
                    break;

                case "rt_par_uint":
                    res = parUIntRunTime.AddParam(new Param(
                        parUIntRunTime.GetIdx, name, true, value, meter, nameLua));
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
                return false;
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
                bool isRunTimeParams = copyPars.getIsRuntime();
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
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Параметр технологической операции.
    /// </summary>
    public class OperationParam : Editor.TreeViewItem
    {
        public OperationParam(Param par)
        {
            this.par = par;

            items = new List<Editor.ITreeViewItem>();
            items.Add(par.nameLua);
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                return par.DisplayText;
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }
        #endregion

        private List<Editor.ITreeViewItem> items;
        private Param par;
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Все параметры технологической операции.
    /// </summary>
    public class OperationParams : Editor.TreeViewItem
    {
        public OperationParams()
        {
            items = new List<Editor.ITreeViewItem>();
        }

        public void AddParam(Param par)
        {
            items.Add(new OperationParam(par));
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = string.Format("{0} ({1})", "Параметры", items.Count);
                return new string[] { res, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                if (items.Count > 0)
                {
                    return items.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        private List<Editor.ITreeViewItem> items;
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Таймеры технологического объекта.
    /// </summary>
    public class TimersManager : Editor.TreeViewItem
    {

        public TimersManager()
        {
        }

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
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Операции технологического объекта.
    /// </summary>
    public class ModesManager : Editor.TreeViewItem
    {

        public ModesManager(TechObject owner)
        {
            this.owner = owner;

            modes = new List<Mode>();
        }

        public ModesManager Clone(TechObject owner)
        {
            ModesManager clone = (ModesManager)MemberwiseClone();

            clone.modes = new List<Mode>();
            foreach (Mode mode in modes)
            {
                clone.modes.Add(mode.Clone(clone.GetModeN, clone));
            }

            return clone;
        }

        public void ModifyDevNames(int oldTechObjectN)
        {
            foreach (Mode mode in modes)
            {
                mode.ModifyDevNames(owner.TechNumber, oldTechObjectN, owner.NameEplan);
            }
        }

        public void SetNewOwnerDevNames(string newTechObjectName,
            int newTechObjectNumber)
        {
            foreach (Mode mode in modes)
            {
                mode.ModifyDevNames(newTechObjectName, newTechObjectNumber,
                    owner.NameEplan);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + "modes =\n" +
                prefix + "\t{\n";

            int i = 1;
            foreach (Mode mode in modes)
            {
                res += prefix + "\t[ " + i++ + " ] =\n";
                res += mode.SaveAsLuaTable(prefix + "\t\t");
            }

            res += prefix + "\t}\n";
            return res;
        }

        /// <summary>
        /// Сохранение ограничений в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveRestrictionAsLua(string prefix)
        {
            string res = "";
            string tmp2 = "";
            int i = 1;
            foreach (Mode mode in modes)
            {
                string tmp = "";
                string comment = "\t\t--Операция №" + i.ToString();

                tmp += mode.SaveRestrictionAsLua(prefix + "\t\t");
                if (tmp != "")
                {
                    tmp2 += prefix + "\t[ " + i.ToString() + " ] =" + comment + "\n" + tmp;
                }
                i++;
            }
            if (tmp2 != "")
            {
                res += prefix + "\t{\n" + tmp2 + prefix + "\t},";
            }

            return res;
        }

        /// <summary>
        /// Получение номера операции в списке операций. Нумерация начинается с 1.
        /// </summary>
        /// <param name="mode">Операция, номер которой хотим получить.</param>
        /// <returns>Номер заданной операции.</returns>
        public int GetModeN(object mode)
        {
            return modes.IndexOf(mode as Mode) + 1;
        }

        /// <summary>
        /// Добавление новой операции.
        /// </summary>
        /// <param name="modeName">Имя операции.</param>
        /// <param name="baseOperationName">Имя базовой операции</param>
        /// <returns>Добавленная операция.</returns>
        public Mode AddMode(string modeName, string baseOperationName)
        {
            Mode newMode = new Mode(modeName, GetModeN, this);

            // Установка имени базовой операции в Mode
            newMode.SetNewValue(baseOperationName, true);

            modes.Add(newMode);

            ChangeRestrictionModeOwner(newMode);

            return newMode;
        }

        /// <summary>
        /// Добавление новой операции.
        /// </summary>
        /// <param name="modeName">Имя операции.</param>
        /// <param name="baseOperationName">Имя базовой операции</param>
        /// <param name="extraParams">Значения параметров базовой операции</param>
        /// <returns>Добавленная операция.</returns>
        public Mode AddMode(string modeName, string baseOperationName, Editor.ObjectProperty[] extraParams)
        {
            Mode newMode = new Mode(modeName, GetModeN, this);

            // Установка имени базовой операции в Mode
            newMode.SetNewValue(baseOperationName, true);

            // Установка параметров базовой операции
            newMode.SetBaseOperExtraParams(extraParams);

            modes.Add(newMode);

            ChangeRestrictionModeOwner(newMode);

            return newMode;
        }

        /// <summary>
        /// Добавление нового шага.
        /// </summary>
        /// <param name="modeN">Номер операции, куда добавляем шаг.</param>
        /// <param name="stateN">Номер состояния, куда добавляем шаг.</param>
        /// <param name="stepName">Имя шага.</param>
        /// <returns>true  - шаг добавлен.</returns>
        /// <returns>false - шаг не добавлен.</returns>
        public bool AddStep(int modeN, int stateN, string stepName)
        {
            if (modes.Count > modeN)
            {
                modes[modeN].AddStep(stateN, stepName);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Изменение технологического объекта-владельца операций.
        /// </summary>
        /// <param name="newOwner">Новый технологический объект-владелец операций.</param>
        /// <returns></returns>
        public void ChngeOwner(TechObject newOwner)
        {
            owner = newOwner;
        }

        public TechObject Owner
        {
            get
            {
                return owner;
            }
        }

        public List<Mode> GetModes
        {
            get
            {
                return modes;
            }
        }

        public void Synch(int[] array)
        {
            foreach (Mode mode in modes)
            {
                mode.Synch(array);
            }
        }

        public void CheckRestriction(int prev, int curr)
        {
            foreach (Mode mode in modes)
            {
                mode.CheckRestriction(prev, curr);
            }
        }

        public void ChangeCrossRestriction(ModesManager oldModesMngr = null)
        {
            for (int i = 0; i < modes.Count; i++)
            {
                if ((oldModesMngr != null) && (i < oldModesMngr.GetModes.Count))
                {
                    modes[i].ChangeCrossRestriction(oldModesMngr.GetModes[i]);
                }
                else
                {
                    modes[i].ChangeCrossRestriction();
                }
            }
            if (oldModesMngr != null)
            {
                if (oldModesMngr.GetModes.Count > modes.Count)
                {
                    for (int i = modes.Count; i < oldModesMngr.GetModes.Count; i++)
                    {
                        int tobjNum = TechObjectManager.GetInstance().GetTechObjectN(owner);
                        TechObjectManager.GetInstance().ChangeModeNum(tobjNum, i + 1, -1);
                    }
                }
            }
        }

        public void ModifyRestrictObj(int oldObjN, int newObjN)
        {
            foreach (Mode mode in modes)
            {
                mode.ModifyRestrictObj(oldObjN, newObjN);

            }
        }

        public void ChangeModeNum(int objNum, int prev, int curr)
        {
            foreach (Mode mode in modes)
            {
                mode.ChangeModeNum(objNum, prev, curr);
            }
        }

        private void ChangeRestrictionModeOwner(Mode newMode)
        {
            int objParentNum = TechObjectManager.GetInstance().GetTechObjectN(owner);
            int modeParentNum = GetModeN(newMode);
            newMode.SetRestrictionOwner(objParentNum, modeParentNum);
        }

        /// <summary>
        /// При перемщении, удалении объекта нужно менять родителей у ограничений
        /// </summary>
        public void SetRestrictionOwner()
        {
            foreach (Mode mode in modes)
            {
                ChangeRestrictionModeOwner(mode);
            }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = "Операции";
                if (modes.Count > 0)
                {
                    res += " (" + modes.Count + ")";
                }

                return new string[] { res, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return modes.ToArray();
            }
        }

        override public bool Delete(object child)
        {
            Mode mode = child as Mode;

            if (mode != null)
            {
                int idx = modes.IndexOf(mode) + 1;

                int tobjNum = TechObjectManager.GetInstance().GetTechObjectN(owner);
                TechObjectManager.GetInstance().ChangeModeNum(tobjNum, idx, -1);

                modes.Remove(mode);

                foreach (Mode newMode in modes)
                {
                    ChangeRestrictionModeOwner(newMode);
                }
                return true;
            }

            return false;
        }

        override public Editor.ITreeViewItem MoveUp(object child)
        {
            Mode mode = child as Mode;

            if (mode != null)
            {
                int index = modes.IndexOf(mode);
                if (index > 0)
                {

                    int tobjNum = TechObjectManager.GetInstance().GetTechObjectN(owner);
                    TechObjectManager.GetInstance().ChangeModeNum(tobjNum, index + 1, index);

                    modes.Remove(mode);
                    modes.Insert(index - 1, mode);

                    foreach (Mode newMode in modes)
                    {
                        ChangeRestrictionModeOwner(newMode);
                    }

                    return modes[index];
                }
            }

            return null;
        }

        override public Editor.ITreeViewItem MoveDown(object child)
        {
            Mode mode = child as Mode;

            if (mode != null)
            {
                int index = modes.IndexOf(mode);
                if (index <= modes.Count - 2)
                {

                    int tobjNum = TechObjectManager.GetInstance().GetTechObjectN(owner);
                    TechObjectManager.GetInstance().ChangeModeNum(tobjNum, index + 1, index + 2);

                    modes.Remove(mode);
                    modes.Insert(index + 1, mode);

                    foreach (Mode newMode in modes)
                    {
                        ChangeRestrictionModeOwner(newMode);
                    }

                    return modes[index];
                }
            }

            return null;
        }

        override public Editor.ITreeViewItem Replace(object child, object copyObject)
        {
            Mode mode = child as Mode;
            if (copyObject is Mode && mode != null)
            {

                Mode newMode = (copyObject as Mode).Clone(GetModeN, this, mode.EditText[0]);
                int index = modes.IndexOf(mode);

                modes.Remove(mode);

                modes.Insert(index, newMode);

                newMode.ModifyDevNames(owner.TechNumber, -1, owner.NameEplan);

                ChangeRestrictionModeOwner(newMode);
                newMode.ChangeCrossRestriction(mode);

                return newMode;
            }

            return null;
        }

        override public bool IsInsertableCopy
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem InsertCopy(object obj)
        {
            if (obj is Mode)
            {
                Mode newMode = (obj as Mode).Clone(GetModeN, this);
                modes.Add(newMode);

                newMode.ModifyDevNames(owner.TechNumber, -1, owner.NameEplan);

                ChangeRestrictionModeOwner(newMode);
                newMode.ChangeCrossRestriction();

                return newMode;
            }

            return null;
        }

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem Insert()
        {
            Mode newMode = new Mode("Новая операция", GetModeN, this);
            modes.Add(newMode);

            ChangeRestrictionModeOwner(newMode);

            return newMode;
        }
        #endregion

        private List<Mode> modes; /// Список операций.
        private TechObject owner; /// Технологический объект.

    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ограничения
    /// </summary>
    public class Restriction : Editor.TreeViewItem
    {
        public Restriction(string Name, string Value, string LuaName, SortedDictionary<int, List<int>> dict)
        {
            name = Name;
            restrictStr = Value;
            luaName = LuaName;
            restrictList = dict;

            ownerObjNum = 0;
            ownerModeNum = 0;
        }

        /// <summary>
        /// Функция для создания копии объекта ограниченй
        /// </summary>
        /// <returns></returns>
        public Restriction Clone()
        {
            Restriction clone = (Restriction)MemberwiseClone();
            clone.restrictList = new SortedDictionary<int, List<int>>();
            SortedDictionary<int, List<int>>.KeyCollection keyColl =
                    restrictList.Keys;
            foreach (int key in keyColl)
            {
                List<int> valueColl = restrictList[key];
                foreach (int val in valueColl)
                {
                    if (clone.restrictList.ContainsKey(key))
                    {
                        clone.restrictList[key].Add(val);
                    }
                    else
                    {
                        List<int> restrictMode = new List<int>();
                        restrictMode.Add(val);
                        clone.restrictList.Add(key, restrictMode);
                    }

                }
            }
            clone.ChangeRestrictStr();
            return clone;
        }

        /// <summary>
        /// Сохранение ограничений в виде скрипта Lua
        /// </summary>
        /// <returns></returns>
        public string SaveRestrictionAsLua(string prefix)
        {
            string res = "";

            res += prefix + "{";

            string tmp = "";
            string comment = "\t\t--" + name; ;
            if (restrictList != null)
            {
                if (restrictList.Count != 0)
                {
                    SortedDictionary<int, List<int>>.KeyCollection keyColl =
                       restrictList.Keys;
                    if (keyColl.Count != 0)
                    {
                        foreach (int key in keyColl)
                        {
                            if (!this.IsLocalRestrictionUse)
                            {
                                if (tmp == "")
                                {
                                    tmp += comment;
                                }
                                tmp += "\n" + prefix;
                                tmp += "[ " + key.ToString() + " ] =\n";
                                tmp += prefix + "\t{\n";
                            }
                            else
                            {
                                tmp += comment + "\n";
                            }
                            List<int> valueColl = restrictList[key];

                            foreach (int val in valueColl)
                            {
                                tmp += prefix + "\t";
                                tmp += "[ " + val.ToString() + " ] = 1,\n";

                            }
                            if (!this.IsLocalRestrictionUse)
                            {
                                tmp += prefix + "\t},";
                            }

                        }
                    }
                }
            }
            if (tmp != "")
            {
                res += tmp;
                res += "\n" + prefix + "},\n";
            }
            else
            {
                res = "";
            }
            return res;
        }


        /// <summary>
        /// Установка новых ограничений
        /// </summary>
        /// <param name="dict">Новый словарь с ограничениями</param>
        /// <returns></returns>
        override public bool SetNewValue(SortedDictionary<int, List<int>> dict)
        {
            SortedDictionary<int, List<int>> oldRestriction =
                new SortedDictionary<int, List<int>>(restrictList);
            restrictList = null;
            restrictList = new SortedDictionary<int, List<int>>(dict);
            //Компануем строку для отображения
            ChangeRestrictStr();

            SortedDictionary<int, List<int>> deletedRestriction =
                GetDeletedRestriction(oldRestriction);
            ClearCrossRestriction(deletedRestriction);
            SetCrossRestriction();
            return true;
        }

        /// <summary>
        /// установка новых ограничений
        /// </summary>
        /// <param name="newRestriction">Новая строка ограничений</param>
        /// <returns></returns>
        public override bool SetNewValue(string newRestriction)
        {
            SortedDictionary<int, List<int>> oldRestriction =
                new SortedDictionary<int, List<int>>(restrictList);
            restrictStr = newRestriction;
            ChangeRestrictList();

            SortedDictionary<int, List<int>> deletedRestriction =
                GetDeletedRestriction(oldRestriction);
            ClearCrossRestriction(deletedRestriction);
            SetCrossRestriction();
            return true;
        }

        public void ChangeCrossRestriction(SortedDictionary<int, List<int>> oldDictionary = null)
        {
            if (oldDictionary != null)
            {
                SortedDictionary<int, List<int>> deletedRestriction =
                                GetDeletedRestriction(oldDictionary);

                ClearCrossRestriction(deletedRestriction);
            }
            SetCrossRestriction();
        }

        /// <summary>
        /// Функция используется для получения словаря, состоящего из удаленных элементов
        /// </summary>
        /// <param name="oldRestriction">Предыдущая версия словаря ограничений</param>
        /// <returns></returns>
        private SortedDictionary<int, List<int>> GetDeletedRestriction(SortedDictionary<int, List<int>> oldRestriction)
        {
            SortedDictionary<int, List<int>> delRestriction =
                new SortedDictionary<int, List<int>>();

            SortedDictionary<int, List<int>>.KeyCollection keyColl =
                     oldRestriction.Keys;
            foreach (int key in keyColl)
            {
                if (!restrictList.ContainsKey(key))
                {
                    List<int> newVal = new List<int>();
                    delRestriction.Add(key, newVal);
                    foreach (int val in oldRestriction[key])
                    {
                        delRestriction[key].Add(val);
                    }
                }
                else
                {
                    foreach (int val in oldRestriction[key])
                    {
                        if (!restrictList[key].Contains(val))
                        {
                            if (!delRestriction.ContainsKey(key))
                            {
                                List<int> newVal = new List<int>();
                                delRestriction.Add(key, newVal);
                                delRestriction[key].Add(val);
                            }
                            else
                            {
                                delRestriction[key].Add(val);
                            }
                        }
                    }
                }
            }
            return delRestriction;
        }

        /// <summary>
        /// Установка перекрестных ограничеий
        /// </summary>
        private void SetCrossRestriction()
        {
            // Для ограничений на последующие операции
            // не должны проставляться симметричные ограничения.
            if (luaName != "NextModeRestriction")
            {
                SortedDictionary<int, List<int>>.KeyCollection keyColl =
                    restrictList.Keys;

                foreach (int key in keyColl)
                {
                    TechObject to = TechObjectManager.GetInstance().Objects[key - 1];
                    foreach (int val in restrictList[key])
                    {
                        Mode mode = to.GetMode(val - 1);
                        mode.AddRestriction(luaName, ownerObjNum, ownerModeNum);
                    }
                }
            }
        }

        /// <summary>
        /// Удаление перекрестных ограничеий
        /// </summary>
        private void ClearCrossRestriction(SortedDictionary<int, List<int>> diffDict)
        {
            // Для ограничений на последующие операции
            // не должны проставляться симметричные ограничения.
            if ((luaName != "NextModeRestriction") && (diffDict.Count != 0))
            {
                SortedDictionary<int, List<int>>.KeyCollection keyColl =
                    diffDict.Keys;

                foreach (int key in keyColl)
                {
                    TechObject to = TechObjectManager.GetInstance().Objects[key - 1];
                    foreach (int val in diffDict[key])
                    {
                        Mode mode = to.GetMode(val - 1);
                        mode.DelRestriction(luaName, ownerObjNum, ownerModeNum);
                    }
                }
            }
        }

        /// <summary>
        /// Изменение словаря огарничений на основании строки ограничний
        /// </summary>
        private void ChangeRestrictList()
        {
            SortedDictionary<int, List<int>> res = new SortedDictionary<int, List<int>>();
            for (int i = 0; i < TechObjectManager.GetInstance().Objects.Count; i++)
            {
                TechObject to = TechObjectManager.GetInstance().Objects[i];
                for (int j = 0; j < to.GetModesManager.GetModes.Count; j++)
                {
                    string restrictPair = "{ " + (i + 1).ToString() + ", " + (j + 1).ToString() + " }";
                    if (restrictStr.Contains(restrictPair))
                    {
                        if (res.ContainsKey(i + 1))
                        {
                            res[i + 1].Add(j + 1);
                        }
                        else
                        {
                            List<int> restrictMode = new List<int>();
                            restrictMode.Add(j + 1);
                            res.Add(i + 1, restrictMode);
                        }
                    }
                }
            }
            restrictList = res;
        }

        /// <summary>
        /// Добавление ограничений
        /// </summary>
        /// <param name="ObjNum">Номер объекта</param>
        /// <param name="ModeNum">Номер операции</param>
        public void AddRestriction(int ObjNum, int ModeNum)
        {
            if (restrictList.ContainsKey(ObjNum))
            {
                if (!restrictList[ObjNum].Contains(ModeNum))
                {
                    restrictList[ObjNum].Add(ModeNum);
                }
            }
            else
            {
                List<int> restrictMode = new List<int>();
                restrictMode.Add(ModeNum);
                restrictList.Add(ObjNum, restrictMode);
            }

            //Компануем строку для отображения
            ChangeRestrictStr();
        }

        /// <summary>
        /// Удаление ограничения
        /// </summary>
        /// <param name="ObjNum">Номер объекта</param>
        /// <param name="ModeNum">Номер операции</param>
        public void DelRestriction(int ObjNum, int ModeNum)
        {
            if (restrictList.ContainsKey(ObjNum))
            {
                if (restrictList[ObjNum].Contains(ModeNum))
                {
                    if (restrictList[ObjNum].Count == 1)
                    {
                        restrictList.Remove(ObjNum);
                    }
                    else
                    {
                        restrictList[ObjNum].Remove(ModeNum);
                    }
                }
            }

            //Компануем строку для отображения
            ChangeRestrictStr();
        }

        /// <summary>
        /// Сортировка словаря ограничений
        /// </summary>
        public void SortRestriction()
        {

            SortedDictionary<int, List<int>>.KeyCollection keyColl =
                    restrictList.Keys;

            foreach (int key in keyColl)
            {
                restrictList[key].Sort();
            }
            ChangeRestrictStr();
        }

        public void SetRestrictionOwner(int objNum, int modeNum)
        {
            ownerObjNum = objNum;
            ownerModeNum = modeNum;
        }

        /// <summary>
        /// Изменение строки ограничений на основании словаря ограничений
        /// </summary>
        private void ChangeRestrictStr()
        {
            string res = "";
            SortedDictionary<int, List<int>>.KeyCollection keyColl =
                    restrictList.Keys;
            foreach (int key in keyColl)
            {
                List<int> valueColl = restrictList[key];
                foreach (int val in valueColl)
                {
                    res += "{ " + key.ToString() + ", " + val.ToString() + " } ";
                }
            }

            restrictStr = res.Trim();
        }

        public void ChangeObjNum(int prev, int curr)
        {
            if (curr != -1)
            {
                // Перемещение объекта вверх
                if (prev > curr)
                {
                    for (int i = curr; i < prev; i++)
                    {
                        if (restrictStr.Contains("{ " + i.ToString()))
                        {
                            restrictStr = restrictStr.Replace("{ " + i.ToString(), "{N " + (i + 1).ToString());
                        }
                    }
                }
                // Перемещение объекта вниз
                if (prev < curr)
                {
                    for (int i = curr; i > prev; i--)
                    {
                        if (restrictStr.Contains("{ " + i.ToString()))
                        {
                            restrictStr = restrictStr.Replace("{ " + i.ToString(), "{N " + (i - 1).ToString());
                        }
                    }
                }
                if (restrictStr.Contains("{ " + prev.ToString()))
                {
                    restrictStr = restrictStr.Replace("{ " + prev.ToString(), "{ " + curr.ToString());
                    if (restrictStr.Contains("N"))
                    {
                        restrictStr = restrictStr.Replace("N", "");
                    }
                }
            }
            // Удаление объекта ( индекс -1 )
            else
            {
                if (restrictStr.Contains("{ " + prev.ToString()))
                {
                    restrictStr += " ";
                    int idx = restrictStr.IndexOf("{ " + prev.ToString());
                    int idx_end = restrictStr.IndexOf("}", idx);
                    restrictStr = restrictStr.Remove(idx, idx_end - idx + 2); // 2й символ для пробела
                    restrictStr.Trim();
                    this.ChangeObjNum(prev, -1);
                }
                else
                {
                    for (int i = prev + 1;
                        i < TechObjectManager.GetInstance().GetTechObj.Count; i++)
                    {
                        if (restrictStr.Contains("{ " + i.ToString()))
                        {
                            restrictStr = restrictStr.Replace("{ " + i.ToString(), "{ " + (i - 1).ToString());
                        }
                    }
                }
            }

            ChangeRestrictList();
        }

        public void ChangeModeNum(int objNum, int prev, int curr)
        {
            if (curr != -1)
            {
                // Перемещение объекта вверх
                if (prev > curr)
                {
                    for (int i = curr; i < prev; i++)
                    {
                        if (restrictStr.Contains("{ " + objNum.ToString() + ", " + i.ToString() + " }"))
                        {
                            restrictStr = restrictStr.Replace("{ " + objNum.ToString() + ", " + i.ToString() + " }",
                                "{ " + objNum.ToString() + ", " + (i + 1).ToString() + " N}");
                        }
                    }
                }
                // Перемещение объекта вниз
                if (prev < curr)
                {
                    for (int i = curr; i > prev; i--)
                    {
                        if (restrictStr.Contains("{ " + objNum.ToString() + ", " + i.ToString() + " }"))
                        {
                            restrictStr = restrictStr.Replace("{ " + objNum.ToString() + ", " + i.ToString() + " }",
                                "{ " + objNum.ToString() + ", " + (i - 1).ToString() + " N}");
                        }
                    }
                }
                if (restrictStr.Contains("{ " + objNum.ToString() + ", " + prev.ToString() + " }"))
                {
                    restrictStr = restrictStr.Replace("{ " + objNum.ToString() + ", " + prev.ToString() + " }",
                            "{ " + objNum.ToString() + ", " + curr.ToString() + " }");

                }
                if (restrictStr.Contains("N"))
                {
                    restrictStr = restrictStr.Replace("N", "");
                }
            }
            // Удаление объекта ( индекс -1 )
            else
            {
                if (restrictStr.Contains("{ " + objNum.ToString() + ", " + prev.ToString() + " }"))
                {
                    restrictStr += " ";
                    restrictStr = restrictStr.Replace("{ " + objNum.ToString() + ", " + prev.ToString() + " } ", "");
                    restrictStr.Trim();
                    this.ChangeModeNum(objNum, prev, -1);
                }
                else
                {
                    for (int i = prev + 1;
                        i < TechObjectManager.GetInstance().GetTechObj[objNum - 1].GetModesManager.GetModes.Count; i++)
                    {
                        if (restrictStr.Contains("{ " + objNum.ToString() + ", " + i.ToString() + " }"))
                        {
                            restrictStr = restrictStr.Replace("{ " + objNum.ToString() + ", " + i.ToString() + " }",
                                "{ " + objNum.ToString() + ", " + (i - 1).ToString() + " }");
                        }
                    }
                }
            }
            ChangeRestrictList();
        }

        /// <summary>
        /// Замена номера объекта при копировании
        /// </summary>
        /// <param name="oldObjN">Номер старого объекта</param>
        /// <param name="newObjN">Номер нового объекта</param>
        public void ModifyRestrictObj(int oldObjN, int newObjN)
        {
            ownerObjNum = newObjN;

            if (IsLocalRestrictionUse)
            {
                if (restrictStr.Contains("{ " + oldObjN.ToString()))
                {
                    restrictStr = restrictStr.Replace("{ " + oldObjN.ToString(), "{ " + newObjN.ToString());
                }
            }
            ChangeRestrictList();
        }


        public SortedDictionary<int, List<int>> RestrictDictionary
        {
            get
            {
                return restrictList;
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return null;
            }
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
            {//Можем редактировать содержимое второй колонки.
                return new int[] { -1, 1 };
            }
        }

        override public string[] DisplayText
        {
            get
            {
                return new string[] { name, restrictStr };
            }
        }

        override public string[] EditText
        {
            get
            {
                return new string[] { "", restrictStr };
            }
        }

        override public bool IsUseRestriction
        {
            get
            {
                return true;
            }
        }

        override public bool IsLocalRestrictionUse
        {
            get
            {
                return false;
            }
        }

        override public bool IsReplaceable
        {
            get
            {
                return true;
            }
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        public string LuaName
        {
            get
            {
                return luaName;
            }
        }

        private string name;
        private string restrictStr;
        protected string luaName;
        private SortedDictionary<int, List<int>> restrictList;

        private int ownerObjNum;
        private int ownerModeNum;

    }


    public class LocalRestriction : Restriction
    {
        public LocalRestriction(string Name, string Value, string LuaName, SortedDictionary<int, List<int>> dict)
            : base(Name, Value, LuaName, dict)
        {

        }

        override public bool IsLocalRestrictionUse
        {
            get
            {
                return true;
            }
        }
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Менеджер ограничений. Содержит ограничения среди всех объектов и для конкретного объекта
    /// </summary>
    public class RestrictionManager : Editor.TreeViewItem
    {
        public RestrictionManager()
        {
            name = "Ограничения";
            restrictions = new List<Restriction>();

            Restriction TotalResriction = new Restriction("Общие ограничения", "",
                "TotalRestriction", new SortedDictionary<int, List<int>>());
            Restriction ThisObjResriction = new LocalRestriction("Ограничения внутри объекта",
                "", "LocalRestriction", new SortedDictionary<int, List<int>>());
            Restriction NextModeResriction = new LocalRestriction("Ограничения на последующие операции",
                "", "NextModeRestriction", new SortedDictionary<int, List<int>>());
            restrictions.Add(ThisObjResriction);
            restrictions.Add(TotalResriction);
            restrictions.Add(NextModeResriction);

        }

        public RestrictionManager Clone()
        {
            RestrictionManager clone = (RestrictionManager)MemberwiseClone();
            clone.restrictions = new List<Restriction>();
            foreach (Restriction rest in restrictions)
            {
                clone.restrictions.Add(rest.Clone());
            }

            return clone;
        }

        public void CheckRestriction(int prev, int curr)
        {
            foreach (Restriction restrict in restrictions)
            {
                restrict.ChangeObjNum(prev, curr);
            }
        }

        public void ChangeModeNum(int objNum, int prev, int curr)
        {
            foreach (Restriction restrict in restrictions)
            {
                restrict.ChangeModeNum(objNum, prev, curr);
            }
        }

        public void ModifyRestrictObj(int oldObjN, int newObjN)
        {
            foreach (Restriction restrict in restrictions)
            {
                restrict.ModifyRestrictObj(oldObjN, newObjN);
            }
        }

        public void ChangeCrossRestriction(RestrictionManager oldRestrictMngr = null)
        {
            for (int i = 0; i < restrictions.Count; i++)
            {
                if (oldRestrictMngr != null)
                {
                    restrictions[i].ChangeCrossRestriction(oldRestrictMngr.Restrictions[i].RestrictDictionary);
                }
                else
                {
                    restrictions[i].ChangeCrossRestriction();
                }
            }
        }

        public string SaveRestrictionAsLua(string prefix)
        {
            string res = "";
            int totalCount = 0;
            for (int i = 0; i < restrictions.Count; i++)
            {
                string tmp = restrictions[i].SaveRestrictionAsLua(prefix + "\t\t");
                if (tmp != "")
                {
                    res += prefix + "\t[ " + (i + 1).ToString() + " ] =\n" + tmp;
                }
                totalCount += restrictions[i].RestrictDictionary.Count;
            }

            if (totalCount == 0)
            {
                res = "";
            }

            return res;
        }

        public List<Restriction> Restrictions
        {

            get
            {
                return restrictions;
            }

        }

        override public string[] DisplayText
        {
            get
            {
                return new string[] { name, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return restrictions.ToArray();
            }
        }

        override public bool IsEditable
        {
            get
            {
                return false;
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return false;
            }
        }

        override public bool IsMoveable
        {
            get
            {
                return false;
            }
        }

        override public bool IsReplaceable
        {
            get
            {
                return true;
            }
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem Replace(object child,
            object copyObject)
        {
            if (child is Restriction)
            {
                Restriction restrict = child as Restriction;

                if (copyObject is Restriction && restrict != null)
                {
                    Restriction copy = copyObject as Restriction;
                    restrict.SetNewValue(copy.EditText[1]);

                    return restrict;
                }
            }
            return null;
        }


        private string name;
        private List<Restriction> restrictions;
    }



    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Состояние операции. Содержит группу шагов, выполняемых последовательно
    /// (или в ином порядке).
    /// </summary>
    public class State : Editor.TreeViewItem
    {
        /// <summary>
        /// Получение шага по номеру (нумерация с -1 - шаг операции, который 
        /// выполняется пока включена операция).
        /// </summary>
        /// <param name="idx">Номер шага.</param>        
        /// <returns>Шаг с заданным номером.</returns>
        public Step this[int idx]
        {
            get
            {
                if (modeStep == null) //Добавляем, если его нет.
                {
                    modeStep = new Step("Во время операции", this.GetStepN, this, isMain);
                    steps.Add(modeStep);
                }

                if (steps.Count > 0)
                {
                    if (idx == -1)
                    {
                        return steps[0];
                    }

                    if (idx < steps.Count - 1)
                    {
                        return steps[idx + 1];
                    }
                }

                return null;
            }
        }

        public int GetStepN(object step)
        {
            return steps.IndexOf(step as Step);
        }

        /// <summary>
        /// Создание нового состояния.
        /// </summary>
        /// <param name="name">Имя состояния.</param>
        /// <param name="isMain">Надо ли дополнительные действия.</param>
        /// <param name="needModeStep">Надо ли основной шаг.</param>
        /// <param name="owner">Владелец состояния (Операция)</param>
        public State(string name, bool isMain, Mode owner, bool needMainStep = false)
        {
            this.name = name;
            this.isMain = isMain;
            this.owner = owner;
            steps = new List<Step>();

            if (needMainStep)
            {
                modeStep = new Step("Во время операции", this.GetStepN, this, true);
                steps.Add(modeStep);
            }
        }

        public State Clone(string name = "")
        {
            State clone = (State)MemberwiseClone();

            if (name != "")
            {
                clone.name = name;
            }

            clone.steps = new List<Step>();

            if (modeStep != null)
            {
                clone.modeStep = modeStep.Clone(clone.GetStepN);
                clone.steps.Add(clone.modeStep);
            }

            for (int idx = 1; idx < steps.Count; idx++)
            {
                clone.steps.Add(steps[idx].Clone(clone.GetStepN));
            }

            return clone;
        }

        public void ModifyDevNames(int newTechObjectN, int oldTechObjectN,
            string techObjectName)
        {
            foreach (Step step in steps)
            {
                step.ModifyDevNames(newTechObjectN, oldTechObjectN,
                    techObjectName);
            }
        }

        public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName)
        {
            foreach (Step step in steps)
            {
                step.ModifyDevNames(newTechObjectName, newTechObjectNumber,
                    oldTechObjectName);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            if (steps.Count == 0) return "";

            string res = "";

            if (modeStep != null)
            {
                res += modeStep.SaveAsLuaTable(prefix, true);
            }

            if (steps.Count > 1)
            {
                int i = 1;
                res += prefix + "steps =\n" +
                    prefix + "\t{\n";

                for (int j = 1; j < steps.Count; j++)
                {
                    res += prefix + "\t[ " + i++ + " ] =\n";
                    res += steps[j].SaveAsLuaTable(prefix + "\t\t");
                }

                res += prefix + "\t},\n";
            }

            if (res != "")
            {
                res = prefix + "--\'" + name + "\'\n" + res;
            }

            return res;
        }

        /// <summary>
        /// Добавление нового шага.
        /// </summary>
        /// <param name="stepName">Имя шага.</param>
        public void AddStep(string stepName)
        {
            Step newStep = new Step(stepName, GetStepN, this);

            if (modeStep == null)
            {
                modeStep = new Step("Во время операции", GetStepN, this, isMain);
                steps.Add(modeStep);
            }

            steps.Add(newStep);
        }

        public List<Step> Steps
        {
            get
            {
                return steps;
            }
        }

        public void Synch(int[] array)
        {
            foreach (Step step in steps)
            {
                step.Synch(array);
            }
        }

        /// <summary>
        /// Удаление списка шагов.
        /// </summary>
        public void Clear()
        {
            steps.Clear();
            modeStep = null;
        }

        //Возврат имени шага
        public string GetStepName()
        {
            return name;
        }

        public Mode Owner
        {
            get
            {
                return owner;
            }
        }

        /// <summary>
        /// Проверка шагов состояния
        /// </summary>
        public void Check()
        {
            List<Step> steps = Steps;
            foreach (Step step in steps)
            {
                step.Check();
            }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = name;
                if (steps.Count > 1)
                {
                    res += " (" + (steps.Count - 1) + ")";
                }

                return new string[] { res, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return steps.ToArray();
            }
        }

        override public bool IsEditable
        {
            get
            {
                return false;
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return false;
            }
        }

        override public bool IsMoveable
        {
            get
            {
                return false;
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
            Step step = child as Step;

            if (step != null)
            {
                if (steps.IndexOf(step) == 0)
                {
                    //Не удаляем шаг операции.
                    return false;
                }

                steps.Remove(step);
                return true;
            }

            Action action = child as Action;
            if (action != null)
            {
                action.Clear();
                return false;
            }

            return false;
        }

        override public Editor.ITreeViewItem MoveUp(object child)
        {
            Step step = child as Step;

            if (step != null)
            {
                int index = steps.IndexOf(step);
                if (index > 1)
                {
                    steps.Remove(step);
                    steps.Insert(index - 1, step);
                    return steps[index];
                }

            }

            return null;
        }

        override public Editor.ITreeViewItem MoveDown(object child)
        {
            Step step = child as Step;

            if (step != null)
            {
                int index = steps.IndexOf(step);
                if (index <= steps.Count - 2 && index > 0)
                {
                    steps.Remove(step);
                    steps.Insert(index + 1, step);
                    return steps[index];
                }

            }

            return null;
        }

        override public Editor.ITreeViewItem Replace(object child,
            object copyObject)
        {
            Step step = child as Step;
            if (copyObject is Step && step != null)
            {
                Step newStep = (copyObject as Step).Clone(GetStepN);
                int index = steps.IndexOf(step);
                steps.Remove(step);

                steps.Insert(index, newStep);

                index = steps.IndexOf(newStep);

                return newStep;
            }

            return null;
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        override public object Copy()
        {
            return this;
        }

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem Insert()
        {
            if (modeStep == null)
            {
                modeStep = new Step("Во время операции", GetStepN, this, isMain);
                steps.Add(modeStep);
                return modeStep;
            }

            Step newStep = new Step("Новый шаг", GetStepN, this);
            steps.Add(newStep);

            return newStep;
        }

        override public bool IsDrawOnEplanPage
        {
            get
            {
                return true;
            }
        }

        override public List<Editor.DrawInfo> GetObjectToDrawOnEplanPage()
        {
            List<Editor.DrawInfo> devToDraw = new List<Editor.DrawInfo>();
            foreach (Step step in steps)
            {
                List<Editor.DrawInfo> devToDrawTmp = step.GetObjectToDrawOnEplanPage();
                foreach (Editor.DrawInfo dinfo in devToDrawTmp)
                {
                    bool isSetFlag = false;
                    for (int i = 0; i < devToDraw.Count; i++)
                    {
                        if (devToDraw[i].dev.Name == dinfo.dev.Name)
                        {
                            isSetFlag = true;
                            if (devToDraw[i].style != dinfo.style)
                            {
                                devToDraw.Add(
                                    new Editor.DrawInfo(Editor.DrawInfo.Style.GREEN_RED_BOX,
                                    devToDraw[i].dev));
                                devToDraw.RemoveAt(i);
                            }
                        }
                    }

                    if (isSetFlag == false)
                    {
                        devToDraw.Add(dinfo);
                    }
                }
            }

            return devToDraw;
        }

        #endregion

        private string name;        ///< Имя.
        internal List<Step> steps;  ///< Список шагов.
        private Step modeStep;      ///< Шаг.
        bool isMain;                ///< Надо ли дополнительные действия.
        private Mode owner;         ///< Владелец элемента
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Операция технологического объекта. Состоит из последовательно (или в ином
    /// порядке) выполняемых шагов.
    /// </summary>
    public class Mode : Editor.TreeViewItem
    {
        /// <summary>
        /// Получение состояния номеру (нумерация с 0).
        /// </summary>
        /// <param name="idy">Номер состояния.</param>        
        /// <returns>Состояние с заданным номером.</returns>
        public State this[int idy]
        {
            get
            {
                if (idy < (int)StateName.STATES_CNT)
                {
                    return stepsMngr[idy];
                }

                return null;
            }
        }

        /// <summary>
        /// Создание новой операции.
        /// </summary>
        /// <param name="name">Имя операции.</param>
        /// <param name="getN">Функция получения номера операции.</param>
        /// <param name="newOwner">Владелец операции (Менеджер операций)</param>
        public Mode(string name, GetN getN, ModesManager newOwner)
        {
            this.name = name;
            this.getN = getN;
            this.owner = newOwner;

            restrictionMngr = new RestrictionManager();

            stepsMngr = new List<State>();

            stepsMngr.Add(new State(StateStr[(int)StateName.RUN], true, this, true));
            for (StateName i = StateName.PAUSE; i < StateName.STATES_CNT; i++)
            {
                stepsMngr.Add(new State(StateStr[(int)i], false, this));
            }

            operPar = new OperationParams();

            baseOperation = new BaseOperation(this); // Экземпляр класса базовой операции

            SetItems();
        }

        /// <summary>
        /// Добавление полей в массив для отображения на дереве.
        /// </summary>
        void SetItems()
        {
            items = new Editor.ITreeViewItem[stepsMngr.Count + 3];

            for (int i = 0; i < stepsMngr.Count; i++)
            {
                items[i] = stepsMngr[i];
            }

            items[stepsMngr.Count] = operPar;
            items[stepsMngr.Count + 1] = restrictionMngr;
            items[stepsMngr.Count + 2] = baseOperation;
        }

        public OperationParams GetOperationParams()
        {
            return operPar;
        }

        public Mode Clone(GetN getN, ModesManager newOwner, string name = "")
        {
            Mode clone = (Mode)MemberwiseClone();
            clone.getN = getN;
            clone.owner = newOwner;

            if (name != "")
            {
                clone.name = name;
            }

            clone.stepsMngr = new List<State>();
            for (int idx = 0; idx < stepsMngr.Count; idx++)
            {
                clone.stepsMngr.Add(stepsMngr[idx].Clone());
            }

            clone.restrictionMngr = restrictionMngr.Clone();
            clone.SetItems();

            return clone;
        }

        /// <summary>
        /// Изменение менеджера операций объекта-владельца операций.
        /// </summary>
        /// <param name="newOwner">Новый менеджер операций.</param>
        /// <returns></returns>
        public void ChangeOwner(ModesManager newOwner)
        {
            owner = newOwner;
        }

        public void ModifyDevNames(int newTechObjectN, int oldTechObjectN,
            string techObjectName)
        {
            foreach (State stpsMngr in stepsMngr)
            {
                stpsMngr.ModifyDevNames(newTechObjectN, oldTechObjectN,
                    techObjectName);
            }
        }

        public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName)
        {
            foreach (State stpsMngr in stepsMngr)
            {
                stpsMngr.ModifyDevNames(newTechObjectName,
                    newTechObjectNumber, oldTechObjectName);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + "{\n" +
                prefix + "name = \'" + name + "\',\n" +
                prefix + "base_operation = \'" + baseOperation.GetName() + "\',\n";

            // Запись параметров базовой операции, если они есть
            if (baseOperation.GetParamsCount() > 0)
            {
                res += baseOperation.SaveAsLuaTable(prefix);
            }

            string tmp = "";
            //Совместимость с предыдущей версией (до введения состояния для
            //операции).
            if (stepsMngr.Count > 0)
            {
                tmp = stepsMngr[0].SaveAsLuaTable(prefix);
                if (tmp != "")
                {
                    res += prefix + "--Совместимость с предыдущей версией (до введения состояния для операции).\n";
                    res += tmp;
                    res += prefix + "--Совместимость с предыдущей версией.\n";
                    res += "\n";
                }
            }

            int i = 1;
            string tmp_2 = "";

            for (int j = 0; j < stepsMngr.Count; j++)
            {
                tmp = stepsMngr[j].SaveAsLuaTable(prefix + "\t\t");
                if (tmp != "")
                {
                    tmp_2 += prefix + "\t[ " + i++ + " ] =\n";
                    tmp_2 += prefix + "\t\t{\n";
                    tmp_2 += tmp;
                    tmp_2 += prefix + "\t\t},\n";
                }
            }
            if (tmp_2 != "")
            {
                res += prefix + "states =\n" +
                    prefix + "\t{\n";
                res += tmp_2;
                res += prefix + "\t},\n";
            }


            res += prefix + "},\n";
            return res;
        }

        /// <summary>
        /// Добавление нового шага.
        /// </summary>
        /// <param name="stepName">Имя шага.</param>
        public void AddStep(int stateN, string stepName)
        {
            if (stateN >= 0 && stateN < (int)StateName.STATES_CNT)
            {
                stepsMngr[stateN].AddStep(stepName);
            }
        }

        public List<Step> MainSteps
        {
            get
            {
                return stepsMngr[0].Steps;
            }
        }

        public void Synch(int[] array)
        {
            foreach (State stpsMngr in stepsMngr)
            {
                stpsMngr.Synch(array);
            }
        }


        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveRestrictionAsLua(string prefix)
        {
            string res = "";
            string tmp = "";
            tmp += restrictionMngr.SaveRestrictionAsLua(prefix);
            if (tmp != "")
            {
                res += prefix + "{\n" + tmp + prefix + "},\n";
            }
            return res;
        }

        public RestrictionManager GetRestrictionManager()
        {
            return restrictionMngr;
        }

        /// <summary>
        /// Функция установки ограничений для операции
        /// </summary>
        /// <param name="luaName">Lua имя объекта</param>
        /// <param name="value">Новая строка ограничений</param>
        public void SetRestriction(string luaName, string value)
        {
            foreach (Restriction restrict in restrictionMngr.Restrictions)
            {
                if (restrict.LuaName == luaName)
                {
                    restrict.SetNewValue(value);
                }
            }
        }

        /// <summary>
        /// Функция установки ограничений для операции
        /// </summary>
        /// <param name="luaName">Lua имя объекта</param>
        /// <param name="value">Новая строка ограничений</param>
        public void AddRestriction(string luaName, int ObjNum, int ModeNum)
        {

            foreach (Restriction restrict in restrictionMngr.Restrictions)
            {
                if (restrict.LuaName == luaName)
                {
                    restrict.AddRestriction(ObjNum, ModeNum);
                }
            }
        }

        /// <summary>
        /// Функция удаления ограничения для операции
        /// </summary>
        public void DelRestriction(string luaName, int ObjNum, int ModeNum)
        {
            foreach (Restriction restrict in restrictionMngr.Restrictions)
            {
                if (restrict.LuaName == luaName)
                {
                    restrict.DelRestriction(ObjNum, ModeNum);
                }
            }
        }

        /// <summary>
        /// Функция для сортировки ограничений после считывания из файла
        /// </summary>
        public void SortRestriction()
        {
            foreach (Restriction restrict in restrictionMngr.Restrictions)
            {
                restrict.SortRestriction();
            }
        }

        /// <summary>
        /// Функция для задания номеров родителей ограничений
        /// </summary>
        public void SetRestrictionOwner(int objNum, int modeNum)
        {
            foreach (Restriction restrict in restrictionMngr.Restrictions)
            {
                restrict.SetRestrictionOwner(objNum, modeNum);
            }
        }

        public void CheckRestriction(int prev, int curr)
        {
            restrictionMngr.CheckRestriction(prev, curr);
        }

        public void ModifyRestrictObj(int oldObjN, int newObjN)
        {

            restrictionMngr.ModifyRestrictObj(oldObjN, newObjN);
        }

        public void ChangeModeNum(int objNum, int prev, int curr)
        {
            restrictionMngr.ChangeModeNum(objNum, prev, curr);
        }

        public void ChangeCrossRestriction(Mode oldMode = null)
        {
            if (oldMode == null)
            {
                restrictionMngr.ChangeCrossRestriction();
            }
            else
            {
                restrictionMngr.ChangeCrossRestriction(oldMode.GetRestrictionManager());
            }
        }

        // Установка параметров базовой операции
        public void SetBaseOperExtraParams(Editor.ObjectProperty[] extraParams)
        {
            baseOperation.SetExtraProperties(extraParams);
        }

        public BaseOperation GetBaseOperation()
        {
            return baseOperation;
        }

        // Получение номера операции
        public int GetModeNumber()
        {
            return getN(this);
        }

        public ModesManager Owner
        {
            get
            {
                return owner;
            }
        }

        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Проверка состояний состоящих из шагов
        /// </summary>
        public void Check()
        {
            List<State> stepsManager = stepsMngr;
            foreach (State state in stepsManager)
            {
                state.Check();
            }
        }
        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = getN(this) + ". " + name;

                return new string[] { res, baseOperation.GetName() };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                //return stepsMngr.ToArray();
                return items;
            }
        }

        override public bool SetNewValue(string newName)
        {
            name = newName;
            return true;
        }

        public override bool SetNewValue(string newBaseOperationName, bool isBaseOper)
        {
            // Инициализация базовой операции по имени
            baseOperation.Init(newBaseOperationName);
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
                //Можем редактировать содержимое двух колонок.
                return new int[] { 0, 1 };
            }
        }

        override public string[] EditText
        {
            get
            {
                return new string[] { name, baseOperation.GetName() };
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public bool IsMoveable
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

        override public Editor.ITreeViewItem Replace(object child,
            object copyObject)
        {

            if (child is State)
            {
                State stpMngr = child as State;
                if (copyObject is State && stpMngr != null)
                {
                    State newStpMngr = (copyObject as State).Clone();
                    int index = stepsMngr.IndexOf(stpMngr);
                    stepsMngr.Remove(stpMngr);
                    stepsMngr.Insert(index, newStpMngr);

                    return newStpMngr;
                }
            }

            if (child is RestrictionManager)
            {
                RestrictionManager restrictMan = child as RestrictionManager;

                if (copyObject is RestrictionManager && restrictMan != null)
                {
                    RestrictionManager copyMan = copyObject as RestrictionManager;
                    for (int i = 0; i < restrictMan.Restrictions.Count; i++)
                    {
                        restrictMan.Replace(restrictMan.Items[i], copyMan.Items[i]);
                    }

                    int objNum = TechObjectManager.GetInstance().GetTechObjectN(owner.Owner);
                    int modeNum = getN(this);

                    foreach (Restriction restrict in restrictMan.Restrictions)
                    {
                        restrict.SetRestrictionOwner(objNum, modeNum);
                    }
                    return restrictMan;
                }
            }

            return null;
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        override public object Copy()
        {
            return this;
        }

        override public bool IsInsertable
        {
            get
            {
                return false;
            }
        }

        override public bool IsDrawOnEplanPage
        {
            get
            {
                return true;
            }
        }

        override public List<Editor.DrawInfo> GetObjectToDrawOnEplanPage()
        {
            List<Editor.DrawInfo> devToDraw = new List<Editor.DrawInfo>();
            foreach (State stpMngr in stepsMngr)
            {
                List<Editor.DrawInfo> devToDrawTmp = stpMngr.GetObjectToDrawOnEplanPage();
                foreach (Editor.DrawInfo dinfo in devToDrawTmp)
                {
                    bool isSetFlag = false;
                    for (int i = 0; i < devToDraw.Count; i++)
                    {
                        if (devToDraw[i].dev.Name == dinfo.dev.Name)
                        {
                            isSetFlag = true;
                            if (devToDraw[i].style != dinfo.style)
                            {
                                devToDraw.Add(
                                    new Editor.DrawInfo(Editor.DrawInfo.Style.GREEN_RED_BOX,
                                    devToDraw[i].dev));
                                devToDraw.RemoveAt(i);
                            }
                        }
                    }

                    if (isSetFlag == false)
                    {
                        devToDraw.Add(dinfo);
                    }
                }
            }

            return devToDraw;
        }

        #endregion

        public enum StateName
        {
            RUN = 0,// Выполнение
            PAUSE,  // Пауза
            STOP,   // Остановка

            STATES_CNT = 3,
        }

        public string[] StateStr =
            {
            "Выполнение",
            "Пауза",
            "Остановка",
            };

        private GetN getN;

        private string name;           ///< Имя операции.
        internal List<State> stepsMngr;///< Список шагов операции для состояний.
        private RestrictionManager restrictionMngr;
        private Editor.ITreeViewItem[] items;

        private OperationParams operPar;

        private ModesManager owner;

        private BaseOperation baseOperation; ///< Базовая операция
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Класс реализующий базовую операцию для технологического объекта
    /// </summary>
    /// 

    public class BaseOperation : Editor.TreeViewItem
    {
        public BaseOperation(Mode owner)
        {
            this.operationName = "";
            this.luaOperationName = "";
            this.owner = owner;
        }

        public BaseOperation(string name, string luaName, Mode owner)
        {
            this.operationName = name;
            this.luaOperationName = luaName;
            this.owner = owner;
        }

        // Конструктор для инициализации хранилища с именами базовых операций
        public BaseOperation(string name, string luaName)
        {
            this.operationName = name;
            this.luaOperationName = luaName;
        }

        // Конструктор для инициализации базовой операции и параметров
        public BaseOperation(string name, string luaName, BaseOperationProperty[] baseOperationProperties)
        {
            this.operationName = name;
            this.luaOperationName = luaName;
            this.baseOperationProperties = baseOperationProperties;
        }

        public string GetName()
        {
            return operationName;
        }

        public string GetLuaName()
        {
            return luaOperationName;
        }

        public void SetName(string operationName)
        {
            this.operationName = operationName;
        }

        public void SetLuaName(string luaName)
        {
            this.luaOperationName = luaName;
        }

        // Получение количества параметров у операции
        public int GetParamsCount()
        {
            if (BaseOperationProperties == null)
            {
                return 0;
            }
            return baseOperationProperties.Length;
        }

        // Инициализация полей при выборе базовой операции
        public void Init(string baseOperName)
        {
            // Базовый объект для поиска операции по этому объекту
            TechObject baseTechObject = owner.Owner.Owner;
            string baseTechObjectName = baseTechObject.GetBaseTechObjectName();

            if (baseTechObjectName != "")
            {
                SetName(baseOperName); // Установка имени базовой операции
                var luaName = DBImitation.FindOperationLuaName(baseOperName);
                SetLuaName(luaName); // Установка имени операции для файла Lua

                baseOperationProperties = new BaseOperationProperty[0]; // Инициализирую список параметров

                // Инициализация операции в зависимости от выбранной операции и базового объекта
                baseOperationProperties = DBImitation.GetOperParams(baseOperName, baseTechObjectName);

                SetItems();
            }
        }

        // Добавление полей в массив для отображения на дереве
        private void SetItems()
        {
            items = new Editor.ITreeViewItem[baseOperationProperties.Length];
            var counter = 0;
            foreach (BaseOperationProperty operParam in baseOperationProperties)
            {
                items[counter] = operParam;
                counter++;
            }
        }

        // Сохранение в виде таблицы Lua
        public string SaveAsLuaTable(string prefix)
        {
            var res = "";
            res += prefix + "props =\n" + prefix + "\t{\n";
            foreach (BaseOperationProperty operParam in baseOperationProperties)
            {
                res += "\t" + prefix + operParam.GetLuaName() + " = \'" + operParam.GetValue() + "\',\n";
            }
            res += prefix + "\t},\n";
            return res;
        }

        // Установка параметров базовой операции
        public void SetExtraProperties(Editor.ObjectProperty[] extraParams)
        {
            foreach (Editor.ObjectProperty extraParam in extraParams)
            {
                baseOperationProperties.First(x => x.GetLuaName().Equals(extraParam.DisplayText[0])).SetValue(extraParam.DisplayText[1]);
            }
        }

        // Возврат параметров базовой операции
        public BaseOperationProperty[] BaseOperationProperties
        {
            get { return baseOperationProperties; }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                if (items.Count() > 0)
                {
                    string res = string.Format("Дополнительные сигналы ({0})", items.Count());
                    return new string[] { res, "" };
                }
                else
                {
                    string res = string.Format("Дополнительные сигналы");
                    return new string[] { res, "" };
                }
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return items;
            }
        }
        #endregion

        private Editor.ITreeViewItem[] items = new Editor.ITreeViewItem[0];
        // Свойства базовой операции для имитационного хранилища
        private BaseOperationProperty[] baseOperationProperties;

        private string operationName; ///< Имя базовой операции
        private string luaOperationName; ///< Имя базовой операции для файла Lua

        private Mode owner;
    }

    /// <summary>    
    /// Свойство для базовой операции.
    /// </summary>
    public class BaseOperationProperty : Editor.ObjectProperty
    {
        public BaseOperationProperty(string luaName, string name, object value) : base(name, value)
        {
            this.luaName = luaName;
            this.name = name;
            this.value = value;
        }

        public string GetLuaName()
        {
            return luaName;
        }

        public string GetName()
        {
            return name;
        }

        public string GetValue()
        {
            return value.ToString();
        }

        public void SetValue(string value)
        {
            this.value = value;
        }

        #region реализация ITreeView
        public override bool SetNewValue(string newValue)
        {
            this.value = newValue;
            return true;
        }

        override public string[] EditText
        {
            get
            {
                return new string[] { luaName, value.ToString() };
            }
        }

        override public string[] DisplayText
        {
            get
            {
                return new string[] { name, value.ToString() };
            }
        }
        #endregion

        private string luaName; // Lua имя свойства
        private string name; // Имя свойства
        private object value; // Значение
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Шаг технологического объекта. Состоит из параллельно выполняемых 
    /// действий.
    /// </summary>
    public class Step : Editor.TreeViewItem
    {
        /// <summary>
        /// Создание нового шага.
        /// </summary>
        /// <param name="name">Имя шага.</param>
        /// <param name="getN">Функция получения номера шага.</param>
        /// <param name="isMode">Признак того, является ли шаг шагом операции.</param>
        /// <param name="owner">Владелец шага (Состояние)</param>
        public Step(string name, GetN getN, State owner, bool isMode = false)
        {
            this.name = name;
            this.getN = getN;
            this.IsMode = isMode;
            this.owner = owner;

            items = new List<Editor.ITreeViewItem>();

            actions = new List<Action>();
            actions.Add(new Action("Включать", this, "opened_devices",
                new Device.DeviceType[3] { Device.DeviceType.V, Device.DeviceType.DO, Device.DeviceType.M }));
            actions.Add(new Action("Включать реверс", this, "opened_reverse_devices",
                new Device.DeviceType[1] { Device.DeviceType.M }));
            actions.Add(new Action("Выключать", this, "closed_devices",
                new Device.DeviceType[3] { Device.DeviceType.V, Device.DeviceType.DO, Device.DeviceType.M }));
            actions[2].DrawStyle = Editor.DrawInfo.Style.RED_BOX;

            actions.Add(new Action_WashSeats("Верхние седла", this, "opened_upper_seat_v"));
            actions[3].DrawStyle = Editor.DrawInfo.Style.GREEN_UPPER_BOX;
            actions.Add(new Action_WashSeats("Нижние седла", this, "opened_lower_seat_v"));
            actions[4].DrawStyle = Editor.DrawInfo.Style.GREEN_LOWER_BOX;

            if (isMode)
            {
                actions.Add(new Action("Сигналы для включения", this, "required_FB",
                    new Device.DeviceType[2] { Device.DeviceType.DI, Device.DeviceType.GS }));
                actions.Add(new ActionWash("Мойка( DI, DO, устройства)", this, "wash_data"));
                actions.Add(new Action_DI_DO("Группы DI -> DO DO ...", this, "DI_DO"));
                actions.Add(new Action_AI_AO("Группы AI -> AO AO ...", this, "AI_AO"));
            }

            items.AddRange(actions.ToArray());

            if (!isMode)
            {
                timeParam = new Editor.ObjectProperty("Время (параметр)", -1);
                nextStepN = new Editor.ObjectProperty("Номер следующего шага", -1);

                items.Add(timeParam);
                items.Add(nextStepN);
            }
        }

        public Step Clone(GetN getN, string name = "")
        {
            Step clone = (Step)MemberwiseClone();
            clone.getN = getN;

            if (name != "")
            {
                clone.name = name.Substring(3);
            }

            clone.actions = new List<Action>();
            foreach (Action action in actions)
            {
                clone.actions.Add(action.Clone());
            }

            clone.items = new List<Editor.ITreeViewItem>();
            clone.items.AddRange(clone.actions.ToArray());

            if (!IsMode)
            {
                clone.timeParam = timeParam.Clone();
                clone.nextStepN = nextStepN.Clone();

                clone.items.Add(clone.timeParam);
                clone.items.Add(clone.nextStepN);
            }

            return clone;
        }

        public void ModifyDevNames(int newTechObjectN, int oldTechObjectN,
            string techObjectName)
        {
            foreach (Action action in actions)
            {
                action.ModifyDevNames(newTechObjectN,
                    oldTechObjectN, techObjectName);
            }
        }

        public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName)
        {
            foreach (Action action in actions)
            {
                action.ModifyDevNames(newTechObjectName, newTechObjectNumber,
                    oldTechObjectName);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <param name="isShortForm">Сохранять ли сокращенном виде (для  
        /// операции без вывода названия шага).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix, bool isShortForm = false)
        {
            string res = "";

            if (isShortForm)
            {
                foreach (Action action in actions)
                {
                    res += action.SaveAsLuaTable(prefix);
                }
            }
            else
            {
                res += prefix + "{\n";
                res += prefix + "name = \'" + name + "\',\n";

                string time_param_n = timeParam.EditText[1].Trim();
                if (time_param_n != "")
                {
                    res += prefix + "time_param_n = " + time_param_n + ",\n";
                }
                string next_step_n = nextStepN.EditText[1].Trim();
                if (next_step_n != "")
                {
                    res += prefix + "next_step_n = " + next_step_n + ",\n";
                }

                foreach (Action action in actions)
                {
                    res += action.SaveAsLuaTable(prefix);
                }

                res += prefix + "},\n";
            }

            return res;
        }

        /// <summary>
        /// Добавление параметров.
        /// </summary>
        /// <param name="time_param_n">Номер параметра со временем шага.</param>
        /// <param name="next_step_n">Номер следующего шага.</param>
        public void SetPar(int timeParamN, int nextStepN)
        {
            this.timeParam.SetNewValue(timeParamN.ToString());
            this.nextStepN.SetNewValue(nextStepN.ToString());
        }

        /// <summary>
        /// Добавление устройства.
        /// </summary>
        /// <param name="actionLuaName">Имя действия в Lua.</param>
        /// <param name="devName">Имя устройства.</param>
        /// <param name="additionalParam">Дополнительный параметр (для сложных действий).</param>
        public bool AddDev(string actionLuaName, string devName, int additionalParam = 0)
        {
            int index = Device.DeviceManager.GetInstance().GetDeviceListNumber(devName);
            foreach (Action act in actions)
            {
                if (act.LuaName == actionLuaName)
                {
                    act.AddDev(index, additionalParam);
                    return true;
                }
            }


#if DEBUG
            MessageBox.Show(
        "Не найдено действие \'" + actionLuaName + "\'");
#endif

            return false;
        }

        /// <summary>
        /// Добавление параметра.
        /// 
        /// Вызывается из Lua-скирпта sys.lua.
        /// </summary>
        /// <param name="actionLuaName">Имя действия в Lua.</param>
        /// <param name="parIdx">Индекс параметра.</param>
        /// <param name="val">Значение параметра.</param>
        public bool AddParam(string actionLuaName, int parIdx, int val)
        {
            foreach (Action act in actions)
            {
                if (act.LuaName == actionLuaName)
                {
                    act.AddParam(parIdx, val);
                    return true;
                }
            }

#if DEBUG
            System.Windows.Forms.MessageBox.Show(
        "Не найдено действие \'" + actionLuaName + "\'");
#endif
            return false;
        }

        public List<Action> GetActions
        {
            get
            {
                return actions;
            }
        }

        public void Synch(int[] array)
        {
            foreach (Action action in actions)
            {
                action.Synch(array);
            }
        }

        public string GetStepName()
        {
            return this.name;
        }

        public int GetStepNumber()
        {
            return getN(this);
        }

        public State Owner
        {
            get
            {
                return owner;
            }
        }

        #region Реализация ITreeViewItem

        override public string[] DisplayText
        {
            get
            {
                if (getN(this) == 0)
                {
                    return new string[] { name, "" };
                }

                return new string[] { getN(this) + ". " + name, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }

        override public bool SetNewValue(string newName)
        {
            name = newName;

            return true;
        }

        override public bool IsEditable
        {
            get
            {
                if (IsMode)
                {
                    return false;
                }

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

        override public bool IsMoveable
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

        override public string[] EditText
        {
            get
            {
                return new string[] { name, "" };
            }
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        override public bool IsDeletable
        {
            get
            {
                if (IsMode)
                {
                    return false;
                }

                return true;
            }
        }

        override public bool Delete(object child)
        {
            Action action = child as Action;

            if (action != null)
            {
                action.Clear();
            }

            return false;
        }

        override public bool IsDrawOnEplanPage
        {
            get { return true; }
        }

        override public List<Editor.DrawInfo> GetObjectToDrawOnEplanPage()
        {
            List<Editor.DrawInfo> devToDraw = new List<Editor.DrawInfo>();
            foreach (Action action in actions)
            {
                devToDraw.AddRange(action.GetObjectToDrawOnEplanPage());
            }

            return devToDraw;
        }

        /// <summary>
        /// Проверка действий в шаге
        /// </summary>
        public void Check()
        {
            List<int> devicesInAction = new List<int>();
            foreach (Action a in actions)
            {
                if (a.GetType().Name == "Action" &&
                    (a.DisplayText[0].Contains("Включать") ||
                    a.DisplayText[0].Contains("Выключать")))
                {
                    devicesInAction.AddRange(a.DeviceIndex);
                }
            }

            List<int> FindEqual = devicesInAction.GroupBy(x => x)
                .SelectMany(y => y.Skip(1)).Distinct().ToList();

            foreach (int i in FindEqual)
            {
                State state = Owner;
                Mode mode = state.Owner;
                ModesManager modesManager = mode.Owner;
                TechObject techObject = modesManager.Owner;
                Device.IODevice device = Device.DeviceManager.GetInstance().GetDeviceByIndex(i);
                string msg = $"Неправильно заданы устройства в шаге " +
                    $"\"{GetStepName()}\", операции \"{mode.Name}\"," +
                    $"технологического объекта \"{techObject.DisplayText[0]}\"";
                EasyEPlanner.ProjectManager.GetInstance().AddLogMessage(msg);
            }
        }
        #endregion

        private bool IsMode ///< Признак шага операции.
        {
            get;
            set;
        }

        private GetN getN;

        private Editor.ObjectProperty nextStepN; ///< Номер следующего шага.
        private Editor.ObjectProperty timeParam; ///< Параметр времени.
        private List<Editor.ITreeViewItem> items;

        private string name;           ///< Имя шага.
        internal List<Action> actions; ///< Список действий шага.
        private State owner;           ///< Владелей элемента
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------   
    /// <summary>
    /// Действие над устройствами (включение, выключение и т.д.).
    /// </summary>

    public class Action : Editor.TreeViewItem
    {

        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться в таблице Lua.</param>
        /// <param name="devTypes">Типы устройств, допустимые для редактирования.</param>
        /// <param name="devSubTypes">Подтипы устройств, допустимые для редактирования.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public Action(string name,
            Step owner,
            string luaName = "",
            Device.DeviceType[] devTypes = null,
            Device.DeviceSubType[] devSubTypes = null)
        {
            this.name = name;
            this.luaName = luaName;
            this.devTypes = devTypes;
            this.devSubTypes = devSubTypes;
            this.deviceIndex = new List<int>();
            this.owner = owner;

            DrawStyle = Editor.DrawInfo.Style.GREEN_BOX;
        }

        public virtual Action Clone()
        {
            Action clone = (Action)MemberwiseClone();

            clone.deviceIndex = new List<int>();
            foreach (int index in deviceIndex)
            {
                clone.deviceIndex.Add(index);
            }

            return clone;
        }

        virtual public void ModifyDevNames(int newTechObjectN, 
            int oldTechObjectN, string techObjectName)
        {
            List<int> tmpIndex = new List<int>();
            foreach (int index in deviceIndex)
            {
                tmpIndex.Add(index);
            }
            
            Device.DeviceManager deviceManager = Device.DeviceManager
                .GetInstance();
            foreach (int index in deviceIndex)
            {
                var newDevName = string.Empty;
                Device.IODevice device = deviceManager.GetDeviceByIndex(index);
                int objNum = device.ObjectNumber;
                string objName = device.ObjectName;

                if (objNum > 0)
                {
                    //Для устройств в пределах объекта меняем номер объекта.
                    if (techObjectName == objName)
                    {
                        // COAG2V1 --> COAG1V1
                        if (objNum == newTechObjectN && oldTechObjectN != -1)
                        {
                            newDevName = objName + oldTechObjectN +
                                device.DeviceType.ToString() + device.
                                DeviceNumber;
                        }
                        if (oldTechObjectN == -1 ||
                            oldTechObjectN == objNum)
                        {
                            //COAG1V1 --> COAG2V1
                            newDevName = objName + newTechObjectN +
                                device.DeviceType.ToString() + device
                                .DeviceNumber;
                        }
                    }
                }

                if (newDevName != string.Empty)
                {
                    int indexOfDeletingElement = tmpIndex.IndexOf(index);
                    tmpIndex.Remove(index);
                    int tmpDevInd = Device.DeviceManager.GetInstance()
                        .GetDeviceListNumber(newDevName);
                    if (tmpDevInd >= 0)
                    {
                        tmpIndex.Insert(indexOfDeletingElement, tmpDevInd);
                    }
                }
            }

            deviceIndex = tmpIndex;
        }

        public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName)
        {
            List<int> tmpIndex = new List<int>();
            foreach (int index in deviceIndex)
            {
                tmpIndex.Add(index);
            }

            Device.DeviceManager deviceManager = Device.DeviceManager
                .GetInstance();
            foreach (int index in deviceIndex)
            {
                string newDevName = string.Empty;
                Device.IODevice device = deviceManager.GetDeviceByIndex(index);
                int objNum = newTechObjectNumber;
                string objName = device.ObjectName;

                if (objName == oldTechObjectName &&
                    device.ObjectNumber == newTechObjectNumber)
                {
                    newDevName = newTechObjectName + objNum +
                        device.DeviceType.ToString() + device.DeviceNumber;
                }

                if (newDevName != string.Empty)
                {
                    int indexOfDeletingElement = tmpIndex.IndexOf(index);
                    tmpIndex.Remove(index);
                    int tmpDevInd = Device.DeviceManager.GetInstance()
                        .GetDeviceListNumber(newDevName);
                    if (tmpDevInd >= 0)
                    {
                        tmpIndex.Insert(indexOfDeletingElement, tmpDevInd);
                    }
                }
            }

            deviceIndex = tmpIndex;
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public virtual string SaveAsLuaTable(string prefix)
        {
            if (deviceIndex.Count == 0) 
            {
                return "";
            }

            string res = prefix;
            if (LuaName != "")
            {
                res += LuaName + " = ";
            }

            res += "--" + name + "\n" + prefix + "\t{\n";

            res += prefix + "\t";
            Device.DeviceManager deviceManager = Device.DeviceManager.
                GetInstance();
            int devicesCounter = 0;
            foreach (int index in deviceIndex)
            {
                if (deviceManager.GetDeviceByIndex(index).Name != "Заглушка")
                {
                    devicesCounter++;
                    res += "'" + deviceManager.GetDeviceByIndex(index).Name + 
                        "', ";
                }
            }

            if (devicesCounter == 0)
            {
                return "";
            }

            res = res.Remove(res.Length - 2, 2);
            res += "\n";

            res += prefix + "\t},\n";
            return res;
        }

        /// <summary>
        /// Добавление устройства к действию.
        /// </summary>
        /// <param name="device">Устройство.</param>
        /// <param name="additionalParam">Дополнительный параметр.</param>
        public virtual void AddDev(int index, int additionalParam)
        {
            deviceIndex.Add(index);
        }

        /// <summary>
        /// Добавление параметра к действию.
        /// </summary>
        /// <param name="index">Индекс параметра.</param>
        /// <param name="val">Значение параметра.</param>
        public virtual void AddParam(int index, int val)
        {
        }

        /// <summary>
        /// Очищение списка устройств.
        /// </summary>
        virtual public void Clear()
        {
            deviceIndex.Clear();
        }

        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение индексов.</param>
        virtual public void Synch(int[] array)
        {
            List<int> del = new List<int>();
            for (int j = 0; j < deviceIndex.Count; j++)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (deviceIndex[j] == i)
                    {
                        if (array[i] < 0)
                        {
                            del.Add(j);
                            break;
                        }
                        if (array[i] > 0)
                        {
                            deviceIndex[j] = array[i];
                            break;
                        }
                    }
                }
            }

            int dx = 0;
            foreach (int index in del)
            {
                deviceIndex.RemoveAt(index - dx++);
            }
        }

        /// <summary>
        /// Получение/установка устройств.
        /// </summary>
        public List<int> DeviceIndex
        {
            get
            {
                return deviceIndex;
            }
            set
            {
                deviceIndex = value;
            }
        }

        public string LuaName
        {
            get
            {
                return luaName;
            }
        }

        /// <summary>
        /// Функция проверки добавляемого устройства
        /// </summary>
        /// <param name="deviceName">Имя устройства</param>
        /// <returns></returns>
        private bool ValidateDevice(string deviceName)
        {
            bool isValidType = false;

            Device.Device device = Device.DeviceManager.GetInstance().
                GetDeviceByEplanName(deviceName);
            Device.DeviceType deviceType = device.DeviceType;
            Device.DeviceSubType deviceSubType = device.DeviceSubType;

            Device.DeviceType[] validTypes;
            Device.DeviceSubType[] validSubTypes;
            GetDevTypes(out validTypes, out validSubTypes);

            if (validTypes == null)
            {
                return true;
            }
            else
            {
                foreach(Device.DeviceType type in validTypes)
                {
                    if (type == deviceType)
                    {
                        isValidType = true;
                        break;
                    }
                    else
                    {
                        isValidType = false;
                    }
                }

                if (validSubTypes != null)
                {
                    bool isValidSubType = false;
                    foreach(Device.DeviceSubType subType in validSubTypes)
                    {
                        if ((subType == deviceSubType) && isValidType)
                        {
                            isValidSubType = true;
                        }
                    }

                    if (isValidSubType && isValidSubType)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return isValidType;
        }

        #region Реализация ITreeViewItem

        override public string[] DisplayText
        {
            get
            {
                Device.DeviceManager deviceManager = Device.DeviceManager.GetInstance();
                string res = "";

                foreach (int index in deviceIndex)
                {
                    if (index >= 0)
                    {
                        res += deviceManager.GetDeviceByIndex(index).Name + " ";
                    }
                }
                if (res != "")
                {
                    res = res.Remove(res.Length - 1);
                }

                return new string[] { name, res };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return null;
            }
        }

        override public bool SetNewValue(string newName)
        {
            newName = newName.Trim();

            if (newName == "")
            {
                Clear();
                return true;
            }

            Match strMatch = Regex.Match(newName,
                Device.DeviceManager.DESCRIPTION_PATTERN_MULTYLINE,
                RegexOptions.IgnoreCase);
            if (!strMatch.Success)
            {
                return false;
            }

            Match match = Regex.Match(newName,
                Device.DeviceManager.DESCRIPTION_PATTERN, RegexOptions.
                IgnoreCase);
            deviceIndex.Clear();
            while (match.Success)
            {
                string str = match.Groups["name"].Value;

                // Если устройство нельзя вставлять сюда - пропускаем его.
                bool isValid = ValidateDevice(str);               
                if (isValid != false)
                {
                    int tmpDeviceIndex = Device.DeviceManager.GetInstance().
                        GetDeviceListNumber(str);
                    if (tmpDeviceIndex >= 0)
                    {
                        deviceIndex.Add(tmpDeviceIndex);
                    }
                }

                match = match.NextMatch();
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
            {//Можем редактировать содержимое второй колонки.
                return new int[] { -1, 1 };
            }
        }

        override public string[] EditText
        {
            get
            {
                Device.DeviceManager deviceManager = Device.DeviceManager.GetInstance();
                string res = "";
                foreach (int index in deviceIndex)
                {
                    res += deviceManager.GetDeviceByIndex(index).Name + " ";
                }

                if (res != "")
                {
                    res = res.Remove(res.Length - 1);
                }

                return new string[] { "", res };
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public bool IsUseDevList
        {
            get
            {
                return true;
            }
        }

        override public void GetDevTypes(out Device.DeviceType[] devTypes,
            out Device.DeviceSubType[] devSubTypes)
        {
            devTypes = this.devTypes;
            devSubTypes = this.devSubTypes;
        }

        override public bool IsDrawOnEplanPage
        {
            get
            {
                return true;
            }
        }

        virtual public Editor.DrawInfo.Style DrawStyle
        {
            get;
            set;
        }

        override public List<Editor.DrawInfo> GetObjectToDrawOnEplanPage()
        {
            Device.DeviceManager deviceManager = Device.DeviceManager.GetInstance();



            List<Editor.DrawInfo> devToDraw = new List<Editor.DrawInfo>();
            foreach (int index in deviceIndex)
            {
                devToDraw.Add(new Editor.DrawInfo(
                    DrawStyle, deviceManager.GetDeviceByIndex(index)));
            }

            return devToDraw;
        }

        public virtual Device.DeviceSubType[] GetDevSubTypes()
        {
            return devSubTypes;
        }
        #endregion


        public string stepName
        {
            get
            {
                return name;
            }
        }

        // private string devicesName;
        protected string luaName;               /// Имя действия в таблице Lua.
        protected string name;                  /// Имя действия.
        protected List<int> deviceIndex;  /// Список устройств.
                                          ///
        protected Device.DeviceType[] devTypes;
        protected Device.DeviceSubType[] devSubTypes;

        protected Step owner; // Владелец элемента
    }

    public class Action_WashSeats : Action
    {
        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться в таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public Action_WashSeats(string name, Step owner, string luaName)
            : base(name, owner, luaName)
        {
            subAction_WashGroupSeats = new List<Action>();
            subAction_WashGroupSeats.Add(
                new Action("Группа", owner, "",
                new Device.DeviceType[1] { Device.DeviceType.V },
                new Device.DeviceSubType[3] {
                Device.DeviceSubType.V_MIXPROOF,
                Device.DeviceSubType.V_AS_MIXPROOF,
                Device.DeviceSubType.V_IOLINK_MIXPROOF }));
        }

        public override Action Clone()
        {
            Action_WashSeats clone = (Action_WashSeats)base.Clone();

            clone.subAction_WashGroupSeats = new List<Action>();

            foreach (Action action in subAction_WashGroupSeats)
            {
                clone.subAction_WashGroupSeats.Add(action.Clone());
            }

            return clone;
        }

        override public void ModifyDevNames(int newTechObjectN, int oldTechObjectN,
            string techObjectName)
        {
            foreach (Action subAction in subAction_WashGroupSeats)
            {
                subAction.ModifyDevNames(newTechObjectN, oldTechObjectN, techObjectName);
            }
        }

        /// <summary>
        /// Добавление устройства к действию.
        /// </summary>
        /// <param name="device">Устройство.</param>
        /// <param name="groupNumber">Дополнительный параметр.</param>
        public override void AddDev(int index, int groupNumber)
        {
            while (subAction_WashGroupSeats.Count <= groupNumber)
            {
                subAction_WashGroupSeats.Add(
                    new Action("Группа", owner, "",
                    new Device.DeviceType[1] { Device.DeviceType.V },
                    new Device.DeviceSubType[3] { 
                        Device.DeviceSubType.V_MIXPROOF,
                        Device.DeviceSubType.V_AS_MIXPROOF,
                        Device.DeviceSubType.V_IOLINK_MIXPROOF }));

                subAction_WashGroupSeats[
                    subAction_WashGroupSeats.Count - 1].DrawStyle = DrawStyle;
            }

            subAction_WashGroupSeats[groupNumber].AddDev(index, 0);

            deviceIndex.Add(index);
        }

        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение индексов.</param>
        override public void Synch(int[] array)
        {
            base.Synch(array);
            foreach (Action subAction in subAction_WashGroupSeats)
            {
                subAction.Synch(array);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        override public string SaveAsLuaTable(string prefix)
        {
            if (subAction_WashGroupSeats.Count == 0) return "";

            string res = "";

            foreach (Action group in subAction_WashGroupSeats)
            {
                res += group.SaveAsLuaTable(prefix + "\t");
            }

            if (res != "")
            {
                res = prefix + luaName + " = --" + name + "\n" +
                    prefix + "\t{\n" +
                    res +
                    prefix + "\t},\n";
            }

            return res;
        }

        #region Реализация ITreeViewItem

        override public string[] DisplayText
        {
            get
            {
                Device.DeviceManager deviceManager = Device.DeviceManager.GetInstance();
                string res = "";

                foreach (Action group in subAction_WashGroupSeats)
                {
                    res += "{";

                    foreach (int index in group.DeviceIndex)
                    {
                        res += deviceManager.GetDeviceByIndex(index).Name + " ";
                    }

                    if (group.DeviceIndex.Count > 0)
                    {
                        res = res.Remove(res.Length - 1);
                    }

                    res += "} ";
                }

                return new string[] { name, res };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return subAction_WashGroupSeats.ToArray();
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public bool Delete(object child)
        {
            Action subAction = child as Action;

            if (subAction != null)
            {
                subAction_WashGroupSeats.Remove(subAction);
                return true;
            }

            return false;
        }

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem Insert()
        {
            subAction_WashGroupSeats.Add(new Action("Группа", owner));
            subAction_WashGroupSeats[
                    subAction_WashGroupSeats.Count - 1].DrawStyle = DrawStyle;

            return subAction_WashGroupSeats[subAction_WashGroupSeats.Count - 1];
        }

        override public void Clear()
        {
            foreach (Action subAction in subAction_WashGroupSeats)
            {
                subAction.Clear();
            }
        }

        override public bool IsUseDevList
        {
            get
            {
                return false;
            }
        }

        override public Editor.DrawInfo.Style DrawStyle
        {
            get
            {
                return base.DrawStyle;
            }
            set
            {
                base.DrawStyle = value;
                if (subAction_WashGroupSeats != null)
                {
                    subAction_WashGroupSeats[0].DrawStyle = DrawStyle;
                }
            }
        }

        #endregion

        private List<Action> subAction_WashGroupSeats;     ///
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Специальное действие - выдача дискретных сигналов при наличии входного 
    /// дискретного сигнала.
    /// </summary>
    public class Action_DI_DO : Action
    {

        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться в таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public Action_DI_DO(string name, Step owner, string luaName)
            : base(name, owner, luaName)
        {
            subAction_DI_DO_Group = new List<Action>();
            subAction_DI_DO_Group.Add(new Action("Группа", owner, "",
                new Device.DeviceType[3] {
                Device.DeviceType.DI, Device.DeviceType.SB, Device.DeviceType.DO }));
        }

        override public Action Clone()
        {
            Action_DI_DO clone = (Action_DI_DO)base.Clone();

            clone.subAction_DI_DO_Group = new List<Action>();
            foreach (Action action in subAction_DI_DO_Group)
            {
                clone.subAction_DI_DO_Group.Add(action.Clone());
            }

            return clone;
        }

        override public void ModifyDevNames(int newTechObjectN, int oldTechObjectN,
            string techObjectName)
        {
            foreach (Action subAction in subAction_DI_DO_Group)
            {
                subAction.ModifyDevNames(newTechObjectN, oldTechObjectN, techObjectName);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        override public string SaveAsLuaTable(string prefix)
        {
            if (subAction_DI_DO_Group.Count == 0) return "";

            string res = "";

            foreach (Action group in subAction_DI_DO_Group)
            {
                res += group.SaveAsLuaTable(prefix + "\t");
            }

            if (res != "")
            {
                res = prefix + luaName + " = --" + name + "\n" +
                    prefix + "\t{\n" +
                    res +
                    prefix + "\t},\n";
            }

            return res;
        }

        /// <summary>
        /// Добавление устройства к действию.
        /// </summary>
        /// <param name="device">Устройство.</param>
        /// <param name="groupNumber">Дополнительный параметр.</param>
        public override void AddDev(int index, int groupNumber)
        {
            while (subAction_DI_DO_Group.Count <= groupNumber)
            {
                subAction_DI_DO_Group.Add(new Action("Группа", owner));
            }

            subAction_DI_DO_Group[groupNumber].AddDev(index, 0);

            deviceIndex.Add(index);
        }

        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение индексов.</param>
        override public void Synch(int[] array)
        {
            base.Synch(array);
            foreach (Action subAction in subAction_DI_DO_Group)
            {
                subAction.Synch(array);
            }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                Device.DeviceManager deviceManager = Device.DeviceManager.GetInstance();
                string res = "";

                foreach (Action group in subAction_DI_DO_Group)
                {
                    res += "{";

                    foreach (int index in group.DeviceIndex)
                    {
                        res += deviceManager.GetDeviceByIndex(index).Name + " ";
                    }

                    if (group.DeviceIndex.Count > 0)
                    {
                        res = res.Remove(res.Length - 1);
                    }

                    res += "} ";
                }

                return new string[] { name, res };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return subAction_DI_DO_Group.ToArray();
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public bool Delete(object child)
        {
            Action subAction = child as Action;

            if (subAction != null)
            {
                subAction_DI_DO_Group.Remove(subAction);
                return true;
            }

            return false;
        }

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem Insert()
        {
            subAction_DI_DO_Group.Add(new Action("Группа", owner));
            return subAction_DI_DO_Group[subAction_DI_DO_Group.Count - 1];
        }

        override public void Clear()
        {
            foreach (Action subAction in subAction_DI_DO_Group)
            {
                subAction.Clear();
            }
        }

        override public bool IsUseDevList
        {
            get
            {
                return false;
            }
        }
        #endregion

        private List<Action> subAction_DI_DO_Group;     ///
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Специальное действие - выдача аналоговых сигналов при наличии входного 
    /// аналогового сигнала.
    /// </summary>
    public class Action_AI_AO : Action
    {

        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться в таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public Action_AI_AO(string name, Step owner, string luaName)
            : base(name, owner, luaName)
        {
            subAction_AI_AO_Group = new List<Action>();
            subAction_AI_AO_Group.Add(new Action("Группа", owner, "",
                new Device.DeviceType[3] {
                Device.DeviceType.AI, Device.DeviceType.AO, Device.DeviceType.M }));
        }

        override public Action Clone()
        {
            Action_AI_AO clone = (Action_AI_AO)base.Clone();

            clone.subAction_AI_AO_Group = new List<Action>();
            foreach (Action action in subAction_AI_AO_Group)
            {
                clone.subAction_AI_AO_Group.Add(action.Clone());
            }

            return clone;
        }

        override public void ModifyDevNames(int newTechObjectN, int oldTechObjectN,
            string techObjectName)
        {
            foreach (Action subAction in subAction_AI_AO_Group)
            {
                subAction.ModifyDevNames(newTechObjectN, oldTechObjectN, techObjectName);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        override public string SaveAsLuaTable(string prefix)
        {
            if (subAction_AI_AO_Group.Count == 0) return "";

            string res = "";

            foreach (Action group in subAction_AI_AO_Group)
            {
                res += group.SaveAsLuaTable(prefix + "\t");
            }

            if (res != "")
            {
                res = prefix + luaName + " = --" + name + "\n" +
                    prefix + "\t{\n" +
                    res +
                    prefix + "\t},\n";
            }

            return res;
        }

        /// <summary>
        /// Добавление устройства к действию.
        /// </summary>
        /// <param name="device">Устройство.</param>
        /// <param name="groupNumber">Дополнительный параметр.</param>
        public override void AddDev(int index, int groupNumber)
        {
            while (subAction_AI_AO_Group.Count <= groupNumber)
            {
                subAction_AI_AO_Group.Add(new Action("Группа", owner));
            }

            subAction_AI_AO_Group[groupNumber].AddDev(index, 0);

            deviceIndex.Add(index);
        }

        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение индексов.</param>
        override public void Synch(int[] array)
        {
            base.Synch(array);
            foreach (Action subAction in subAction_AI_AO_Group)
            {
                subAction.Synch(array);
            }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                Device.DeviceManager deviceManager = Device.DeviceManager.GetInstance();
                string res = "";

                foreach (Action group in subAction_AI_AO_Group)
                {
                    res += "{";

                    foreach (int index in group.DeviceIndex)
                    {
                        res += deviceManager.GetDeviceByIndex(index).Name + " ";
                    }

                    if (group.DeviceIndex.Count > 0)
                    {
                        res = res.Remove(res.Length - 1);
                    }

                    res += "} ";
                }

                return new string[] { name, res };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return subAction_AI_AO_Group.ToArray();
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public bool Delete(object child)
        {
            Action subAction = child as Action;

            if (subAction != null)
            {
                subAction_AI_AO_Group.Remove(subAction);
                return true;
            }

            return false;
        }

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem Insert()
        {
            subAction_AI_AO_Group.Add(new Action("Группа", owner));
            return subAction_AI_AO_Group[subAction_AI_AO_Group.Count - 1];
        }

        override public void Clear()
        {
            foreach (Action subAction in subAction_AI_AO_Group)
            {
                subAction.Clear();
            }
        }

        override public bool IsUseDevList
        {
            get
            {
                return false;
            }
        }
        #endregion

        private List<Action> subAction_AI_AO_Group;     ///
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Специальное действие - обработка сигналов во время мойки.
    /// </summary>
    public class ActionWash : Action
    {

        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться в таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public ActionWash(string name, Step owner, string luaName)
            : base(name, owner, luaName)
        {
            vGroups = new List<Action>();
            vGroups.Add(new Action("DI", owner, "DI",
                new Device.DeviceType[1] { Device.DeviceType.DI }));
            vGroups.Add(new Action("DO", owner, "DO",
                new Device.DeviceType[1] { Device.DeviceType.DO }));

            vGroups.Add(new Action("Устройства", owner, "devices",
                new Device.DeviceType[3] { Device.DeviceType.M,
                Device.DeviceType.V, Device.DeviceType.DO }));

            vGroups.Add(new Action("Реверсные устройства", owner, "rev_devices",
                new Device.DeviceType[] { Device.DeviceType.M,
                Device.DeviceType.V }));

            items = new List<Editor.ITreeViewItem>();
            foreach (Action action in vGroups)
            {
                items.Add(action);
            }

            pumpFreq = new Editor.ObjectProperty("Частота насосов (параметр)",
                -1);
            items.Add(pumpFreq);
        }

        override public Action Clone()
        {
            ActionWash clone = (ActionWash)base.Clone();

            clone.vGroups = new List<Action>();
            foreach (Action action in vGroups)
            {
                clone.vGroups.Add(action.Clone());
            }

            clone.items.Clear();
            clone.items = new List<Editor.ITreeViewItem>();
            foreach (Action action in clone.vGroups)
            {
                clone.items.Add(action);
            }

            clone.pumpFreq = pumpFreq.Clone();
            clone.items.Add(clone.pumpFreq);
            return clone;
        }

        override public void ModifyDevNames(int newTechObjectN, int oldTechObjectN,
            string techObjectName)
        {
            foreach (Action subAction in vGroups)
            {
                subAction.ModifyDevNames(newTechObjectN, oldTechObjectN, techObjectName);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public override string SaveAsLuaTable(string prefix)
        {
            if (vGroups.Count == 0) return "";

            string res = "";

            foreach (Action group in vGroups)
            {
                string tmp = group.SaveAsLuaTable(prefix + "\t");
                if (tmp != "")
                {
                    res += tmp;
                }
            }

            string pumpFreqVal = pumpFreq.EditText[1].Trim();

            if (res != "")
            {
                res = prefix + luaName + " = --" + name + "\n" +
                    prefix + "\t{\n" +
                    res +
                    (pumpFreqVal == "-1" ? "" : prefix + "\tpump_freq = " + pumpFreqVal + ",\n") +
                    prefix + "\t},\n";
            }

            return res;
        }

        /// <summary>
        /// Добавление устройства к действию.
        /// </summary>
        /// <param name="device">Устройство.</param>
        /// <param name="additionalParam">Дополнительный параметр.</param>
        public override void AddDev(int index, int additionalParam)
        {
            if (additionalParam < vGroups.Count /*Количество групп*/ )
            {
                (vGroups[additionalParam] as Action).AddDev(index, 0);
            }

            deviceIndex.Add(index);
        }

        /// <summary>
        /// Добавление параметра к действию.
        /// </summary>
        /// <param name="index">Индекс параметра.</param>
        /// <param name="val">Значение параметра.</param>
        public override void AddParam(int index, int val)
        {
            pumpFreq.SetValue(val);
        }

        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение индексов.</param>
        override public void Synch(int[] array)
        {
            base.Synch(array);
            foreach (Action subAction in vGroups)
            {
                subAction.Synch(array);
            }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                Device.DeviceManager deviceManager = Device.DeviceManager.GetInstance();
                string res = "";

                foreach (Action group in vGroups)
                {
                    res += "{";

                    foreach (int index in group.DeviceIndex)
                    {
                        res += deviceManager.GetDeviceByIndex(index).Name + " ";
                    }

                    if (group.DeviceIndex.Count > 0)
                    {
                        res = res.Remove(res.Length - 1);
                    }

                    res += "} ";
                }

                res += "{" + pumpFreq.DisplayText[1] + "}";

                return new string[] { name, res };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }

        override public void Clear()
        {
            foreach (Action subAction in vGroups)
            {
                subAction.Clear();
            }
        }

        override public bool IsEditable
        {
            get
            {
                return false;
            }
        }

        override public bool IsUseDevList
        {
            get
            {
                return false;
            }
        }
        #endregion

        List<Action> vGroups;
        private Editor.ObjectProperty pumpFreq; ///< Частота насоса, параметр.

        List<Editor.ITreeViewItem> items;
    }



    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Интерфейс менеджера технологических объектов проекта.
    /// </summary>
    public interface ITechObjectManager
    {
        void LoadFromLuaStr(string LuaStr, string projectName);
        void LoadRestriction(string LuaStr);
        string SaveAsLuaTable(string prefixStr);
        string SavePrgAsLuaTable(string prefixStr);
        void GetObjectForXML(TreeNode rootNode);
        void SetCDBXTagView(bool combineTag);
        string SaveRestrictionAsLua(string prefixStr);
        List<TechObject> GetTechObjects();      
        void CheckConfiguration();
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Менеджер технологических объектов проекта.
    /// </summary>
    public class TechObjectManager : Editor.TreeViewItem, ITechObjectManager
    {

        private TechObjectManager()
        {
            lua = new Lua();

            lua.RegisterFunction("ADD_TECH_OBJECT", this,
                GetType().GetMethod("AddObject"));

            //Для отладки Lua скриптов.
            LuaFunction resF = lua.RegisterFunction("PRINT", this,
                GetType().GetMethod("ShowMessage")); 

            string systemFilesPath = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location) +
                "\\Lua";

            string sysLuaPath = Path.Combine(systemFilesPath, "sys.lua");
            if (Directory.Exists(systemFilesPath) == true)
            {
                lua.DoFile(sysLuaPath);
            }
            else
            {
                CopySystemFiles(systemFilesPath);
                lua.DoFile(sysLuaPath);
            }

            objects = new List<TechObject>();

            cdbxTagView = false;
        }

        /// <summary>
        /// Копирует системные .lua файлы если они не загрузились
        /// в теневое хранилище (Win 7 fix).
        /// </summary>
        private void CopySystemFiles(string shadowAssemblySystemFilesDir)
        {
            const string luaDirectory = "\\Lua";
            Directory.CreateDirectory(shadowAssemblySystemFilesDir);

            string systemFilesPath = Path.GetDirectoryName(EasyEPlanner.
                AddInModule.OriginalAssemblyPath) + luaDirectory;

            DirectoryInfo systemFilesDirectory = new DirectoryInfo(
                systemFilesPath);
            FileInfo[] systemFiles = systemFilesDirectory.GetFiles();
            foreach (FileInfo systemFile in systemFiles)
            {
                string pathToFile = Path.Combine(shadowAssemblySystemFilesDir,
                    systemFile.Name);
                systemFile.CopyTo(pathToFile, true);
            }
        }

        public void ShowMessage(string msg)
        {
            MessageBox.Show(msg);
        }

        /// <summary>
        /// Получение номера операции в списке операций. Нумерация начинается с 1.
        /// </summary>
        /// <param name="mode">Операция, номер которой хотим получить.</param>
        /// <returns>Номер заданной операции.</returns>
        public int GetTechObjectN(object techObject)
        {
            return objects.IndexOf(techObject as TechObject) + 1;
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + "init_tech_objects_modes = function()\n" +
                prefix + "\treturn\n" +
                prefix + "\t{\n";

            foreach (TechObject obj in objects)
            {
                res += obj.SaveAsLuaTable(prefix + "\t\t");
            }

            res += prefix + "\t}\n" +
                prefix + "end\n";

            res = res.Replace("\t", "    ");
            return res;
        }

        // Сохранение файла prg.lua
        public string SavePrgAsLuaTable(string prefix)
        {
            // Словарь привязанных агрегатов к аппарату
            var attachedObjects = new Dictionary<int, string>();

            var res = "";
            res += "local prg =\n\t{\n";
            res += SaveVariablesAsLuaTable(prefix, out attachedObjects);
            res += "\t}\n";

            if (attachedObjects.Count > 0)
            {
                res += SaveBindingAsLuaTable(attachedObjects);
            }

            res += SaveParamsAsLuaTable(prefix);
            res += SaveFunctionalityAsLuaTable();
            res += "\nreturn prg";
            res.Replace("\t", "    ");

            return res;
        }

        // Сохранение переменных prg.lua
        private string SaveVariablesAsLuaTable(string prefix, out Dictionary<int, string> attachedObjectsDict)
        {
            // Флаг на поиск привязанных агрегатов в аппарате
            const int objS88Level = 1;

            // Словарь с агрегатами, привязанных к аппарату, обращение по номеру аппарата
            var attachedObjects = new Dictionary<int, string>();

            var res = "";
            var previouslyObjectName = ""; // Имя предыдущего объекта

            for (int i = 0; i < objects.Count; i++)
            {
                if (previouslyObjectName != objects[i].NameEplanForFile.ToLower() && previouslyObjectName != "")
                {
                    res += "\n";
                }

                res += prefix + objects[i].NameEplanForFile.ToLower() + objects[i].TechNumber + " = OBJECT" + GetTechObjectN(objects[i]) + ",\n";

                // Если есть привязка, помечаю, какие агрегаты к какому аппарату привязаны
                if (objects[i].S88Level == objS88Level & objects[i].AttachedObjects != string.Empty)
                {
                    // Т.к объекты начинаются с 1
                    attachedObjects[i + 1] = objects[i].AttachedObjects;
                }

                // Записал, обозначил, что этот объект уже записан
                previouslyObjectName = objects[i].NameEplanForFile.ToLower();
            }

            attachedObjectsDict = attachedObjects;

            return res;
        }

        // Сохранение привязок prg.lua
        private string SaveBindingAsLuaTable(Dictionary<int, string> attachedObjects)
        {
            var res = "";
            res += "\n"; // Пустая строка для разделения

            string previouslyObjectName = ""; // Предыдущий объект
            bool isInt = false; // Проверка на число
            foreach (var val in attachedObjects)
            {
                var techObj = GetTObject(val.Key);
                var attachedObjs = val.Value.Split(' ');
                foreach (string value in attachedObjs)
                {
                    isInt = int.TryParse(value, out _);
                    if (isInt)
                    {
                        var attachedTechObject = GetTObject(Convert.ToInt32(value));
                        var attachedTechObjectType = attachedTechObject.NameEplanForFile.ToLower();
                        var attachedTechObjNameForFile = attachedTechObjectType + attachedTechObject.TechNumber;
                        var techObjNameForFile = "prg." + techObj.NameEplanForFile.ToLower() + techObj.TechNumber;

                        if (previouslyObjectName != techObj.NameEplanForFile.ToLower() && previouslyObjectName != "")
                        {
                            res += "\n"; // Отступ, если изменен тип объекта
                        }

                        switch (attachedTechObjectType)
                        {
                            case "mixer_node_tank":
                                res += techObjNameForFile + ".mixer_node = " + attachedTechObjNameForFile + "\n";
                                break;

                            case "cooler_node_tank":
                                res += techObjNameForFile + ".cooler_node = " + attachedTechObjNameForFile + "\n";
                                break;

                            case "heater_node_tank":
                                res += techObjNameForFile + ".heater_node = " + attachedTechObjNameForFile + "\n";
                                break;
                        }

                        previouslyObjectName = techObj.NameEplanForFile.ToLower();
                    } 
                    else
                    {
                        string msg = $"В объекте \"{techObj.EditText[0]} {techObj.TechNumber}\" ошибка заполнения поля \"Привязанные устройства\"\n";
                        EasyEPlanner.ProjectManager.GetInstance().AddLogMessage(msg);
                    }
                }
            }
            res += "\n";
            return res;
        }

        // Сохранение информации о базовой операции, сигналах и шагах в prg.lua
        private string SaveParamsAsLuaTable(string prefix)
        {
            var res = "";
            foreach (TechObject obj in objects)
            {
                var modesManager = obj.GetModesManager;
                var modes = modesManager.GetModes;
                foreach (Mode mode in modes)
                {
                    var baseOperation = mode.GetBaseOperation();
                    switch (baseOperation.GetName())
                    {
                        case "Мойка":
                            var objName = "prg." + obj.NameEplan.ToLower() + obj.TechNumber.ToString();

                            res += objName + ".operations = \t\t--Операции.\n";
                            res += prefix + "{\n";
                            res += prefix + baseOperation.GetLuaName().ToUpper() + " = " + mode.GetModeNumber() + ",\t\t--Мойка CIP.\n";
                            res += prefix + "}\n";

                            res += objName + ".steps = \t\t--Шаги операций.\n";
                            res += prefix + "{\n";
                            res += prefix + baseOperation.GetLuaName().ToUpper() + " = \n";
                            res += prefix + prefix + "{\n";
                            res += prefix + prefix + "DRAINAGE = " + mode.stepsMngr[0].steps.First(x => x.GetStepName().Contains("Дренаж")).GetStepNumber() + ",\n";
                            res += prefix + prefix + "}\n";
                            res += prefix + "}\n";

                            foreach (BaseOperationProperty param in baseOperation.BaseOperationProperties)
                            {
                                res += objName + "." + param.GetLuaName() + " = " + param.GetValue() + "\n";
                            }

                            res += "\n"; // Отступ перед новым объектом
                            break;
                    }
                }
            }
            return res;
        }

        // Сохранение базовой функциональности объектов
        public string SaveFunctionalityAsLuaTable()
        {
            var previouslyObjectName = "";
            var res = "";

            foreach (TechObject obj in objects)
            {
                if (previouslyObjectName != obj.NameEplanForFile.ToLower() && previouslyObjectName != "")
                {
                    res += "\n"; // Отступ, если изменен тип объекта
                }
                var basicObj = DBImitation.GetNameEplan(obj.DisplayText[1]).ToLower().Split('_');
                var objName = obj.NameEplanForFile.ToLower() + obj.TechNumber;
                res += "add_functionality(" + objName + ", " + "basic_" + basicObj[0] + ")\n";

                previouslyObjectName = obj.NameEplanForFile.ToLower();
            }
            return res;
        }

        /// <summary>
        /// Сохранение ограничений в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveRestrictionAsLua(string prefix)
        {
            string res = prefix + "restrictions =\n" + prefix + "\t{";

            foreach (TechObject obj in objects)
            {
                res += obj.SaveRestrictionAsLua(prefix + "\t");
            }

            res += "\n" + prefix + "\t}\n";


            res = res.Replace("\t", "    ");
            return res;
        }

        /// <summary>
        /// Проверка технологического объекта
        /// на правильность ввода и др.
        /// </summary>
        public virtual void CheckConfiguration()
        {
            List<TechObject> TObjects = GetTechObjects();

            foreach (TechObject obj in TObjects)
            {
                if (obj.DisplayText[1].Length == 0)
                {
                    string objName = obj.EditText[0] + " " + obj.TechNumber;
                    string msg = string.Format("Не выбран базовый объект - \"{0}\"\n", objName);
                    EasyEPlanner.ProjectManager.GetInstance().AddLogMessage(msg);
                }

                // Проверка операций объекта
                obj.Check();
            }

        }

        /// <summary>
        /// Добавление технологического объекта. Вызывается из Lua.
        /// </summary>
        /// <returns>Добавленный технологический объект.</returns>
        public TechObject AddObject(int techN, string name, int techType,
            string nameEplan, int cooperParamNumber, string NameBC, string baseTechObjectName,
            string attachedObjects)
        {
            TechObject obj = new TechObject(name, GetTechObjectN, techN,
                techType, nameEplan, cooperParamNumber, NameBC, attachedObjects);

            // Установка значения базового аппарата
            obj.SetNewValue(baseTechObjectName, true);

            objects.Add(obj);

            return obj;
        }

        /// <summary>
        /// Получение объекта по номеру
        /// </summary>
        /// <param name="i">индекс </param>
        /// <returns></returns>
        public TechObject GetTObject(int i)
        {
            if (objects != null)
            {
                if (objects.Count >= i)
                {
                    return objects[i - 1];
                }
            }
            return null;
        }

        /// <summary>
        /// Получение списка объектов
        /// </summary>
        /// <returns></returns>
        public List<TechObject> GetTechObjects()
        {
            return Objects;
        }

        /// <summary>
        /// Формирование узлов для операций, шагов и параметров объектов.
        /// </summary>
        /// <param name="rootNode">корневой узел</param>
        public void GetObjectForXML(TreeNode rootNode)
        {
            TreeNode systemNode = new TreeNode("SYSTEM");
            systemNode.Nodes.Add("SYSTEM.UP_TIME", "SYSTEM.UP_TIME");
            systemNode.Nodes.Add("SYSTEM.WASH_VALVE_SEAT_PERIOD", "SYSTEM.WASH_VALVE_SEAT_PERIOD");
            systemNode.Nodes.Add("SYSTEM.P_V_OFF_DELAY_TIME", "SYSTEM.P_V_OFF_DELAY_TIME");
            systemNode.Nodes.Add("SYSTEM.WASH_VALVE_UPPER_SEAT_TIME", "SYSTEM.WASH_VALVE_UPPER_SEAT_TIME");
            systemNode.Nodes.Add("SYSTEM.WASH_VALVE_LOWER_SEAT_TIME", "SYSTEM.WASH_VALVE_LOWER_SEAT_TIME");
            systemNode.Nodes.Add("SYSTEM.CMD", "SYSTEM.CMD");
            systemNode.Nodes.Add("SYSTEM.CMD_ANSWER", "SYSTEM.CMD_ANSWER");
            systemNode.Nodes.Add("SYSTEM.P_RESTRICTIONS_MODE", "SYSTEM.P_RESTRICTIONS_MODE");
            systemNode.Nodes.Add("SYSTEM.P_RESTRICTIONS_MANUAL_TIME", "SYSTEM.P_RESTRICTIONS_MANUAL_TIME");
            systemNode.Nodes.Add("SYSTEM.P_AUTO_PAUSE_OPER_ON_DEV_ERR", "SYSTEM.P_AUTO_PAUSE_OPER_ON_DEV_ERR");

            rootNode.Nodes.AddRange(new TreeNode[] { systemNode });

            for (int num = 1; num <= Objects.Count; num++)
            {
                TechObject item = Objects[num - 1];


                TreeNode objNode = new TreeNode(item.NameBC + item.TechNumber.ToString());

                TreeNode objModesNode = new TreeNode(item.NameBC + item.TechNumber.ToString() + "_Операции");
                TreeNode objOperStateNode = new TreeNode(item.NameBC + item.TechNumber.ToString() + "_Состояния_Операций");
                TreeNode objAvOperNode = new TreeNode(item.NameBC + item.TechNumber.ToString() + "_Доступность");
                TreeNode objStepsNode = new TreeNode(item.NameBC + item.TechNumber.ToString() + "_Шаги");
                TreeNode objParamsNode = new TreeNode(item.NameBC + item.TechNumber.ToString() + "_Параметры");

                string obj = "OBJECT" + num.ToString();
                string mode = obj + ".MODES";
                string oper = obj + ".OPERATIONS";
                string av = obj + ".AVAILABILITY";
                string step = mode + "_STEPS";
                if (cdbxTagView == true)
                {
                    objNode.Nodes.Add(obj + ".CMD", obj + ".CMD");
                }
                else
                {
                    objModesNode.Nodes.Add(obj + ".CMD", obj + ".CMD");
                }


                int stCount = item.GetModesManager.GetModes.Count / 33;
                for (int i = 0; i <= stCount; i++)
                {
                    string number = "[ " + (i + 1).ToString() + " ]";

                    if (cdbxTagView == true)
                    {
                        objNode.Nodes.Add("OBJECT" + num.ToString() + ".ST" + number, "OBJECT" + num.ToString() + ".ST" + number);
                    }
                    else
                    {
                        objModesNode.Nodes.Add("OBJECT" + num.ToString() + ".ST" + number, "OBJECT" + num.ToString() + ".ST" + number);
                    }
                }

                for (int i = 1; i <= item.GetModesManager.GetModes.Count; i++)
                {
                    string number = "[ " + i.ToString() + " ]";
                    if (cdbxTagView == true)
                    {
                        objNode.Nodes.Add(mode + number, mode + number);
                        objNode.Nodes.Add(oper + number, oper + number);
                        objNode.Nodes.Add(av + number, av + number);
                        objNode.Nodes.Add(step + number, step + number);
                    }
                    else
                    {
                        objModesNode.Nodes.Add(mode + number, mode + number);
                        objOperStateNode.Nodes.Add(oper + number, oper + number);
                        objAvOperNode.Nodes.Add(av + number, av + number);
                        objStepsNode.Nodes.Add(step + number, step + number);
                    }


                }

                string sFl = obj + ".S_PAR_F";
                string sUi = obj + ".S_PAR_UI";
                string rtFl = obj + ".RT_PAR_F";
                string rtUi = obj + ".RT_PAR_UI";
                int count = item.GetParamsManager().Items[0].Items.Length;

                for (int i = 1; i <= count; i++)
                {
                    string number = "[ " + i.ToString() + " ]";

                    if (cdbxTagView == true)
                    {
                        objNode.Nodes.Add(sFl + number, sFl + number);
                    }
                    else
                    {
                        objParamsNode.Nodes.Add(sFl + number, sFl + number);
                    }
                }

                count = item.GetParamsManager().Items[1].Items.Length;
                for (int i = 1; i <= count; i++)
                {
                    string number = "[ " + i.ToString() + " ]";

                    if (cdbxTagView == true)
                    {
                        objNode.Nodes.Add(sUi + number, sUi + number);
                    }
                    else
                    {
                        objParamsNode.Nodes.Add(sUi + number, sUi + number);
                    }
                }

                count = item.GetParamsManager().Items[2].Items.Length;
                for (int i = 1; i <= count; i++)
                {
                    string number = "[ " + i.ToString() + " ]";

                    if (cdbxTagView == true)
                    {
                        objNode.Nodes.Add(rtFl + number, rtFl + number);
                    }
                    else
                    {
                        objParamsNode.Nodes.Add(rtFl + number, rtFl + number);
                    }
                }

                count = item.GetParamsManager().Items[3].Items.Length;
                for (int i = 1; i <= count; i++)
                {
                    string number = "[ " + i.ToString() + " ]";

                    if (cdbxTagView == true)
                    {
                        objNode.Nodes.Add(rtUi + number, rtUi + number);
                    }
                    else
                    {
                        objParamsNode.Nodes.Add(rtUi + number, rtUi + number);
                    }
                }
                if (cdbxTagView == true)
                {
                    rootNode.Nodes.AddRange(new TreeNode[] { objNode });
                }
                else
                {
                    rootNode.Nodes.AddRange(new TreeNode[] { objModesNode, objOperStateNode, objAvOperNode, objStepsNode, objParamsNode });
                }
            }
        }

        /// <summary>
        /// Получение экземпляра класса.
        /// </summary>
        /// <returns>Единственный экземпляр класса.</returns>
        public static TechObjectManager GetInstance()
        {
            if (null == instance)
            {
                instance = new TechObjectManager();
            }

            return instance;
        }

        public List<TechObject> Objects
        {
            get
            {
                return objects;
            }
        }

        public void LoadFromLuaStr(string LuaStr, string projectName)
        {
            this.projectName = projectName;

            objects.Clear(); //Очищение объектов.

            lua.DoString("init_tech_objects_modes = nil"); //Сброс описания объектов.
            try
            {
                //Выполнения Lua скрипта с описанием объектов.
                lua.DoString(LuaStr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ". Исправьте скрипт вручную.",
                    "Ошибка обработки Lua-скрипта");
            }
            try
            {
                //Создание объектов C# из скрипта Lua.
                object[] res = lua.DoString("if init ~= nil then init() end");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка обработки Lua-скрипта: " +
                    ex.Message + ".\n" +
                    "Source: " + ex.Source);
            }

        }

        public void LoadRestriction(string LuaStr)
        {

            lua.RegisterFunction("Get_TECH_OBJECT", this,
                GetType().GetMethod("GetTObject"));

            lua.DoFile(System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location) +
                "\\Lua\\sys_restriction.lua");

            //Выполнения Lua скрипта с описанием объектов.
            lua.DoString(LuaStr);
            lua.DoString("init_restriction()");
        }

        public List<TechObject> GetTechObj
        {
            get
            {
                return objects;
            }
        }

        public void Synch(int[] array)
        {
            foreach (TechObject obj in objects)
            {
                obj.Synch(array);
            }
        }

        public TreeView SaveDevicesAsTree()
        {
            TreeView tree = new TreeView();
            foreach (TechObject techObj in objects)
            {
                string techName = GetTechObjectN(techObj).ToString() + ". " +
                    techObj.EditText[0] + " " + techObj.TechNumber.ToString();
                TreeNode objNode = new TreeNode(techName);
                objNode.Tag = techObj;
                foreach (Mode mode in techObj.GetModesManager.GetModes)
                {
                    string modeName = techObj.GetModesManager.GetModeN(mode).ToString() + ". " + mode.EditText[0];
                    TreeNode modeNode = new TreeNode();
                    Step commonStep = mode.MainSteps[0];
                    string[] res = new string[] { modeName, "",commonStep.GetActions[ 0 ].EditText[ 1 ],
                        commonStep.GetActions[ 2 ].EditText[ 1 ],
                        commonStep.GetActions[ 3 ].EditText[ 1 ],
                        commonStep.GetActions[ 4 ].EditText[ 1 ],
                        commonStep.GetActions[ 5 ].EditText[ 1 ],
                        commonStep.GetActions[ 6 ].Items[ 0 ].EditText[ 1 ],
                        commonStep.GetActions[ 6 ].Items[ 1 ].EditText[ 1 ],
                        commonStep.GetActions[ 6 ].Items[ 2 ].EditText[ 1 ],
                        commonStep.GetActions[ 7 ].EditText[ 1 ]};

                    modeNode.Tag = res;

                    for (int i = 1; i < mode.MainSteps.Count; i++)
                    {
                        commonStep = mode.MainSteps[i];
                        string stepName;
                        TreeNode stepNode = new TreeNode();

                        stepName = i.ToString() + ". " + commonStep.EditText[0];
                        string[] resStep = new string[] { stepName, commonStep.GetActions[ 0 ].EditText[ 1 ],
                            commonStep.GetActions[ 2 ].EditText[ 1 ],
                            commonStep.GetActions[ 3 ].EditText[ 1 ],
                            commonStep.GetActions[ 4 ].EditText[ 1 ], "", "", "", "",""};

                        stepNode.Tag = resStep;
                        modeNode.Nodes.Add(stepNode);
                    }
                    objNode.Nodes.Add(modeNode);
                }

                tree.Nodes.Add(objNode);
            }
            return tree;
        }

        public TreeView SaveParamsAsTree()
        {
            TreeView tree = new TreeView();
            foreach (TechObject techObj in objects)
            {
                string techName = GetTechObjectN(techObj).ToString() + ". " +
                    techObj.EditText[0] + " " + techObj.TechNumber.ToString();
                TreeNode objNode = new TreeNode(techName);
                objNode.Tag = techObj;
                string[] ParamsType = { "S_PAR_F", "S_PAR_UI", "RT_PAR_F", "RT_PAR_UI" };
                for (int j = 0; j < techObj.Params.Items.Length; j++)
                {

                    if (techObj.Params.Items[j].Items != null)
                    {
                        TreeNode parTypeNode = new TreeNode(ParamsType[j]);
                        parTypeNode.Tag = ParamsType[j];
                        objNode.Nodes.Add(parTypeNode);
                        for (int i = 0; i < techObj.Params.Items[j].Items.Length; i++)
                        {
                            Param param = techObj.Params.Items[j].Items[i] as Param;
                            string parName = (i + 1).ToString() + ". " + param.EditText[0] + " , " + param.GetMeter();
                            TreeNode parNode = new TreeNode(parName);
                            parNode.Tag = param;
                            parTypeNode.Nodes.Add(parNode);
                        }
                    }
                }

                tree.Nodes.Add(objNode);
            }
            return tree;

        }

        /// <summary>
        /// Проверка и исправление ограничений при удалении/перемещении объекта
        /// </summary>
        /// <param name="TO_N"></param>
        public void CheckRestriction(int prev, int curr)
        {
            foreach (TechObject to in objects)
            {
                to.CheckRestriction(prev, curr);
            }
        }

        /// <summary>
        /// Изменение номеров владельцев ограничений
        /// </summary>
        /// <param name="TO_N"></param>
        public void SetRestrictionOwner()
        {
            foreach (TechObject to in objects)
            {
                to.SetRestrictionOwner();
            }
        }

        /// <summary>
        /// Проверка и исправление ограничений при удалении/перемещении операции
        /// </summary>
        /// <param name="TO_N"></param>
        public void ChangeModeNum(int objNum, int prev, int curr)
        {
            foreach (TechObject to in objects)
            {
                to.ChangeModeNum(objNum, prev, curr);
            }
        }

        #region Реализация ITreeViewItem

        override public string[] DisplayText
        {
            get
            {
                string res = "\"" + projectName + "\"";
                if (objects.Count > 0)
                {
                    res += " (" + objects.Count + ")";
                }

                return new string[] { res, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return objects.ToArray();
            }
        }

        override public bool Delete(object child)
        {
            TechObject techObject = child as TechObject;

            if (techObject != null)
            {
                int idx = objects.IndexOf(techObject) + 1;
                CheckRestriction(idx, -1);

                objects.Remove(techObject);

                SetRestrictionOwner();
                return true;
            }

            return false;
        }

        override public Editor.ITreeViewItem MoveDown(object child)
        {
            TechObject techObject = child as TechObject;

            if (techObject != null)
            {
                int index = objects.IndexOf(techObject);
                if (index <= objects.Count - 2)
                {
                    CheckRestriction(index + 1, index + 2);

                    objects.Remove(techObject);
                    objects.Insert(index + 1, techObject);

                    SetRestrictionOwner();
                    return objects[index];
                }
            }

            return null;
        }

        override public Editor.ITreeViewItem MoveUp(object child)
        {
            TechObject techObject = child as TechObject;

            if (techObject != null)
            {
                int index = objects.IndexOf(techObject);
                if (index > 0)
                {
                    CheckRestriction(index + 1, index);

                    objects.Remove(techObject);
                    objects.Insert(index - 1, techObject);

                    SetRestrictionOwner();
                    return objects[index];
                }
            }

            return null;
        }

        override public bool IsInsertableCopy
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem InsertCopy(object obj)
        {
            if (obj is TechObject)
            {
                int newN = 1;
                if (objects.Count > 0)
                {
                    newN = objects[objects.Count - 1].TechNumber + 1;
                }

                //Старый и новый номер объекта - для замены в ограничениях
                int oldObjN = GetTechObjectN(obj as TechObject);
                int newObjN = objects.Count + 1;

                TechObject newObject = (obj as TechObject).Clone(GetTechObjectN, newN, oldObjN, newObjN);
                objects.Add(newObject);

                newObject.ChangeCrossRestriction();

                return newObject;
            }

            return null;
        }

        override public Editor.ITreeViewItem Replace(object child,
            object copyObject)
        {
            TechObject techObject = child as TechObject;
            if (copyObject is TechObject && techObject != null)
            {
                int newN = techObject.TechNumber;

                //Старый и новый номер объекта - для замены в ограничениях
                int oldObjN = GetTechObjectN(copyObject as TechObject);
                int newObjN = GetTechObjectN(child as TechObject);

                TechObject newObject = (copyObject as TechObject).Clone(
                    GetTechObjectN, newN, oldObjN, newObjN);
                int index = objects.IndexOf(techObject);
                objects.Remove(techObject);

                objects.Insert(index, newObject);

                index = objects.IndexOf(newObject);

                newObject.ChangeCrossRestriction(techObject);

                return newObject;
            }

            return null;
        }

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem Insert()
        {
            TechObject newTechObject = null;

            if (objects.Count > 0)
            {
                if (objects.Count == 1)
                {
                    newTechObject =
                        new TechObject("Танк", GetTechObjectN, 1, 2, "TANK", -1, "TankObj", "");
                }
                else
                {
                    newTechObject = new TechObject(
                        objects[objects.Count - 1].EditText[0],
                        GetTechObjectN, objects[objects.Count - 1].TechNumber + 1,
                        objects[objects.Count - 1].TechType,
                        objects[objects.Count - 1].NameEplan,
                        objects[objects.Count - 1].CooperParamNumber,
                        objects[objects.Count - 1].NameBC,
                        objects[objects.Count - 1].AttachedObjects);
                }
            }
            else
            {
                newTechObject =
                    new TechObject("Гребенка", GetTechObjectN, 1, 1, "COMB", -1, "Grebenka", "");
            }

            objects.Add(newTechObject);
            return newTechObject;
        }

        public void SetCDBXTagView(bool combineTag)
        {
            cdbxTagView = combineTag;
        }

        #endregion

        private bool cdbxTagView;
        private LuaInterface.Lua lua;              ///< Экземпляр Lua.
        private List<TechObject> objects;          ///< Технологические объекты.
        private static TechObjectManager instance; ///< Единственный экземпляр класса.
        private string projectName;                ///< Имя проекта.
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологический объект проекта (танк, гребенка).
    /// </summary>
    public class TechObject : Editor.TreeViewItem
    {
        /// <summary>
        /// Класс для обозначения устройства (ОУ) в Eplan'е. При изменении
        /// также меняются названия устройств, участвующих в операциях объекта.
        /// </summary>
        public class NameInEplan : Editor.ObjectProperty
        {
            /// <param name="nameEplan">Обозначение устройства.</param>
            /// <param name="owner">Технологический объект-родитель.</param>
            public NameInEplan(string nameEplan, TechObject owner)
                : base("OУ", nameEplan)
            {
                this.owner = owner;
            }

            /// <summary>
            /// При изменении обозначения устройства (ОУ) также меняются
            /// названия устройств, участвующих в операциях объекта.
            /// </summary>
            public override bool SetNewValue(string newValue)
            {
                owner.SetNewEplanName(newValue);
                base.SetNewValue(newValue);
                owner.CompareEplanNames();

                return true;
            }

            public override bool NeedRebuildParent
            {
                get { return true; }
            }

            private TechObject owner;
        }

        private class TechObjectN : Editor.ObjectProperty
        {
            public TechObjectN(TechObject techObject, int value)
                : base("Номер", techObject.TechNumber.ToString())
            {
                this.techObject = techObject;

                SetValue(value);
            }

            public override bool SetNewValue(string newValue)
            {
                int oldNumber = Convert.ToInt32(EditText[1]);

                bool res = base.SetNewValue(newValue);
                if (res)
                {
                    techObject.ModifyDevNames(oldNumber);
                }

                return true;
            }

            public override bool NeedRebuildParent
            {
                get { return true; }
            }

            private TechObject techObject;
        }

        // Класс иерархического уровня устройства
        public class ObjS88Level : Editor.ObjectProperty
        {
            public ObjS88Level(int s88Level, TechObject owner) : base("S88Level", s88Level.ToString())
            {
                this.owner = owner;
                SetValue(s88Level);
            }

            public override bool SetNewValue(string newValue)
            {
                base.SetNewValue(newValue);
                return true;
            }

            override public bool IsEditable
            {
                get
                {
                    return false;
                }
            }

            public override bool NeedRebuildParent { get { return true; } }

            private TechObject owner;
        }

        // Класс привязанных агрегатов к аппарату
        public class AttachedToObjects : Editor.ObjectProperty
        {
            public AttachedToObjects(string attachedObjects, TechObject owner) : base("Привязанные агрегаты", attachedObjects)
            {
                this.owner = owner;
                SetValue(attachedObjects);
            }

            public override bool SetNewValue(string newValue)
            {
                base.SetNewValue(newValue);
                return true;
            }

            public override bool NeedRebuildParent { get { return true; } }

            private TechObject owner;

            // Иерархический номер для агрегата
            public string unitS88Number = "2";

            override public bool IsEditable
            {
                get
                {
                    if (owner.s88Level.EditText[1] != unitS88Number)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + "{\n" +
                prefix + "n          = " + TechNumber + ",\n" +
                prefix + "tech_type  = " + TechType + ",\n" +
                prefix + "name       = \'" + name + "\',\n" +
                prefix + "name_eplan = \'" + NameEplan + "\',\n" +
                prefix + "name_BC    = \'" + NameBC + "\',\n" +
                prefix + "cooper_param_number = " + CooperParamNumber + ",\n" +
                prefix + "base_tech_object = \'" + baseTechObject.GetName() + "\',\n" +
                prefix + "attached_objects = \'" + AttachedObjects + "\',\n";

            res += timers.SaveAsLuaTable(prefix);
            res += parameters.SaveAsLuaTable(prefix);
            res += "\n";

            res += modes.SaveAsLuaTable(prefix);

            res += prefix + "},\n";

            return res;
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveRestrictionAsLua(string prefix)
        {
            string res = "";
            string tmp = "";
            string comment = "\t\t--Объект №" + getN(this);

            tmp += modes.SaveRestrictionAsLua(prefix);
            if (tmp != "")
            {
                res += prefix + "\n" +
                    prefix + "[ " + getN(this) + " ] =" + comment + "\n" + tmp;
            }

            return res;
        }

        /// <summary>
        /// Создание технологического объекта.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="getN">Функция получения порядкового номера.</param>
        /// <param name="technologicalNumber">Технологический номер.</param>
        /// <param name="nameEplan">ОУ объекта в Eplan'е.</param>
        /// <param name="cooperParamNumber">Время совместного перехода шагов (параметр).</param>        
        public TechObject(string name, GetN getN, int technologicalNumber,
            int techType, string nameEplan, int cooperParamNumber, string NameBC,
            string attachedObjects)
        {
            this.name = name;
            this.getN = getN;

            this.techNumber = new TechObjectN(this, technologicalNumber);
            this.techType = new Editor.ObjectProperty("Тип", techType);
            this.nameBC = new Editor.ObjectProperty("Имя объекта Monitor", NameBC);
            this.nameEplan = new NameInEplan(nameEplan, this);
            this.cooperParamNumber = new Editor.ObjectProperty(
                "Время совместного перехода шагов (параметр)", cooperParamNumber);

            this.s88Level = new ObjS88Level(0, this);
            this.attachedObjects = new AttachedToObjects(attachedObjects, this);

            baseTechObject = new BaseTechObject(); // Экземпляр класса базового агрегата

            modes = new ModesManager(this);
            timers = new TimersManager();
            parameters = new ParamsManager();

            SetItems();
        }

        public TechObject Clone(GetN getN, int newNumber, int oldObjN, int newObjN)
        {
            TechObject clone = (TechObject)MemberwiseClone();

            clone.techNumber = new TechObjectN(clone, newNumber);

            clone.techType = new Editor.ObjectProperty("Тип", TechType);
            clone.nameBC = new Editor.ObjectProperty("Имя объекта Monitor",
                NameBC);
            clone.nameEplan = new NameInEplan(NameEplan, clone);

            clone.s88Level = new ObjS88Level(S88Level, clone);
            clone.attachedObjects = new AttachedToObjects(AttachedObjects, clone);

            clone.getN = getN;

            clone.modes = modes.Clone(clone);
            clone.modes.ChngeOwner(clone);
            clone.modes.ModifyDevNames(TechNumber);

            clone.modes.ModifyRestrictObj(oldObjN, newObjN);

            clone.parameters = parameters.Clone();

            clone.SetItems();

            return clone;
        }

        private void SetItems()
        {
            items = new Editor.ITreeViewItem[10];
            items[0] = this.s88Level;
            items[1] = this.techNumber;
            items[2] = this.techType;
            items[3] = this.nameEplan;
            items[4] = this.nameBC;
            items[5] = this.attachedObjects;
            items[6] = this.cooperParamNumber;

            items[7] = modes;
            items[8] = parameters;
            items[9] = timers;
        }
        /// <summary>
        /// Добавление операции.
        /// </summary>
        /// <param name="modeName">Имя операции.</param>
        /// <param name="baseOperationName">Имя базовой операции.</param>
        /// <param name="operExtraParams">Параметры базовой операции</param>
        /// <returns>Добавленная операция.</returns>
        public Mode AddMode(string modeName, string baseOperationName, LuaTable operExtraParams)
        {
            if (operExtraParams.Keys.Count > 0)
            {
                var extraParams = ConvertLuaTableToCArray(operExtraParams);
                return modes.AddMode(modeName, baseOperationName, extraParams);
            }
            else
            {
                return modes.AddMode(modeName, baseOperationName);
            }

        }

        // Конвертация значений LuaTable в C#
        public Editor.ObjectProperty[] ConvertLuaTableToCArray(LuaTable table)
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

        // Получение операции. 
        public Mode GetMode(int i)
        {
            if (modes.GetModes != null)
            {
                if (modes.GetModes.Count > i)
                {
                    return modes.GetModes[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Установка количества таймеров.
        /// </summary>
        /// <param name="count">Количество таймеров.</param>        
        public void SetTimersCount(int count)
        {
            timers.Count = count;
        }

        public void ModifyDevNames(int oldNumber)
        {
            modes.ModifyDevNames(oldNumber);
        }

        public void SetNewEplanName(string newTechObjectName)
        {
            modes.SetNewOwnerDevNames(newTechObjectName, this.TechNumber);
        }

        /// <summary>
        /// Получение менеджера параметров.
        /// </summary>
        /// <returns>Менеджер параметров.</returns>        
        public ParamsManager GetParamsManager()
        {
            return parameters;
        }


        /// <summary>
        /// Получение параметров.
        /// </summary>
        /// <returns>Параметры.</returns>
        public ParamsManager Params
        {
            get
            {
                return parameters;
            }
        }

        public int TechNumber
        {
            get
            {
                if (techNumber == null)
                {
                    return 0;
                }

                try
                {
                    return Convert.ToInt32(techNumber.EditText[1]);
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            set
            {
                techNumber.SetValue(value.ToString());
            }
        }

        public int TechType
        {
            get
            {
                return Convert.ToInt32(techType.EditText[1]);
            }
        }

        // Уровень по S88
        public int S88Level
        {
            get
            {
                return Convert.ToInt32(s88Level.EditText[1]);
            }
            set
            {
                s88Level.SetNewValue(value.ToString());
            }
        }

        // Привязанные агрегаты
        public string AttachedObjects
        {
            get
            {
                return attachedObjects.EditText[1];
            }
        }

        public string NameBC
        {
            get
            {
                return Convert.ToString(nameBC.EditText[1]);
            }
        }

        /// <summary>
        /// Получение ОУ объекта в Eplan'е.
        /// </summary>
        public string NameEplan
        {
            get
            {
                return nameEplan.EditText[1];
            }
        }

        private NameInEplan nameEplan; /// ОУ объекта в Eplan'е.

        /// <summary>
        /// Получение номера параметра со временем переходного включения 
        /// операций.
        /// </summary>
        public int CooperParamNumber
        {
            get
            {
                return Convert.ToInt32(cooperParamNumber.EditText[1]);
            }
        }

        public ModesManager GetModesManager
        {
            get
            {
                return modes;
            }
        }

        public void Synch(int[] array)
        {
            modes.Synch(array);
        }

        public void CheckRestriction(int prev, int curr)
        {
            modes.CheckRestriction(prev, curr);
        }

        public void ChangeCrossRestriction(TechObject oldObject = null)
        {
            if (oldObject != null)
            {
                modes.ChangeCrossRestriction(oldObject.GetModesManager);
            }
            else
            {
                modes.ChangeCrossRestriction();
            }
        }

        public void ChangeModeNum(int objNum, int prev, int curr)
        {
            modes.ChangeModeNum(objNum, prev, curr);
        }

        public void SetRestrictionOwner()
        {
            modes.SetRestrictionOwner();
        }

        public string NameEplanForFile
        {
            get
            {
                return nameEplanForFile;
            }
            set
            {
                nameEplanForFile = value;
            }
        }

        private string nameEplanForFile = string.Empty; // ОУ для файла main.prg.lua
        // Сравнение имен Eplan базового тех. объекта с текущим
        public void CompareEplanNames()
        {
            // Если не выбран базовый объект, то пустое имя
            // Если выбран, то сравниваем имена
            if (baseTechObject.GetS88Level() != 0)
            {
                string baseObjectNameEplan = baseTechObject.GetNameEplan().ToLower();
                string thisObjectNameEplan = NameEplan.ToLower();
                // Если тех. объект не содержит базовое ОУ, то добавить его.
                if (thisObjectNameEplan.Contains(baseObjectNameEplan) == false)
                {
                    NameEplanForFile = baseTechObject.GetNameEplan() + "_" + NameEplan;
                }
                else
                {
                    NameEplanForFile = NameEplan;
                }
            }
            else
            {
                NameEplanForFile = string.Empty;
            }
        }

        //  Получение имени базовой операции
        public string GetBaseTechObjectName()
        {
            return baseTechObject.GetName();
        }

        /// <summary>
        /// Номер параметра со временем совместного включения операций для шагов.
        /// </summary>
        private Editor.ObjectProperty cooperParamNumber;

        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Проверка операций технологического объекта
        /// </summary>
        public void Check()
        {
            ModesManager modesManager = GetModesManager;
            List<Mode> modes = modesManager.GetModes;
            foreach (Mode mode in modes)
            {
                mode.Check();
            }

        }

        #region Реализация ITreeViewItem

        override public string[] DisplayText
        {
            get
            {
                return new string[] {
                    getN( this ) + ". " + name + ' ' + techNumber.EditText[ 1 ],
                     baseTechObject.GetName() };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return items;
            }
        }

        override public bool SetNewValue(string newName)
        {
            name = newName;

            return true;
        }

        override public bool SetNewValue(string newValue, bool isExtraValue)
        {
            // Получил имя базового аппарата из LUA и записал в класс
            baseTechObject.SetName(newValue);

            // Нашел базовый объект и присвоил значения из него переменным
            BaseTechObject techObjFromDB = DBImitation.GetTObject(newValue);

            // Установил ОУ
            string nameEplan = techObjFromDB.GetNameEplan();
            baseTechObject.SetNameEplan(nameEplan);

            //Установил S88Level
            int s88Level = techObjFromDB.GetS88Level();
            baseTechObject.SetS88Level(s88Level);
            S88Level = baseTechObject.GetS88Level();

            // Т.к установили новое значение, произошла смена базового объекта
            // Надо сравнить ОУ и изменить его, если требуется
            CompareEplanNames();
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
                //Можем редактировать содержимое двух колонок.
                return new int[] { 0, 1 };
            }
        }

        override public string[] EditText
        {
            get
            {
                return new string[] { name, baseTechObject.GetName() };
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public bool IsMoveable
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

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        override public object Copy()
        {
            return this;
        }

        override public Editor.ITreeViewItem Replace(object child,
            object copyObject)
        {
            if (child is ParamsManager)
            {
                ParamsManager paramMan = child as ParamsManager;

                if (copyObject is ParamsManager && paramMan != null)
                {
                    ParamsManager copyMan = copyObject as ParamsManager;
                    for (int i = 0; i < 4; i++)
                    {
                        paramMan.Replace(paramMan.Items[i], copyMan.Items[i]);
                    }
                    return paramMan;
                }
            }
            return null;
        }

        #endregion

        private TechObjectN techNumber;   ///< Номер объекта технологический.
        private Editor.ObjectProperty techType;     ///< Тип объекта технологический.

        private string name;              ///< Имя объекта.
        private Editor.ObjectProperty nameBC;            ///< Имя объекта в Monitor.
        private ModesManager modes;       ///< Операции объекта.
        private TimersManager timers;     ///< Таймеры объекта.
        private ParamsManager parameters; ///< Параметры объекта.

        private Editor.ITreeViewItem[] items;   ///< Параметры объекта для редактирования.
                                                ///
        private GetN getN;

        private BaseTechObject baseTechObject; ///< Базовый аппарат (технологический объект)

        private ObjS88Level s88Level; // Уровень объекта в спецификации S88
        private AttachedToObjects attachedObjects; // Привязанные агрегаты

    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Класс реализующий базовый аппарат для технологического объекта
    /// </summary>
    /// 

    public class BaseTechObject
    {
        public BaseTechObject()
        {
            this.name = "";
            this.nameEplan = "";
        }

        public BaseTechObject(string name, string nameEplan)
        {
            this.name = name;
            this.nameEplan = nameEplan;
        }

        // Конструктор для имитации БД
        public BaseTechObject(string name, string nameEplan, int s88Level, BaseOperation[] baseOperations)
        {
            this.name = name;
            this.nameEplan = nameEplan;
            this.s88Level = s88Level;
            this.baseOperations = baseOperations;
        }

        public string GetName()
        {
            return name;
        }

        public string GetNameEplan()
        {
            return nameEplan;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetNameEplan(string nameEplan)
        {
            this.nameEplan = nameEplan;
        }

        public void SetS88Level(int s88Level)
        {
            this.s88Level = s88Level;
        }

        public int GetS88Level()
        {
            return s88Level;
        }

        public BaseOperation[] BaseOperations
        {
            get { return baseOperations; }
        }

        private string name; // Отображаемое имя технологического объекта
        private string nameEplan; // ОУ объекта в Eplan
        private int s88Level; // Уровень объекта по S88

        private BaseOperation[] baseOperations = new BaseOperation[0]; // Базовые операции базового объекта
    }

    // Имитационное хранилище БД
    public static class DBImitation
    {
        // Базовые операции
        private static BaseOperation[] baseOperationsList = new BaseOperation[]
        {
            new BaseOperation("", ""),
            new BaseOperation("Мойка", "WASHING_CIP"),
            new BaseOperation("Наполнение", "luaName1"),
            new BaseOperation("Хранение", "luaName2"),
            new BaseOperation("Выдача", "luaName3"),
        };

        // Возврат базовых операций
        public static BaseOperation[] GetBaseOperations()
        {
            return baseOperationsList;
        }

        // Возврат базовых технологических объектов
        public static BaseTechObject[] GetBaseTechObjects()
        {
            return baseTechObjectArr;
        }

        // Имитиация хранимой процедуры поиска ОУ по имени базового технологического объекта
        public static string GetNameEplan(string baseTechObjectName)
        {
            string nameEplan = "";

            foreach (BaseTechObject baseTechObject in baseTechObjectArr)
            {
                if (baseTechObject.GetName() == baseTechObjectName)
                {
                    nameEplan = baseTechObject.GetNameEplan();
                }
            }

            return nameEplan;
        }

        // Получение тех. объекта по номеру
        public static BaseTechObject GetTObject(string name)
        {
            foreach (BaseTechObject baseTechObject in baseTechObjectArr)
            {
                if (name == baseTechObject.GetName())
                {
                    return baseTechObject;
                }
            }
            return null;
        }

        // Поиск Lua имени операции
        public static string FindOperationLuaName(string name)
        {
            var luaName = "";
            luaName = baseOperationsList.First(x => x.GetName().Contains(name)).GetLuaName();
            return luaName;
        }

        // Возврат параметров базовой перации по имени из БД
        public static BaseOperationProperty[] GetOperParams(string baseOperName, string baseObjectName)
        {
            BaseTechObject currObj = baseTechObjectArr.First(x => x.GetName().Equals(baseObjectName));
            BaseOperation currOper = currObj.BaseOperations.First(x => x.GetName().Equals(baseOperName));
            BaseOperationProperty[] operationParams = currOper.BaseOperationProperties;
            if (operationParams == null) return new BaseOperationProperty[0];
            return operationParams;
        }

        //---------------- Init params ---------------------------------------------

        // ------TANK PARAMS ----------------------
        // Мойка
        private static BaseOperationProperty[] tankWashParams = new BaseOperationProperty[]
        {
            new BaseOperationProperty("CIP_WASH_END", "Мойка завершена", ""),
            new BaseOperationProperty("DI_CIP_FREE", "МСА свободна", "")
        };

        // Наполнение
        private static BaseOperationProperty[] tankFillParams = new BaseOperationProperty[]
        {
            new BaseOperationProperty("Param1", "Параметр 1", ""),
            new BaseOperationProperty("Param2", "Параметр 2", "")
        };

        // Хранение
        private static BaseOperationProperty[] tankStorageParams = new BaseOperationProperty[]
        {
            new BaseOperationProperty("Param3", "Параметр 3", ""),
            new BaseOperationProperty("Param4", "Параметр 4", "")
        };

        // Выдача
        private static BaseOperationProperty[] tankDeliveryParams = new BaseOperationProperty[]
        {
            new BaseOperationProperty("Param5", "Параметр 5", ""),
            new BaseOperationProperty("Param6", "Параметр 6", "")
        };

        //---------------------- TEST EMPTY PARAMS ------------------------
        private static BaseOperationProperty[] emptyParams = new BaseOperationProperty[0];

        //---------------- Init operations ---------------------------------------------

        // Базовые операции
        private static BaseOperation[] baseTankOperations = new BaseOperation[]
        {
            new BaseOperation("", ""),
            new BaseOperation("Мойка", "WASHING_CIP", tankWashParams),
            new BaseOperation("Наполнение", "luaName1", tankFillParams),
            new BaseOperation("Хранение", "luaName2", tankStorageParams),
            new BaseOperation("Выдача", "luaName3", tankDeliveryParams),
        };

        private static BaseOperation[] baseTestOperations = new BaseOperation[]
        {
            new BaseOperation("", ""),
            new BaseOperation("Мойка", "WASHING_CIP", emptyParams),
            new BaseOperation("Наполнение", "luaName1", emptyParams),
            new BaseOperation("Хранение", "luaName2", emptyParams),
            new BaseOperation("Выдача", "luaName3", emptyParams),
        };

        //---------------- Init objects ---------------------------------------------

        public static BaseTechObject[] baseTechObjectArr = new BaseTechObject[]
        {
            new BaseTechObject("", "", 0, baseTestOperations),
            new BaseTechObject("Автомат", "automat", 2, baseTestOperations),
            new BaseTechObject("Бойлер", "boil", 2, baseTestOperations),
            new BaseTechObject("Мастер", "master", 1, baseTestOperations),
            new BaseTechObject("Линия", "line", 2, baseTestOperations),
            new BaseTechObject("Линия приемки", "line_in", 2, baseTestOperations),
            new BaseTechObject("Линия выдачи", "line_out", 2, baseTestOperations),
            new BaseTechObject("Пастеризатор", "pasteurizator", 2, baseTestOperations),
            new BaseTechObject("Пост", "post", 2, baseTestOperations),
            new BaseTechObject("Танк", "tank", 1, baseTankOperations),
            new BaseTechObject("Узел подогрева", "heater_node", 2, baseTestOperations),
            new BaseTechObject("Узел охлаждения", "cooler_node", 2, baseTestOperations),
            new BaseTechObject("Узел перемешивания", "mix_node", 2, baseTestOperations)
        };
    }
}
