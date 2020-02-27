using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый танк
    /// </summary>
    public class BaseTank : BaseTechObject
    {
        public BaseTank() : base()
        {
            S88Level = 1;
            Name = "Танк";
            EplanName = "tank";
            BaseOperations = DataBase.Imitation.TankOperations();
            BaseProperties = DataBase.Imitation.TankProperties();
            BasicName = "tank";
            Equipment = DataBase.Imitation.TankEquipment();
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

            //TODO: master add

            foreach (Mode mode in modes)
            {
                var baseOperation = mode.GetBaseOperation();
                switch (baseOperation.Name)
                {
                    case "Мойка":
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

                        foreach (BaseProperty param in baseOperation
                            .Properties)
                        {
                            if (param.CanSave())
                            {
                                string val = param.Value ==
                                "" ? "nil" : param.Value;
                                res += objName + "." + param.LuaName +
                                    " = prg.control_modules." + val + "\n";
                            }
                        }

                        res += "\n";
                        break;

                    case "Наполнение":
                        var fillNumber = mode.GetModeNumber();
                        res += objName + $".operations.FILL = {fillNumber}\n";

                        //TODO: operation after fill

                        break;

                    case "Хранение":
                        var storingNumber = mode.GetModeNumber();
                        res += objName + $".operations.STORING = " +
                            $"{storingNumber}\n";
                        break;

                    case "Выдача":
                        var outNumber = mode.GetModeNumber();
                        res += objName + $".operations.OUT = " +
                            $"{outNumber}\n";
                        break;
                }
            }

            res += "\n";

            return res;
        }
    }
}
