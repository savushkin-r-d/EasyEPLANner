using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый бачок
    /// </summary>
    public class BaseWaterTank : BaseTechObject
    {
        public BaseWaterTank() : base()
        {
            S88Level = 2;
            Name = "Бачок откачки лёдводы";
            EplanName = "_tank";
            BaseOperations = DataBase.Imitation.WaterTankOperations();
            BaseProperties = DataBase.Imitation.WaterTankProperties();
            BasicName = "cooler_tank";
            Equipment = DataBase.Imitation.WaterTankEquipment();
            AggregateProperties = DataBase.Imitation.EmptyAggregateProperties();
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
            res += SaveOperationsParameters(objName, prefix);
            res += SaveEquipment(objName);

            return res;
        }

        /// <summary>
        /// Сохранить параметры операций
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        public string SaveOperationsParameters(string objName, string prefix)
        {
            var res = "";

            var modesManager = this.Owner.ModesManager;
            var modes = modesManager.Modes;
            if (modes.Where(x => x.DisplayText[1] != "").Count() == 0)
            {
                return res;
            }

            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Properties.Count < 1)
                {
                    continue;
                }

                res += $"{objName}.{baseOperation.LuaName} =\n";
                res += prefix + "{\n";
                foreach (BaseProperty param in baseOperation.Properties)
                {
                    if (param.CanSave())
                    {
                        string val = param.Value == "" ? "nil" : param.Value;
                        res += $"{prefix}{param.LuaName} = {val},\n";
                    }
                }
                res += prefix + "}\n";
            }

            return res;
        }
        #endregion
    }
}
