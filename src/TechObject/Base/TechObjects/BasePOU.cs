using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый пастеризатор
    /// </summary>
    public class BasePOU : BaseTechObject
    {
        public BasePOU() : base()
        {
            S88Level = 2;
            Name = "Пастеризатор";
            EplanName = "pasteurizator";
            BaseOperations = DataBase.Imitation.POUOperations();
            BaseProperties = DataBase.Imitation.POUProperties();
            BasicName = "pasteurizator";
            Equipment = DataBase.Imitation.EmptyEquipment();
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
                switch (baseOperation.Name)
                {
                    case "Мойка":
                        res += SaveWashOperationParameters(objName,
                            baseOperation);
                        break;

                    default:
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
                                string val = param
                                    .Value == "" ? "nil" : param.Value;
                                res += $"{prefix}{param.LuaName} = {val},\n";
                            }
                        }
                        res += prefix + "}\n";
                        break;
                }
            }

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

            res += "\n";
            return res;
        }
        #endregion
    }
}
