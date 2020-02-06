using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechObject
{
    /// <summary>
    /// Интерфейс менеджера технологических объектов проекта.
    /// </summary>
    public interface ITechObjectManager
    {
        void LoadFromLuaStr(string LuaStr, string projectName);
        void LoadRestriction(string LuaStr);
        string SaveAsLuaTable(string prefixStr);
        string SavePrgAsLuaTable(string prefixStr);
        void GetObjectForXML(TreeNode rootNode);
        void SetCDBXTagView(bool combineTag);
        string SaveRestrictionAsLua(string prefixStr);
        List<TechObject> GetTechObjects();
    }

    /// <summary>
    /// Получение номера объекта в списке. Нумерация начинается с 1.
    /// </summary>
    public delegate int GetN(object obj);

    /// <summary>
    /// Менеджер технологических объектов проекта.
    /// </summary>
    public class TechObjectManager : Editor.TreeViewItem, ITechObjectManager
    {

        private TechObjectManager()
        {
            lua = new Lua();

            lua.RegisterFunction("ADD_TECH_OBJECT", this,
                GetType().GetMethod("AddObject"));

            //Для отладки Lua скриптов.
            LuaFunction resF = lua.RegisterFunction("PRINT", this,
                GetType().GetMethod("ShowMessage"));

            string systemFilesPath = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location) +
                "\\Lua";

            string sysLuaPath = Path.Combine(systemFilesPath, "sys.lua");
            if (Directory.Exists(systemFilesPath) == true)
            {
                lua.DoFile(sysLuaPath);
            }
            else
            {
                CopySystemFiles(systemFilesPath);
                lua.DoFile(sysLuaPath);
            }

            objects = new List<TechObject>();

            cdbxTagView = false;
        }

        /// <summary>
        /// Копирует системные .lua файлы если они не загрузились
        /// в теневое хранилище (Win 7 fix).
        /// </summary>
        private void CopySystemFiles(string shadowAssemblySystemFilesDir)
        {
            const string luaDirectory = "\\Lua";
            Directory.CreateDirectory(shadowAssemblySystemFilesDir);

            string systemFilesPath = Path.GetDirectoryName(EasyEPlanner.
                AddInModule.OriginalAssemblyPath) + luaDirectory;

            DirectoryInfo systemFilesDirectory = new DirectoryInfo(
                systemFilesPath);
            FileInfo[] systemFiles = systemFilesDirectory.GetFiles();
            foreach (FileInfo systemFile in systemFiles)
            {
                string pathToFile = Path.Combine(shadowAssemblySystemFilesDir,
                    systemFile.Name);
                systemFile.CopyTo(pathToFile, true);
            }
        }

        public void ShowMessage(string msg)
        {
            MessageBox.Show(msg);
        }

        /// <summary>
        /// Получение номера операции в списке операций. 
        /// Нумерация начинается с 1.
        /// </summary>
        /// <param name="mode">Операция, номер которой хотим получить.</param>
        /// <returns>Номер заданной операции.</returns>
        public int GetTechObjectN(object techObject)
        {
            return objects.IndexOf(techObject as TechObject) + 1;
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + "init_tech_objects_modes = function()\n" +
                prefix + "\treturn\n" +
                prefix + "\t{\n";

            foreach (TechObject obj in objects)
            {
                res += obj.SaveAsLuaTable(prefix + "\t\t");
            }

            res += prefix + "\t}\n" +
                prefix + "end\n";

            res = res.Replace("\t", "    ");
            return res;
        }

        // Сохранение файла prg.lua
        public string SavePrgAsLuaTable(string prefix)
        {
            // Словарь привязанных агрегатов к аппарату
            var attachedObjects = new Dictionary<int, string>();

            var res = "";
            res += "local prg =\n\t{\n";
            res += SaveVariablesAsLuaTable(prefix, out attachedObjects);
            res += "\t}\n";

            if (attachedObjects.Count > 0)
            {
                res += SaveBindingAsLuaTable(attachedObjects);
            }

            res += SaveParamsAsLuaTable(prefix);
            res += SaveFunctionalityAsLuaTable();
            res += "\nreturn prg";
            res.Replace("\t", "    ");

            return res;
        }

        // Сохранение переменных prg.lua
        private string SaveVariablesAsLuaTable(string prefix, 
            out Dictionary<int, string> attachedObjectsDict)
        {
            // Словарь с агрегатами, привязанных к аппарату, 
            //обращение по номеру аппарата
            var attachedObjects = new Dictionary<int, string>();

            var res = "";
            var previouslyObjectName = ""; // Имя предыдущего объекта

            for (int i = 0; i < objects.Count; i++)
            {
                if (previouslyObjectName != objects[i].NameEplanForFile
                    .ToLower() && previouslyObjectName != "")
                {
                    res += "\n";
                }

                res += prefix + objects[i].NameEplanForFile.ToLower() + 
                    objects[i].TechNumber + " = OBJECT" + 
                    GetTechObjectN(objects[i]) + ",\n";

                // Если есть привязка, помечаю, какие агрегаты к 
                //какому аппарату привязаны
                if (objects[i].AttachedObjects != string.Empty)
                {
                    // Т.к объекты начинаются с 1
                    attachedObjects[i + 1] = objects[i].AttachedObjects;
                }

                // Записал, обозначил, что этот объект уже записан
                previouslyObjectName = objects[i].NameEplanForFile.ToLower();
            }

            attachedObjectsDict = attachedObjects;

            return res;
        }

        // Сохранение привязок prg.lua
        private string SaveBindingAsLuaTable(
            Dictionary<int, string> attachedObjects)
        {
            var res = "";
            res += "\n"; // Пустая строка для разделения

            string previouslyObjectName = ""; // Предыдущий объект
            bool isInt = false; // Проверка на число
            foreach (var val in attachedObjects)
            {
                var techObj = GetTObject(val.Key);
                var attachedObjs = val.Value.Split(' ');
                foreach (string value in attachedObjs)
                {
                    isInt = int.TryParse(value, out _);
                    if (isInt)
                    {
                        var attachedTechObject = GetTObject(
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
                            res += "\n"; // Отступ, если изменен тип объекта
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
                        EasyEPlanner.ProjectManager.GetInstance()
                            .AddLogMessage(msg);
                    }
                }
            }
            res += "\n";
            return res;
        }

        // Сохранение информации о базовой операции, сигналах и шагах в prg.lua
        private string SaveParamsAsLuaTable(string prefix)
        {
            var res = "";
            foreach (TechObject obj in objects)
            {
                var modesManager = obj.ModesManager;
                var modes = modesManager.Modes;
                foreach (Mode mode in modes)
                {
                    var baseOperation = mode.GetBaseOperation();
                    switch (baseOperation.Name)
                    {
                        case "Мойка":
                            var objName = "prg." + obj.NameEplanForFile
                                .ToLower() + obj.TechNumber.ToString();

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
                                    string val = param.GetValue() ==
                                    "" ? "nil" : param.GetValue();
                                    res += objName + "." + param.GetLuaName() +
                                        " = " + val + "\n";
                                }                               
                            }

                            res += "\n"; // Отступ перед новым объектом
                            break;
                    }
                }
            }
            return res;
        }

        // Сохранение базовой функциональности объектов
        public string SaveFunctionalityAsLuaTable()
        {
            var previouslyObjectName = "";
            var res = "";

            foreach (TechObject obj in objects)
            {
                if (previouslyObjectName != obj.NameEplanForFile.ToLower() && 
                    previouslyObjectName != "")
                {
                    res += "\n"; // Отступ, если изменен тип объекта
                }
                var basicObj = DataBase.Imitation
                    .GetBasicName(obj.DisplayText[1]).ToLower();
                var objName = obj.NameEplanForFile.ToLower() + obj.TechNumber;
                res += "add_functionality(prg." + objName + ", " + "basic_" + 
                    basicObj + ")\n";

                previouslyObjectName = obj.NameEplanForFile.ToLower();
            }
            return res;
        }

        /// <summary>
        /// Сохранение ограничений в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveRestrictionAsLua(string prefix)
        {
            string res = prefix + "restrictions =\n" + prefix + "\t{";

            foreach (TechObject obj in objects)
            {
                res += obj.SaveRestrictionAsLua(prefix + "\t");
            }

            res += "\n" + prefix + "\t}\n";


            res = res.Replace("\t", "    ");
            return res;
        }

        /// <summary>
        /// Проверка технологического объекта
        /// на правильность ввода и др.
        /// </summary>
        /// <returns>Строка с ошибками</returns>
        public string Check()
        {
            var errors = string.Empty;
            List<TechObject> TObjects = GetTechObjects();

            foreach (TechObject obj in TObjects)
            {
                if (obj.DisplayText[1].Length == 0)
                {
                    string objName = obj.EditText[0] + " " + obj.TechNumber;
                    string msg = string.Format("Не выбран базовый объект - " +
                        "\"{0}\"\n", objName);
                    errors += msg;
                }

                errors += obj.Check();
            }

            return errors;

        }

        /// <summary>
        /// Добавление технологического объекта. Вызывается из Lua.
        /// </summary>
        /// <returns>Добавленный технологический объект.</returns>
        public TechObject AddObject(int techN, string name, int techType,
            string nameEplan, int cooperParamNumber, string NameBC, 
            string baseTechObjectName, string attachedObjects)
        {
            TechObject obj = new TechObject(name, GetTechObjectN, techN,
                techType, nameEplan, cooperParamNumber, NameBC, 
                attachedObjects);

            // Установка значения базового аппарата
            obj.SetNewValue(baseTechObjectName, true);

            objects.Add(obj);

            return obj;
        }

        /// <summary>
        /// Получение объекта по номеру
        /// </summary>
        /// <param name="i">индекс </param>
        /// <returns></returns>
        public TechObject GetTObject(int i)
        {
            if (objects != null)
            {
                if (objects.Count >= i)
                {
                    return objects[i - 1];
                }
            }
            return null;
        }

        /// <summary>
        /// Получение списка объектов
        /// </summary>
        /// <returns></returns>
        public List<TechObject> GetTechObjects()
        {
            return Objects;
        }

        /// <summary>
        /// Формирование узлов для операций, шагов и параметров объектов.
        /// </summary>
        /// <param name="rootNode">корневой узел</param>
        public void GetObjectForXML(TreeNode rootNode)
        {
            TreeNode systemNode = new TreeNode("SYSTEM");
            systemNode.Nodes.Add("SYSTEM.UP_TIME", "SYSTEM.UP_TIME");
            systemNode.Nodes.Add("SYSTEM.WASH_VALVE_SEAT_PERIOD", 
                "SYSTEM.WASH_VALVE_SEAT_PERIOD");
            systemNode.Nodes.Add("SYSTEM.P_V_OFF_DELAY_TIME", 
                "SYSTEM.P_V_OFF_DELAY_TIME");
            systemNode.Nodes.Add("SYSTEM.WASH_VALVE_UPPER_SEAT_TIME", 
                "SYSTEM.WASH_VALVE_UPPER_SEAT_TIME");
            systemNode.Nodes.Add("SYSTEM.WASH_VALVE_LOWER_SEAT_TIME", 
                "SYSTEM.WASH_VALVE_LOWER_SEAT_TIME");
            systemNode.Nodes.Add("SYSTEM.CMD", "SYSTEM.CMD");
            systemNode.Nodes.Add("SYSTEM.CMD_ANSWER", "SYSTEM.CMD_ANSWER");
            systemNode.Nodes.Add("SYSTEM.P_RESTRICTIONS_MODE", 
                "SYSTEM.P_RESTRICTIONS_MODE");
            systemNode.Nodes.Add("SYSTEM.P_RESTRICTIONS_MANUAL_TIME", 
                "SYSTEM.P_RESTRICTIONS_MANUAL_TIME");
            systemNode.Nodes.Add("SYSTEM.P_AUTO_PAUSE_OPER_ON_DEV_ERR", 
                "SYSTEM.P_AUTO_PAUSE_OPER_ON_DEV_ERR");

            rootNode.Nodes.AddRange(new TreeNode[] { systemNode });

            for (int num = 1; num <= Objects.Count; num++)
            {
                TechObject item = Objects[num - 1];


                TreeNode objNode = new TreeNode(item.NameBC + 
                    item.TechNumber.ToString());

                TreeNode objModesNode = new TreeNode(item.NameBC + 
                    item.TechNumber.ToString() + "_Операции");
                TreeNode objOperStateNode = new TreeNode(item.NameBC + 
                    item.TechNumber.ToString() + "_Состояния_Операций");
                TreeNode objAvOperNode = new TreeNode(item.NameBC + 
                    item.TechNumber.ToString() + "_Доступность");
                TreeNode objStepsNode = new TreeNode(item.NameBC + 
                    item.TechNumber.ToString() + "_Шаги");
                TreeNode objParamsNode = new TreeNode(item.NameBC + 
                    item.TechNumber.ToString() + "_Параметры");

                string obj = "OBJECT" + num.ToString();
                string mode = obj + ".MODES";
                string oper = obj + ".OPERATIONS";
                string av = obj + ".AVAILABILITY";
                string step = mode + "_STEPS";
                if (cdbxTagView == true)
                {
                    objNode.Nodes.Add(obj + ".CMD", obj + ".CMD");
                }
                else
                {
                    objModesNode.Nodes.Add(obj + ".CMD", obj + ".CMD");
                }


                int stCount = item.ModesManager.Modes.Count / 33;
                for (int i = 0; i <= stCount; i++)
                {
                    string number = "[ " + (i + 1).ToString() + " ]";

                    if (cdbxTagView == true)
                    {
                        objNode.Nodes.Add("OBJECT" + num.ToString() + ".ST" + 
                            number, "OBJECT" + num.ToString() + ".ST" + number);
                    }
                    else
                    {
                        objModesNode.Nodes.Add("OBJECT" + num.ToString() + 
                            ".ST" + number, "OBJECT" + num.ToString() + ".ST" + 
                            number);
                    }
                }

                for (int i = 1; i <= item.ModesManager.Modes.Count; i++)
                {
                    string number = "[ " + i.ToString() + " ]";
                    if (cdbxTagView == true)
                    {
                        objNode.Nodes.Add(mode + number, mode + number);
                        objNode.Nodes.Add(oper + number, oper + number);
                        objNode.Nodes.Add(av + number, av + number);
                        objNode.Nodes.Add(step + number, step + number);
                    }
                    else
                    {
                        objModesNode.Nodes.Add(mode + number, mode + number);
                        objOperStateNode.Nodes.Add(oper + number, oper + 
                            number);
                        objAvOperNode.Nodes.Add(av + number, av + number);
                        objStepsNode.Nodes.Add(step + number, step + number);
                    }


                }

                string sFl = obj + ".S_PAR_F";
                string sUi = obj + ".S_PAR_UI";
                string rtFl = obj + ".RT_PAR_F";
                string rtUi = obj + ".RT_PAR_UI";
                int count = item.GetParamsManager().Items[0].Items.Length;

                for (int i = 1; i <= count; i++)
                {
                    string number = "[ " + i.ToString() + " ]";

                    if (cdbxTagView == true)
                    {
                        objNode.Nodes.Add(sFl + number, sFl + number);
                    }
                    else
                    {
                        objParamsNode.Nodes.Add(sFl + number, sFl + number);
                    }
                }

                count = item.GetParamsManager().Items[1].Items.Length;
                for (int i = 1; i <= count; i++)
                {
                    string number = "[ " + i.ToString() + " ]";

                    if (cdbxTagView == true)
                    {
                        objNode.Nodes.Add(sUi + number, sUi + number);
                    }
                    else
                    {
                        objParamsNode.Nodes.Add(sUi + number, sUi + number);
                    }
                }

                count = item.GetParamsManager().Items[2].Items.Length;
                for (int i = 1; i <= count; i++)
                {
                    string number = "[ " + i.ToString() + " ]";

                    if (cdbxTagView == true)
                    {
                        objNode.Nodes.Add(rtFl + number, rtFl + number);
                    }
                    else
                    {
                        objParamsNode.Nodes.Add(rtFl + number, rtFl + number);
                    }
                }

                count = item.GetParamsManager().Items[3].Items.Length;
                for (int i = 1; i <= count; i++)
                {
                    string number = "[ " + i.ToString() + " ]";

                    if (cdbxTagView == true)
                    {
                        objNode.Nodes.Add(rtUi + number, rtUi + number);
                    }
                    else
                    {
                        objParamsNode.Nodes.Add(rtUi + number, rtUi + number);
                    }
                }
                if (cdbxTagView == true)
                {
                    rootNode.Nodes.AddRange(new TreeNode[] { objNode });
                }
                else
                {
                    rootNode.Nodes.AddRange(new TreeNode[] { objModesNode, 
                        objOperStateNode, objAvOperNode, objStepsNode, 
                        objParamsNode });
                }
            }
        }

        /// <summary>
        /// Получение экземпляра класса.
        /// </summary>
        /// <returns>Единственный экземпляр класса.</returns>
        public static TechObjectManager GetInstance()
        {
            if (null == instance)
            {
                instance = new TechObjectManager();
            }

            return instance;
        }

        public List<TechObject> Objects
        {
            get
            {
                return objects;
            }
        }

        public void LoadFromLuaStr(string LuaStr, string projectName)
        {
            this.projectName = projectName;

            objects.Clear(); //Очищение объектов.

            //Сброс описания объектов.
            lua.DoString("init_tech_objects_modes = nil"); 
            try
            {
                //Выполнения Lua скрипта с описанием объектов.
                lua.DoString(LuaStr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ". Исправьте скрипт вручную.",
                    "Ошибка обработки Lua-скрипта");
            }
            try
            {
                //Создание объектов C# из скрипта Lua.
                object[] res = lua.DoString("if init ~= nil then init() end");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка обработки Lua-скрипта: " +
                    ex.Message + ".\n" +
                    "Source: " + ex.Source);
            }

        }

        public void LoadRestriction(string LuaStr)
        {

            lua.RegisterFunction("Get_TECH_OBJECT", this,
                GetType().GetMethod("GetTObject"));

            lua.DoFile(System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location) +
                "\\Lua\\sys_restriction.lua");

            //Выполнения Lua скрипта с описанием объектов.
            lua.DoString(LuaStr);
            lua.DoString("init_restriction()");
        }

        public List<TechObject> GetTechObj
        {
            get
            {
                return objects;
            }
        }

        public void Synch(int[] array)
        {
            foreach (TechObject obj in objects)
            {
                obj.Synch(array);
            }
        }

        public TreeView SaveDevicesAsTree()
        {
            TreeView tree = new TreeView();
            foreach (TechObject techObj in objects)
            {
                string techName = GetTechObjectN(techObj).ToString() + ". " +
                    techObj.EditText[0] + " " + techObj.TechNumber.ToString();
                TreeNode objNode = new TreeNode(techName);
                objNode.Tag = techObj;
                foreach (Mode mode in techObj.ModesManager.Modes)
                {
                    string modeName = techObj.ModesManager.GetModeN(mode)
                        .ToString() + ". " + mode.EditText[0];
                    TreeNode modeNode = new TreeNode();
                    Step commonStep = mode.MainSteps[0];
                    string[] res = new string[] { 
                        modeName, 
                        "",
                        commonStep.GetActions[ 0 ].EditText[ 1 ],
                        commonStep.GetActions[ 2 ].EditText[ 1 ],
                        commonStep.GetActions[ 3 ].EditText[ 1 ],
                        commonStep.GetActions[ 4 ].EditText[ 1 ],
                        commonStep.GetActions[ 5 ].EditText[ 1 ],
                        commonStep.GetActions[ 6 ].Items[ 0 ].EditText[ 1 ],
                        commonStep.GetActions[ 6 ].Items[ 1 ].EditText[ 1 ],
                        commonStep.GetActions[ 6 ].Items[ 2 ].EditText[ 1 ],
                        commonStep.GetActions[ 7 ].EditText[ 1 ]
                    };

                    modeNode.Tag = res;

                    for (int i = 1; i < mode.MainSteps.Count; i++)
                    {
                        commonStep = mode.MainSteps[i];
                        string stepName;
                        TreeNode stepNode = new TreeNode();

                        stepName = i.ToString() + ". " + commonStep.EditText[0];
                        string[] resStep = new string[] { 
                            stepName, 
                            commonStep.GetActions[ 0 ].EditText[ 1 ],
                            commonStep.GetActions[ 2 ].EditText[ 1 ],
                            commonStep.GetActions[ 3 ].EditText[ 1 ],
                            commonStep.GetActions[ 4 ].EditText[ 1 ], 
                            "", 
                            "", 
                            "", 
                            "",
                            ""
                        };

                        stepNode.Tag = resStep;
                        modeNode.Nodes.Add(stepNode);
                    }
                    objNode.Nodes.Add(modeNode);
                }

                tree.Nodes.Add(objNode);
            }
            return tree;
        }

        public TreeView SaveParamsAsTree()
        {
            TreeView tree = new TreeView();
            foreach (TechObject techObj in objects)
            {
                string techName = GetTechObjectN(techObj).ToString() + ". " +
                    techObj.EditText[0] + " " + techObj.TechNumber.ToString();
                TreeNode objNode = new TreeNode(techName);
                objNode.Tag = techObj;
                string[] ParamsType = { "S_PAR_F", "S_PAR_UI", "RT_PAR_F", 
                    "RT_PAR_UI" };
                for (int j = 0; j < techObj.Params.Items.Length; j++)
                {

                    if (techObj.Params.Items[j].Items != null)
                    {
                        TreeNode parTypeNode = new TreeNode(ParamsType[j]);
                        parTypeNode.Tag = ParamsType[j];
                        objNode.Nodes.Add(parTypeNode);
                        for (int i = 0; i < techObj.Params.Items[j].Items
                            .Length; i++)
                        {
                            Param param = techObj.Params.Items[j]
                                .Items[i] as Param;
                            string parName = (i + 1).ToString() + ". " + 
                                param.EditText[0] + " , " + param.GetMeter();
                            TreeNode parNode = new TreeNode(parName);
                            parNode.Tag = param;
                            parTypeNode.Nodes.Add(parNode);
                        }
                    }
                }

                tree.Nodes.Add(objNode);
            }
            return tree;
        }

        /// <summary>
        /// Проверка и исправление ограничений при удалении/перемещении объекта
        /// </summary>
        public void CheckRestriction(int prev, int curr)
        {
            foreach (TechObject to in objects)
            {
                to.CheckRestriction(prev, curr);
            }
        }

        /// <summary>
        /// Изменение номеров владельцев ограничений
        /// </summary>
        public void SetRestrictionOwner()
        {
            foreach (TechObject to in objects)
            {
                to.SetRestrictionOwner();
            }
        }

        /// <summary>
        /// Проверка и исправление ограничений при удалении/перемещении
        /// операции </summary>
        public void ChangeModeNum(int objNum, int prev, int curr)
        {
            foreach (TechObject to in objects)
            {
                to.ChangeModeNum(objNum, prev, curr);
            }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = "\"" + projectName + "\"";
                if (objects.Count > 0)
                {
                    res += " (" + objects.Count + ")";
                }

                return new string[] { res, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return objects.ToArray();
            }
        }

        override public bool Delete(object child)
        {
            TechObject techObject = child as TechObject;

            if (techObject != null)
            {
                int idx = objects.IndexOf(techObject) + 1;
                CheckRestriction(idx, -1);

                objects.Remove(techObject);

                SetRestrictionOwner();
                return true;
            }

            return false;
        }

        override public Editor.ITreeViewItem MoveDown(object child)
        {
            TechObject techObject = child as TechObject;

            if (techObject != null)
            {
                int index = objects.IndexOf(techObject);
                if (index <= objects.Count - 2)
                {
                    CheckRestriction(index + 1, index + 2);

                    objects.Remove(techObject);
                    objects.Insert(index + 1, techObject);

                    SetRestrictionOwner();
                    return objects[index];
                }
            }

            return null;
        }

        override public Editor.ITreeViewItem MoveUp(object child)
        {
            TechObject techObject = child as TechObject;

            if (techObject != null)
            {
                int index = objects.IndexOf(techObject);
                if (index > 0)
                {
                    CheckRestriction(index + 1, index);

                    objects.Remove(techObject);
                    objects.Insert(index - 1, techObject);

                    SetRestrictionOwner();
                    return objects[index];
                }
            }

            return null;
        }

        override public bool IsInsertableCopy
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem InsertCopy(object obj)
        {
            if (obj is TechObject)
            {
                int newN = 1;
                if (objects.Count > 0)
                {
                    newN = objects[objects.Count - 1].TechNumber + 1;
                }

                //Старый и новый номер объекта - для замены в ограничениях
                int oldObjN = GetTechObjectN(obj as TechObject);
                int newObjN = objects.Count + 1;

                TechObject newObject = (obj as TechObject).Clone(
                    GetTechObjectN, newN, oldObjN, newObjN);
                objects.Add(newObject);

                newObject.ChangeCrossRestriction();

                return newObject;
            }

            return null;
        }

        override public Editor.ITreeViewItem Replace(object child,
            object copyObject)
        {
            TechObject techObject = child as TechObject;
            if (copyObject is TechObject && techObject != null)
            {
                int newN = techObject.TechNumber;

                //Старый и новый номер объекта - для замены в ограничениях
                int oldObjN = GetTechObjectN(copyObject as TechObject);
                int newObjN = GetTechObjectN(child as TechObject);

                TechObject newObject = (copyObject as TechObject).Clone(
                    GetTechObjectN, newN, oldObjN, newObjN);
                int index = objects.IndexOf(techObject);
                objects.Remove(techObject);

                objects.Insert(index, newObject);

                index = objects.IndexOf(newObject);

                newObject.ChangeCrossRestriction(techObject);

                return newObject;
            }

            return null;
        }

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem Insert()
        {
            TechObject newTechObject = null;

            if (objects.Count > 0)
            {
                if (objects.Count == 1)
                {
                    newTechObject = new TechObject("Танк", GetTechObjectN, 1,
                        2, "TANK", -1, "TankObj", "");
                }
                else
                {
                    newTechObject = new TechObject(
                        objects[objects.Count - 1].EditText[0],
                        GetTechObjectN, objects[objects.Count - 1]
                        .TechNumber + 1,
                        objects[objects.Count - 1].TechType,
                        objects[objects.Count - 1].NameEplan,
                        objects[objects.Count - 1].CooperParamNumber,
                        objects[objects.Count - 1].NameBC,
                        objects[objects.Count - 1].AttachedObjects);
                }
            }
            else
            {
                newTechObject =
                    new TechObject("Гребенка", GetTechObjectN, 1, 1, "COMB", 
                    -1, "Grebenka", "");
            }

            objects.Add(newTechObject);
            return newTechObject;
        }

        public void SetCDBXTagView(bool combineTag)
        {
            cdbxTagView = combineTag;
        }

        #endregion

        private bool cdbxTagView;
        private LuaInterface.Lua lua;              /// Экземпляр Lua.
        private List<TechObject> objects;          /// Технологические объекты.
        private static TechObjectManager instance; /// Единственный экземпляр.
        private string projectName;                /// Имя проекта.
    }
}
