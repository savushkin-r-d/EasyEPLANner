using System;
using System.Collections.Generic;
using System.Linq;
using TechObject;
using EplanDevice;

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

            string res = GenerateRequireModules();
            res += "\n";
            res += "-- Основные объекты проекта " +
                "(объекты, описанные в Eplan).\n";
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
        /// Генерация строк описания require модулей Lua
        /// </summary>
        /// <returns></returns>
        private static string GenerateRequireModules()
        {
            List<TechObject.TechObject> objects = techObjectManager.TechObjects;
            List<string> requireModules = objects
                .Where(x => x.BaseTechObject != null &&
                x.BaseTechObject.LuaModuleName != string.Empty)
                .Select(x =>
                    $"require( \"{x.BaseTechObject.LuaModuleName}\" )\n")
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            string description = "--Базовая функциональность\n";
            requireModules.Insert(0, description);

            return string.Join(string.Empty, requireModules);
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
            var objects = techObjectManager.TechObjects;

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
        /// <param name="attachedObjectsDict">Словарь привязанных объектов.
        /// Key: номер технологического объекта. Value: номера связанных 
        /// объектов в строке с разделителем в виде пробела.</param>
        /// <returns></returns>
        private static string SaveObjectsBindingToPrgLua(
            Dictionary<int, string> attachedObjectsDict)
        {
            var res = string.Empty;
            var temp = string.Empty;

            var previouslyObjectName = string.Empty;
            foreach (KeyValuePair<int, string> techObjectPair in attachedObjectsDict)
            {
                TechObject.TechObject mainObj = TechObjectManager.GetInstance().GetTObject(techObjectPair.Key);
                string mainObjNameLowCase = mainObj.NameEplanForFile.ToLower();
                string techObjNameForFile = $"prg.{mainObjNameLowCase}{mainObj.TechNumber}";
                List<TechObject.TechObject> bindedObjects = GetBindedObjectsList(techObjectPair);
                Dictionary<string, List<TechObject.TechObject>> mainObjAttachedObjectGroups =
                    GetAttachedObjectsGroupsDict(bindedObjects);
                foreach (KeyValuePair<string, List<TechObject.TechObject>> groupPair in mainObjAttachedObjectGroups)
                {
                    string groupName = groupPair.Key;
                    var objectsDescription = new List<string>();
                    foreach (TechObject.TechObject obj in groupPair.Value)
                    {
                        string objDescription =
                            $"prg.{obj.NameEplanForFile.ToLower()}{obj.TechNumber}";
                        objectsDescription.Add(objDescription);
                    }

                    if (previouslyObjectName != mainObjNameLowCase &&
                        previouslyObjectName != string.Empty)
                    {
                        temp += "\n";
                    }

                    if (groupPair.Value.Count > 1)
                    {
                        temp += $"{techObjNameForFile}.{groupName}" +
                           $" = {{ {string.Join(", ", objectsDescription)} }}\n";
                    }
                    else
                    {
                        temp += $"{techObjNameForFile}.{groupName}" +
                            $" = {objectsDescription.First()}\n";
                    }

                    previouslyObjectName = mainObjNameLowCase;
                }
            }

            if (temp != string.Empty)
            {
                res += $"\n{temp}\n";
            }

            return res;
        }

        /// <summary>
        /// Получить список привязанных к технологическому объекту объектов.
        /// </summary>
        /// <param name="techObjectPair">Key: номер технологического объекта.
        /// Value: номера связанных объектов в строке с разделителем
        /// в виде пробела.</param>
        /// <returns></returns>
        private static List<TechObject.TechObject> GetBindedObjectsList(
            KeyValuePair<int, string> techObjectPair)
        {
            var techObjects = new List<TechObject.TechObject>();

            TechObject.TechObject techObject = techObjectManager
                .GetTObject(techObjectPair.Key);
            string[] attachedObjs = techObjectPair.Value.Split(' ');
            foreach(string objNumStr in attachedObjs)
            {
                bool isDigit = int.TryParse(objNumStr, out int objNum);
                if (isDigit)
                {
                    TechObject.TechObject attachedTechObject = 
                        techObjectManager.GetTObject(objNum);
                    if (attachedTechObject == null)
                    {
                        string objName =
                            $"{techObject.Name}{techObject.TechNumber}";
                        string msg = $"Для объекта {objName} не найден " +
                            $"привязанный агрегат под номером {objNum}.\n";
                        Logs.AddMessage(msg);
                        continue;
                    }

                    techObjects.Add(attachedTechObject);
                }
                else
                {
                    string msg = $"В объекте \"{techObject.EditText[0]} " +
                        $"{techObject.TechNumber}\" ошибка заполнения поля " +
                        $"\"Привязанные устройства\"\n";
                    Logs.AddMessage(msg);
                }
            }

            return techObjects;
        }

        /// <summary>
        /// Получить словарь со связанными устройствами, которые разбиты по
        /// фунцкиональным группам.
        /// </summary>
        /// <param name="bindedObjects">Список привязанных объектов к объекту
        /// </param>
        /// <returns></returns>
        private static Dictionary<string, List<TechObject.TechObject>> GetAttachedObjectsGroupsDict(
            List<TechObject.TechObject> bindedObjects)
        {
            var dict = new Dictionary<string, List<TechObject.TechObject>>();

            foreach(var obj in bindedObjects)
            {
                var group = obj.BaseTechObject.BindingName;
                if (dict.ContainsKey(group))
                {
                    dict[group].Add(obj);
                }
                else
                {
                    var list = new List<TechObject.TechObject>() { obj };
                    dict.Add(group, list);
                }
            }

            return dict;
        }

        /// <summary>
        /// Сохранить описание объектов для prg.lua
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        private static string SaveObjectsInformationToPrgLua(string prefix)
        {
            string objectsInfo = string.Empty;
            string operations = string.Empty;
            string operationsSteps = string.Empty;
            string operationsParameters = string.Empty;
            string equipments = string.Empty;

            var objects = techObjectManager.TechObjects;
            IBaseTechObjectSaver baseTechObjectSaver =
                new BaseTechObjectSaver();
            foreach (TechObject.TechObject obj in objects)
            {
                BaseTechObject baseObj = obj.BaseTechObject;
                if(baseObj == null)
                {
                    continue;
                }

                var objName = "prg." + obj.NameEplanForFile.ToLower() +
                    obj.TechNumber.ToString();

                objectsInfo += baseTechObjectSaver.
                    SaveObjectInfoToPrgLua(obj, baseObj, objName, prefix);

                var modesManager = obj.ModesManager;
                var modes = modesManager.Modes;
                bool haveBaseOperations = modes
                    .Where(x => x.DisplayText[1] != string.Empty).Count() != 0;
                if (haveBaseOperations)
                {
                    operations += baseTechObjectSaver
                        .SaveOperations(objName, prefix, modes);
                    operationsSteps += baseTechObjectSaver
                        .SaveOperationsSteps(objName, prefix, modes);
                    operationsParameters += baseTechObjectSaver
                        .SaveOperationsParameters(objName, prefix, modes);
                }

                equipments += baseTechObjectSaver.SaveEquipment(obj, objName);
            }

            var accumulatedData = new string[]
            {
                objectsInfo,
                operations,
                operationsSteps,
                operationsParameters,
                equipments
            };

            var result = string.Empty;
            foreach (var stringsForSave in accumulatedData)
            {
                if (stringsForSave != string.Empty)
                {
                    result += stringsForSave + "\n";
                }
            }

            return result;
        }

        /// <summary>
        /// Сохранить базовую функциональность в prg.lua
        /// </summary>
        /// <returns></returns>
        private static string SaveFunctionalityToPrgLua()
        {
            var previouslyObjectName = "";
            var res = "";
            var objects = techObjectManager.TechObjects;

            foreach (TechObject.TechObject obj in objects)
            {
                string basicObj = "";
                if(obj.BaseTechObject != null)
                {
                    basicObj = obj.BaseTechObject.BasicName;
                }

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
        private static ITechObjectManager techObjectManager;
    }
}