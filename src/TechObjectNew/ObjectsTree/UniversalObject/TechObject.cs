using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using NewEditor;

namespace NewTechObject
{
    /// <summary>
    /// Технологический объект проекта (танк, гребенка).
    /// </summary>
    public class TechObject : TreeViewItem
    {
        /// <summary>
        /// Класс для обозначения устройства (ОУ) в Eplan'е. При изменении
        /// также меняются названия устройств, участвующих в операциях объекта.
        /// </summary>
        public class NameInEplan : ObjectProperty
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

        /// <summary>
        /// Класс внутреннего номера технологического объекта
        /// </summary>
        private class TechObjectN : ObjectProperty
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

        /// <summary>
        /// Класс привязки агрегатов к аппарату
        /// </summary>
        public class AttachedToObjects : ObjectProperty
        {
            public AttachedToObjects(string attachedObjects, 
                TechObject techObject) : base("Привязанные агрегаты", 
                    attachedObjects)
            {
                this.techObject = techObject;
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

            public override bool SetNewValue(
                SortedDictionary<int, List<int>> newDict)
            {
                var objectsList = new List<int>();
                foreach(var objectNumber in newDict.Keys)
                {
                    objectsList.Add(objectNumber);
                }
                objectsList.Sort();

                return SetNewValue(string.Join(" ", objectsList));
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
                    List<BaseParameter> properties = removingBaseTechObject
                        .AggregateParameters;

                    var deletingProperties = new List<BaseParameter>();
                    if (properties.Count != 0)
                    {
                        deletingProperties.AddRange(properties);
                    }

                    if (removingBaseTechObject.MainAggregateParameter != null)
                    {
                        deletingProperties.Add(removingBaseTechObject
                            .MainAggregateParameter);
                    }
                    TechObject thisTechObject = techObject;
                    List<Mode> modes = thisTechObject.ModesManager.Modes;
                    foreach (var mode in modes)
                    {
                        mode.BaseOperation.RemoveProperties(deletingProperties);
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
                    List<BaseParameter> properties = attachedBaseTechObject
                        .AggregateParameters;

                    var addingProperties = new List<BaseParameter>();
                    if (properties.Count != 0 )
                    {
                        addingProperties.AddRange(properties);
                    }

                    if (attachedBaseTechObject.MainAggregateParameter != null)
                    {
                        addingProperties.Add(attachedBaseTechObject
                            .MainAggregateParameter);
                    }
                    TechObject thisThechObject = techObject;
                    List<Mode> modes = thisThechObject.ModesManager.Modes;
                    foreach(var mode in modes)
                    {
                        if (mode.BaseOperation.Name == "")
                        {
                            continue;
                        }

                        mode.BaseOperation.AddProperties(addingProperties,
                            attachedBaseTechObject);
                        mode.BaseOperation.Check();
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
                    int objGlobalNumber = TechObjectManager.GetInstance()
                        .GetTechObjectN(techObject);
                    res += $"Проверьте привязанные агрегаты в объекте: " +
                        $"{objGlobalNumber}." +
                        $"{techObject.Name + techObject.TechNumber}. " +
                        $"В поле присутствуют агрегаты, которые нельзя " +
                        $"привязывать.\n";
                }

                InitAttachedAgregates(numbers);
                return res;
            }

            #region реализация ITreeViewItem
            public override bool NeedRebuildParent 
            { 
                get 
                { 
                    return true; 
                } 
            }

            override public bool IsEditable
            {
                get
                {
                    if (techObject.BaseTechObject.Name == "Танк" ||
                        techObject.BaseTechObject.Name == "Линия" ||
                        techObject.BaseTechObject.Name == "Линия приемки" ||
                        techObject.BaseTechObject.Name == "Линия выдачи")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public override bool IsFilled
            {
                get
                {
                    if(Value != "")
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

            private TechObject techObject;
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <param name="globalNum">Глобальный номер объекта</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix, int globalNum)
        {
            string res = "\t[ " + globalNum + " ] =\n"+
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
                res += prefix + "[ " /*+ getN(this)*/ + " ] =" + comment + "\n" + 
                    tmp + "\n";
            }

            return res;
        }

        /// <summary>
        /// Создание технологического объекта.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="getN">Функция получения порядкового локального номера.
        /// </param>
        /// <param name="technologicalNumber">Технологический номер.</param>
        /// <param name="nameEplan">ОУ объекта в Eplan'е.</param>
        /// <param name="cooperParamNumber">Время совместного перехода шагов 
        /// (параметр).</param>
        /// <param name="attachedObjects">Привязанные объекты</param>
        /// <param name="baseTechObject">Базовый технологический объект</param>
        /// <param name="NameBC">Имя объекта Monitor</param>
        /// <param name="techType">Номер типа</param>
        public TechObject(string name, GetN getLocalNum, 
            int technologicalNumber, int techType, string nameEplan, 
            int cooperParamNumber, string NameBC, string attachedObjects, 
            BaseTechObject baseTechObject)
        {
            this.name = name;
            this.getLocalNum = getLocalNum;

            this.techNumber = new TechObjectN(this, technologicalNumber);
            this.techType = new ObjectProperty("Тип", techType);
            this.nameBC = new ObjectProperty("Имя объекта Monitor", 
                NameBC);
            this.nameEplan = new NameInEplan(nameEplan, this);
            this.cooperParamNumber = new ObjectProperty(
                "Время совместного перехода шагов (параметр)", 
                cooperParamNumber);

            this.attachedObjects = new AttachedToObjects(attachedObjects, 
                this);

            modes = new ModesManager(this);

            parameters = new Params();
            parameters.Parent = this;
            
            equipment = new Equipment(this);

            InitBaseTechObject(baseTechObject);
            SetItems();
        }

        /// <summary>
        /// Инициализация базового объекта
        /// </summary>
        /// <param name="baseTechObject">Базовый объект</param>
        private void InitBaseTechObject(BaseTechObject baseTechObject)
        {
            if (baseTechObject != null)
            {
                this.baseTechObject = baseTechObject.Clone();
                
                equipment.AddItems(baseTechObject.Equipment);
                equipment.Check();

                // Установили новое значение, произошла смена базового объекта
                // Надо сравнить ОУ и изменить его, если требуется
                CompareEplanNames();
            }
        }

        /// <summary>
        /// Сравнение имен Eplan базового тех. объекта с текущим.
        /// </summary>
        public void CompareEplanNames()
        {
            // Если не выбран базовый объект, то пустое имя
            // Если выбран, то сравниваем имена
            if (baseTechObject.S88Level >= 0)
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

        public TechObject Clone(GetN getLocalNum, int newNumber)
        {
            TechObject clone = (TechObject)MemberwiseClone();

            clone.techNumber = new TechObjectN(clone, newNumber);
            clone.techType = new ObjectProperty("Тип", TechType);
            clone.nameBC = new ObjectProperty("Имя объекта Monitor", NameBC);
            clone.nameEplan = new NameInEplan(NameEplan, clone);
            clone.attachedObjects = new AttachedToObjects(AttachedObjects.Value, 
                clone);

            clone.getLocalNum = getLocalNum;

            clone.baseTechObject = baseTechObject.Clone(clone);

            clone.parameters = parameters.Clone();

            clone.modes.ChngeOwner(clone);
            clone.modes = modes.Clone(clone);
            clone.modes.ModifyDevNames(TechNumber);

            clone.equipment = equipment.Clone(clone);
            clone.equipment.ModifyDevNames();

            clone.SetItems();
            return clone;
        }

        private void SetItems()
        {
            items = new ITreeViewItem[9];
            items[0] = this.techNumber;
            items[1] = this.techType;
            items[2] = this.nameEplan;
            items[3] = this.nameBC;
            items[4] = this.attachedObjects;
            items[5] = this.cooperParamNumber;

            items[6] = modes;
            items[7] = parameters;
            items[8] = equipment;
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
                    .NEditorConvertLuaTableToCArray(operExtraParams);
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
        /// Получение параметров.
        /// </summary>
        /// <returns>Параметры</returns>
        public Params GetParams()
        {
            return parameters;
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

        /// <summary>
        /// ОУ для файла main.prg.lua
        /// </summary>
        public string NameEplanForFile { get; set; }

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
        private ObjectProperty cooperParamNumber;

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

            errors += GetParams().Check(objName);

            return errors;
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                return new string[] {
                    getLocalNum( this ) + ". " + name + ' ' + 
                    techNumber.EditText[ 1 ], "" };
            }
        }

        override public ITreeViewItem[] Items
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
                return new string[] { name, "" };
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
                    .Select(x => x as BaseParameter).ToArray();
                foreach(var equip in objEquips)
                {
                    equip.SetNewValue("");
                }
            }

            var parameters = child as Params;
            if (parameters != null)
            {
                parameters.Clear();
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

        override public ITreeViewItem Replace(object child,
            object copyObject)
        {
            if (child is Params)
            {
                var pars = child as Params;
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
            }

            if (child is Equipment && copyObject is Equipment)
            {
                var equipment = child as Equipment;
                BaseParameter[] objEquips = equipment.Items
                    .Select(x => x as BaseParameter).ToArray();
                BaseParameter[] copyEquips = (copyObject as Equipment)
                    .Items.Select(x => x as BaseParameter).ToArray();
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
                return child as ITreeViewItem;
            }
            return null;
        }

        public override bool ContainsBaseObject
        {
            get
            {
                return true;
            }
        }

        public override bool IsMainObject
        {
            get
            {
                return true;
            }
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
                return ImageIndexEnum.TechObject;
            }
        }
        #endregion

        public override string GetLinkToHelpPage()
        {
            string ostisLink = EasyEPlanner.ProjectManager.GetInstance()
                .GetOstisHelpSystemLink();
            if (baseTechObject.S88Level == 1)
            {
                return ostisLink + "?sys_id=unit";

            }
            else
            {
                return ostisLink + "?sys_id=equipment_module";
            }
        }

        private TechObjectN techNumber; /// Номер объекта технологический.
        private ObjectProperty techType; /// Тип объекта технологический.

        private string name; /// Имя объекта.
        private ObjectProperty nameBC; /// Имя объекта в Monitor.
        private ModesManager modes; /// Операции объекта.
        private Params parameters; /// Параметры объекта.

        private ITreeViewItem[] items; /// Параметры объекта для редактирования.

        private GetN getLocalNum;

        /// Базовый аппарат (технологический объект)
        private BaseTechObject baseTechObject; 
        private AttachedToObjects attachedObjects; // Привязанные агрегаты
        private Equipment equipment; // Оборудование объекта
    }
}
