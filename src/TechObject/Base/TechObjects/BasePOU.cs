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

            res += base.SaveOperations(objName, prefix);
            res += SaveOperationsSteps(objName, prefix);
            res += base.SaveEquipment(objName);

            return res;
        }

        /// <summary>
        /// Сохранить шаги операций объекта.
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        private string SaveOperationsSteps(string objName, string prefix)
        {
            var res = "";

            var modesManager = this.Owner.ModesManager;
            var modes = modesManager.Modes;
            if (modes.Where(x => x.DisplayText[1] != "").Count() == 0)
            {
                return res;
            }

            res += objName + ".steps = \t\t--Шаги операций.\n";
            res += prefix + "{\n";
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Name != "")
                {
                    res += SaveSteps(prefix, objName, mode, baseOperation);
                }
            }
            res += prefix + "}\n";

            return res;
        }

        /// <summary>
        /// Сохранить шаги операций
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <param name="objName">Имя объекта</param>
        /// <param name="mode">Операция</param>
        /// <param name="baseOperation">Базовая операция</param>
        private string SaveSteps(string prefix, string objName,
            Mode mode, BaseOperation baseOperation)
        {
            var res = "";

            res += prefix + baseOperation.LuaName.ToUpper() + " =\n";
            res += prefix + prefix + "{\n";

            string temp = "";
            foreach (var step in mode.MainSteps)
            {
                if (step.GetBaseStepName() == "")
                {
                    continue;
                }

                temp += prefix + prefix + step.GetBaseStepLuaName() +
                    " = " + step.GetStepNumber() + ",\n";
            }

            if (temp.Length == 0)
            {
                string emptyTable = prefix + baseOperation.LuaName.ToUpper() +
                    " = { },\n";
                return emptyTable;
            }

            res += temp;
            res += prefix + prefix + "},\n";
            return res;
        }
        #endregion
    }
}
