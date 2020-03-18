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

                if (objects[i].AttachedObjects != string.Empty)
                {
                    // Т.к объекты начинаются с 1
                    attachedObjects[i + 1] = objects[i].AttachedObjects;
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

                        if (attachedTechObjectType.Contains("mix_node"))
                        {
                            res += techObjNameForFile + ".mix_node = " +
                                    "prg." + attachedTechObjNameForFile + "\n";
                        }
                        else if (attachedTechObjectType.Contains("cooler_node"))
                        {
                            res += techObjNameForFile + ".cooler_node = " +
                                    "prg." + attachedTechObjNameForFile + "\n";
                        }
                        else if (attachedTechObjectType.Contains("heater_node"))
                        {
                            res += techObjNameForFile + ".heater_node = " +
                                    "prg." + attachedTechObjNameForFile + "\n";
                        }

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
        /// Сохранить информацию об операциях объекта в prg.lua
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

                res += obj.BaseTechObject
                    .SaveObjectInfoToPrgLua(objName, prefix);

                res += obj.BaseTechObject
                    .SaveOperationsToPrgLua(objName, prefix);

                res += SaveObjectEquipmentToPrgLua(obj, objName);
            }
            return res;
        }

        /// <summary>
        /// Сохранить оборудование технологического объекта
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="objName">Имя для сохранения</param>
        /// <returns></returns>
        private static string SaveObjectEquipmentToPrgLua(
            TechObject.TechObject obj, string objName)
        {
            var res = "";
            var equipment = obj.Equipment;
            bool needWhiteSpace = false;

            foreach (Editor.ITreeViewItem item in equipment.Items)
            {
                var property = item as BaseProperty;
                var value = property.Value;
                var luaName = property.LuaName;

                if (value != "")
                {
                    res += objName + $".{luaName} = " +
                        $"prg.control_modules.{value}\n";
                }
                else
                {
                    res += objName + $".{luaName} = nil\n";
                }

                needWhiteSpace = true;
            }

            if (needWhiteSpace)
            {
                res += "\n";
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