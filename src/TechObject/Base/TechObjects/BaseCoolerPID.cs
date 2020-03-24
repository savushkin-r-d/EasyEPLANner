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
    public class BaseCoolerPID : BaseTechObject
    {
        public BaseCoolerPID() : base()
        {
            S88Level = 2;
            Name = "Узел охлаждения ПИД";
            EplanName = "cooler_node_PID";
            BaseOperations = DataBase.Imitation.CoolerNodePIDOperations();
            BaseProperties = DataBase.Imitation.CoolerPIDProperties();
            BasicName = "cooler_node_PID";
            Equipment = DataBase.Imitation.CoolerNodePIDEquipment();
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
            res += SaveObjectInfoToPrgLua(objName, prefix);

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

        /// <summary>
        /// Сохранить информацию об объекте в prg.lua
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        public string SaveObjectInfoToPrgLua(string objName, 
            string prefix)
        {
            var res = "";
            res += objName + $".PID_n = {Owner.GlobalNumber}\n";
            return res;
        }
        #endregion
    }
}
