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
    public class Recipe : TreeViewItem
    {
        public Recipe(string Name)
        {
            name = Name;
            parameters = new List<RParam>();
        }

        public static Recipe GetInstance()
        {
            if (instance == null)
            {
                instance = new Recipe("Рецепт №");
            }

            return instance;
        }

        public void AddParam( int ObjId, int ObjParam, float DefValue )
        {
            bool isNewParamPair = CheckForDupicateParam(ObjId, ObjParam);
            if (isNewParamPair)
            {
                RParam param = new RParam(ObjId, ObjParam, DefValue);
                param.AddParent(this);
                parameters.Add(param);
            }
        }

        public void DeleteParam(int ObjId, int ObjParam)
        {
            parameters.RemoveAll(par => 
                par.ObjID == ObjId && par.ObjParam == ObjParam);
        }

        public void DeleteParamByObj(int ObjId)
        {
            parameters.RemoveAll(par => par.ObjID == ObjId);
        }

        private bool CheckForDupicateParam(int ObjId, int ObjParam)
        {
            foreach(RParam par in parameters)
            {
                bool isPairMatched = 
                    (par.ObjID == ObjId) && (par.ObjParam == ObjParam);
                if (isPairMatched)
                {
                    return false;
                }
            }
            return true;
        }

        internal Recipe Clone()
        {
            Recipe clone = (Recipe)MemberwiseClone();
            clone.parameters = new List<RParam>();
            foreach(RParam par in parameters)
            {
                RParam newPar = par.Clone();
                newPar.AddParent(this);
                clone.parameters.Add(newPar);
            }
            return clone;
        }

        #region Реализация работы с Lua
        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <param name="globalNum">Глобальный номер объекта</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix, int globalNum)
        {
            string tmp = "";

            string res = "\t[ " + globalNum + " ] =\n" +
                prefix + "{\n" +
                prefix + "name = '" + name + "',\n" +
                prefix + "params = \n" + 
                prefix + "\t{\n";

            foreach (RParam rParam in parameters)
            {
                int num = parameters.IndexOf(rParam) + 1;
                tmp = rParam.SaveAsLuaTable(prefix+prefix);
                if (tmp != "")
                {
                    res += prefix + "\t[ " + num + " ] =\n" + prefix + 
                        prefix + "{\n" + tmp;
                }
            }

            res += prefix + "\t},\n" +
                prefix + "},\n";

            return res;
        }
        #endregion

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                return new string[] { name, string.Empty };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return parameters.ToArray();
            }
        }

        /// Минимальное количество параметров, которые должны входить в пару
        /// {n ,m}, где n - номер объекта, m - номер параметра объекта.
        private const int minParamsCount = 2;

        override public bool SetNewValue(string newValue)
        {
            //Если строка содержит пары чисел в скобках { },
            //нужно добавить параметры в список, иначе
            //заменить имя рецепта.
            if (newValue.Contains("{"))
            {
                string[] separators = { "{", "}" };
                var pairs = newValue.Split(separators,
                    StringSplitOptions.RemoveEmptyEntries).
                    Select(s => s.Trim()).
                    Where(x => !string.IsNullOrWhiteSpace(x)).ToList(); 

                string prevValue = CheckedObjects;
                var prevPairs = prevValue.Split(separators,
                    StringSplitOptions.RemoveEmptyEntries).
                    Select(s => s.Trim()).
                    Where(x => !string.IsNullOrWhiteSpace(x)).ToList();


                foreach (string pair in pairs)
                {
                    string pattern = @"\d{1,}";
                    Regex rgx = new Regex(pattern);

                    int[] intMatch = rgx.Matches(pair).Cast<Match>()
                        .Select(x => int.Parse(x.Value))
                        .ToArray();
                    if (intMatch.Length < minParamsCount)
                    {
                        continue;
                    }

                    AddParam(intMatch[0], intMatch[1], 0);

                    if (prevPairs.Contains(pair))
                    {
                        prevPairs.Remove(pair);
                    }
                }

                foreach(string pair in prevPairs)
                {
                    int[] intMatch = pair.Where(Char.IsDigit).Select(
                        x => int.Parse(x.ToString())).ToArray();
                    if (intMatch.Length < minParamsCount)
                    {
                        continue;
                    }

                    int objId = intMatch[0];
                    int objParam = intMatch[1];
                    DeleteParam(objId, objParam);
                }
            }
            else
            {
                name = newValue;
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
                return new int[] { 0 , -1 };
            }
        }

        override public string[] EditText
        {
            get
            {
                return new string[] { name, "" };
            }
        }

        public override string CheckedObjects
        {
            get
            {
                string pairs = "";
                foreach (RParam par in parameters)
                {
                    pairs += "{ " + par.ObjID.ToString() + " " + 
                        par.ObjParam.ToString() + " } ";
                }
                return pairs;
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        public override bool Delete(object child)
        {
            RParam param = child as RParam;
            if (param != null)
            {
                parameters.Remove( param );
                return true;
            }

            return false;
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

        public override bool IsUseDevList
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

        override public bool IsInsertableCopy
        {
            get
            {
                return true;
            }
        }

        public override ITreeViewItem InsertCopy(object obj)
        {
            var par = obj as RParam;
            if (par != null)
            {
                if (CheckForDupicateParam(par.ObjID, par.ObjParam))
                {
                    RParam newPar = par.Clone();
                    newPar.AddParent(this);
                    parameters.Add(newPar);
                    return newPar;
                }
            }

            return null;
        }

        override public ITreeViewItem Replace(object child, object copyObject)
        {
            var replacedPar = child as RParam;
            var copyPar = copyObject as RParam;
            bool objectsNotNull = replacedPar != null && copyPar != null;
            if (objectsNotNull)
            {
                if (CheckForDupicateParam(copyPar.ObjID, copyPar.ObjParam))
                {
                    RParam newPar = copyPar.Clone();

                    int replacedIdx = parameters.IndexOf(replacedPar);
                    parameters.Remove(replacedPar);
                    parameters.Insert(replacedIdx, newPar);

                    newPar.AddParent(this);
                    return newPar;
                }
            }

            return null;
        }

        public override void GetDisplayObjects(out Device.DeviceType[] devTypes,
            out Device.DeviceSubType[] devSubTypes, out bool displayParameters)
        {
            devSubTypes = null; // Not used;
            devTypes = null;
            displayParameters = true;
        }
        #endregion

        private static Recipe instance;
        private string name;
        private List<RParam> parameters;
    }

}
