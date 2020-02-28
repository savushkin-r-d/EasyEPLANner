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

        #region Сохранение в prg.lua
        /// <summary>
        /// Сохранить информацию об объекте в prg.lua
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        public override string SaveObjectInfoToPrgLua(string objName,
            string prefix)
        {
            var res = "";

            var objects = this.Owner.Parent as TechObjectManager;
            var masterObj = objects.Objects
                .Where(x => x.Name.Contains("Мастер")).FirstOrDefault();
            if (masterObj != null)
            {
                res += objName + ".master = prg." + masterObj.NameEplan
                    .ToLower() + masterObj.TechNumber + "\n";
            }

            // Параметры сбрасываемые до мойки.
            res += objName + ".reset_before_wash =\n" +
                prefix + "{\n" +
                prefix + objName + ".PAR_FLOAT.V_ACCEPTING_CURRENT,\n" +
                prefix + objName + ".PAR_FLOAT.PRODUCT_TYPE,\n" +
                prefix + objName + ".PAR_FLOAT.V_ACCEPTING_SET\n" +
                prefix + "}\n";

            res += "\n";

            return res;
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
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.GetBaseOperation();
                switch (baseOperation.Name)
                {
                    case "Мойка":
                        res += SaveWashOperation(prefix, objName, mode,
                            baseOperation);
                        break;

                    case "Наполнение":
                        res += SaveFillOperation(objName, mode, baseOperation);
                        break;

                    case "Хранение":
                        res += SaveStoringOperation(objName, mode);
                        break;

                    case "Выдача":
                        res += SaveOutOperation(objName, mode);
                        break;
                }
            }

            res += "\n";
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
                    string val = param.Value ==
                    "" ? "nil" : param.Value;
                    res += objName + "." + param.LuaName +
                        " = prg.control_modules." + val + "\n";
                }
            }

            return res;
        }

        /// <summary>
        /// Сохранить операцию наполнения.
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="mode">Операция</param>
        /// <param name="baseOperation">Базовая операция</param>
        /// <returns></returns>
        private string SaveFillOperation(string objName,
            Mode mode, BaseOperation baseOperation)
        {
            var res = "";

            var fillNumber = mode.GetModeNumber();
            res += objName + $".operations.FILL = {fillNumber}\n";

            res += SaveFillOperationParameters(objName, baseOperation);

            return res;
        }

        /// <summary>
        /// Сохранить параметры операции наполнения.
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="baseOperation">Базовая операция</param>
        /// <returns></returns>
        private string SaveFillOperationParameters(string objName,
            BaseOperation baseOperation)
        {
            var res = "";

            foreach (BaseProperty param in baseOperation.Properties)
            {
                if (param.CanSave())
                {
                    string val = param.Value == "" ? "nil" : param.Value;

                    switch (param.LuaName)
                    {
                        case "OPERATION_AFTER_FILL":
                            var modes = this.Owner.ModesManager.Modes;
                            var mode = modes
                                .Where(x => x.GetModeNumber().ToString() == val)
                                .FirstOrDefault();

                            if (mode != null)
                            {
                                val =mode.GetBaseOperation().LuaName.ToUpper();
                            }

                            if (val != "nil")
                            {
                                res += objName + "." + param.LuaName +
                                $" = {objName}.operations." + val + "\n";
                            }
                            else
                            {
                                res += objName + "." + param.LuaName +
                                    $" = {val}\n";
                            }
                            break;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Сохранить операцию хранения
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="mode">Операция</param>
        /// <returns></returns>
        private string SaveStoringOperation(string objName, Mode mode)
        {
            var res = "";

            var storingNumber = mode.GetModeNumber();
            res += objName + $".operations.STORING = " +
                $"{storingNumber}\n";

            return res;
        }

        /// <summary>
        /// Сохранить операцию выдачи
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="mode">Операция</param>
        /// <returns></returns>
        private string SaveOutOperation(string objName, Mode mode)
        {
            var res = "";

            var outNumber = mode.GetModeNumber();
            res += objName + $".operations.OUT = " +
                $"{outNumber}\n";

            return res;
        }
        #endregion
    }
}
