using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TechObject;
using EasyEPlanner;

namespace Editor
{
    class TechObjectsImporter
    {
        /// <summary>
        /// Singleton
        /// </summary>
        /// <returns></returns>
        public static TechObjectsImporter GetInstance()
        {
            if (techObjectsImporter == null)
            {
                techObjectsImporter = new TechObjectsImporter();
            }
            return techObjectsImporter;
        }

        /// <summary>
        /// Приватный конструктор
        /// </summary>
        private TechObjectsImporter() 
        {
            importedObjects = new Dictionary<int, ITreeViewItem>();
            techObjectManager = TechObjectManager.GetInstance();
            objectTree = new List<ITreeViewItem>();
            InitLuaScripts();
        }

        private void InitLuaScripts()
        {
            lua = new Lua();
            lua.RegisterFunction("ADD_TECH_OBJECT", this,
                GetType().GetMethod("LoadObjects"));
            lua.RegisterFunction("Get_TECH_OBJECT", this,
                GetType().GetMethod("GetImportedObjectImportRestrictions"));
            try
            {
                LoadScriptsForImport();
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки скриптов для импорта");
            }
        }

        /// <summary>
        /// Загрузить скрипты для импорта объектов.
        /// </summary>
        private void LoadScriptsForImport()
        {
            const string descriptionFileName = "sys.lua";
            string sysLuaPath = Path
                .Combine(ProjectManager.GetInstance().SystemFilesPath, 
                descriptionFileName);
            lua.DoFile(sysLuaPath);

            string restrictionsFileName = "sys_restriction.lua";
            string pathToRestrictionInitializer = Path
                .Combine(ProjectManager.GetInstance().SystemFilesPath, 
                restrictionsFileName);
            lua.DoFile(pathToRestrictionInitializer);
        }

        /// <summary>
        /// Загрузить объекты для импорта из LUA файла.
        /// </summary>
        /// <param name="pathToFile"></param>
        public void LoadImportingObjects(string pathToFile)
        {
            importedObjects.Clear();
            objectTree.Clear();

            var sr = new StreamReader(pathToFile);
            string dataFromFile = sr.ReadToEnd();
            sr.Close();

            try
            {
                lua.DoString(dataFromFile);
                lua.DoString("if init ~= nil then init() end");
                lua.DoString("init_restriction()");
            }
            catch
            {
                string message = "Ошибка при загрузке объектов из " +
                    "импортируемого файла.";
                throw new Exception(message);
            }
        }

        #region импорт объектов
        /// <summary>
        /// Импорт объекта, вызывается из LUA.
        /// </summary>
        /// <param name="techN">Номер объекта</param>
        /// <param name="name">Имя</param>
        /// <param name="techType">Тип</param>
        /// <param name="nameEplan">Имя Eplan</param>
        /// <param name="cooperParamNumber">Время совместного перехода</param>
        /// <param name="NameBC">Имя монитор</param>
        /// <param name="baseTechObjectName">Базовый объект</param>
        /// <param name="attachedObjects">Привязанные агрегаты</param>
        public TechObject.TechObject LoadObjects(int globalNumber, int techN, 
            string name, int techType, string nameEplan, int cooperParamNumber, 
            string NameBC, string baseTechObjectName, string attachedObjects)
        {
            var baseTechObject = BaseTechObjectManager.GetInstance()
                .GetTechObject(baseTechObjectName);

            TechObject.TechObject obj = new TechObject.TechObject(name, 
                null, techN, techType, nameEplan.ToUpper(), 
                cooperParamNumber, NameBC, attachedObjects, baseTechObject);

            if (baseTechObject != null)
            {
                AddIdentifiedObjectWhenLoadFromLua(obj);
            }
            else
            {
                AddUnidentifiedObjectWhenLoadFromLua(obj);
            }

            importedObjects.Add(globalNumber, obj);

            return obj;
        }

        /// <summary>
        /// Добавить опознанный объект при загрузке из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        private void AddIdentifiedObjectWhenLoadFromLua(
            TechObject.TechObject obj)
        {
            BaseTechObject baseTechObject = obj.BaseTechObject;
            var type = (BaseTechObjectManager.ObjectType)baseTechObject
                .S88Level;
            string name = BaseTechObjectManager.GetInstance()
                        .GetS88NameFromLevel(baseTechObject.S88Level);
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
            }
        }

        /// <summary>
        /// Добавить ячейку процесса (мастер) из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <returns></returns>
        private void AddProcessCellFromLua(TechObject.TechObject obj)
        {
            var processCellItem = objectTree.Where(x => x is ProcessCell)
                        .FirstOrDefault() as ProcessCell;
            if (processCellItem == null)
            {
                processCellItem = new ProcessCell();
                objectTree.Add(processCellItem);
            }

            processCellItem.AddObjectWhenLoadFromLua(obj);
        }

        /// <summary>
        /// Добавить аппарат из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="name">Имя объекта</param>
        /// <returns></returns>
        private void AddS88ObjectFromLua(TechObject.TechObject obj,
            string name)
        {
            var s88Item = objectTree
                .Where(x => x is S88Object && x.DisplayText[0].Contains(name))
                .FirstOrDefault() as S88Object;
            if (s88Item == null)
            {
                s88Item = new S88Object(name);
                objectTree.Add(s88Item);
            }

            s88Item.AddObjectWhenLoadFromLua(obj);
        }

        /// <summary>
        /// Добавить неопознанный объект при добавлении из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        private void AddUnidentifiedObjectWhenLoadFromLua(
            TechObject.TechObject obj)
        {
            var unidentifiedObject = objectTree
                .Where(x => x is Unidentified)
                .FirstOrDefault() as Unidentified;
            if (unidentifiedObject == null)
            {
                unidentifiedObject = new Unidentified();
                objectTree.Add(unidentifiedObject);
            }

            unidentifiedObject.AddUnidentifiedObject(obj);
        }
        #endregion


        /// <summary>
        /// Получить импортированный объект для импорта ограничений,
        /// вызывается из LUA.
        /// </summary>
        /// <returns></returns>
        public TechObject.TechObject GetImportedObjectImportRestrictions(
            int objectN)
        {
            return importedObjects[objectN] as TechObject.TechObject;
        }

        /// <summary>
        /// Импортировать объекты в редактор
        /// </summary>
        /// <param name="checkedItems">Выбранные на дереве объекты</param>
        public void Import(List<ITreeViewItem> checkedItems)
        {
            foreach(var item in checkedItems)
            {
                techObjectManager
                    .ImportObject(item as TechObject.TechObject);
            }

            (techObjectManager as ITreeViewItem).AddParent(null);
        }

        public ITreeViewItem[] RootItems
        {
            get
            {
                return objectTree.ToArray();
            }
        } 

        private List<ITreeViewItem> objectTree;
        private Dictionary<int, ITreeViewItem> importedObjects;
        private Lua lua;
        private static TechObjectsImporter techObjectsImporter;
        private ITechObjectManager techObjectManager;
    }
}
