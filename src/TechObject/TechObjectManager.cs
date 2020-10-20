using EasyEPlanner;
using Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LuaInterface;
using System.Windows.Forms;

namespace TechObject
{
    /// <summary>
    /// Получение номера объекта в списке. Нумерация начинается с 1.
    /// </summary>
    public delegate int GetN(object obj);

    /// <summary>
    /// Менеджер объектов редактора.
    /// </summary>
    public class TechObjectManager : TreeViewItem, ITechObjectManager
    {
        private TechObjectManager()
        {
            InitLua();

            treeObjects = new List<ITreeViewItem>();
            techObjects = new List<TechObject>();
            techObjectManagerChecker = new TechObjectChecker(this);
            techObjectXMLMaker = new TechObjectXMLMaker(this);
        }

        #region Инициализация LUA-скриптов
        /// <summary>
        /// Инициализировать Lua-скрипт для чтения описания объектов
        /// </summary>
        private void InitLua()
        {
            lua = new Lua();
            InitReadDescriptionLuaScript();
            InitReadRestrictionsLuaScript();
        }

        /// <summary>
        /// Загрузка скрипта для чтения описания объектов.
        /// </summary>
        private void InitReadDescriptionLuaScript()
        {
            lua.RegisterFunction("ADD_TECH_OBJECT", this,
                GetType().GetMethod("AddObject"));
            const string sysFileName = "sys.lua";
            string sysLuaPath = Path.Combine(ProjectManager.GetInstance()
                .SystemFilesPath, sysFileName);
            lua.DoFile(sysLuaPath);
        }

        /// <summary>
        /// Загрузка скрипта для чтения ограничений объектов.
        /// </summary>
        private void InitReadRestrictionsLuaScript()
        {
            lua.RegisterFunction("Get_TECH_OBJECT", this,
                GetType().GetMethod("GetTObject"));
            string restrictionFileName = "sys_restriction.lua";
            string pathToRestrictionInitializer = Path
                .Combine(ProjectManager.GetInstance().SystemFilesPath,
                restrictionFileName);
            lua.DoFile(pathToRestrictionInitializer);
        }
        #endregion

        /// <summary>
        /// Получить экземпляр класса менеджера объектов
        /// </summary>
        /// <returns></returns>
        public static TechObjectManager GetInstance()
        {
            if (instance == null)
            {
                instance = new TechObjectManager();
            }

            return instance;
        }

        /// <summary>
         /// Получение объекта по номеру
         /// </summary>
         /// <param name="i">Номер объекта</param>
         /// <returns></returns>
        public TechObject GetTObject(int i)
        {
            if (techObjects != null && techObjects.Count >= i)
            {
                return techObjects[i - 1];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Получение номера операции в списке операций. 
        /// Нумерация начинается с 1.
        /// </summary>
        /// <param name="mode">Операция, номер которой хотим получить.</param>
        /// <returns>Номер заданной операции.</returns>
        public int GetTechObjectN(object techObject)
        {
            return techObjects.IndexOf(techObject as TechObject) + 1;
        }

        /// <summary>
        /// Получить номер объекта по его отображаемому имени в дереве.
        /// </summary>
        /// <param name="displayText">Отображаемый текст</param>
        /// <returns></returns>
        public int GetTechObjectN(string displayText)
        {
            TechObject findedObject = TechObjects
                .Where(x => x.DisplayText[0] == displayText).FirstOrDefault();

            if(findedObject != null)
            {
                return techObjects.IndexOf(findedObject) + 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Проверка и исправление ограничений при удалении/перемещении
        /// операции 
        /// </summary>
        public void ChangeModeNum(int objNum, int oldNum, int newNum)
        {
            foreach (TechObject to in TechObjects)
            {
                to.ChangeModeNum(objNum, oldNum, newNum);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = "";
            foreach (TechObject obj in TechObjects)
            {
                int num = TechObjects.IndexOf(obj) + 1;
                res += obj.SaveAsLuaTable(prefix + "\t\t", num);
            }
            res = res.Replace("\t", "    ");
            return res;
        }

        /// <summary>
        /// Сохранение ограничений в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveRestrictionAsLua(string prefix)
        {
            var res = "";
            foreach (TechObject obj in TechObjects)
            {
                int num = TechObjects.IndexOf(obj) + 1;
                res += obj.SaveRestrictionAsLua(prefix + "\t", num);
            }
            res = res.Replace("\t", "    ");
            return res;
        }

        #region Загрузка описания из LUA
        /// <summary>
        /// Загрузка описания проекта из строки
        /// </summary>
        /// <param name="LuaStr">Строка с описанием</param>
        /// <param name="projectName">Имя проекта</param>
        public void LoadDescription(string LuaStr, string projectName)
        {
            ProjectName = projectName;
            treeObjects.Clear();
            techObjects.Clear();

            //Сброс описания объектов.
            lua.DoString("init_tech_objects_modes = nil");
            try
            {
                //Выполнения Lua скрипта с описанием объектов.
                lua.DoString(LuaStr);
            }
            catch (Exception ex)
            {
                string message = $"{ex.Message}. Ошибка обработки " +
                    $"Lua-скрипта описания проекта. " +
                    $"Данные не будут сохранены.";
                throw new Exception(message);
            }
            try
            {
                //Создание объектов C# из скрипта Lua.
                object[] res = lua.DoString("if init ~= nil then init() end");
            }
            catch (Exception ex)
            {
                string message = $"{ex.Message}. Ошибка обработки " +
                    $"Lua-скрипта для загрузки данных проекта. " +
                    $"Данные не будут сохранены.\n" +
                    $"Source: {ex.Source}";
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Загрузка ограничений объектов
        /// </summary>
        /// <param name="LuaStr">Описание ограничений объектов</param>
        public void LoadRestriction(string LuaStr)
        {
            //Выполнения Lua скрипта с описанием объектов.
            lua.DoString(LuaStr);
            lua.DoString("init_restriction()");
        }
        #endregion

        #region Добавление объекта из LUA
        /// <summary>
        /// Добавление технологического объекта. Вызывается из Lua.
        /// </summary>
        /// <returns>Добавленный технологический объект.</returns>
        /// <param name="globalNumber">Глобальный номер объекта, используется
        /// при импорте из файла</param>
        public TechObject AddObject(int globalNumber, int techN, string name,
            int techType, string nameEplan, int cooperParamNumber,
            string NameBC, string baseTechObjectName, string attachedObjects)
        {
            // globalNumber игнорируется в этом методе, но используется при
            // импорте описания из файла (аналогичная сигнатура, другое тело).

            var baseTechObject = BaseTechObjectManager.GetInstance()
                .GetTechObject(baseTechObjectName);
            // getN - null т.к он будет другой, ниже по функциям.
            TechObject obj = new TechObject(name, null, techN,
                techType, nameEplan.ToUpper(), cooperParamNumber, NameBC,
                attachedObjects, baseTechObject);

            AddObject(obj);

            return obj;
        }

        /// <summary>
        /// Добавление технологического объекта.
        /// </summary>
        /// <param name="obj">Добавляемый объект</param>
        private void AddObject(TechObject obj)
        {
            if (obj.BaseTechObject != null)
            {
                AddIdentifiedObjectWhenLoadFromLua(obj);
            }
            else
            {
                AddUnidentifiedObjectWhenLoadFromLua(obj);
            }

            techObjects.Add(obj);
        }

        /// <summary>
        /// Добавить опознанный объект при загрузке из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        private void AddIdentifiedObjectWhenLoadFromLua(TechObject obj)
        {
            BaseTechObject baseTechObject = obj.BaseTechObject;
            var type = (BaseTechObjectManager.ObjectType)baseTechObject
                .S88Level;
            string name = BaseTechObjectManager.GetInstance()
                        .GetS88Name(baseTechObject.S88Level);
            switch (type)
            {
                case BaseTechObjectManager.ObjectType.ProcessCell:
                     AddProcessCellFromLua(obj);
                    break;

                case BaseTechObjectManager.ObjectType.Unit:
                    AddS88ObjectFromLua(obj, name);
                    break;

                case BaseTechObjectManager.ObjectType.Aggregate:
                    AddS88ObjectFromLua(obj, name);
                    break;

                case BaseTechObjectManager.ObjectType.UserObject:
                    AddUserObject(obj);
                    break;
            }
        }

        /// <summary>
        /// Добавить ячейку процесса (мастер) из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <returns></returns>
        private void AddProcessCellFromLua(TechObject obj)
        {
            var masterItem = treeObjects.Where(x => x is ProcessCell)
                        .FirstOrDefault() as ProcessCell;
            if (masterItem == null)
            {
                masterItem = new ProcessCell();
                treeObjects.Add(masterItem);
            }

            masterItem.AddObjectWhenLoadFromLua(obj);
        }

        /// <summary>
        /// Добавить аппарат из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="name">Имя объекта</param>
        /// <returns></returns>
        private void AddS88ObjectFromLua(TechObject obj, string name)
        {
            var s88Item = treeObjects
                .Where(x => x is S88Object && x.DisplayText[0].Contains(name))
                .FirstOrDefault() as S88Object;
            if (s88Item == null)
            {
                s88Item = new S88Object(name);
                treeObjects.Add(s88Item);
            }

            s88Item.AddObjectWhenLoadFromLua(obj);
        }

        /// <summary>
        /// Добавить пользовательский объект из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        private void AddUserObject(TechObject obj)
        {
            var userObject = treeObjects.Where(x => x is UserObject)
                .FirstOrDefault() as UserObject;
            if(userObject == null)
            {
                userObject = new UserObject();
                treeObjects.Add(userObject);
            }

            userObject.AddObjectWhenLoadFromLua(obj);
        }

        /// <summary>
        /// Добавить неопознанный объект при добавлении из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        private void AddUnidentifiedObjectWhenLoadFromLua(TechObject obj)
        {
            var unidentifiedObject = treeObjects
                .Where(x => x is Unidentified)
                .FirstOrDefault() as Unidentified;
            if (unidentifiedObject == null)
            {
                unidentifiedObject = new Unidentified();
                treeObjects.Add(unidentifiedObject);
            }

            unidentifiedObject.AddUnidentifiedObject(obj);
        }
        #endregion

        public void ImportObject(TechObject importingObject)
        {
            AddObject(importingObject);
        }

        /// <summary>
        /// Проверка технологического объекта на правильность ввода и др.
        /// </summary>
        /// <returns>Строка с ошибками</returns>
        public string Check()
        {
            return techObjectManagerChecker.Check();
        }

        /// <summary>
        /// Формирование узлов для операций, шагов и параметров объектов.
        /// </summary>
        /// <param name="rootNode">корневой узел</param>
        /// <param name="combineTags">Сгруппировать тэги в один подтип</param>
        /// <param name="useNewNames">Использовать имена объектов вместо
        /// OBJECT</param>
        public void GetObjectForXML(TreeNode rootNode, bool combineTags,
            bool useNewNames)
        {
            techObjectXMLMaker.GetObjectForXML(rootNode, combineTags,
                useNewNames);
        }

        #region Синхронизация устройств в объектах
        /// <summary>
        /// Синхронизация устройств в объектах
        /// </summary>
        /// <param name="array">Индексная таблица</param>
        public void Synch(int[] array)
        {
            foreach (TechObject obj in TechObjects)
            {
                obj.Synch(array);
            }
        }
        #endregion

        #region Реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                string res = "\"" + ProjectName + "\"";
                if (treeObjects.Count > 0)
                {
                    res += " (" + treeObjects.Count + ")";
                }

                return new string[] { res, "" };
            }
        }

        public override ITreeViewItem[] Items
        {
            get
            {
                return treeObjects.ToArray();
            }
        }

        public override ImageIndexEnum ImageIndex
        {
            get
            {
                return ImageIndexEnum.TechObjectManager;
            }
        }

        public override bool IsInsertableCopy
        {
            get
            {
                return true;
            }
        }

        public override ITreeViewItem InsertCopy(object obj)
        {
            var techObj = obj as TechObject;
            if(techObj != null && techObj.MarkToCut)
            {
                ChooseObjectTypes(out string selectedType,
                    out string selectedSubType);
                if (selectedType != null && selectedSubType != null)
                {
                    ITreeViewItem insertedItem = InsertType(selectedType,
                        techObj);
                    return insertedItem;
                }
            }

            return null;
        }

        public override bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        public override ITreeViewItem Insert()
        {
            ChooseObjectTypes(out string selectedType,
                out string selectedSubType);
            if (selectedType != null && selectedSubType != null)
            {
                ITreeViewItem insertedItem = InsertType(selectedType);
                return insertedItem;
            }

            return null;
        }

        /// <summary>
        /// Выбор типа и подтипа объекта на форме.
        /// </summary>
        /// <param name="selectedType">Тип</param>
        /// <param name="selectedSubType">Подтип</param>
        private void ChooseObjectTypes(out string selectedType,
            out string selectedSubType)
        {
            var objectsAdderForm = new ObjectsAdder();
            objectsAdderForm.ShowDialog();
            selectedType = ObjectsAdder.LastSelectedType;
            selectedSubType = ObjectsAdder.LastSelectedSubType;
        }

        private ITreeViewItem InsertType(string selectedType,
            TechObject techObj = null)
        {
            ITreeViewItem treeItem = GetTreeItem(selectedType);
            ITreeViewItem innerItem;

            bool needInsert = techObj == null;
            if (needInsert)
            {
                innerItem = treeItem.Insert();
            }
            else
            {
                innerItem = treeItem.InsertCopy(techObj);
            }

            if (innerItem != null)
            {
                if (!treeObjects.Contains(treeItem))
                {
                    treeObjects.Add(treeItem);
                }

                return treeItem;
            }

            return null;
        }

        /// <summary>
        /// Создать объект дерева, описывающий базу по S88.
        /// </summary>
        /// <param name="selectedType">Выбранный на форме тип объекта</param>
        /// <returns></returns>
        private ITreeViewItem GetTreeItem(string selectedType)
        {
            ITreeViewItem treeItem = treeObjects
                .Where(x => x.EditText[0] == selectedType)
                .FirstOrDefault();
            if (treeItem == null)
            {
                switch(selectedType)
                {
                    case ProcessCell.Name:
                        return new ProcessCell();

                    case UserObject.Name:
                        return new UserObject();

                    default:
                        return new S88Object(selectedType);
                }
            }
            else
            {
                return treeItem;
            }
        }

        public override bool Delete(object child)
        {
            var treeViewItem = child as ITreeViewItem;
            if(treeViewItem != null && treeObjects.Contains(treeViewItem))
            {
                foreach(var item in treeViewItem.Items)
                {
                    treeViewItem.Delete(item);
                }

                if(treeViewItem.Items.Count() == 0)
                {
                    treeObjects.Remove(child as ITreeViewItem);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// Объект мастера проекта.
        /// </summary>
        public TechObject ProcessCellObject
        {
            get
            {
                var masterObject = treeObjects.Where(x => x is ProcessCell)
                    .FirstOrDefault() as ProcessCell;
                if (masterObject != null)
                {
                    return masterObject.ProcessCellObject;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Получить количество аппаратов в проекте
        /// </summary>
        public int UnitsCount
        {
            get
            {
                int unitS88Level = (int)BaseTechObjectManager.ObjectType.Unit;
                var unitsCount = TechObjects
                    .Where(x => x.BaseTechObject != null)
                    .Where(x => x.BaseTechObject.S88Level == unitS88Level)
                    .Count();
                return unitsCount;
            }
        }

        /// <summary>
        /// Получить количество агрегатов в проекте
        /// </summary>
        public int EquipmentModulesCount
        {
            get
            {
                int aggregateS88Level = (int)BaseTechObjectManager.ObjectType
                    .Aggregate;
                var aggregatesCount = TechObjects
                    .Where(x => x.BaseTechObject != null)
                    .Where(x => x.BaseTechObject.S88Level == aggregateS88Level)
                    .Count();
                return aggregatesCount;
            }
        }

        /// <summary>
        /// Имя проекта.
        /// </summary>
        private string ProjectName { get;set; }

        /// <summary>
        /// Список всех технологических объектов в дереве.
        /// </summary>
        public List<TechObject> TechObjects
        {
            get
            {
                return techObjects;
            }
        }

        /// <summary>
        /// Единственный объект менеджера объектов.
        /// </summary>
        private static TechObjectManager instance;

        /// <summary>
        /// Список объектов дерева.
        /// </summary>
        private List<ITreeViewItem> treeObjects;

        /// <summary>
        /// Список всех технологических объектов в дереве.
        /// </summary>
        private List<TechObject> techObjects;

        /// <summary>
        /// Экземпляр LUA.
        /// </summary>
        private Lua lua;

        /// <summary>
        /// Класс для проверки технологических объектов.
        /// </summary>
        private TechObjectChecker techObjectManagerChecker;

        /// <summary>
        /// Класс, генерирующий информацию для базы каналов по объектам
        /// </summary>
        private TechObjectXMLMaker techObjectXMLMaker;
    }
}