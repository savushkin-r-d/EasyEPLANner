using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
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

            InitBaseTechObjectsInitializer();
            string description = LoadBaseTechObjectsDescription();
            InitBaseObjectsFromLua(description);
        }

        /// <summary>
        /// Инициализация читателя базовых объектов.
        /// </summary>
        private void InitBaseTechObjectsInitializer()
        {
            string fileName = "sys_base_objects_initializer.lua";
            string pathToFile = Path.Combine(
                ProjectManager.GetInstance().SystemFilesPath, fileName);
            lua.DoFile(pathToFile);
        }

        /// <summary>
        /// Загрузка описание базовых объектов
        /// </summary>
        /// <returns>Описание</returns>
        private string LoadBaseTechObjectsDescription()
        {
            var fileName = "sys_base_objects_description.lua";
            var pathToFile = Path.Combine(
                ProjectManager.GetInstance().SystemFilesPath, fileName);
            if (!File.Exists(pathToFile))
            {
                string template = EasyEPlanner.Properties.Resources
                    .ResourceManager
                    .GetString("SysBaseObjectsDescriptionPattern");
                File.WriteAllText(pathToFile, template);
                MessageBox.Show("Файл с описанием базовых объектов не найден." +
                    " Будет создан пустой файл (без описания).", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var reader = new StreamReader(pathToFile,
                Encoding.GetEncoding("UTF-8"));
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
            var obj = BaseTechObject.EmptyBaseTechObject();
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
        public string GetS88NameFromLevel(int s88Level)
        {
            switch(s88Level)
            {
                case (int)ObjectType.ProcessCell:
                    return "Ячейка процесса";

                case (int)ObjectType.Unit:
                    return "Аппарат";

                case (int)ObjectType.Aggregate:
                    return "Агрегат";

                default:
                    return null;
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

        /// <summary>
        /// Получить базовый объект ячейки процесса проекта.
        /// </summary>
        public BaseTechObject ProcessCell
        {
            get
            {
                return Objects
                    .Where(x => x.S88Level == (int)ObjectType.ProcessCell)
                    .First();
            }
        }

        /// <summary>
        /// Получить аппараты.
        /// </summary>
        public List<BaseTechObject> Units
        {
            get
            {
                return Objects.
                    Where(x => x.S88Level == (int)ObjectType.Unit)
                    .ToList();
            }
        }

        /// <summary>
        /// Получить агрегаты.
        /// </summary>
        public List<BaseTechObject> Aggregates
        {
            get
            {
                return Objects
                    .Where(x => x.S88Level == (int)ObjectType.Aggregate)
                    .ToList();
            }
        }

        public enum ObjectType
        {
            ProcessCell = 0,
            Unit = 1,
            Aggregate = 2
        }

        private List<BaseTechObject> baseTechObjects;
        private static BaseTechObjectManager baseTechObjectManager;
        private Lua lua;
    }
}
