using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyEPlanner;

namespace TechObject
{
    /// <summary>
    /// Интерфейс менеджера технологических объектов проекта.
    /// </summary>
    public interface ITechObjectManager
    {
        void LoadDescription(string LuaStr, string projectName);
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

            InitTechObjectsLuaScript();

            objects = new List<TechObject>();
            cdbxTagView = false;
        }

        /// <summary>
        /// Инициализировать Lua-скрипт для чтения описания объектов
        /// </summary>
        private void InitTechObjectsLuaScript()
        {
            const string fileName = "sys.lua";
            string sysLuaPath = Path.Combine(ProjectManager.GetInstance()
                .SystemFilesPath, fileName);
            lua.DoFile(sysLuaPath);
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
            string res = "";
            foreach (TechObject obj in objects)
            {
                res += obj.SaveAsLuaTable(prefix + "\t\t");
            }
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
            var res = "";
            foreach (TechObject obj in objects)
            {
                res += obj.SaveRestrictionAsLua(prefix + "\t");
            }
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
        /// <param name="globalNumber">Глобальный номер объекта, используется
        /// при импорте из файла</param>
        public TechObject AddObject(int globalNumber, int techN, string name, 
            int techType, string nameEplan, int cooperParamNumber, 
            string NameBC, string baseTechObjectName, string attachedObjects)
        {
            // globalNumber игнорируется в этом методе, но используется при
            // импорте описания из файла (аналогичная сигнатура, другое тело).
            TechObject obj = new TechObject(name, GetTechObjectN, techN,
                techType, nameEplan.ToUpper(), cooperParamNumber, NameBC, 
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

        #region XML Report
        /// <summary>
        /// Формирование узлов для операций, шагов и параметров объектов.
        /// </summary>
        /// <param name="rootNode">корневой узел</param>
        public void GetObjectForXML(TreeNode rootNode)
        {
            GenerateSystemNode(rootNode);      
            for (int num = 1; num <= Objects.Count; num++)
            {
                TechObject item = Objects[num - 1];

                var objNode = new TreeNode($"{item.NameBC}{item.TechNumber}");

                var objModesNode = new TreeNode(item.NameBC + 
                    item.TechNumber.ToString() + "_Операции");
                var objOperStateNode = new TreeNode(item.NameBC + 
                    item.TechNumber.ToString() + "_Состояния_Операций");
                var objAvOperNode = new TreeNode(item.NameBC + 
                    item.TechNumber.ToString() + "_Доступность");
                var objStepsNode = new TreeNode(item.NameBC + 
                    item.TechNumber.ToString() + "_Шаги");
                var objSingleStepsNode = new TreeNode(item.NameBC +
                    item.TechNumber.ToString() + "_Одиночные_Шаги");
                var objParamsNode = new TreeNode(item.NameBC + 
                    item.TechNumber.ToString() + "_Параметры");

                string objName = GenerateObjectName(item, num);
                GenerateCMDTags(objName, objNode, objModesNode);
                GenerateSTTags(item, objName, objNode, objModesNode);
                GenerateModesOpersAvsStepsTags(item, objName, objNode, 
                    objModesNode, objOperStateNode, objAvOperNode, 
                    objStepsNode);

                GenerateSingleStepsTags(item, objName, objNode, 
                    objSingleStepsNode);

                string sFl = objName + ".S_PAR_F";
                int count = item.GetParams().Items.Length;
                GenerateParametersTags(count, objNode, objParamsNode, sFl);

                var singleNodes = new TreeNode[] { objModesNode, 
                    objOperStateNode, objAvOperNode, objStepsNode, 
                    objSingleStepsNode, objParamsNode};
                GenerateRootNode(rootNode, objNode, singleNodes);

                if(item.BaseTechObject.IsPID)
                {
                    GeneratePIDNode(rootNode, item.GlobalNumber);
                }
            }
        }

        /// <summary>
        /// Генерация системных тегов
        /// </summary>
        /// <param name="rootNode">Узловой узел</param>
        private void GenerateSystemNode(TreeNode rootNode)
        {
            var systemNode = new TreeNode("SYSTEM");
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
            rootNode.Nodes.Add(systemNode);
        }

        /// <summary>
        /// Генерация имени объекта
        /// </summary>
        /// <param name="item">Объект</param>
        /// <param name="itemNumber">Глобальный номер</param>
        /// <returns></returns>
        private string GenerateObjectName(TechObject item, int itemNumber)
        {
            if (cdbxNewNames == true)
            {
                return item.NameBC.ToUpper() + item.TechNumber.ToString();
            }
            else
            {
                return "OBJECT" + itemNumber.ToString();
            }
        }

        /// <summary>
        /// Генерация CMD-тэгов для объекта
        /// </summary>
        /// <param name="obj">Имя объекта</param>
        private void GenerateCMDTags(string obj, TreeNode objNode, 
            TreeNode objModesNode)
        {
            string tagName = obj + ".CMD";
            if (cdbxTagView == true)
            {
                objNode.Nodes.Add(tagName, tagName);
            }
            else
            {
                objModesNode.Nodes.Add(tagName, tagName);
            }
        }

        /// <summary>
        /// Генерация ST-тегов для проекта
        /// </summary>
        /// <param name="item">Объект</param>
        /// <param name="objName">Имя объекта</param>
        private void GenerateSTTags(TechObject item, string objName, 
            TreeNode objNode, TreeNode objModesNode)
        {
            // 33 - Magic number
            int stCount = item.ModesManager.Modes.Count / 33;
            for (int i = 0; i <= stCount; i++)
            {
                string number = "[ " + (i + 1).ToString() + " ]";
                string fullTagName = objName + ".ST" + number;
                if (cdbxTagView == true)
                {
                    objNode.Nodes.Add(fullTagName, fullTagName);
                }
                else
                {
                    objModesNode.Nodes.Add(fullTagName, fullTagName);
                }
            }
        }

        /// <summary>
        /// Генерация тэгов по операциям, шагам, доступности, состояниям
        /// </summary>
        /// <param name="item">Объект</param>
        /// <param name="itemNumber">Глобальный номер</param>
        private void GenerateModesOpersAvsStepsTags(TechObject item, string obj,
            TreeNode objNode, TreeNode objModesNode, TreeNode objOperStateNode,
            TreeNode objAvOperNode, TreeNode objStepsNode)
        {
            string mode = obj + ".MODES";
            string step = mode + "_STEPS";
            string oper = obj + ".OPERATIONS";
            string av = obj + ".AVAILABILITY";
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
                    objOperStateNode.Nodes.Add(oper + number, oper + number);
                    objAvOperNode.Nodes.Add(av + number, av + number);
                    objStepsNode.Nodes.Add(step + number, step + number);
                }
            }
        }

        /// <summary>
        /// Генерация одиночных шагов для объекта
        /// </summary>
        /// <param name="item">Объект</param>
        /// <param name="objName">Имя объекта</param>
        /// <param name="objSingleStepsNode"></param>
        private void GenerateSingleStepsTags(TechObject item, string objName, 
            TreeNode objNode, TreeNode objSingleStepsNode)
        {
            List<Mode> modes = item.ModesManager.Modes;
            for(int modeNum = 1; modeNum <= modes.Count; modeNum++)
            {
                // Шаги "Пауза" и "Остановка" игнорируются
                int stepsCount = modes[modeNum - 1].MainSteps.Count;
                for (int stepNum = 1; stepNum <= stepsCount; stepNum++)
                {
                    string stepTag = $"{objName}.STEPS{modeNum}[ {stepNum} ]";
                    if(cdbxTagView == true)
                    {
                        objNode.Nodes.Add(stepTag, stepTag);
                    }
                    else
                    {
                        objSingleStepsNode.Nodes.Add(stepTag, stepTag);
                    }
                }
            }
        }

        /// <summary>
        /// Генерация тэгов параметров объекта
        /// </summary>
        /// <param name="paramsCount">Количество параметров</param>
        /// <param name="objNode"></param>
        /// <param name="tagName">Имя тэга</param>
        private void GenerateParametersTags(int paramsCount, TreeNode objNode,
            TreeNode objParamsNode, string tagName)
        {
            for (int i = 1; i <= paramsCount; i++)
            {
                string number = "[ " + i.ToString() + " ]";
                string fullTagName = tagName + number;
                if (cdbxTagView == true)
                {
                    objNode.Nodes.Add(fullTagName, fullTagName);
                }
                else
                {
                    objParamsNode.Nodes.Add(fullTagName, fullTagName);
                }
            }
        }

        /// <summary>
        /// Генерация главного узла для экспорта в XML
        /// </summary>
        private void GenerateRootNode(TreeNode rootNode, TreeNode objNode,
            TreeNode[] singleNodes)
        {
            if (cdbxTagView == true)
            {
                rootNode.Nodes.Add(objNode);
            }
            else
            {
                rootNode.Nodes.AddRange(singleNodes);
            }
        }

        /// <summary>
        /// Генерация объекта-ПИДа
        /// </summary>
        /// <param name="rootNode">Главный узел</param>
        private void GeneratePIDNode(TreeNode rootNode, int num)
        {
            string tagName = $"PID{num}";
            TreeNode pidNode;
            if (cdbxTagView == true)
            {
                pidNode = new TreeNode($"{tagName}");
            }
            else
            {
                pidNode = new TreeNode($"{tagName}_Параметры");
            }

            const int rtParCount = 2;
            for (int i = 1; i <= rtParCount; i++)
            {
                string nodeDescription = $"{tagName}.RT_PAR_F[ {i} ]";
                pidNode.Nodes.Add(nodeDescription, nodeDescription);
            }

            const int sParCount = 14;
            for (int i = 1; i <= sParCount; i++)
            {
                string nodeDescription = $"{tagName}.S_PAR_F[ {i} ]";
                pidNode.Nodes.Add(nodeDescription, nodeDescription);
            }

            rootNode.Nodes.Add(pidNode);
        }
        #endregion

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

        public void LoadDescription(string LuaStr, string projectName)
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

        /// <summary>
        /// Загрузка ограничений объектов
        /// </summary>
        /// <param name="LuaStr">Описание ограничений объектов</param>
        public void LoadRestriction(string LuaStr)
        {
            lua.RegisterFunction("Get_TECH_OBJECT", this,
                GetType().GetMethod("GetTObject"));

            string fileName = "sys_restriction.lua";
            string pathToRestrictionInitializer = Path.Combine(
                ProjectManager.GetInstance().SystemFilesPath, fileName);
            lua.DoFile(pathToRestrictionInitializer);

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

        /// <summary>
        /// Удалить этот агрегат из привязки к аппарату
        /// </summary>
        /// <param name="techObject">Агрегат</param>
        private void RemoveAttachingToUnit(TechObject techObject)
        {
            int objNum = techObject.GlobalNumber;
            foreach(var obj in Objects) 
            {
                if(obj.AttachedObjects.Value == "")
                {
                    continue;
                }

                List<int> attachedObjectsNums = obj.AttachedObjects.Value
                    .Split(' ')
                    .Select(int.Parse).ToList();
                if (attachedObjectsNums.Contains(objNum))
                {
                    attachedObjectsNums.Remove(objNum);
                    obj.AttachedObjects
                        .SetNewValue(string.Join(" ", attachedObjectsNums));
                }
            }
        }

        /// <summary>
        /// Изменение привязки объектов при перемещении объекта по дереву
        /// </summary>
        /// <param name="newIndex">Новый индекс объекта</param>
        /// <param name="oldIndex">Старый индекс объекта</param>
        private void ChangeAttachedObjectsAfterMove(int oldIndex, int newIndex)
        {
            int oldObjNum = oldIndex + 1;
            int newObjNum = newIndex + 1;
            foreach (var techObj in Objects)
            {
                string attachingObjectsStr = techObj.AttachedObjects.Value;
                string[] attachingObjectsArr = attachingObjectsStr.Split(' ');
                for(int index = 0; index < attachingObjectsArr.Length; index++)
                {
                    if(attachingObjectsArr[index] == newObjNum.ToString())
                    {
                        attachingObjectsArr[index] = oldObjNum.ToString();
                    }
                    else if (attachingObjectsArr[index] == oldObjNum.ToString())
                    {
                        attachingObjectsArr[index] = newObjNum.ToString();
                    }
                }
                techObj.AttachedObjects
                    .SetValue(string.Join(" ", attachingObjectsArr));
            }
        }

        /// <summary>
        /// Изменение привязки объектов при удалении объекта из дерева
        /// </summary>
        /// <param name="deletedObjectNum">Номер удаленного объекта</param>
        private void ChangeAttachedObjectsAfterDelete(int deletedObjectNum)
        {
            foreach(var techObj in Objects)
            {
                if (techObj.AttachedObjects.Value == "" ||
                    techObj.BaseTechObject.IsAttachable)
                {
                    continue;
                }

                string attachingObjectsStr = techObj.AttachedObjects.Value;
                int[] attachingObjectsArr = attachingObjectsStr.Split(' ')
                    .Select(int.Parse).ToArray();
                for (int index = 0; index < attachingObjectsArr.Length; index++)
                {
                    int attachedObjectNum = attachingObjectsArr[index];
                    if (attachedObjectNum > deletedObjectNum)
                    {
                        attachingObjectsArr[index] = attachedObjectNum - 1;
                    }
                }
                techObj.AttachedObjects
                    .SetValue(string.Join(" ", attachingObjectsArr));
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
                if (techObject.BaseTechObject.IsAttachable)
                {
                    RemoveAttachingToUnit(techObject);
                }

                int idx = objects.IndexOf(techObject) + 1;
                CheckRestriction(idx, -1);

                objects.Remove(techObject);

                SetRestrictionOwner();
                ChangeAttachedObjectsAfterDelete(idx);
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
                    ChangeAttachedObjectsAfterMove(index, index + 1);
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
                    ChangeAttachedObjectsAfterMove(index, index - 1);
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
                newObject.Equipment.ModifyDevNames();

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
                    new TechObject("Мастер", GetTechObjectN, 1, 1, "MASTER", 
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
