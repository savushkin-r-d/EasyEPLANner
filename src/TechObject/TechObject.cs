using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
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
                newValue = newValue.ToUpper();
                owner.ModifyDevNames(newValue);
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
            public ObjS88Level(int s88Level, TechObject owner) : 
                base("S88Level", s88Level.ToString())
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
            public AttachedToObjects(string attachedObjects, TechObject owner)
                : base("Привязанные агрегаты", attachedObjects)
            {
                this.owner = owner;
                SetValue(attachedObjects);
            }

            public override bool SetNewValue(string newValue)
            {
                string oldValue = this.Value;

                List<int> numbers = GetValidTechObjNums(newValue);
                newValue = string.Join(" ", numbers);

                base.SetNewValue(newValue);

                List<int> deletedAgregatesNumbers = FindDeletedAgregates(
                    oldValue, newValue);
                RemoveDeletedAgregates(deletedAgregatesNumbers);

                InitAttachedAgregates(numbers);
                return true;
            }

            /// <summary>
            /// Получить корректные номера технологических объектов из
            /// входной строки
            /// </summary>
            /// <param name="inputString">Входная строка</param>
            /// <returns></returns>
            private List<int> GetValidTechObjNums(string inputString)
            {
                var numbers = new List<int>();
                string[] numbersAsStringArray = inputString.Split(' ')
                    .ToArray();

                foreach (var numAsString in numbersAsStringArray)
                {
                    int number;
                    int.TryParse(numAsString, out number);
                    if (number == 0)
                    {
                        continue;
                    }

                    TechObject obj = TechObjectManager.GetInstance()
                        .GetTObject(number);
                    if (obj != null && obj.BaseTechObject.IsAttachable)
                    {
                        numbers.Add(number);
                    }
                }

                numbers = numbers.Distinct().ToList();
                return numbers;
            }

            /// <summary>
            /// Найти удаленные агрегаты
            /// </summary>
            /// <param name="oldValue">Старое значение поля</param>
            /// <param name="newValue">Новое значение поля</param>
            /// <returns></returns>
            private List<int> FindDeletedAgregates(string oldValue, 
                string newValue)
            {
                var oldNumbers = new List<int>();
                var newNumbers = new List<int>();
                if (oldValue != null && oldValue != "")
                {
                    oldNumbers = oldValue.Split(' ').Select(int.Parse).ToList();
                }
                if (newValue != null && newValue != "")
                {
                    newNumbers = newValue.Split(' ').Select(int.Parse).ToList();
                }

                foreach(var newNum in newNumbers)
                {
                    if (oldNumbers.Contains(newNum))
                    {
                        oldNumbers.Remove(newNum);
                    }
                }

                return oldNumbers;
            }

            /// <summary>
            /// Удалить привязку агрегатов из аппарата.
            /// </summary>
            /// <param name="aggregatesNumbers">Список агрегатов</param>
            private void RemoveDeletedAgregates(List<int> aggregatesNumbers)
            {
                if (aggregatesNumbers.Count == 0)
                {
                    return;
                }

                foreach(var number in aggregatesNumbers)
                {
                    TechObject removingAgregate = TechObjectManager
                        .GetInstance().GetTObject(number);
                    BaseTechObject removingBaseTechObject = removingAgregate
                        .BaseTechObject;
                    BaseProperty[] properties = removingBaseTechObject
                        .AggregateProperties;
                    if (properties.Length == 0)
                    {
                        continue;
                    }

                    TechObject thisTechObject = owner;
                    List<Mode> modes = thisTechObject.ModesManager.Modes;
                    foreach (var mode in modes)
                    {
                        mode.BaseOperation.RemoveProperties(properties);
                    }
                }
            }

            /// <summary>
            /// Инициализация данных 
            /// </summary>
            /// <param name="objectsNumbrers">Список корректных номеров
            /// привязанных агрегатов</param>
            private void InitAttachedAgregates(List<int> objectsNumbrers)
            {
                foreach(var number in objectsNumbrers)
                {
                    TechObject attachedAggregate = TechObjectManager
                        .GetInstance().GetTObject(number);
                    BaseTechObject attachedBaseTechObject = attachedAggregate
                        .BaseTechObject;
                    BaseProperty[] properties = attachedBaseTechObject
                        .AggregateProperties;
                    if (properties.Length == 0)
                    {
                        continue;
                    }

                    TechObject thisThechObject = owner;
                    List<Mode> modes = thisThechObject.ModesManager.Modes;
                    foreach(var mode in modes)
                    {
                        if (mode.BaseOperation.Name == "")
                        {
                            continue;
                        }

                        mode.BaseOperation.AddProperties(properties);
                    }
                }
            }

            /// <summary>
            /// Проверить привязанные объекты на инициализацию и 
            /// инициализировать, если не инициализировано.
            /// </summary>
            public string Check()
            {
                var res = "";
                List<int> numbers = GetValidTechObjNums(this.Value);
                string checkedValue = string.Join(" ", numbers);
                if (checkedValue != this.Value)
                {
                    res += $"Проверьте привязанные агрегаты в объекте: " +
                        $"{owner.GlobalNumber}." +
                        $"{owner.Name + owner.TechNumber}. " +
                        $"В поле присутствуют агрегаты, которые нельзя " +
                        $"привязывать.\n";
                }

                InitAttachedAgregates(numbers);
                return res;
            }

            public override bool NeedRebuildParent 
            { 
                get 
                { 
                    return true; 
                } 
            }

            private TechObject owner;

            override public bool IsEditable
            {
                get
                {
                    if (owner.baseTechObject.Name == "Танк" ||
                        owner.baseTechObject.Name == "Линия" ||
                        owner.baseTechObject.Name == "Линия приемки" ||
                        owner.baseTechObject.Name == "Линия выдачи")
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
            string res = "\t[ " + getN(this) + " ] =\n"+
                prefix + "{\n" +
                prefix + "n          = " + TechNumber + ",\n" +
                prefix + "tech_type  = " + TechType + ",\n" +
                prefix + "name       = \'" + name + "\',\n" +
                prefix + "name_eplan = \'" + NameEplan + "\',\n" +
                prefix + "name_BC    = \'" + NameBC + "\',\n" +
                prefix + "cooper_param_number = " + CooperParamNumber + ",\n" +
                prefix + "base_tech_object = \'" + baseTechObject.EplanName + 
                "\',\n" +
                prefix + "attached_objects = \'" + AttachedObjects.Value + "\',\n";

            res += timers.SaveAsLuaTable(prefix);
            res += parameters.SaveAsLuaTable(prefix);
            res += "\n";

            res += modes.SaveAsLuaTable(prefix);

            res += equipment.SaveAsLuaTable(prefix);

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
            string comment = $"\t\t--{this.Name} {this.TechNumber}";

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
            int techType, string nameEplan, int cooperParamNumber, 
            string NameBC, string attachedObjects)
        {
            this.name = name;
            this.getN = getN;

            this.techNumber = new TechObjectN(this, technologicalNumber);
            this.techType = new Editor.ObjectProperty("Тип", techType);
            this.nameBC = new Editor.ObjectProperty("Имя объекта Monitor", 
                NameBC);
            this.nameEplan = new NameInEplan(nameEplan, this);
            this.cooperParamNumber = new Editor.ObjectProperty(
                "Время совместного перехода шагов (параметр)", 
                cooperParamNumber);

            this.s88Level = new ObjS88Level(0, this);
            this.attachedObjects = new AttachedToObjects(attachedObjects, 
                this);

            // Экземпляр класса базового агрегата
            baseTechObject = new BaseTechObject(this); 

            modes = new ModesManager(this);
            timers = new TimersManager();
            parameters = new ParamsManager();

            equipment = new Equipment(this);

            SetItems();
        }

        public TechObject Clone(GetN getN, int newNumber, int oldObjN, 
            int newObjN)
        {
            TechObject clone = (TechObject)MemberwiseClone();

            clone.techNumber = new TechObjectN(clone, newNumber);
            clone.techType = new Editor.ObjectProperty("Тип", TechType);
            clone.nameBC = new Editor.ObjectProperty("Имя объекта Monitor",
                NameBC);
            clone.nameEplan = new NameInEplan(NameEplan, clone);
            clone.s88Level = new ObjS88Level(S88Level, clone);
            clone.attachedObjects = new AttachedToObjects(AttachedObjects.Value, 
                clone);

            clone.getN = getN;
      
            clone.baseTechObject = baseTechObject.Clone(clone);

            clone.parameters = parameters.Clone();

            clone.modes.ChngeOwner(clone);
            clone.modes = modes.Clone(clone);
            clone.modes.ModifyDevNames(TechNumber);
            clone.modes.ModifyRestrictObj(oldObjN, newObjN);

            clone.equipment = equipment.Clone(clone);
            clone.equipment.ModifyDevNames();

            clone.SetItems();
            return clone;
        }

        private void SetItems()
        {
            items = new Editor.ITreeViewItem[11];
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
            items[10] = equipment;
        }
        /// <summary>
        /// Добавление операции.
        /// </summary>
        /// <param name="modeName">Имя операции.</param>
        /// <param name="baseOperationName">Имя базовой операции.</param>
        /// <param name="operExtraParams">Параметры базовой операции</param>
        /// <returns>Добавленная операция.</returns>
        public Mode AddMode(string modeName, string baseOperationName, 
            LuaTable operExtraParams)
        {
            if (operExtraParams.Keys.Count > 0)
            {
                var extraParams = StaticHelper.LuaHelper
                    .ConvertLuaTableToCArray(operExtraParams);
                return modes.AddMode(modeName, baseOperationName, extraParams);
            }
            else
            {
                return modes.AddMode(modeName, baseOperationName);
            }
        }

        /// <summary>
        /// Добавить элемент оборудования
        /// </summary>
        /// <param name="equipmentName">Имя</param>
        /// <param name="value">Значение</param>
        public void AddEquipment(string equipmentName, string value)
        {
            equipment.AddEquipment(equipmentName, value);
        }

        // Получение операции. 
        public Mode GetMode(int i)
        {
            if (modes.Modes != null)
            {
                if (modes.Modes.Count > i)
                {
                    return modes.Modes[i];
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
            equipment.ModifyDevNames();
        }

        public void ModifyDevNames(string newTechObjectName)
        {
            modes.ModifyDevNames(newTechObjectName, this.TechNumber);
            equipment.ModifyDevNames(newTechObjectName, this.TechNumber);
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

        /// <summary>
        /// Привязанные к аппарату агрегаты.
        /// </summary>
        public AttachedToObjects AttachedObjects
        {
            get
            {
                return attachedObjects;
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

        public ModesManager ModesManager
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
                modes.ChangeCrossRestriction(oldObject.ModesManager);
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
            if (baseTechObject.S88Level != 0)
            {
                string baseObjectNameEplan = baseTechObject.EplanName
                    .ToLower();
                string thisObjectNameEplan = NameEplan.ToLower();
                // Если тех. объект не содержит базовое ОУ, то добавить его.
                if (thisObjectNameEplan.Contains(baseObjectNameEplan) == false)
                {
                    NameEplanForFile = baseTechObject.EplanName + "_" + 
                        NameEplan;
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

        /// <summary>
        /// Базовый технологический объект
        /// </summary>
        public BaseTechObject BaseTechObject
        {
            get
            {
                return baseTechObject;
            }
        }

        /// <summary>
        /// Номер параметра со временем совместного включения операций 
        /// для шагов.
        /// </summary>
        private Editor.ObjectProperty cooperParamNumber;

        public string Name
        {
            get 
            { 
                return name; 
            }
        }

        /// <summary>
        /// Получить оборудование объекта.
        /// </summary>
        public Equipment Equipment
        {
            get
            {
                return equipment;
            }
        }

        /// <summary>
        /// Глобальный порядковый номер объекта.
        /// </summary>
        public int GlobalNumber
        {
            get
            {
                var number = this.DisplayText[0].Split('.')[0];
                number = number.Trim();
                return int.Parse(number);
            }
        }

        /// <summary>
        /// Проверка операций технологического объекта
        /// </summary>
        /// <returns>Строка с ошибками</returns>
        public string Check()
        {
            var errors = string.Empty;
            var objName = this.DisplayText[0];
            bool setBaseTechObj = this.DisplayText[1].Length > 0 ? true : false;


            if (setBaseTechObj == false)
            {
                string msg = string.Format("Не выбран базовый объект - " +
                    "\"{0}\"\n", objName);
                errors += msg;
            }
            else
            {
                errors += Equipment.Check();
                errors += attachedObjects.Check();
            }

            ModesManager modesManager = ModesManager;
            List<Mode> modes = modesManager.Modes;
            foreach (var mode in modes)
            {
                errors += mode.Check();
            }

            errors += Params.Check(objName);

            return errors;
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                return new string[] {
                    getN( this ) + ". " + name + ' ' + 
                    techNumber.EditText[ 1 ], baseTechObject.Name };
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
            if (baseTechObject.Name == newValue ||
                baseTechObject.EplanName == newValue)
            {
                return false;
            }

            if (baseTechObject.Name != "" && 
                (newValue != baseTechObject.Name ||
                newValue != baseTechObject.EplanName))
            {
                baseTechObject.ResetBaseOperations();
                equipment.Clear();
            }

            BaseTechObject techObjFromDB = DataBase.Imitation.GetTechObject(
                newValue);
            techObjFromDB.Owner = baseTechObject.Owner;
            baseTechObject = techObjFromDB;
            S88Level = baseTechObject.S88Level;
            equipment.AddItems(baseTechObject.Equipment);
            equipment.Check();

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
                return new string[] { name, baseTechObject.Name };
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        public override bool Delete(object child)
        {
            if (child is Equipment)
            {
                var equipment = child as Equipment;
                var objEquips = equipment.Items
                    .Select(x => x as BaseProperty).ToArray();
                foreach(var equip in objEquips)
                {
                    equip.SetNewValue("");
                }
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

            if (child is Equipment && copyObject is Equipment)
            {
                var equipment = child as Equipment;
                BaseProperty[] objEquips = equipment.Items
                    .Select(x => x as BaseProperty).ToArray();
                BaseProperty[] copyEquips = (copyObject as Equipment)
                    .Items.Select(x => x as BaseProperty).ToArray();
                foreach (var objEquip in objEquips)
                {
                    foreach(var copyEquip in copyEquips)
                    {
                        if (objEquip.LuaName == copyEquip.LuaName)
                        {
                            objEquip.SetNewValue(copyEquip.Value);
                        }
                    }
                }
                equipment.ModifyDevNames(this.NameEplan, this.TechNumber);
                return child as Editor.ITreeViewItem;
            }
            return null;
        }
        #endregion

        private TechObjectN techNumber; /// Номер объекта технологический.
        private Editor.ObjectProperty techType; /// Тип объекта технологический.

        private string name; /// Имя объекта.
        private Editor.ObjectProperty nameBC; /// Имя объекта в Monitor.
        private ModesManager modes; /// Операции объекта.
        private TimersManager timers; /// Таймеры объекта.
        private ParamsManager parameters; /// Параметры объекта.

        private Editor.ITreeViewItem[] items; /// Параметры объекта для редактирования.

        private GetN getN;

        /// Базовый аппарат (технологический объект)
        private BaseTechObject baseTechObject; 

        private ObjS88Level s88Level; // Уровень объекта в спецификации S88
        private AttachedToObjects attachedObjects; // Привязанные агрегаты

        private Equipment equipment; // Оборудование объекта
    }
}
