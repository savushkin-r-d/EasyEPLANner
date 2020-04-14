﻿using LuaInterface;
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
        void GetObjectForXML(TreeNode rootNode);
        void SetCDBXTagView(bool combineTag);
        string SaveRestrictionAsLua(string prefixStr);
        List<TechObject> GetTechObjects();
        void SetCDBXNewNames(bool useNewNames);
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

            const string spireLicense = "Spire.License.dll";
            const string spireXLS = "Spire.XLS.dll";
            const string spirePDF = "Spire.Pdf.dll";
            
            string assemblyPath = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
            string systemFilesPath = assemblyPath + "\\Lua";
            
            string SpireLicensePath = Path.Combine(assemblyPath, spireLicense);
            string SpireXLSPath = Path.Combine(assemblyPath, spireXLS);
            string SpirePDFPath = Path.Combine(assemblyPath, spirePDF);

            if (File.Exists(SpireLicensePath) == false ||
                File.Exists(SpireXLSPath) == false ||
                File.Exists(SpirePDFPath) == false)
            {
                var files = new string[] { spireLicense, spireXLS, spirePDF };
                CopySpireXLSFiles(assemblyPath, files);
            }

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
        /// <param name="shadowAssemblySystemFilesDir">Путь к Lua файлам
        /// в теневом хранилище Eplan</param>
        /// </summary>
        private void CopySystemFiles(string shadowAssemblySystemFilesDir)
        {
            const string luaDirectory = "\\Lua";
            Directory.CreateDirectory(shadowAssemblySystemFilesDir);

            string systemFilesPath = Path.GetDirectoryName(EasyEPlanner.
                AddInModule.OriginalAssemblyPath) + luaDirectory;

            var systemFilesDirectory = new DirectoryInfo(
                systemFilesPath);
            FileInfo[] systemFiles = systemFilesDirectory.GetFiles();
            foreach (FileInfo systemFile in systemFiles)
            {
                string pathToFile = Path.Combine(shadowAssemblySystemFilesDir,
                    systemFile.Name);
                systemFile.CopyTo(pathToFile, true);
            }
        }

        /// <summary>
        /// Копировать файлы библиотек Spire XLS
        /// </summary>
        /// <param name="shadowAssemblySpireFilesDir">Путь к библиотекам
        /// в теневом хранилище Eplan</param>
        /// <param name="files">Имена файлов</param>
        private void CopySpireXLSFiles(string shadowAssemblySpireFilesDir,
            string[] files)
        {
            string originalPath = Path.GetDirectoryName(EasyEPlanner
                .AddInModule.OriginalAssemblyPath);
            var libsDir = new DirectoryInfo(originalPath);
            foreach(FileInfo file in libsDir.GetFiles())
            {
                if (files.Contains(file.Name))
                {
                    string path = Path.Combine(shadowAssemblySpireFilesDir, 
                        file.Name);
                    file.CopyTo(path, true);
                }
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

            errors += CheckTypeField();
            errors += CheckObjectMonitorField();

            foreach (var obj in Objects)
            {
                errors += obj.Check();
            }

            return errors;

        }

        /// <summary>
        /// Проверить поле тип у объекта.
        /// </summary>
        private string CheckTypeField()
        {
            var errorsList = new List<string>();
            foreach(var obj in Objects)
            {
                var matches = Objects.Where(x => x.TechType == obj.TechType &&
                x.TechNumber == obj.TechNumber)
                    .Select(x => GetTechObjectN(x))
                    .ToArray();

                if (matches.Count() > 1)
                {
                    errorsList.Add($"У объектов {string.Join(",", matches)} " +
                        $"совпадает поле \"Тип\"\n");
                }
            }

            errorsList = errorsList.Distinct().ToList();
            return string.Join("", errorsList);
        }

        /// <summary>
        /// Проверить поле имени объекта Monitor у объекта.
        /// </summary>
        private string CheckObjectMonitorField()
        {
            var errorsList = new List<string>();
            foreach(var obj in Objects)
            {
                var matches = Objects.Where(x => x.NameBC == obj.NameBC &&
                x.TechNumber == obj.TechNumber)
                    .Select(x => GetTechObjectN(x))
                    .ToArray();

                if (matches.Count() > 1)
                {
                    errorsList.Add($"У объектов {string.Join(",", matches)} " +
                        $"совпадает поле \"Имя объекта Monitor\"\n");
                }
            }

            errorsList = errorsList.Distinct().ToList();
            return string.Join("", errorsList);
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

                string obj = "";
                if (cdbxNewNames == true)
                {
                    obj = item.NameBC.ToUpper() + item.TechNumber.ToString();
                }
                else
                {
                    obj = "OBJECT" + num.ToString();
                }

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
                        objNode.Nodes.Add(obj + ".ST" + number, 
                            obj + ".ST" + number);
                    }
                    else
                    {
                        objModesNode.Nodes.Add(obj + ".ST" + number,
                            obj + ".ST" + number);
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
                int count = item.GetParamsManager().Float.Items.Length;

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
                    rootNode.Nodes.AddRange(new TreeNode[] 
                    { 
                        objModesNode, 
                        objOperStateNode, 
                        objAvOperNode, 
                        objStepsNode, 
                        objParamsNode 
                    });
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

        /// <summary>
        /// Получить количество аппаратов в проекте
        /// </summary>
        public int UnitsCount
        {
            get
            {
                var units = objects.Where(x => x.S88Level == 1).ToList();
                return units.Count;
            }
        }

        /// <summary>
        /// Получить количество агрегатов в проекте
        /// </summary>
        public int EquipmentModulesCount
        {
            get
            {
                var equipmentModules = objects.Where(x => x.S88Level == 2)
                    .ToList();
                return equipmentModules.Count;
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
                        objects[objects.Count - 1].AttachedObjects.Value);
                }
            }
            else
            {
                newTechObject =
                    new TechObject("Мастер", GetTechObjectN, 1, 1, "Master", 
                    -1, "MasterObj", "");
            }

            objects.Add(newTechObject);
            return newTechObject;
        }

        public void SetCDBXTagView(bool combineTag)
        {
            cdbxTagView = combineTag;
        }

        public void SetCDBXNewNames(bool useNewNames)
        {
            cdbxNewNames = useNewNames;
        }

        #endregion

        private bool cdbxTagView;
        private bool cdbxNewNames;

        private LuaInterface.Lua lua;              /// Экземпляр Lua.
        private List<TechObject> objects;          /// Технологические объекты.
        private static TechObjectManager instance; /// Единственный экземпляр.
        private string projectName;                /// Имя проекта.
    }
}
