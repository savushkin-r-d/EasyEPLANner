using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый бойлер
    /// </summary>
    public class BaseBoiler : BaseTechObject
    {
        public BaseBoiler() : base()
        {
            S88Level = 2;
            Name = "Бойлер";
            EplanName = "boil";
            BaseOperations = DataBase.Imitation.BoilerOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "boiler";
            Equipment = DataBase.Imitation.EmptyEquipment();
            AggregateProperties = DataBase.Imitation
                .BoilerAggregateProperties();
        }

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
