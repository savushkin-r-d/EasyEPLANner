using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый узел охлаждения
    /// </summary>
    public class BaseCooler : BaseTechObject
    {
        public BaseCooler() : base()
        {
            S88Level = 2;
            Name = "Узел охлаждения";
            EplanName = "cooler_node";
            BaseOperations = DataBase.Imitation.CoolerNodeOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "cooler_node";
            Equipment = DataBase.Imitation.CoolerNodeEquipment();
            AggregateProperties = DataBase.Imitation
                .CoolerNodeAggregateProperties();
        }

        /// <summary>
        /// Можно ли привязывать данный объект к другим объектам.
        /// </summary>
        public override bool IsAttachable
        {
            get
            {
                return true;
            }
        }

        #region сохранение prg.lua
        /// <summary>
        /// Сохранить информацию об операциях объекта в prg.lua
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        public override string SaveToPrgLua(string objName,
            string prefix)
        {
            var res = "";

            res += SaveOperations(objName, prefix);
            res += SaveOperationsSteps(objName, prefix);
            res += SaveEquipment(objName);

            return res;
        }
        #endregion
    }
}
