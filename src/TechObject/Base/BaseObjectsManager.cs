using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            string assemblyPath = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
            string systemFilesPath = assemblyPath + "\\Lua";
            string baseObjectsInitializerLuaFile = Path
                .Combine(systemFilesPath, "sys_base_objects_initializer.lua");
            lua.DoFile(baseObjectsInitializerLuaFile);

            var luaDescriptionPath = Path.Combine(systemFilesPath, 
                "sys_base_objects_description.lua");
            if (!File.Exists(luaDescriptionPath))
            {
                string template = EasyEPlanner.Properties.Resources
                    .ResourceManager
                    .GetString("SysBaseObjectsDescriptionPattern");
                File.WriteAllText(luaDescriptionPath, template);
            }

            var reader = new StreamReader(luaDescriptionPath,
                Encoding.GetEncoding("UTF-8"));

            string readedDescription = reader.ReadToEnd();
            LoadBaseObjectsFromLua(readedDescription);
        }

        /// <summary>
        /// Загрузка базовых объектов из LUA.
        /// </summary>
        /// <param name="LuaStr">Скрипт с данными</param>
        public void LoadBaseObjectsFromLua(string LuaStr)
        {
            lua.DoString("base_tech_objects = nil");
            try
            {
                lua.DoString(LuaStr);
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
            int s88Level, string basicName)
        {
            if (BaseTechObjects.Count == 0)
            {
                // Пустой объект, если не должно быть выбрано никаких объектов
                BaseTechObjects.Add(BaseTechObject.EmptyBaseTechObject());
            }

            var obj = BaseTechObject.EmptyBaseTechObject();
            obj.Name = name;
            obj.EplanName = eplanName;
            obj.S88Level = s88Level;
            obj.BasicName = basicName;

            BaseTechObjects.Add(obj);
            return obj;
        }

        /// <summary>
        /// Базовые технологические объекты
        /// </summary>
        public List<BaseTechObject> BaseTechObjects
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

        private List<BaseTechObject> baseTechObjects;
        private static BaseTechObjectManager baseTechObjectManager;
        private Lua lua;
    }
}
