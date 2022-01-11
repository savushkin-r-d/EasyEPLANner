using Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Recipe
{
    public class Recipe : TreeViewItem
    {
        public Recipe(string name)
        {
            this.name = name;
            parameters = new List<RecipeParameter>();
        }

        public static Recipe GetInstance()
        {
            if (instance == null)
            {
                instance = new Recipe("Рецепт №");
            }

            return instance;
        }

        public void AddParam(int objId, int objParam, float defaultValue)
        {
            bool newParamPair = CheckForDupicateParam(objId, objParam);
            if (newParamPair)
            {
                var param = new RecipeParameter(objId, objParam, defaultValue);
                param.AddParent(this);
                parameters.Add(param);
            }
        }

        public void DeleteParam(int objId, int objParam)
        {
            parameters.RemoveAll(par =>
                par.ObjectId == objId && par.ObjectParameterId == objParam);
        }

        public void DeleteParamByObj(int objId)
        {
            parameters.RemoveAll(par => par.ObjectId == objId);
        }

        private bool CheckForDupicateParam(int objId, int objParam)
        {
            foreach (RecipeParameter par in parameters)
            {
                bool isPairMatched =
                    (par.ObjectId == objId) && (par.ObjectParameterId == objParam);
                if (isPairMatched)
                {
                    return false;
                }
            }

            return true;
        }

        internal Recipe Clone()
        {
            var clone = (Recipe)MemberwiseClone();
            clone.parameters = new List<RecipeParameter>();
            foreach (RecipeParameter par in parameters)
            {
                RecipeParameter newPar = par.Clone();
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
            string res = "\t[ " + globalNum + " ] =\n" +
                prefix + "{\n" +
                prefix + "name = '" + name + "',\n" +
                prefix + "params = \n" +
                prefix + "\t{\n";

            foreach (RecipeParameter rParam in parameters)
            {
                int num = parameters.IndexOf(rParam) + 1;
                string tmp = rParam.SaveAsLuaTable(prefix + prefix);
                if (tmp != string.Empty)
                {
                    res += prefix + "\t[ " + num + " ] =\n" + prefix +
                        prefix + "{\n" + tmp;
                }
            }

            res += prefix + "\t},\n" + prefix + "},\n";

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

        override public bool SetNewValue(string newValue)
        {
            // Размер пары параметра {n, m},
            // где n - номер объекта, m - номер параметра объекта.
            const int pairSize = 2;

            //Если строка содержит пары чисел в скобках { },
            //нужно добавить параметры в список, иначе
            //заменить имя рецепта.
            if (newValue.Contains("{"))
            {
                string[] separators = { "{", "}" };
                var pairs = newValue
                    .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();

                string prevValue = CheckedObjects;
                var prevPairs = prevValue
                    .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();

                foreach (string pair in pairs)
                {
                    string pattern = @"\d{1,}";
                    var rgx = new Regex(pattern);

                    int[] intMatch = rgx.Matches(pair)
                        .Cast<Match>()
                        .Select(x => int.Parse(x.Value))
                        .ToArray();
                    if (intMatch.Length != pairSize)
                    {
                        continue;
                    }

                    int objId = intMatch[0];
                    int objParamId = intMatch[1];
                    AddParam(objId, objParamId, 0);

                    if (prevPairs.Contains(pair))
                    {
                        prevPairs.Remove(pair);
                    }
                }

                foreach (string pair in prevPairs)
                {
                    int[] intMatch = pair
                        .Where(Char.IsDigit)
                        .Select(x => int.Parse(x.ToString()))
                        .ToArray();
                    if (intMatch.Length != pairSize)
                    {
                        continue;
                    }

                    int objId = intMatch[0];
                    int objParamId = intMatch[1];
                    DeleteParam(objId, objParamId);
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
                return new int[] { 0, -1 };
            }
        }

        override public string[] EditText
        {
            get
            {
                return new string[] { name, string.Empty };
            }
        }

        public override string CheckedObjects
        {
            get
            {
                string pairs = string.Empty;
                foreach (RecipeParameter par in parameters)
                {
                    pairs += "{ " + par.ObjectId.ToString() + " " +
                        par.ObjectParameterId.ToString() + " } ";
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
            var param = child as RecipeParameter;
            if (param != null)
            {
                parameters.Remove(param);
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
            var par = obj as RecipeParameter;
            if (par != null)
            {
                bool newParamPair =
                    CheckForDupicateParam(par.ObjectId, par.ObjectParameterId);
                if (newParamPair)
                {
                    RecipeParameter newPar = par.Clone();
                    newPar.AddParent(this);
                    parameters.Add(newPar);
                    return newPar;
                }
            }

            return null;
        }

        override public ITreeViewItem Replace(object child, object copyObject)
        {
            var replacedPar = child as RecipeParameter;
            var copyPar = copyObject as RecipeParameter;
            bool objectsNotNull = replacedPar != null && copyPar != null;
            if (objectsNotNull)
            {
                if (CheckForDupicateParam(copyPar.ObjectId, copyPar.ObjectParameterId))
                {
                    RecipeParameter newPar = copyPar.Clone();

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
        private List<RecipeParameter> parameters;
    }
}
