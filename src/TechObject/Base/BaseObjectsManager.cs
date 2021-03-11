using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using EasyEPlanner;

namespace TechObject
{
    /// <summary>
    /// Менеджер базовых технологических объектов
    /// </summary>
    public class BaseTechObjectManager
    {
        /// <summary>
        /// Получить базовый технологический объект по обычному названию.
        /// </summary>
        /// <param name="name">Название объекта</param>
        /// <returns></returns>
        public BaseTechObject GetTechObject(string name)
        {
            foreach (var baseTechObject in baseTechObjects)
            {
                if (name == baseTechObject.Name ||
                    name == baseTechObject.EplanName)
                {
                    return baseTechObject.Clone();
                }
            }
            return null;
        }

        private BaseTechObjectManager()
        {
            baseTechObjects = new List<BaseTechObject>();
            lua = new Lua();
            lua.RegisterFunction("AddBaseObject", this, GetType()
                .GetMethod("AddBaseObject"));

            LoadBaseTechObjectsFromDescription();
        }

        /// <summary>
        /// Загрузка базовых объектов из их файлов описания
        /// </summary>
        private void LoadBaseTechObjectsFromDescription()
        {
            string sysFilesPath = ProjectManager.GetInstance()
                .SystemFilesPath;
            InitBaseTechObjectsInitializer(sysFilesPath);

            string pathToDir = Path.Combine(sysFilesPath,
                defaultDirectoryWithDescription);
            IList<string> fileNames = 
                GetDescriptionFilesNamesFromDefaultDirectory(pathToDir);
            if (fileNames.Count > 0)
            {
                foreach(var fileName in fileNames)
                {
                    string descriptionFilePath = Path
                        .Combine(pathToDir, fileName);
                    string description = LoadBaseTechObjectsDescription(
                        descriptionFilePath);
                    InitBaseObjectsFromLua(description);
                }
            }
            else
            {
                WriteDefaultTemplateAndShowError(pathToDir);
            }
        }

        /// <summary>
        /// Показать ошибку о том, что не найдены файлы описания и записать
        /// стандартный файл описания.
        /// </summary>
        /// <param name="pathToDirectory">Путь к каталогу, где хранятся
        /// файлы описания</param>
        private void WriteDefaultTemplateAndShowError(string pathToDirectory)
        {
            string templateDescriptionFilePath = Path.Combine(
                    pathToDirectory, defaultDescriptionFileName);
            string template = EasyEPlanner.Properties.Resources
                .ResourceManager
                .GetString("SysBaseObjectsDescriptionPattern");
            File.WriteAllText(templateDescriptionFilePath, template,
                EncodingDetector.UTF8);
            MessageBox.Show("Файлы с описанием базовых объектов не " +
                "найдены. Будет создан пустой файл с шаблоном описания. " +
                $"Путь: {templateDescriptionFilePath}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Инициализация читателя базовых объектов.
        /// </summary>
        private void InitBaseTechObjectsInitializer(string pathToSystemFiles)
        {
            string fileName = "sys_base_objects_initializer.lua";
            string pathToFile = Path.Combine(pathToSystemFiles, fileName);
            lua.DoFile(pathToFile);
        }

        /// <summary>
        /// Получить имена файлов с описанием объектов
        /// </summary>
        /// <param name="pathToCatalog">Путь к каталогу с файлами</param>
        /// <returns></returns>
        private string[] GetDescriptionFilesNamesFromDefaultDirectory(
            string pathToCatalog)
        {
            if (Directory.Exists(pathToCatalog))
            {
                return Directory.GetFiles(pathToCatalog);
            }
            else
            {
                Directory.CreateDirectory(pathToCatalog);
                return new string[0];
            }
        }

        /// <summary>
        /// Загрузка описание базовых объектов
        /// </summary>
        /// <returns>Описание</returns>
        private string LoadBaseTechObjectsDescription(string pathToFile)
        {
            var reader = new StreamReader(pathToFile,
                EncodingDetector.DetectFileEncoding(pathToFile));
            string readedDescription = reader.ReadToEnd();
            return readedDescription;
        }

        /// <summary>
        /// Загрузка базовых объектов из описания LUA.
        /// </summary>
        /// <param name="luaString">Скрипт с описанием</param>
        private void InitBaseObjectsFromLua(string luaString)
        {
            lua.DoString("base_tech_objects = nil");
            try
            {
                lua.DoString(luaString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ". Исправьте скрипт вручную.",
                    "Ошибка обработки Lua-скрипта с описанием " +
                    "базовых объектов.");
            }
            try
            {
                string script = "if init_base_objects ~= nil " +
                    "then init_base_objects() end";
                lua.DoString(script);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обработки Lua-скрипта с " +
                    " инициализацией базовых объектов: " + ex.Message + ".\n" +
                    "Source: " + ex.Source);
            }

        }

        /// <summary>
        /// Единственный экземпляр класса (Singleton)
        /// </summary>
        /// <returns></returns>
        public static BaseTechObjectManager GetInstance()
        {
            if (baseTechObjectManager == null)
            {
                baseTechObjectManager = new BaseTechObjectManager();
            }
            return baseTechObjectManager;
        }

        /// <summary>
        /// Добавить базовый объект
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="eplanName">Имя в eplan</param>
        /// <param name="s88Level">Уровень по ISA88</param>
        /// <param name="basicName">Базовое имя для функциональности</param>
        /// <param name="bindingName">Имя для привязки к объекту</param>
        /// <param name="isPID">Является ли объект ПИД-регулятором</param>
        /// <returns></returns>
        public BaseTechObject AddBaseObject(string name, string eplanName,
            int s88Level, string basicName, string bindingName, bool isPID)
        {
            var obj = new BaseTechObject();
            obj.Name = name;
            obj.EplanName = eplanName;
            obj.S88Level = s88Level;
            obj.BasicName = basicName;
            obj.BindingName = bindingName;
            obj.IsPID = isPID;

            baseTechObjects.Add(obj);
            return obj;
        }

        /// <summary>
        /// Получить имя объекта S88 по его уровню
        /// </summary>
        /// <param name="s88Level">Уровень объекта</param>
        /// <returns></returns>
        public string GetS88Name(int s88Level)
        {
            switch(s88Level)
            {
                case (int)ObjectType.ProcessCell:
                    return "Ячейка процесса";

                case (int)ObjectType.Unit:
                    return "Аппарат";

                case (int)ObjectType.Aggregate:
                    return "Агрегат";

                case (int)ObjectType.UserObject:
                    return "Пользовательский объект";

                default:
                    return null;
            }
        }

        /// <summary>
        /// Получить S88 уровень из типа объекта
        /// </summary>
        /// <param name="type">Тип объекта (Аппарат, агрегат и др.)</param>
        /// <returns></returns>
        public int GetS88Level(string type)
        {
            switch (type)
            {
                case "Ячейка процесса":
                    return (int)ObjectType.ProcessCell;

                case "Аппарат":
                    return (int)ObjectType.Unit;

                case "Агрегат":
                    return (int)ObjectType.Aggregate;

                case "Пользовательский объект":
                    return (int)ObjectType.UserObject;

                default:
                    return -1;
            }
        }

        /// <summary>
        /// Базовые технологические объекты
        /// </summary>
        public List<BaseTechObject> Objects
        {
            get
            {
                var objects = new List<BaseTechObject>();
                foreach (var obj in baseTechObjects)
                {
                    objects.Add(obj.Clone());
                }
                return objects;
            }
        }

        public enum ObjectType
        {
            ProcessCell = 0,
            Unit = 1,
            Aggregate = 2,
            UserObject = 3
        }

        const string defaultDescriptionFileName = "DescriptionTemplate.lua";
        const string defaultDirectoryWithDescription = "BaseObjectsDescriptionFiles";
        private List<BaseTechObject> baseTechObjects;
        private static BaseTechObjectManager baseTechObjectManager;
        private Lua lua;
    }
}
