using EasyEPlanner;
using NewEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LuaInterface;

namespace NewTechObject
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

            objects = new List<ITreeViewItem>();
            techObjectsList = new List<TechObject>();
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
            if (techObjectsList != null && techObjectsList.Count >= i)
            {
                return techObjectsList[i - 1];
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
            return techObjectsList.IndexOf(techObject as TechObject) + 1;
        }

        /// <summary>
        /// Получить номер объекта по его отображаемому имени в дереве.
        /// </summary>
        /// <param name="displayText">Отображаемый текст</param>
        /// <returns></returns>
        public int GetTechObjectN(string displayText)
        {
            TechObject findedObject = Objects
                .Where(x => x.DisplayText[0] == displayText).FirstOrDefault();

            if(findedObject != null)
            {
                return techObjectsList.IndexOf(findedObject) + 1;
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
            foreach (TechObject to in Objects)
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
            foreach (TechObject obj in Objects)
            {
                int num = Objects.IndexOf(obj) + 1;
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
            foreach (TechObject obj in Objects)
            {
                int num = Objects.IndexOf(obj) + 1;
                res += obj.SaveRestrictionAsLua(prefix + "\t", num);
            }
            res = res.Replace("\t", "    ");
            return res;
        }

        #region загрузка описания из LUA
        /// <summary>
        /// Загрузка описания проекта из строки
        /// </summary>
        /// <param name="LuaStr">Строка с описанием</param>
        /// <param name="projectName">Имя проекта</param>
        public void LoadDescription(string LuaStr, string projectName)
        {
            ProjectName = projectName;
            objects.Clear();
            techObjectsList.Clear();

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

        #region добавление объекта из LUA
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

            if (baseTechObject != null)
            {
                AddIdentifiedObjectWhenLoadFromLua(obj);
            }
            else
            {
                AddUnidentifiedObjectWhenLoadFromLua(obj);
            }

            techObjectsList.Add(obj);

            return obj;
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
                        .GetS88NameFromLevel(baseTechObject.S88Level);
            switch (type)
            {
                case BaseTechObjectManager.ObjectType.Master:
                     AddMasterFromLua(obj);
                    break;

                case BaseTechObjectManager.ObjectType.Unit:
                    
                    AddS88ObjectFromLua(obj, name);
                    break;

                case BaseTechObjectManager.ObjectType.Aggregate:
                    AddS88ObjectFromLua(obj, name);
                    break;
            }
        }

        /// <summary>
        /// Добавить Мастер из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <returns></returns>
        private void AddMasterFromLua(TechObject obj)
        {
            var masterItem = objects.Where(x => x is Master)
                        .FirstOrDefault() as Master;
            if (masterItem == null)
            {
                masterItem = new Master();
                objects.Add(masterItem);
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
            var s88Item = objects
                .Where(x => x is S88Object && x.DisplayText[0].Contains(name))
                .FirstOrDefault() as S88Object;
            if (s88Item == null)
            {
                s88Item = new S88Object(name);
                objects.Add(s88Item);
            }

            s88Item.AddObjectWhenLoadFromLua(obj);
        }

        /// <summary>
        /// Добавить неопознанный объект при добавлении из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        private void AddUnidentifiedObjectWhenLoadFromLua(TechObject obj)
        {
            var unidentifiedObject = objects
                .Where(x => x is Unidentified)
                .FirstOrDefault() as Unidentified;
            if (unidentifiedObject == null)
            {
                unidentifiedObject = new Unidentified();
                objects.Add(unidentifiedObject);
            }

            unidentifiedObject.AddUnidentifiedObject(obj);
        }
        #endregion

        #region реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                string res = "\"" + ProjectName + "\"";
                if (objects.Count > 0)
                {
                    res += " (" + objects.Count + ")";
                }

                return new string[] { res, "" };
            }
        }

        public override ITreeViewItem[] Items
        {
            get
            {
                return objects.ToArray();
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
                if (!objects.Contains(treeItem))
                {
                    objects.Add(treeItem);
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
            ITreeViewItem treeItem = objects
                .Where(x => x.DisplayText[0].Contains(selectedType))
                .FirstOrDefault();
            if (treeItem == null)
            {
                if (selectedType == "Мастер")
                {
                    return new Master();
                }
                else
                {
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
            if(treeViewItem != null && objects.Contains(treeViewItem))
            {
                objects.Remove(child as ITreeViewItem);
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
        public TechObject Master
        {
            get
            {
                var masterObject = objects.Where(x => x is Master)
                    .FirstOrDefault() as Master;
                if (masterObject != null)
                {
                    return masterObject.MasterObject;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Имя проекта.
        /// </summary>
        private string ProjectName { get;set; }

        /// <summary>
        /// Список всех технологических объектов в дереве.
        /// </summary>
        public List<TechObject> Objects
        {
            get
            {
                return techObjectsList;
            }
        }

        /// <summary>
        /// Единственный объект менеджера объектов.
        /// </summary>
        private static TechObjectManager instance;

        /// <summary>
        /// Список объектов дерева.
        /// </summary>
        private List<ITreeViewItem> objects;

        /// <summary>
        /// Список всех технологических объектов в дереве.
        /// </summary>
        private List<TechObject> techObjectsList;

        /// <summary>
        /// Экземпляр LUA.
        /// </summary>
        private Lua lua;
    }
}