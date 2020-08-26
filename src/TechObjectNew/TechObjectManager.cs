using EasyEPlanner;
using NewEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            TechObject findedObject = TechObjectsList
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
            foreach (TechObject to in TechObjectsList)
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
            foreach (TechObject obj in TechObjectsList)
            {
                int num = TechObjectsList.IndexOf(obj) + 1;
                res += obj.SaveAsLuaTable(prefix + "\t\t", num);
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
            const int masterLevel = 0;
            const int unitLevel = 1;
            const int aggregateLevel = 2;

            BaseTechObject baseTechObject = obj.BaseTechObject;
            switch(baseTechObject.S88Level)
            {
                case masterLevel:
                     AddMasterFromLua(obj);
                    break;

                case unitLevel:
                    AddUnitFromLua(obj);
                    break;

                case aggregateLevel:
                    AddAggregateFromLua(obj);
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
        /// <returns></returns>
        private void AddUnitFromLua(TechObject obj)
        {
            var unitItem = objects.Where(x => x is Unit)
                        .FirstOrDefault() as Unit;
            if (unitItem == null)
            {
                unitItem = new Unit();
                objects.Add(unitItem);
            }

            unitItem.AddObjectWhenLoadFromLua(obj);
        }

        /// <summary>
        /// Добавить агрегат из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <returns></returns>
        private void AddAggregateFromLua(TechObject obj)
        {
            var aggregateItem = objects.Where(x => x is Aggregate)
                        .FirstOrDefault() as Aggregate;
            if (aggregateItem == null)
            {
                aggregateItem = new Aggregate();
                objects.Add(aggregateItem);
            }

            aggregateItem.AddObjectWhenLoadFromLua(obj);
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

        public override bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        public override ITreeViewItem Insert()
        {
            var objectsAdderForm = new ObjectsAdder();
            objectsAdderForm.ShowDialog();
            string selectedType = ObjectsAdder.LastSelectedType;
            string selectedSubType = ObjectsAdder.LastSelectedSubType;
            if(selectedType != null && selectedSubType != null)
            {
                var treeItem = GetTreeItem(selectedType);
                var innerItem = treeItem.Insert();
                if(innerItem != null)
                {
                    if(!objects.Contains(treeItem))
                    {
                        objects.Add(treeItem);
                    }

                    return treeItem;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Создать объект дерева, описывающий базу по S88.
        /// </summary>
        /// <param name="selectedType">Выбранный на форме тип объекта</param>
        /// <returns></returns>
        private ITreeViewItem GetTreeItem(string selectedType)
        {
            var treeItem = objects
                .Where(x => x.DisplayText[0].Contains(selectedType))
                .FirstOrDefault();
            if (treeItem == null)
            {
                switch (selectedType)
                {
                    case "Мастер":
                        return new Master();

                    case "Аппарат":
                        return new Unit();

                    case "Агрегат":
                        return new Aggregate();

                    default:
                        return null;
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
        public List<TechObject> TechObjectsList
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