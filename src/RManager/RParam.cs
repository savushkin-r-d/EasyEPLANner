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
    public class RParam : TreeViewItem
    {
        public RParam(int ObjId, int ObjParam, float DefaultVal)
        {
            objId = ObjId;
            objParam = ObjParam;
            defaultVal = DefaultVal;
        }

        internal RParam Clone()
        {
            RParam clone = (RParam)MemberwiseClone();
            clone.objId = objId;
            clone.objParam = objParam;
            clone.defaultVal = defaultVal;
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
            string res = prefix + "obj_id = " + objId + ",\n" +
                prefix + "par_num = "+ objParam + ",\n" +
                prefix + "def_value = " + defaultVal + ",\n" +
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
                    TechObjectManager.GetInstance().GetTObject(objId);
                string toName = 
                    techObject.Name + " №" + techObject.TechNumber.ToString();
                string toParamName = 
                    techObject.GetParamsManager().Float.GetParam(objParam - 1).GetName() + 
                    ", " + techObject.GetParamsManager().Float.GetParam(objParam - 1).GetMeter();

                string propName = toName + ": " + toParamName;

                return new string[] { propName  , defaultVal.ToString() };
            }
        }

        public override int[] EditablePart
        {
            get
            {               
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
                return new string[] { "", defaultVal.ToString() };
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
                int[] intMatch = 
                    newVal.Where(Char.IsDigit).Select(
                        x => int.Parse(x.ToString())).ToArray();
                if (intMatch.Length == 2)
                {
                    if (CheckObjParam(intMatch[0], intMatch[1]))
                    {
                        objId = intMatch[0];
                        objParam = intMatch[1];
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
                defaultVal = double.Parse(newVal);
            }

            return true;
        }
        #endregion

        private bool CheckObjParam(int ObjId, int ObjParam)
        {
            TechObject.TechObject techObject = 
                TechObjectManager.GetInstance().GetTObject(ObjId);
            if (techObject != null)
            {
                if (techObject.GetParamsManager().Float.GetParam(ObjParam) != null)
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

        public int ObjID
        {
            get
            {
                return objId;
            }
        }

        public int ObjParam
        {
            get
            {
                return objParam;
            }
        }

        private int objId;
        private int objParam;
        private double defaultVal;
    }
}
