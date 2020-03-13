using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовая линия выдачи
    /// </summary>
    public class BaseLineOut : BaseTechObject
    {
        public BaseLineOut() : base()
        {
            S88Level = 2;
            Name = "Линия выдачи";
            EplanName = "line_out";
            BaseOperations = DataBase.Imitation.LineOperations();
            BaseProperties = DataBase.Imitation.LineProperties();
            BasicName = "line";
            Equipment = DataBase.Imitation.EmptyEquipment();
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

        #region Сохранение в prg.lua
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
                    case "Мойка":
                        res += SaveWashOperation(prefix, objName, mode,
                            baseOperation);
                        break;
                }
            }
            return res;
        }

        /// <summary>
        /// Сохранить операцию мойки в prg.lua
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <param name="objName">Имя объекта</param>
        /// <param name="mode">Операция</param>
        /// <param name="baseOperation">Базовая операция</param>
        /// <returns></returns>
        private string SaveWashOperation(string prefix, string objName,
            Mode mode, BaseOperation baseOperation)
        {
            var res = "";

            res += objName + ".operations = \t\t--Операции.\n";
            res += prefix + "{\n";
            res += prefix + baseOperation.LuaName
                .ToUpper() + " = " + mode.GetModeNumber() +
                ",\t\t--Мойка CIP.\n";
            res += prefix + "}\n";

            res += objName + ".steps = \t\t--Шаги операций.\n";
            res += prefix + "{\n";
            var containsDrainage = mode.stepsMngr[0].steps
                .Where(x => x.GetStepName()
                .Contains("Дренаж")).FirstOrDefault();

            if (containsDrainage != null)
            {
                res += prefix + baseOperation.LuaName
                .ToUpper() + " =\n";
                res += prefix + prefix + "{\n";
                res += prefix + prefix + "DRAINAGE = " +
                    mode.stepsMngr[0].steps.Where(x => x
                    .GetStepName().Contains("Дренаж"))
                    .FirstOrDefault()
                    .GetStepNumber() + ",\n";
                res += prefix + prefix + "}\n";
            }
            else
            {
                res += prefix + baseOperation.LuaName
                    .ToUpper() + " = { },\n";
            }

            res += prefix + "}\n";
            res += SaveWashOperationParameters(objName, baseOperation);
            res += "\n";

            return res;
        }

        /// <summary>
        /// Сохранить параметры операции мойки
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="baseOperation">Базовая операция</param>
        /// <returns></returns>
        private string SaveWashOperationParameters(string objName,
            BaseOperation baseOperation)
        {
            var res = "";

            foreach (BaseProperty param in baseOperation.Properties)
            {
                if (param.CanSave())
                {
                    string val = param.Value == "" ? "nil" : param.Value;
                    if (val != "nil")
                    {
                        res += objName + "." + param.LuaName +
                            " = prg.control_modules." + val + "\n";
                    }
                    else
                    {
                        res += objName + "." + param.LuaName +
                            " = " + val + "\n";
                    }
                }
            }

            return res;
        }
        #endregion
    }
}
