using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;
using Device;

namespace EasyEPlanner
{
    static class PrgLuaSaver
    {
        /// <summary>
        /// Сохранить Prg.lua как таблицу Lua
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <returns>Возвращает файл в виде строки</returns>
        public static string Save(string prefix)
        {
            var attachedObjects = new Dictionary<int, string>();
            deviceManager = DeviceManager.GetInstance();
            techObjectManager = TechObjectManager.GetInstance();

            var res = "";
            res += "local prg =\n\t{\n";
            res += SaveDevicesToPrgLua(prefix);
            res += SaveVariablesToPrgLua(prefix, out attachedObjects);
            res += "\t}\n";

            if (attachedObjects.Count > 0)
            {
                res += SaveObjectsBindingToPrgLua(attachedObjects);
            }

            res += SaveObjectsInformationToPrgLua(prefix);
            res += SaveFunctionalityToPrgLua();
            res += "return prg";
            res = res.Replace("\t", "    ");

            return res;
        }

        /// <summary>
        /// Сохранить устройства в prg.lua
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        private static string SaveDevicesToPrgLua(string prefix)
        {
            var res = prefix + "control_modules =\n" +
                prefix + "\t{\n";

            foreach (IODevice dev in deviceManager.Devices)
            {
                if (dev.DeviceType == DeviceType.Y ||
                    dev.DeviceType == DeviceType.DEV_VTUG)
                {
                    continue;
                }

                if (dev.ObjectNumber > 0 && dev.ObjectName == "")
                {
                    res += "\t_";
                }
                else
                {
                    res += "\t";
                }
                res += prefix + dev.Name + " = " + dev.DeviceType.ToString() +
                    "(\'" + dev.Name + "\'),\n";
            }

            res += prefix + "},\n\n";
            res = res.Replace("\t", "    ");

            return res;
        }

        /// <summary>
        /// Сохранить переменные в prg.lua
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <param name="attachedObjectsDict">Привязанные объекты</param>
        /// <returns></returns>
        private static string SaveVariablesToPrgLua(string prefix,
            out Dictionary<int, string> attachedObjectsDict)
        {
            var attachedObjects = new Dictionary<int, string>();

            var res = "";
            var previouslyObjectName = "";
            var objects = techObjectManager.Objects;

            for (int i = 0; i < objects.Count; i++)
            {
                if (previouslyObjectName != objects[i].NameEplan
                    .ToLower() && previouslyObjectName != "")
                {
                    res += "\n";
                }

                var varForSave = objects[i].NameEplanForFile
                    .ToLower() + objects[i].TechNumber + " = OBJECT" +
                    techObjectManager.GetTechObjectN(objects[i]) + ",\n";
                if (objects[i].NameEplanForFile.ToLower() == "")
                {
                    res += prefix + "--" + varForSave;
                }
                else
                {
                    res += prefix + varForSave;
                }

                if (objects[i].AttachedObjects.Value != string.Empty)
                {
                    // Т.к объекты начинаются с 1
                    attachedObjects[i + 1] = objects[i].AttachedObjects.Value;
                }

                previouslyObjectName = objects[i].NameEplan.ToLower();
            }
            attachedObjectsDict = attachedObjects;

            return res;
        }

        /// <summary>
        /// Сохранить привязку объектов к объектам в prg.lua
        /// </summary>
        /// <param name="attachedObjects">Словарь привязанных объектов</param>
        /// <returns></returns>
        private static string SaveObjectsBindingToPrgLua(
            Dictionary<int, string> attachedObjects)
        {
            var res = "";
            res += "\n";

            string previouslyObjectName = "";
            bool isDigit = false;
            foreach (var val in attachedObjects)
            {
                var techObj = techObjectManager.GetTObject(val.Key);
                var attachedObjs = val.Value.Split(' ');
                foreach (string value in attachedObjs)
                {
                    isDigit = int.TryParse(value, out _);
                    if (isDigit)
                    {
                        var attachedTechObject = techObjectManager.GetTObject(
                            Convert.ToInt32(value));
                        if(attachedTechObject == null)
                        {
                            string objName = techObj.Name + techObj.TechNumber
                                .ToString();
                            string msg = $"Для объекта {objName} не найден " +
                                $"привязанный агрегат под номером {value}.\n";
                            Logs.AddMessage(msg);
                            continue;
                        }

                        var attachedTechObjectType = attachedTechObject
                            .NameEplanForFile.ToLower();
                        var attachedTechObjNameForFile =
                            attachedTechObjectType +
                            attachedTechObject.TechNumber;
                        var techObjNameForFile = "prg." +
                            techObj.NameEplanForFile.ToLower() +
                            techObj.TechNumber;

                        if (previouslyObjectName != techObj.NameEplanForFile
                            .ToLower() && previouslyObjectName != "")
                        {
                            res += "\n";
                        }

                        res += GenerateStringForAttachingObject(
                            attachedTechObject, techObjNameForFile,
                            attachedTechObjNameForFile);

                        previouslyObjectName = techObj.NameEplanForFile
                            .ToLower();
                    }
                    else
                    {
                        string msg = $"В объекте \"{techObj.EditText[0]} " +
                            $"{techObj.TechNumber}\" ошибка заполнения поля " +
                            $"\"Привязанные устройства\"\n";
                        Logs.AddMessage(msg);
                    }
                }
            }
            res += "\n";
            return res;
        }

        /// <summary>
        /// Генерировать строку для записи привязки агрегатов к аппаратам в
        /// prg.lua
        /// </summary>
        /// <param name="attachedTechObject">Привязанный тех. объект</param>
        /// <param name="attachedTechObjNameForFile">Имя привязанного
        /// тех. объекта для файла</param>
        /// <param name="techObjNameForFile">Имя тех. объекта к которому
        /// привязывается объект</param>
        /// <returns></returns>
        private static string GenerateStringForAttachingObject(
            TechObject.TechObject attachedTechObject, 
            string techObjNameForFile, string attachedTechObjNameForFile)
        {
            var res = "";

            if (attachedTechObject.BaseTechObject is BaseMixer)
            {
                res += techObjNameForFile + ".mix_node = " +
                        "prg." + attachedTechObjNameForFile + "\n";
            }

            if (attachedTechObject.BaseTechObject is BaseCooler ||
                attachedTechObject.BaseTechObject is BaseCoolerPID)
            {
                res += techObjNameForFile + ".cooler_node = " +
                        "prg." + attachedTechObjNameForFile + "\n";
            }
            
            if (attachedTechObject.BaseTechObject is BaseHeater ||
                attachedTechObject.BaseTechObject is BaseHeaterPID)
            {
                res += techObjNameForFile + ".heater_node = " +
                        "prg." + attachedTechObjNameForFile + "\n";
            }
            
            if (attachedTechObject.BaseTechObject is BasePressurePID)
            {
                res += techObjNameForFile + ".pressure_node = " +
                    "prg." + attachedTechObjNameForFile + "\n";
            }
            
            if (attachedTechObject.BaseTechObject is BaseFlowNodePID)
            {
                res += techObjNameForFile + ".flow_node = " +
                    "prg." + attachedTechObjNameForFile + "\n";
            }
            
            if (attachedTechObject.BaseTechObject is BaseWaterTank ||
                attachedTechObject.BaseTechObject is BaseWaterTankPID)
            {
                res += techObjNameForFile + ".ice_water_pump_tank = " +
                    "prg." + attachedTechObjNameForFile + "\n";
            }

            return res;
        }

        /// <summary>
        /// Сохранить описание объектов для prg.lua
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        private static string SaveObjectsInformationToPrgLua(string prefix)
        {
            var res = "";
            var objects = techObjectManager.Objects;

            foreach (TechObject.TechObject obj in objects)
            {
                var objName = "prg." + obj.NameEplanForFile.ToLower() +
                    obj.TechNumber.ToString();
                res += obj.BaseTechObject.SaveToPrgLua(objName, prefix);
            }
            return res;
        }

        /// <summary>
        /// Сохранить базовую функциональность в prg.lua
        /// </summary>
        /// <returns></returns>
        private static string SaveFunctionalityToPrgLua()
        {
            var previouslyObjectName = "";
            var res = "";
            var objects = techObjectManager.Objects;

            foreach (TechObject.TechObject obj in objects)
            {
                var basicObj = DataBase.Imitation
                    .GetBasicName(obj.DisplayText[1]);

                if (previouslyObjectName != obj.NameEplanForFile.ToLower() &&
                    previouslyObjectName != "")
                {
                    res += "\n";
                }

                var objName = obj.NameEplanForFile.ToLower() + obj.TechNumber;
                var functionalityForSave = "add_functionality(prg." +
                    objName + ", " + "basic_" + basicObj + ")\n";
                if (basicObj == "")
                {
                    res += "--" + functionalityForSave;
                }
                else
                {
                    res += functionalityForSave;
                }
                previouslyObjectName = obj.NameEplanForFile.ToLower();
            }
            return res;
        }

        private static DeviceManager deviceManager;
        private static TechObjectManager techObjectManager;
    }
}