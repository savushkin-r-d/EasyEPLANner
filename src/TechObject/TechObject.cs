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
                prefix + "base_tech_object = \'" + baseTechObject.Name + 
                "\',\n" +
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
            baseTechObject = new BaseTechObject(); 

            modes = new ModesManager(this);
            timers = new TimersManager();
            parameters = new ParamsManager();

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
            clone.attachedObjects = new AttachedToObjects(AttachedObjects, 
                clone);

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
        public Mode AddMode(string modeName, string baseOperationName, 
            LuaTable operExtraParams)
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

        //  Получение имени базовой операции
        public string GetBaseTechObjectName()
        {
            return baseTechObject.Name;
        }

        /// <summary>
        /// Номер параметра со временем совместного включения операций 
        /// для шагов.
        /// </summary>
        private Editor.ObjectProperty cooperParamNumber;

        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Проверка операций технологического объекта
        /// </summary>
        /// <returns>Строка с ошибками</returns>
        public string Check()
        {
            var errors = string.Empty;

            ModesManager modesManager = GetModesManager;
            List<Mode> modes = modesManager.GetModes;
            foreach (var mode in modes)
            {
                errors += mode.Check();
            }

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
            // Получил имя базового аппарата из LUA и записал в класс
            baseTechObject.Name = newValue;

            // Нашел базовый объект и присвоил значения из него переменным
            BaseTechObject techObjFromDB = DataBase.Imitation
                .GetTObject(newValue);

            // Установил ОУ
            string nameEplan = techObjFromDB.EplanName;
            baseTechObject.EplanName = nameEplan;

            //Установил S88Level
            int s88Level = techObjFromDB.S88Level;
            baseTechObject.S88Level = s88Level;
            S88Level = baseTechObject.S88Level;

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
    }
}