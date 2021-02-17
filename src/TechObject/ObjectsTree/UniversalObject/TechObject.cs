using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using Editor;
using System.Text.RegularExpressions;

namespace TechObject
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
                newValue = Regex.Replace(newValue,
                    StaticHelper.CommonConst.RusAsEngPattern,
                    StaticHelper.CommonConst.RusAsEnsEvaluator);

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

            public override bool IsDeletable
            {
                get
                {
                    return false;
                }
            }

            public override bool SetNewValue(string newValue)
            {
                const int minimalNumber = 0;
                int oldNumber = Convert.ToInt32(EditText[1]);
                bool validNewNum = int.TryParse(newValue, out int newNumber);
                if (validNewNum && newNumber > minimalNumber)
                {
                    bool res = base.SetNewValue(newValue);
                    if (res)
                    {
                        techObject.ModifyDevNames(oldNumber);
                    }
                    return true;
                }

                return false;
            }

            public override bool NeedRebuildParent
            {
                get { return true; }
            }

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
            string baseObjectName = "";
            if (baseTechObject != null)
            {
                baseObjectName = baseTechObject.EplanName;
            }

            string res = "\t[ " + globalNum + " ] =\n" +
                prefix + "{\n" +
                prefix + "n          = " + TechNumber + ",\n" +
                prefix + "tech_type  = " + TechType + ",\n" +
                prefix + "name       = \'" + name + "\',\n" +
                prefix + "name_eplan = \'" + NameEplan + "\',\n" +
                prefix + "name_BC    = \'" + NameBC + "\',\n" +
                prefix + "cooper_param_number = " + CooperParamNumber + ",\n" +
                prefix + "base_tech_object = \'" + baseObjectName + "\',\n" +
                prefix + AttachedObjects.WorkStrategy.LuaName + " = \'" + 
                AttachedObjects.Value + "\',\n";

            if (baseTechObject != null &&
                baseTechObject.ObjectGroupsList.Count > 0)
            {
                string objectGroups = string.Empty;
                foreach(var objectGroup in baseTechObject.ObjectGroupsList)
                {
                    if (objectGroup.Value == string.Empty)
                    {
                        continue;
                    }

                    objectGroups += prefix + prefix +
                        objectGroup.WorkStrategy.LuaName + " = \'" +
                        objectGroup.Value + "\',\n";
                }

                if (objectGroups != string.Empty)
                {
                    res += prefix + "tank_groups =\n";
                    res += prefix + "\t{\n";
                    res += objectGroups;
                    res += prefix + "\t},\n";
                }
            }

            res += paramsManager.SaveAsLuaTable(prefix);
            res += systemParams.SaveAsLuaTable(prefix);
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
        /// <param name="globalNum">Глобальный номер объекта</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveRestrictionAsLua(string prefix, int globalNum)
        {
            string res = "";
            string tmp = "";
            string comment = $"\t\t--{this.Name} {this.TechNumber}";

            tmp += modes.SaveRestrictionAsLua(prefix);
            if (tmp != "")
            {
                res += prefix + "[ " + globalNum + " ] =" + comment + "\n" +
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
                cooperParamNumber, -1);

            var allowedObjects = new List<BaseTechObjectManager.ObjectType>()
            {
                BaseTechObjectManager.ObjectType.Aggregate
            };
            string attachObjectsName = "Привязанные агрегаты";
            string attachObjectsLuaName = "attached_objects";
            this.attachedObjects = new AttachedObjects(attachedObjects, 
                this, new AttachedObjectStrategy.AttachedWithInitStrategy(
                    attachObjectsName, attachObjectsLuaName, allowedObjects));

            modes = new ModesManager(this);

            paramsManager = new ParamsManager();
            paramsManager.Parent = this;

            string sysParName = "Системные параметры";
            string sysParLuName = "system_parameters";
            systemParams = new SystemParams(sysParName, sysParLuName);
            systemParams.Parent = this;
            
            equipment = new Equipment(this);

            InitBaseTechObject(baseTechObject);
            SetItems();
        }

        /// <summary>
        /// Инициализация базового объекта
        /// </summary>
        /// <param name="baseTechObject">Базовый объект</param>
        public void InitBaseTechObject(BaseTechObject baseTechObject)
        {
            if (baseTechObject != null)
            {
                this.baseTechObject = baseTechObject.Clone(this);

                systemParams
                    .SetUpFromBaseTechObject(baseTechObject.SystemParams);
                equipment.AddItems(baseTechObject.Equipment);
                SetItems();
            }

            // Установили новое значение, произошла смена базового объекта
            // Надо сравнить ОУ и изменить его, если требуется
            CompareEplanNames();
        }

        /// <summary>
        /// Автоматическая настройка технологического объекта на основе его 
        /// базового объекта.
        /// </summary>
        public void SetUpFromBaseTechObject()
        {
            if(BaseTechObject == null)
            {
                return;
            }

            systemParams.SetUpFromBaseTechObject(BaseTechObject.SystemParams);
            ModesManager.SetUpFromBaseTechObject(BaseTechObject);
            //TODO: paramsManager.SetUpFromBaseTechObject(BaseTechObject.Parameters);
        }

        /// <summary>
        /// Сравнение имен Eplan базового тех. объекта с текущим.
        /// </summary>
        /// <param name="baseTechObject">Базовый объект</param>
        public void CompareEplanNames()
        {
            // Если не выбран базовый объект, то пустое имя
            // Если выбран, то сравниваем имена
            if (baseTechObject != null && baseTechObject.S88Level >= 0)
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

        public TechObject Clone(GetN getLocalNum, int newNumber,
            int oldGlobalNum, int newGlobalNum)
        {
            TechObject clone = (TechObject)MemberwiseClone();

            clone.techNumber = new TechObjectN(clone, newNumber);
            clone.techType = new ObjectProperty("Тип", TechType);
            clone.nameBC = new ObjectProperty("Имя объекта Monitor", NameBC);
            clone.nameEplan = new NameInEplan(NameEplan, clone);
            clone.attachedObjects = new AttachedObjects(AttachedObjects.Value, 
                clone, AttachedObjects.WorkStrategy);

            clone.getLocalNum = getLocalNum;

            if(baseTechObject != null)
            {
                clone.baseTechObject = baseTechObject.Clone(clone);
            }

            clone.paramsManager = paramsManager.Clone();

            clone.systemParams = systemParams.Clone();

            clone.modes = modes.Clone(clone);
            clone.modes.ModifyDevNames(TechNumber);
            clone.modes.ModifyRestrictObj(oldGlobalNum, newGlobalNum);

            clone.equipment = equipment.Clone(clone);
            clone.equipment.ModifyDevNames();

            clone.SetItems();
            return clone;
        }

        private void SetItems()
        {
            var itemsList = new List<ITreeViewItem>();
            itemsList.Add(techNumber);
            itemsList.Add(techType);
            itemsList.Add(nameEplan);
            itemsList.Add(nameBC);
            
            if (attachedObjects.IsEditable == true)
            {
                itemsList.Add(attachedObjects);
            }

            itemsList.Add(cooperParamNumber); // ??
            itemsList.Add(modes);
            itemsList.Add(paramsManager);

            if (baseTechObject?.SystemParams.Count > 0)
            {
                itemsList.Add(systemParams);
            }

            itemsList.Add(equipment);

            if (baseTechObject?.UseGroups == true)
            {
                itemsList.AddRange(baseTechObject.ObjectGroupsList);
            }

            items = itemsList.ToArray();
        }

        #region Установить свойства объектов и добавить их по необходимости
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
            equipment.SetEquipmentValue(equipmentName, value);
        }

        /// <summary>
        /// Добавить объекты группы объектов
        /// </summary>
        /// <param name="luaName">Lua-имя группы</param>
        /// <param name="value">Значение</param>
        public void AddGroupObjects(string luaName, string value)
        {
            if (baseTechObject?.UseGroups == true)
            {
                var foundGroup = baseTechObject.ObjectGroupsList
                    .Where(x => x.WorkStrategy.LuaName == luaName)
                    .FirstOrDefault();
                if (foundGroup != null)
                {
                    foundGroup.SetValue(value);
                }
            }
        }

        /// <summary>
        /// Установить значение системных параметров
        /// </summary>
        /// <param name="luaName">Lua-имя параметра</param>
        /// <param name="value">Значение параметра</param>
        public void AddSystemParameters(string luaName, string value)
        {
            if (baseTechObject?.SystemParams.Count > 0)
            {
                var foundParam = systemParams.GetParam(luaName);
                foundParam.Value.SetNewValue(value);
            }
        }
        #endregion

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
        /// Получение менеджера параметров.
        /// </summary>
        /// <returns>Менеджер параметров</returns>
        public ParamsManager GetParamsManager()
        {
            return paramsManager;
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
        public AttachedObjects AttachedObjects
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

        #region Синхронизация устройств в объекте.
        public void Synch(int[] array)
        {
            modes.Synch(array);
            equipment.Synch(array);
        }
        #endregion

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

        #region Проверки внутри объекта
        /// <summary>
        /// Проверка операций технологического объекта
        /// </summary>
        /// <returns>Строка с ошибками</returns>
        public string Check()
        {
            var errors = string.Empty;
            string fullobjName = DisplayText[0];
            string shortObjName = EditText[0];

            errors += CheckObjectName(shortObjName, fullobjName);
            errors += CheckBaseTechObject(fullobjName);
            errors += CheckNameEplan(fullobjName);
            errors += CheckModes();
            errors += GetParamsManager().Check(fullobjName);

            return errors;
        }

        /// <summary>
        /// Проверка имени объекта
        /// </summary>
        /// <param name="shortName">Имя объекта без номера и доп. информации
        /// </param>
        /// <param name="fullName">Полное название с номером и доп. информацией
        /// </param>
        /// <returns></returns>
        private string CheckObjectName(string shortName, string fullName)
        {
            const int maxSize = 50;
            int objNameLength = shortName.Length;
            if (objNameLength > maxSize)
            {
                return $"Имя объекта \"{fullName}\" превышает " +
                    $"максимальный размер в {maxSize} символов. " +
                    $"Размер: {objNameLength}.\n";
            }

            return string.Empty;
        }

        /// <summary>
        /// Проверить базовый технологический объект и его зависимости.
        /// </summary>
        /// <param name="objName">Полное название объекта</param>
        /// <returns></returns>
        private string CheckBaseTechObject(string objName)
        {
            string errors = string.Empty;
            bool setBaseTechObj = BaseTechObject != null ? true : false;

            if (setBaseTechObj == false)
            {
                string msg = string.Format("Неопознанный объект - " +
                    "\"{0}\"\n", objName);
                errors += msg;
            }
            else
            {
                errors += Equipment.Check();
                errors += attachedObjects.Check();
            }

            return errors;
        }

        /// <summary>
        /// Проверить ОУ объекта
        /// </summary>
        /// <param name="objName">Полное название объекта</param>
        /// <returns></returns>
        private string CheckNameEplan(string objName)
        {
            if (nameEplan.Value == string.Empty)
            {
                return $"Не задано ОУ в объекте \"{objName}\"\n";
            }

            return string.Empty;
        }

        /// <summary>
        /// Проверить операции объекта
        /// </summary>
        /// <returns></returns>
        private string CheckModes()
        {
            string errors = string.Empty;
            ModesManager modesManager = ModesManager;
            List<Mode> modes = modesManager.Modes;
            foreach (var mode in modes)
            {
                errors += mode.Check();
            }

            return errors;
        }
        #endregion

        /// <summary>
        /// Установить делегат для поиска локального номера объекта;
        /// </summary>
        /// <param name="getLocalNumMethod">Метод</param>
        public void SetGetLocalN(GetN getLocalNumMethod)
        {
            getLocalNum = getLocalNumMethod;
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                int localNum = 0;
                int globalNum = 0;
                if (getLocalNum == null)
                {
                    return new string[] {
                        localNum + ". " + name + " №" +
                        techNumber.EditText[ 1 ] + $" (#{globalNum})", "" };
                }
                else
                {
                    globalNum = TechObjectManager.GetInstance()
                        .GetTechObjectN(this);
                    return new string[] {
                        getLocalNum( this ) + ". " + name + " №" +
                        techNumber.EditText[ 1 ] + $" (#{globalNum})", "" };
                }
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
            ITreeViewItem item = child as ITreeViewItem;
            if(item != null)
            {
                item.Delete(this);
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

        override public ITreeViewItem Replace(object child,
            object copyObject)
        {
            var pars = child as ParamsManager;
            var copyPars = copyObject as ParamsManager;
            bool parsNotNull = pars != null && copyPars != null;
            if (parsNotNull)
            {
                pars.Replace(pars, copyPars);

                pars.AddParent(this);
                return pars;
            }

            var equipment = child as Equipment;
            var copyEquipment = copyObject as Equipment;
            bool equipmentNotNull = equipment != null && copyEquipment != null;
            if (equipmentNotNull)
            {
                BaseParameter[] objEquips = equipment.Items
                    .Select(x => x as BaseParameter).ToArray();
                BaseParameter[] copyEquips = copyEquipment.Items
                    .Select(x => x as BaseParameter).ToArray();
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
                equipment.ModifyDevNames(NameEplan, TechNumber);

                equipment.AddParent(this);
                return equipment;
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

        /// <summary>
        /// Сброс базового объекта
        /// </summary>
        public void ResetBaseTechObject()
        {
            ModesManager.ClearBaseOperations();
            systemParams.Clear();
            equipment.Clear();
            attachedObjects.SetNewValue(string.Empty);
            baseTechObject = null;
            CompareEplanNames();
        }

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

        public int GetLocalNum
        {
            get
            {
                return getLocalNum(this);
            }
        }

        private TechObjectN techNumber; /// Номер объекта технологический.
        private ObjectProperty techType; /// Тип объекта технологический.

        private string name; /// Имя объекта.
        private ObjectProperty nameBC; /// Имя объекта в Monitor.
        private ModesManager modes; /// Операции объекта.
        private ParamsManager paramsManager; /// Менеджер параметров объекта.
        private SystemParams systemParams; /// Системные параметры объекта.

        private ITreeViewItem[] items; /// Параметры объекта для редактирования.

        private GetN getLocalNum; /// Делегат для функции поиска номера объекта.

        /// Базовый аппарат (технологический объект)
        private BaseTechObject baseTechObject; 
        private AttachedObjects attachedObjects; // Привязанные агрегаты
        private Equipment equipment; // Оборудование объекта
    }
}
