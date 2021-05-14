using System.Collections.Generic;
using System.Linq;

namespace TechObject
{
    public interface IBaseTechObjectSaver
    {
        /// <summary>
        /// Сохранить информацию об объекте в prg.lua
        /// </summary>
        /// <param name="obj">Технологический объект</param>
        /// <param name="baseObj">Сохраняемый базовый объект</param>
        /// <param name="objName">Уникальное имя объекта</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        string SaveObjectInfoToPrgLua(TechObject obj, BaseTechObject baseObj,
            string objName, string prefix);

        /// <summary>
        /// Сохранить операции объекта
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <param name="modes">Операции объекта</param>
        /// <returns></returns>
        string SaveOperations(string objName, string prefix, List<Mode> modes);

        /// <summary>
        /// Сохранить номера шагов операций объекта.
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <param name="modes">Операции объекта</param>
        /// <returns></returns>
        string SaveOperationsSteps(string objName, string prefix,
            List<Mode> modes);

        /// <summary>
        /// Сохранить параметры операций базового объекта танк.
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <param name="modes">Операции объекта</param>
        /// <returns></returns>
        string SaveOperationsParameters(string objName, string prefix,
            List<Mode> modes);

        /// <summary>
        /// Сохранить оборудование технологического объекта
        /// </summary>
        /// <param name="objName">Имя для сохранения</param>
        /// <param name="obj">Объект с параметрами</param>
        /// <returns></returns>
        string SaveEquipment(TechObject obj, string objName);
    }

    public class BaseTechObjectSaver : IBaseTechObjectSaver
    {
        public string SaveObjectInfoToPrgLua(TechObject obj,
            BaseTechObject baseObj, string objName, string prefix)
        {
            string res = string.Empty;

            res += SaveTankAdditionalParameters(baseObj, objName, prefix);
            res += SaveLineAdditionalParameters(baseObj, objName, prefix);
            res += SaveObjectProperties(obj, objName, prefix);

            return res;
        }

        /// <summary>
        /// Сохранить доп. информацию о танке
        /// </summary>
        /// <param name="obj">Сохраняемый базовый объект</param>
        /// <param name="objName">Уникальное имя объекта</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        private string SaveTankAdditionalParameters(BaseTechObject obj,
            string objName, string prefix)
        {
            var res = string.Empty;
            if (obj.S88Level == (int)BaseTechObjectManager.ObjectType.Unit)
            {
                var masterObj = TechObjectManager.GetInstance()
                    .ProcessCellObject;
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
            }

            return res;
        }

        /// <summary>
        /// Сохранить доп. информацию о линии
        /// </summary>
        /// <param name="obj">Сохраняемый базовый объект</param>
        /// <param name="objName">Уникальное имя объекта</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        private string SaveLineAdditionalParameters(BaseTechObject obj,
            string objName, string prefix)
        {
            var res = string.Empty;

            if (obj.UseGroups && obj.ObjectGroupsList.Count > 0)
            {
                foreach (var objectGroup in obj.ObjectGroupsList)
                {
                    if (objectGroup.Value == string.Empty)
                    {
                        continue;
                    }

                    string objectNames = objectGroup.GetAttachedObjectsName()
                        .Select(x => $"{prefix}prg.{x},\n")
                        .Aggregate((x, y) => x + y);
                    res += $"{objName}.{objectGroup.WorkStrategy.LuaName} =\n";
                    res += $"{prefix}{{\n";
                    res += $"{objectNames}";
                    res += $"{prefix}}}\n";
                }
            }

            bool containsFillOrPumping = obj.BaseOperations
                .Any(x => x.LuaName == "FILL" || x.LuaName == "PUMPING");
            if (obj.UseGroups && containsFillOrPumping)
            {
                res += $"{objName}.reset_before_wash =\n" +
                    prefix + "{\n" +
                    prefix + $"{objName}.PAR_FLOAT.PROD_V,\n" +
                    prefix + $"{objName}.PAR_FLOAT.WATER_V,\n" +
                    prefix + "}\n";
            }

            return res;
        }

        private string SaveObjectProperties(TechObject obj, string objName,
            string prefix)
        {
            string res = string.Empty;
            if (obj.BaseProperties.Count > 0)
            {
                res += $"{objName}.properties =\n";
                res += $"{prefix}{{\n";
                foreach(var property in obj.BaseProperties.Properties)
                {
                    bool skip = property.IsEmpty;
                    if (skip)
                    {
                        continue;
                    }

                    res += $"{prefix}{property.SaveToPrgLua(string.Empty)},\n";
                }
                res += $"{prefix}}}\n";
            }

            return res;
        }

        public string SaveOperations(string objName, string prefix,
            List<Mode> modes)
        {
            var res = "";

            string saveOperations = "";
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Name != "")
                {
                    saveOperations += prefix + baseOperation.LuaName.ToUpper() +
                        " = " + mode.GetModeNumber() + ",\n";
                }
            }

            bool isEmpty = saveOperations == "";
            if (!isEmpty)
            {
                res += objName + ".operations = \t\t--Операции.\n";
                res += prefix + "{\n";
                res += saveOperations;
                res += prefix + "}\n";
            }

            return res;
        }

        public string SaveOperationsSteps(string objName, string prefix,
            List<Mode> modes)
        {
            var res = string.Empty;

            string operation = string.Empty;
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Name == string.Empty)
                {
                    continue;
                }

                string statesString = MakeStatesWithStepsString(mode, prefix);

                bool statesIsEmpty = statesString == string.Empty;
                if (!statesIsEmpty)
                {
                    operation +=
                        $"{prefix}{baseOperation.LuaName.ToUpper()} =\n";
                    operation += $"{prefix}{prefix}{{\n";
                    operation += statesString;
                    operation += $"{prefix}{prefix}}},\n";
                }
            }

            bool operationIsEmpty = operation == string.Empty;
            if(!operationIsEmpty)
            {
                res += $"{objName}.steps = \t\t--Шаги операций.\n";
                res += $"{prefix}{{\n";
                res += operation;
                res += $"{prefix}}}\n";
            }

            return res;
        }

        /// <summary>
        /// Генерация строки с состояниями и шагами операции
        /// </summary>
        /// <param name="mode">Операция</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        private string MakeStatesWithStepsString(Mode mode, string prefix)
        {
            const int stepPrefixLength = 3;
            const int statePrefixLength = 2;
            string stepPrefix = string
                .Concat(Enumerable.Repeat(prefix, stepPrefixLength));
            string statePrefix = string
                .Concat(Enumerable.Repeat(prefix, statePrefixLength));

            string statesDescription = string.Empty;
            foreach (State state in mode.States)
            {
                string steps = string.Empty;
                foreach(Step step in state.Steps)
                {
                    if (step.GetBaseStepName() == string.Empty)
                    {
                        continue;
                    }

                    steps += $"{stepPrefix}{step.GetBaseStepLuaName()} =" +
                        $" {step.GetStepNumber()},\n";
                }

                if (steps != string.Empty)
                {
                    statesDescription += $"{statePrefix}{state.Type} =\n";
                    statesDescription += $"{stepPrefix}{{\n";
                    statesDescription += steps;
                    statesDescription += $"{stepPrefix}}},\n";
                }
            }

            return statesDescription;
        }

        public string SaveOperationsParameters(string objName, string prefix,
            List<Mode> modes)
        {
            var res = "";
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Properties.Count == 0)
                {
                    continue;
                }

                string paramsForSave = "";
                foreach (var parameter in baseOperation.Properties)
                {
                    bool isEmpty = parameter.IsEmpty ||
                        parameter.Value == "" ||
                        parameter.NeedDisable;
                    if (isEmpty)
                    {
                        continue;
                    }

                    string paramCode = parameter.SaveToPrgLua(prefix);
                    if (!string.IsNullOrEmpty(paramCode))
                    {
                        paramsForSave += $"{paramCode},\n";
                    }
                }

                bool needSaveParameters = paramsForSave != "";
                if (needSaveParameters)
                {
                    res += $"{objName}.{baseOperation.LuaName} =\n";
                    res += prefix + "{\n";
                    res += paramsForSave;
                    res += prefix + "}\n";
                }
            }

            return res;
        }

        public string SaveEquipment(TechObject obj, string objName)
        {
            var res = "";
            Equipment equipment = obj.Equipment;
            foreach (var item in equipment.Items)
            {
                var property = item as BaseParameter;

                string equipmentCode = property.SaveToPrgLua(string.Empty);
                if (string.IsNullOrEmpty(equipmentCode))
                {
                    continue;
                }

                res += $"{objName}.{equipmentCode}\n";
            }

            return res;
        }
    }
}