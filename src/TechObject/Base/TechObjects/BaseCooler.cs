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
        public override string SaveOperationsToPrgLua(string objName,
            string prefix)
        {
            var res = "";

            var modesManager = this.Owner.ModesManager;
            var modes = modesManager.Modes;
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.GetBaseOperation();
                switch (baseOperation.Name)
                {
                    case "Охлаждение":
                        res += SaveCoolingOperation(prefix, objName, mode,
                            baseOperation);
                        break;
                }
            }

            res += "\n";
            return res;
        }

        /// <summary>
        /// Сохранить операцию охлаждения.
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <param name="objName">Имя объекта</param>
        /// <param name="mode">Операция</param>
        /// <param name="baseOperation">Базовая операция</param>
        /// <returns></returns>
        private string SaveCoolingOperation(string prefix, string objName,
            Mode mode, BaseOperation baseOperation)
        {
            var res = "";

            res += objName + ".operations = \t\t--Операции.\n";
            res += prefix + "{\n";
            res += prefix + baseOperation.LuaName.ToUpper() + " = " + 
                mode.GetModeNumber() + ",\t\t--Охлаждение.\n";
            res += prefix + "}\n";

            return res;
        }
        #endregion
    }
}
