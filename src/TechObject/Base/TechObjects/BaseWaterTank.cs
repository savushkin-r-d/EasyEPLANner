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
            Name = "Бачок";
            EplanName = "_tank";
            BaseOperations = DataBase.Imitation.WaterTankOperations();
            BaseProperties = DataBase.Imitation.WaterTankProperties();
            BasicName = "cooler";
            Equipment = DataBase.Imitation.WaterTankEquipment();
            AggregateProperties = DataBase.Imitation.EmptyAggregateProperties();
        }

        /// <summary>
        /// Клонировать объект
        /// </summary>
        /// <param name="techObject">Новый владелец базового объекта</param>
        /// <returns></returns>
        public override BaseTechObject Clone(TechObject techObject)
        {
            var cloned = DataBase.Imitation.BaseTechObjects()
                .Where(x => x.Name == this.Name)
                .FirstOrDefault();
            cloned.Owner = techObject;
            return cloned;
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
            res += SaveOperationsParameters(objName);
            res += SaveEquipment(objName);

            return res;
        }

        /// <summary>
        /// Сохранить параметры операций
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <returns></returns>
        public string SaveOperationsParameters(string objName)
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
                foreach (BaseProperty param in baseOperation.Properties)
                {
                    if (param.CanSave())
                    {
                        string val = param.Value == "" ? "nil" : param.Value;
                        res += $"{objName}.{baseOperation.LuaName}" +
                            $".{param.LuaName} = {val}\n";
                    }
                }
            }

            return res;
        }
        #endregion
    }
}
