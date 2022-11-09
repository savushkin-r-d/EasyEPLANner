using System.Collections.Generic;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Ограничения
    /// </summary>
    public class Restriction : TreeViewItem
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
        /// Функция для создания копии объекта ограничений
        /// </summary>
        /// <returns></returns>
        public Restriction Clone()
        {
            var clone = (Restriction)MemberwiseClone();
            clone.restrictList = CreateCopyOfRestrictList(restrictList);
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

        public virtual void SetValue(IDictionary<int, List<int>> dict)
        {
            // Затычка для LocalRestriction.
        }

        /// <summary>
        /// Установка новых ограничений
        /// </summary>
        /// <param name="dict">Новый словарь с ограничениями</param>
        /// <returns></returns>
        override public bool SetNewValue(IDictionary<int, List<int>> dict)
        {
            var oldRestriction = CreateCopyOfRestrictList(restrictList);
            restrictList = CreateCopyOfRestrictList(dict);
            //Генерируем строку для отображения
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
            var oldRestriction = CreateCopyOfRestrictList(restrictList);
            restrictStr = newRestriction;
            ChangeRestrictList();

            SortedDictionary<int, List<int>> deletedRestriction =
                GetDeletedRestriction(oldRestriction);
            ClearCrossRestriction(deletedRestriction);
            SetCrossRestriction();
            return true;
        }

        protected SortedDictionary<int, List<int>> CreateCopyOfRestrictList(
            IDictionary<int, List<int>> copyingRestrictList)
        {
            var dict = new SortedDictionary<int, List<int>>();

            foreach(var objIndex in copyingRestrictList.Keys)
            {
                foreach(var modeIndex in copyingRestrictList[objIndex])
                {
                    if (dict.ContainsKey(objIndex))
                    {
                        dict[objIndex].Add(modeIndex);
                    }
                    else
                    {
                        dict.Add(objIndex, new List<int>() { modeIndex });
                    }
                }
            }

            return dict;
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
            var delRestriction = new SortedDictionary<int, List<int>>();

            SortedDictionary<int, List<int>>.KeyCollection keyColl =
                     oldRestriction.Keys;
            foreach (int key in keyColl)
            {
                if (!restrictList.ContainsKey(key))
                {
                    var newVal = new List<int>();
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
                                var newVal = new List<int>();
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
        /// Установка перекрестных ограничений
        /// </summary>
        protected void SetCrossRestriction()
        {
            if (LuaName != skippedRestrictionName)
            {
                SortedDictionary<int, List<int>>.KeyCollection keyColl =
                    restrictList.Keys;

                foreach (int key in keyColl)
                {
                    TechObject to = TechObjectManager.GetInstance()
                        .TechObjects[key - 1];
                    foreach (int val in restrictList[key])
                    {
                        Mode mode = to.GetMode(val - 1);
                        if (mode != null)
                        {
                            mode.AddRestriction(luaName, ownerObjNum,
                                ownerModeNum);
                        }
                        else
                        {
                            throw new System.Exception();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Удаление перекрестных ограничений
        /// </summary>
        protected void ClearCrossRestriction(SortedDictionary<int, 
            List<int>> diffDict)
        {
            // Для ограничений на последующие операции
            // не должны проставляться симметричные ограничения.
            if (LuaName != skippedRestrictionName && diffDict.Count != 0)
            {
                SortedDictionary<int, List<int>>.KeyCollection keyColl =
                    diffDict.Keys;

                foreach (int key in keyColl)
                {
                    TechObject to = TechObjectManager.GetInstance()
                        .TechObjects[key - 1];
                    foreach (int val in diffDict[key])
                    {
                        Mode mode = to.GetMode(val - 1);
                        if (mode != null)
                        {
                            mode.DelRestriction(luaName, ownerObjNum, 
                                ownerModeNum);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Изменение словаря ограничений на основании строки ограничений
        /// </summary>
        protected void ChangeRestrictList()
        {
            var res = new SortedDictionary<int, List<int>>();
            var techObjectsList = TechObjectManager.GetInstance()
                .TechObjects;
            for (int i = 0; i < techObjectsList.Count; i++)
            {
                TechObject to = techObjectsList[i];
                for (int j = 0; j < to.ModesManager.Modes.Count; j++)
                {
                    string restrictPair = $"{{ {i+1}, {j + 1} }}";
                    if (restrictStr.Contains(restrictPair))
                    {
                        if (res.ContainsKey(i + 1))
                        {
                            res[i + 1].Add(j + 1);
                        }
                        else
                        {
                            var restrictMode = new List<int>();
                            restrictMode.Add(j + 1);
                            res.Add(i + 1, restrictMode);
                        }
                    }
                }
            }
            restrictList = res;
            ChangeRestrictStr();
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
                    restrictList[ObjNum].Sort();
                }
            }
            else
            {
                var restrictMode = new List<int>();
                restrictMode.Add(ModeNum);
                restrictList.Add(ObjNum, restrictMode);
                restrictList[ObjNum].Sort();
            }

            //Генерируем строку для отображения
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

            //Генерируем строку для отображения
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

        /// <summary>
        /// Установить владельца ограничений.
        /// </summary>
        /// <param name="objNum">Номер объекта</param>
        /// <param name="modeNum">Номер операции</param>
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
                    res += $"{{ {key}, {val} }} ";
                }
            }

            restrictStr = res.Trim();
        }

        /// <summary>
        /// Изменить номер объекта.
        /// </summary>
        /// <param name="oldNum">Предыдущий номер объекта</param>
        /// <param name="newNum">Текущий номер объекта</param>
        public void ChangeObjNum(int oldNum, int newNum)
        {
            const int markForDelete = -1;
            if (newNum != markForDelete)
            {
                // Замена нового на старый
                if (restrictStr.Contains($"{{ {newNum},"))
                {
                    restrictStr = restrictStr
                        .Replace($"{{ {newNum},", $"{{N {oldNum},");
                }

                //Замена старого на новый
                if (restrictStr.Contains($"{{ {oldNum},"))
                {
                    restrictStr = restrictStr
                       .Replace($"{{ {oldNum},", $"{{ {newNum},");
                }

                // Убираем букву-заглушку, которая закрывала уже измененный
                // текст от изменений
                if (restrictStr.Contains("N"))
                {
                    restrictStr = restrictStr.Replace("N", "");
                }
            }
            else
            {
                // Удаление объекта (индекс -1)
                if (restrictStr.Contains($"{{ {oldNum},"))
                {
                    restrictStr += " ";
                    int idx = restrictStr.IndexOf($"{{ {oldNum},");
                    int idx_end = restrictStr.IndexOf("}", idx);
                    // 2й символ для пробела
                    restrictStr = restrictStr.Remove(idx, idx_end - idx + 2); 
                    restrictStr = restrictStr.Trim();
                    ChangeObjNum(oldNum, markForDelete);
                }
                else
                {
                    var techObjectsList = TechObjectManager.GetInstance()
                        .TechObjects;
                    for (int i = oldNum + 1; i <= techObjectsList.Count; i++)
                    {
                        if (restrictStr.Contains($"{{ {i},"))
                        {
                            restrictStr = restrictStr
                                .Replace($"{{ {i},", $"{{ {i - 1},");
                        }
                    }
                }
            }

            ChangeRestrictList();
        }

        /// <summary>
        /// Изменить номер операции
        /// </summary>
        /// <param name="objNum">Номер объекта</param>
        /// <param name="prev">Старый номер операции</param>
        /// <param name="curr">Новый номер операции</param>
        public void ChangeModeNum(int objNum, int prev, int curr)
        {
            const int markAsDelete = -1;
            if (curr != markAsDelete)
            {               
                // Замена нового на старый    
                if (restrictStr.Contains($"{{ {objNum}, {curr} }}"))
                {
                    restrictStr = restrictStr
                        .Replace($"{{ {objNum}, {curr} }}",
                        $"{{ {objNum}, {prev} N}}");
                }

                //Замена старого на новый
                if (restrictStr.Contains($"{{ {objNum}, {prev} }}"))
                {
                    restrictStr = restrictStr
                        .Replace($"{{ {objNum}, {prev} }}",
                        $"{{ {objNum}, {curr} }}");
                }

                // Убираем букву-заглушку, которая закрывала уже измененный
                // текст от изменений
                if (restrictStr.Contains("N"))
                {
                    restrictStr = restrictStr.Replace("N", "");
                }
            }
            // Удаление объекта (индекс -1)
            else
            {
                if (restrictStr.Contains($"{{ {objNum}, {prev} }}"))
                {
                    restrictStr += " ";
                    restrictStr = restrictStr
                        .Replace($"{{ {objNum}, {prev} }} ", "");
                    restrictStr = restrictStr.Trim();
                    ChangeModeNum(objNum, prev, markAsDelete);
                }
                else
                {
                    var modesCount = TechObjectManager.GetInstance()
                        .TechObjects[objNum - 1].ModesManager.Modes.Count;
                    for (int i = prev + 1; i <= modesCount; i++)
                    {
                        if (restrictStr.Contains($"{{ {objNum}, {i} }}"))
                        {
                            restrictStr = restrictStr
                                .Replace($"{{ {objNum}, {i} }}",
                                $"{{ {objNum}, {i - 1} }}");
                        }
                    }
                }
            }
            ChangeRestrictList();
        }

        /// <summary>
        /// Замена номера объекта при копировании. Кроме общих ограничений.
        /// </summary>
        /// <param name="oldObjN">Номер старого объекта</param>
        /// <param name="newObjN">Номер нового объекта</param>
        public void ModifyRestrictObj(int oldObjN, int newObjN)
        {
            ownerObjNum = newObjN;

            if (IsLocalRestrictionUse)
            {
                if (restrictStr.Contains($"{{ {oldObjN},"))
                {
                    restrictStr = restrictStr
                        .Replace($"{{ {oldObjN},", $"{{ {newObjN},");
                }
            }
            ChangeRestrictList();
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public SortedDictionary<int, List<int>> RestrictDictionary
        {
            get
            {
                return restrictList;
            }
        }

        #region Реализация ITreeViewItem
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

        public override bool IsFilled
        {
            get
            {
                if(restrictList.Count > 0 )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        public string LuaName
        {
            get
            {
                return luaName;
            }
        }

        private const string skippedRestrictionName = "NextModeRestriction";

        private string name;
        private string restrictStr;
        protected string luaName;
        protected SortedDictionary<int, List<int>> restrictList;

        private int ownerObjNum;
        private int ownerModeNum;
    }
}
