using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using StaticHelper;

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
            string pathToFile = Path.Combine(CommonConst.systemFilesPath, 
                fileName);
            lua.DoFile(pathToFile);
        }

        /// <summary>
        /// Загрузка описание базовых объектов
        /// </summary>
        /// <returns>Описание</returns>
        private string LoadBaseTechObjectsDescription()
        {
            var fileName = "sys_base_objects_description.lua";
            var pathToFile = Path.Combine(CommonConst.systemFilesPath, 
                fileName);
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
        /// <returns></returns>
        public BaseTechObject AddBaseObject(string name, string eplanName,
            int s88Level, string basicName, string bindingName)
        {
            if (baseTechObjects.Count == 0)
            {
                // Пустой объект, если не должно быть выбрано никаких объектов
                baseTechObjects.Add(BaseTechObject.EmptyBaseTechObject());
            }

            var obj = BaseTechObject.EmptyBaseTechObject();
            obj.Name = name;
            obj.EplanName = eplanName;
            obj.S88Level = s88Level;
            obj.BasicName = basicName;
            obj.BindingName = bindingName;

            baseTechObjects.Add(obj);
            return obj;
        }

        /// <summary>
        /// Базовые технологические объекты
        /// </summary>
        public List<BaseTechObject> GetBaseTechObjects()
        {
            var objects = new List<BaseTechObject>();
            foreach (var obj in baseTechObjects)
            {
                objects.Add(obj.Clone());
            }
            return objects;
        }

        private List<BaseTechObject> baseTechObjects;
        private static BaseTechObjectManager baseTechObjectManager;
        private Lua lua;
    }
}
