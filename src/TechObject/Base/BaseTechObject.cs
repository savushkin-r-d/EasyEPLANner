using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Класс реализующий базовый аппарат для технологического объекта
    /// </summary>
    public class BaseTechObject
    {
        public BaseTechObject()
        {
            this.name = "";
            this.nameEplan = "";
        }

        public BaseTechObject(TechObject owner)
        {
            this.Owner = owner;
            this.name = "";
            this.nameEplan = "";
        }

        public BaseTechObject(string name, string nameEplan)
        {
            this.name = name;
            this.nameEplan = nameEplan;
        }

        // Конструктор для имитации БД
        public BaseTechObject(string name, string nameEplan, int s88Level, 
            BaseOperation[] baseOperations, string basicName)
        {
            this.name = name;
            this.nameEplan = nameEplan;
            this.s88Level = s88Level;
            this.baseOperations = baseOperations;
            this.basicName = basicName;
        }

        public string GetName()
        {
            return name;
        }

        public string GetNameEplan()
        {
            return nameEplan;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetNameEplan(string nameEplan)
        {
            this.nameEplan = nameEplan;
        }

        public void SetS88Level(int s88Level)
        {
            this.s88Level = s88Level;
        }

        public int GetS88Level()
        {
            return s88Level;
        }

        public string GetBasicName()
        {
            return basicName;
        }

        public BaseOperation[] BaseOperations
        {
            get 
            { 
                return baseOperations; 
            }
            set
            {
                baseOperations = value;
            }
        }

        public string BasicName
        {
            get
            {
                return basicName;
            }
            set
            {
                basicName = value;
            }
        }

        public TechObject Owner
        {
            get
            {
                return techObject;
            }
            set
            {
                techObject = value;
            }
        }

        public BaseTechObject Clone(ModesManager modesManager)
        {
            var baseTechobject = new BaseTechObject();
            baseTechobject.name = this.name;
            baseTechobject.nameEplan = this.nameEplan;
            baseTechobject.basicName = this.basicName;
            baseTechobject.s88Level = this.s88Level;
            baseTechobject.Owner = modesManager.Owner;

            var baseOperations = new List<BaseOperation>();
            foreach(var mode in modesManager.Modes)
            {
                var operation = mode.GetBaseOperation();
                baseOperations.Add(operation);
            }

            baseTechobject.BaseOperations = baseOperations.ToArray();

            return baseTechobject;
        }

        public void ResetBaseOperations()
        {
            foreach (BaseOperation operation in BaseOperations)
            {
                operation.Init("");
            }
        }

        private string name; // Отображаемое имя технологического объекта
        private string nameEplan; // ОУ объекта в Eplan
        private int s88Level; // Уровень объекта по S88
        private string basicName; // Базовое имя для функциональности

        // Базовые операции базового объекта
        private BaseOperation[] baseOperations = new BaseOperation[0];
        // Владелец объекта
        private TechObject techObject;
    }
}
