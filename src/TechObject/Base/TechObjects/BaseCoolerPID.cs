﻿using System;
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
            BaseProperties = DataBase.Imitation.EmptyProperties();
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

            res += SaveOperations(objName, prefix);

            return res;
        }

        /// <summary>
        /// Сохранить операцию охлаждения.
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <param name="objName">Имя объекта</param>
        /// <returns></returns>
        private string SaveOperations(string objName, string prefix)
        {
            var res = "";

            var modesManager = this.Owner.ModesManager;
            var modes = modesManager.Modes;
            if (modes.Where(x => x.DisplayText[1] != "").Count() == 0)
            {
                return res;
            }

            res += objName + ".operations = \t\t--Операции.\n";
            res += prefix + "{\n";
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.GetBaseOperation();
                if (baseOperation.Name != "")
                {
                    res += prefix + baseOperation.LuaName.ToUpper() + " = " +
                        mode.GetModeNumber() + ",\n";
                }
            }
            res += prefix + "}\n";

            return res;
        }
        #endregion
    }
}
