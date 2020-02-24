using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Ограничения
    /// </summary>
    public class Restriction : Editor.TreeViewItem
    {
        public Restriction(string Name, string Value, string LuaName, 
            SortedDictionary<int, List<int>> dict)
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

        public virtual void SetValue(SortedDictionary<int, List<int>> dict)
        {
            // Затычка для LocalRestriction.
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

        public void ChangeCrossRestriction(SortedDictionary<int, 
            List<int>> oldDictionary = null)
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
        /// Функция используется для получения словаря, состоящего 
        /// из удаленных элементов</summary>
        /// <param name="oldRestriction">Предыдущая версия словаря 
        /// ограничений</param>
        /// <returns></returns>
        protected SortedDictionary<int, List<int>> GetDeletedRestriction(
            SortedDictionary<int, List<int>> oldRestriction)
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
        protected void SetCrossRestriction()
        {
            // Для ограничений на последующие операции
            // не должны проставляться симметричные ограничения.
            if (luaName != "NextModeRestriction")
            {
                SortedDictionary<int, List<int>>.KeyCollection keyColl =
                    restrictList.Keys;

                foreach (int key in keyColl)
                {
                    TechObject to = TechObjectManager.GetInstance()
                        .Objects[key - 1];
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
        protected void ClearCrossRestriction(SortedDictionary<int, 
            List<int>> diffDict)
        {
            // Для ограничений на последующие операции
            // не должны проставляться симметричные ограничения.
            if ((luaName != "NextModeRestriction") && (diffDict.Count != 0))
            {
                SortedDictionary<int, List<int>>.KeyCollection keyColl =
                    diffDict.Keys;

                foreach (int key in keyColl)
                {
                    TechObject to = TechObjectManager.GetInstance()
                        .Objects[key - 1];
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
        protected void ChangeRestrictList()
        {
            var res = new SortedDictionary<int, List<int>>();
            for (int i = 0; i < TechObjectManager.GetInstance().Objects.Count; 
                i++)
            {
                TechObject to = TechObjectManager.GetInstance().Objects[i];
                for (int j = 0; j < to.ModesManager.Modes.Count; j++)
                {
                    string restrictPair = "{ " + (i + 1).ToString() + ", " + 
                        (j + 1).ToString() + " }";
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
        protected void ChangeRestrictStr()
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
                            restrictStr = restrictStr.Replace("{ " + 
                                i.ToString(), "{N " + (i + 1).ToString());
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
                            restrictStr = restrictStr.Replace("{ " + 
                                i.ToString(), "{N " + (i - 1).ToString());
                        }
                    }
                }
                if (restrictStr.Contains("{ " + prev.ToString()))
                {
                    restrictStr = restrictStr.Replace("{ " + prev.ToString(), 
                        "{ " + curr.ToString());
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
                    // 2й символ для пробела
                    restrictStr = restrictStr.Remove(idx, idx_end - idx + 2); 
                    restrictStr.Trim();
                    this.ChangeObjNum(prev, -1);
                }
                else
                {
                    for (int i = prev + 1;
                        i < TechObjectManager.GetInstance().GetTechObj.Count; 
                        i++)
                    {
                        if (restrictStr.Contains("{ " + i.ToString()))
                        {
                            restrictStr = restrictStr.Replace("{ " + 
                                i.ToString(), "{ " + (i - 1).ToString());
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
                        if (restrictStr.Contains("{ " + objNum.ToString() + 
                            ", " + i.ToString() + " }"))
                        {
                            restrictStr = restrictStr.Replace("{ " + 
                                objNum.ToString() + ", " + i.ToString() + " }",
                                "{ " + objNum.ToString() + ", " + 
                                (i + 1).ToString() + " N}");
                        }
                    }
                }
                // Перемещение объекта вниз
                if (prev < curr)
                {
                    for (int i = curr; i > prev; i--)
                    {
                        if (restrictStr.Contains("{ " + objNum.ToString() + 
                            ", " + i.ToString() + " }"))
                        {
                            restrictStr = restrictStr.Replace("{ " + 
                                objNum.ToString() + ", " + i.ToString() + " }",
                                "{ " + objNum.ToString() + ", " + 
                                (i - 1).ToString() + " N}");
                        }
                    }
                }
                if (restrictStr.Contains("{ " + objNum.ToString() + ", " + 
                    prev.ToString() + " }"))
                {
                    restrictStr = restrictStr.Replace("{ " + 
                        objNum.ToString() + ", " + prev.ToString() + " }",
                            "{ " + objNum.ToString() + ", " + 
                            curr.ToString() + " }");

                }
                if (restrictStr.Contains("N"))
                {
                    restrictStr = restrictStr.Replace("N", "");
                }
            }
            // Удаление объекта ( индекс -1 )
            else
            {
                if (restrictStr.Contains("{ " + objNum.ToString() + ", " + 
                    prev.ToString() + " }"))
                {
                    restrictStr += " ";
                    restrictStr = restrictStr.Replace("{ " + 
                        objNum.ToString() + ", " + prev.ToString() + " } ", "");
                    restrictStr.Trim();
                    this.ChangeModeNum(objNum, prev, -1);
                }
                else
                {
                    for (int i = prev + 1;
                        i < TechObjectManager.GetInstance()
                        .GetTechObj[objNum - 1].ModesManager
                        .Modes.Count; i++)
                    {
                        if (restrictStr.Contains("{ " + objNum.ToString() + 
                            ", " + i.ToString() + " }"))
                        {
                            restrictStr = restrictStr.Replace("{ " + 
                                objNum.ToString() + ", " + i.ToString() + " }",
                                "{ " + objNum.ToString() + ", " + 
                                (i - 1).ToString() + " }");
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
                    restrictStr = restrictStr.Replace("{ " + 
                        oldObjN.ToString(), "{ " + newObjN.ToString());
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
            {
                //Можем редактировать содержимое второй колонки.
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
        protected SortedDictionary<int, List<int>> restrictList;

        private int ownerObjNum;
        private int ownerModeNum;
    }
}
