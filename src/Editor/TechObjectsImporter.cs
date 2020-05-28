using Editor;
using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TechObject;

namespace EasyEPlanner
{
    class TechObjectsImporter
    {
        /// <summary>
        /// Приватный конструктор
        /// </summary>
        private TechObjectsImporter() 
        {
            importedObjects = new Dictionary<int, ITreeViewItem>();

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
            TechObject.TechObject obj = new TechObject.TechObject(name, 
                GetTechObjectN, techN, techType, nameEplan.ToUpper(), 
                cooperParamNumber, NameBC, attachedObjects);
            
            importedObjects.Add(globalNumber, obj);

            obj.SetNewValue(baseTechObjectName, true);

            return obj;
        }

        /// <summary>
        /// Получение номера объекта
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <returns></returns>
        private int GetTechObjectN(object obj)
        {
            return importedObjects.Where(x => x.Value == obj).Select(x => x.Key)
                .First();
        }

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
        public void Import(List<int> checkedItems)
        {
            ITreeViewItem techObjectsManager = TechObjectManager.GetInstance();
            foreach(var num in checkedItems)
            {
                var obj = importedObjects[num] as TechObject.TechObject;
                // Убираем привязку при импорте.
                obj.AttachedObjects.SetValue("");
                techObjectsManager.InsertCopy(obj);
            }
        }

        /// <summary>
        /// Список импортированных объектов
        /// </summary>
        public string[] ImportedObjectsNamesArray
        {
            get
            {
                if (importedObjects.Count != 0)
                {
                    return importedObjects.Select(x => x.Value.DisplayText[0])
                        .ToArray();
                }
                else
                {
                    return new string[0];
                }
            }
        }

        private Dictionary<int, ITreeViewItem> importedObjects;
        private Lua lua;
        private static TechObjectsImporter techObjectsImporter;
    }
}
