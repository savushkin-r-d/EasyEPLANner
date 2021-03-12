using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TechObject
{
    public interface IBaseTechObjectManager
    {
        BaseTechObject AddBaseObject(string name, string eplanName,
            int s88Level, string basicName, string bindingName, bool isPID);

        BaseTechObject GetTechObject(string name);

        string GetS88Name(int s88Level);

        int GetS88Level(string type);

        List<BaseTechObject> Objects { get; }
    }

    /// <summary>
    /// Менеджер базовых технологических объектов
    /// </summary>
    public class BaseTechObjectManager : IBaseTechObjectManager
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
        }

        /// <summary>
        /// Единственный экземпляр класса (Singleton)
        /// </summary>
        /// <returns></returns>
        public static IBaseTechObjectManager GetInstance()
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

            bool noEquals = baseTechObjects.Where(x => x.Name == obj.Name ||
                x.EplanName == obj.EplanName).Count() == 0;
            if (noEquals)
            {
                baseTechObjects.Add(obj);
            }

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

        private List<BaseTechObject> baseTechObjects;
        private static IBaseTechObjectManager baseTechObjectManager;
    }
}
