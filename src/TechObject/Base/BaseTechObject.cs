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
            get { return baseOperations; }
        }

        private string name; // Отображаемое имя технологического объекта
        private string nameEplan; // ОУ объекта в Eplan
        private int s88Level; // Уровень объекта по S88
        private string basicName; // Базовое имя для функциональности

        // Базовые операции базового объекта
        private BaseOperation[] baseOperations = new BaseOperation[0]; 
    }
}
