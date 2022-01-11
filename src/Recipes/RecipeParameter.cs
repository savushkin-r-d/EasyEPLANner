using Editor;
using System;
using System.Linq;
using TechObject;

namespace Recipe
{
    public class RecipeParameter : TreeViewItem
    {
        public RecipeParameter(int objId, int objParam, float defaultValue)
        {
            this.objectId = objId;
            this.objectParameterId = objParam;
            this.defaultValue = defaultValue;
        }

        internal RecipeParameter Clone()
        {
            var clone = (RecipeParameter)MemberwiseClone();
            clone.objectId = objectId;
            clone.objectParameterId = objectParameterId;
            clone.defaultValue = defaultValue;
            return clone;
        }

        #region Реализация работы с Lua
        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + "obj_id = " + objectId + ",\n" +
                prefix + "par_num = " + objectParameterId + ",\n" +
                prefix + "def_value = " + defaultValue + ",\n" +
                prefix + "},\n";

            return res;
        }
        #endregion

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                TechObject.TechObject techObject = 
                    TechObjectManager.GetInstance().GetTObject(objectId);
                Param techObjectParameter = techObject.GetParamsManager()
                    .Float.GetParam(objectParameterId - 1);
                string techObjectParameterStr = 
                    $"{techObjectParameter.GetName()}, " +
                    $"{techObjectParameter.GetMeter()}";
                string techObjectStr =
                    $"{techObject.Name} №{techObject.TechNumber}";
                string recipeParameterName =
                    $"{techObjectStr}: {techObjectParameterStr}";

                return new string[]
                {
                    recipeParameterName,
                    defaultValue.ToString()
                };
            }
        }

        public override int[] EditablePart
        {
            get
            {
                //Можем редактировать второй столбец
                return new int[] { -1, 1 };
            }
        }

        public override bool IsEditable
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
                return new string[] { string.Empty, defaultValue.ToString() };
            }
        }

        override public bool IsDeletable
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

        public override bool IsReplaceable
        {
            get
            {
                return true;
            }
        }

        override public bool SetNewValue(string newVal)
        {
            if (newVal.Contains("{"))
            {
                int[] intMatch = newVal
                    .Where(Char.IsDigit)
                    .Select(x => int.Parse(x.ToString()))
                    .ToArray();
                bool correctParameterPair = intMatch.Length == 2;
                if (correctParameterPair)
                {
                    int newObjId = intMatch[0];
                    int newParamId = intMatch[1];
                    bool parameterExists =
                        CheckObjectParameterExistence(newObjId, newParamId);
                    if (parameterExists)
                    {
                        objectId = newObjId;
                        objectParameterId = newParamId;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                defaultValue = double.Parse(newVal);
            }

            return true;
        }
        #endregion

        private bool CheckObjectParameterExistence(int objId, int objParam)
        {
            TechObject.TechObject techObject = TechObjectManager
                .GetInstance().GetTObject(objId);
            if (techObject != null)
            {
                bool parameterExists = techObject.GetParamsManager().Float
                    .GetParam(objParam) != null;
                if (parameterExists)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public int ObjectId
        {
            get
            {
                return objectId;
            }
        }

        public int ObjectParameterId
        {
            get
            {
                return objectParameterId;
            }
        }

        private int objectId;
        private int objectParameterId;
        private double defaultValue;
    }
}
