using EasyEPlanner;
using Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LuaInterface;
using System.Windows.Forms;

namespace TechObject
{
    /// <summary>
    /// Получение номера объекта в списке. Нумерация начинается с 1.
    /// </summary>
    public delegate int GetN(object obj);

    /// <summary>
    /// Менеджер объектов редактора.
    /// </summary>
    public class TechObjectManager : TreeViewItem, ITechObjectManager
    {
        private TechObjectManager()
        {
            InitLua();

            treeObjects = new List<ITreeViewItem>();
            techObjects = new List<TechObject>();
            techObjectManagerChecker = new TechObjectChecker(this);
            techObjectXMLMaker = new TechObjectXMLMaker(this);
        }

        #region Инициализация LUA-скриптов
        /// <summary>
        /// Инициализировать Lua-скрипт для чтения описания объектов
        /// </summary>
        private void InitLua()
        {
            lua = new Lua();
            InitReadDescriptionLuaScript();
            InitReadRestrictionsLuaScript();
        }

        /// <summary>
        /// Загрузка скрипта для чтения описания объектов.
        /// </summary>
        private void InitReadDescriptionLuaScript()
        {
            lua.RegisterFunction("ADD_TECH_OBJECT", this,
                GetType().GetMethod("AddObject"));
            const string sysFileName = "sys.lua";
            string sysLuaPath = Path.Combine(ProjectManager.GetInstance()
                .SystemFilesPath, sysFileName);
            lua.DoFile(sysLuaPath);
        }

        /// <summary>
        /// Загрузка скрипта для чтения ограничений объектов.
        /// </summary>
        private void InitReadRestrictionsLuaScript()
        {
            lua.RegisterFunction("Get_TECH_OBJECT", this,
                GetType().GetMethod("GetTObject"));
            string restrictionFileName = "sys_restriction.lua";
            string pathToRestrictionInitializer = Path
                .Combine(ProjectManager.GetInstance().SystemFilesPath,
                restrictionFileName);
            lua.DoFile(pathToRestrictionInitializer);
        }
        #endregion

        /// <summary>
        /// Получить экземпляр класса менеджера объектов
        /// </summary>
        /// <returns></returns>
        public static TechObjectManager GetInstance()
        {
            if (instance == null)
            {
                instance = new TechObjectManager();
            }

            return instance;
        }

        /// <summary>
         /// Получение объекта по номеру
         /// </summary>
         /// <param name="i">Номер объекта</param>
         /// <returns></returns>
        public TechObject GetTObject(int i)
        {
            if (techObjects != null && techObjects.Count >= i)
            {
                return techObjects[i - 1];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Получение номера операции в списке операций. 
        /// Нумерация начинается с 1.
        /// </summary>
        /// <param name="mode">Операция, номер которой хотим получить.</param>
        /// <returns>Номер заданной операции.</returns>
        public int GetTechObjectN(object techObject)
        {
            return techObjects.IndexOf(techObject as TechObject) + 1;
        }

        /// <summary>
        /// Получить номер объекта по его отображаемому имени в дереве.
        /// </summary>
        /// <param name="displayText">Отображаемый текст</param>
        /// <returns></returns>
        public int GetTechObjectN(string displayText)
        {
            TechObject findedObject = TechObjects
                .Where(x => x.DisplayText[0] == displayText).FirstOrDefault();

            if(findedObject != null)
            {
                return techObjects.IndexOf(findedObject) + 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Проверка и исправление ограничений при удалении/перемещении
        /// операции 
        /// </summary>
        public void ChangeModeNum(int objNum, int oldNum, int newNum)
        {
            foreach (TechObject to in TechObjects)
            {
                to.ChangeModeNum(objNum, oldNum, newNum);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = "";
            foreach (TechObject obj in TechObjects)
            {
                int num = TechObjects.IndexOf(obj) + 1;
                res += obj.SaveAsLuaTable(prefix + "\t\t", num);
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
            foreach (TechObject obj in TechObjects)
            {
                int num = TechObjects.IndexOf(obj) + 1;
                res += obj.SaveRestrictionAsLua(prefix + "\t", num);
            }
            res = res.Replace("\t", "    ");
            return res;
        }

        #region Загрузка описания из LUA
        /// <summary>
        /// Загрузка описания проекта из строки
        /// </summary>
        /// <param name="LuaStr">Строка с описанием</param>
        /// <param name="projectName">Имя проекта</param>
        public void LoadDescription(string LuaStr, string projectName)
        {
            ProjectName = projectName;
            treeObjects.Clear();
            techObjects.Clear();

            //Сброс описания объектов.
            lua.DoString("init_tech_objects_modes = nil");
            try
            {
                //Выполнения Lua скрипта с описанием объектов.
                lua.DoString(LuaStr);
            }
            catch (Exception ex)
            {
                string message = $"{ex.Message}. Ошибка обработки " +
                    $"Lua-скрипта описания проекта. " +
                    $"Данные не будут сохранены.";
                throw new Exception(message);
            }
            try
            {
                //Создание объектов C# из скрипта Lua.
                object[] res = lua.DoString("if init ~= nil then init() end");
            }
            catch (Exception ex)
            {
                string message = $"{ex.Message}. Ошибка обработки " +
                    $"Lua-скрипта для загрузки данных проекта. " +
                    $"Данные не будут сохранены.\n" +
                    $"Source: {ex.Source}";
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Загрузка ограничений объектов
        /// </summary>
        /// <param name="LuaStr">Описание ограничений объектов</param>
        public void LoadRestriction(string LuaStr)
        {
            //Выполнения Lua скрипта с описанием объектов.
            lua.DoString(LuaStr);
            lua.DoString("init_restriction()");
        }
        #endregion

        #region Добавление объекта из LUA
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

            var baseTechObject = BaseTechObjectManager.GetInstance()
                .GetTechObject(baseTechObjectName);
            // getN - null т.к он будет другой, ниже по функциям.
            TechObject obj = new TechObject(name, null, techN,
                techType, nameEplan.ToUpper(), cooperParamNumber, NameBC,
                attachedObjects, baseTechObject);

            AddObject(obj);

            return obj;
        }

        /// <summary>
        /// Добавление технологического объекта.
        /// </summary>
        /// <param name="obj">Добавляемый объект</param>
        private void AddObject(TechObject obj)
        {
            if (obj.BaseTechObject != null)
            {
                AddIdentifiedObject(obj);
            }
            else
            {
                AddUnidentifiedObject(obj);
            }

            techObjects.Add(obj);
        }

        /// <summary>
        /// Добавить опознанный объект при загрузке из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        private void AddIdentifiedObject(TechObject obj)
        {
            BaseTechObject baseTechObject = obj.BaseTechObject;
            var type = (BaseTechObjectManager.ObjectType)baseTechObject
                .S88Level;
            string name = BaseTechObjectManager.GetInstance()
                        .GetS88Name(baseTechObject.S88Level);
            switch (type)
            {
                case BaseTechObjectManager.ObjectType.ProcessCell:
                     AddProcessCell(obj);
                    break;

                case BaseTechObjectManager.ObjectType.Unit:
                    AddS88Object(obj, name);
                    break;

                case BaseTechObjectManager.ObjectType.Aggregate:
                    AddS88Object(obj, name);
                    break;

                case BaseTechObjectManager.ObjectType.UserObject:
                    AddUserObject(obj);
                    break;
            }
        }

        /// <summary>
        /// Добавить ячейку процесса (мастер) из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <returns></returns>
        private void AddProcessCell(TechObject obj)
        {
            var masterItem = treeObjects.Where(x => x is ProcessCell)
                        .FirstOrDefault() as ProcessCell;
            if (masterItem == null)
            {
                masterItem = new ProcessCell(instance);
                treeObjects.Add(masterItem);
            }

            masterItem.AddObjectWhenLoadFromLua(obj);
        }

        /// <summary>
        /// Добавить аппарат из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="name">Имя объекта</param>
        /// <returns></returns>
        private void AddS88Object(TechObject obj, string name)
        {
            var s88Item = treeObjects
                .Where(x => x is S88Object && x.DisplayText[0].Contains(name))
                .FirstOrDefault() as S88Object;
            if (s88Item == null)
            {
                s88Item = new S88Object(name, instance);
                treeObjects.Add(s88Item);
            }

            s88Item.AddObjectWhenLoadFromLua(obj);
        }

        /// <summary>
        /// Добавить пользовательский объект из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        private void AddUserObject(TechObject obj)
        {
            var userObject = treeObjects.Where(x => x is UserObject)
                .FirstOrDefault() as UserObject;
            if(userObject == null)
            {
                userObject = new UserObject(instance);
                treeObjects.Add(userObject);
            }

            userObject.AddObjectWhenLoadFromLua(obj);
        }

        /// <summary>
        /// Добавить неопознанный объект при добавлении из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        private void AddUnidentifiedObject(TechObject obj)
        {
            var unidentifiedObject = treeObjects
                .Where(x => x is Unidentified)
                .FirstOrDefault() as Unidentified;
            if (unidentifiedObject == null)
            {
                unidentifiedObject = new Unidentified(instance);
                treeObjects.Add(unidentifiedObject);
            }

            unidentifiedObject.AddUnidentifiedObject(obj);
        }
        #endregion

        public void ImportObject(TechObject importingObject)
        {
            AddObject(importingObject);
        }

        /// <summary>
        /// Проверка технологического объекта на правильность ввода и др.
        /// </summary>
        /// <returns>Строка с ошибками</returns>
        public string Check()
        {
            return techObjectManagerChecker.Check();
        }

        /// <summary>
        /// Формирование узлов для операций, шагов и параметров объектов.
        /// </summary>
        /// <param name="rootNode">корневой узел</param>
        /// <param name="combineTags">Сгруппировать тэги в один подтип</param>
        /// <param name="useNewNames">Использовать имена объектов вместо
        /// OBJECT</param>
        public void GetObjectForXML(TreeNode rootNode, bool combineTags,
            bool useNewNames)
        {
            techObjectXMLMaker.GetObjectForXML(rootNode, combineTags,
                useNewNames);
        }

        #region Синхронизация устройств в объектах
        /// <summary>
        /// Синхронизация устройств в объектах
        /// </summary>
        /// <param name="array">Индексная таблица</param>
        public void Synch(int[] array)
        {
            foreach (TechObject obj in TechObjects)
            {
                obj.Synch(array);
            }
        }
        #endregion

        #region Реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                string res = "\"" + ProjectName + "\"";
                if (treeObjects.Count > 0)
                {
                    res += " (" + treeObjects.Count + ")";
                }

                return new string[] { res, "" };
            }
        }

        public override ITreeViewItem[] Items
        {
            get
            {
                return treeObjects.ToArray();
            }
        }

        public override ImageIndexEnum ImageIndex
        {
            get
            {
                return ImageIndexEnum.TechObjectManager;
            }
        }

        public override bool IsInsertableCopy
        {
            get
            {
                return true;
            }
        }

        public override ITreeViewItem InsertCopy(object obj)
        {
            var techObj = obj as TechObject;
            if(techObj != null && techObj.MarkToCut)
            {
                ChooseObjectTypes(out string selectedType,
                    out string selectedSubType);
                if (selectedType != null && selectedSubType != null)
                {
                    ITreeViewItem insertedItem = InsertType(selectedType,
                        techObj);

                    insertedItem.AddParent(this);
                    return insertedItem;
                }
            }

            return null;
        }

        public override bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        public override ITreeViewItem Insert()
        {
            ChooseObjectTypes(out string selectedType,
                out string selectedSubType);
            if (selectedType != null && selectedSubType != null)
            {
                ITreeViewItem insertedItem = InsertType(selectedType);

                insertedItem.AddParent(this);
                return insertedItem;
            }

            return null;
        }

        /// <summary>
        /// Выбор типа и подтипа объекта на форме.
        /// </summary>
        /// <param name="selectedType">Тип</param>
        /// <param name="selectedSubType">Подтип</param>
        private void ChooseObjectTypes(out string selectedType,
            out string selectedSubType)
        {
            var objectsAdderForm = new ObjectsAdder();
            objectsAdderForm.ShowDialog();
            selectedType = ObjectsAdder.LastSelectedType;
            selectedSubType = ObjectsAdder.LastSelectedSubType;
        }

        private ITreeViewItem InsertType(string selectedType,
            TechObject techObj = null)
        {
            ITreeViewItem treeItem = GetTreeItem(selectedType);
            ITreeViewItem innerItem;

            bool needInsert = techObj == null;
            if (needInsert)
            {
                innerItem = treeItem.Insert();
            }
            else
            {
                innerItem = treeItem.InsertCopy(techObj);
            }

            if (innerItem != null)
            {
                if (!treeObjects.Contains(treeItem))
                {
                    treeObjects.Add(treeItem);
                }

                return treeItem;
            }

            return null;
        }

        /// <summary>
        /// Создать объект дерева, описывающий базу по S88.
        /// </summary>
        /// <param name="selectedType">Выбранный на форме тип объекта</param>
        /// <returns></returns>
        private ITreeViewItem GetTreeItem(string selectedType)
        {
            ITreeViewItem treeItem = treeObjects
                .Where(x => x.EditText[0] == selectedType)
                .FirstOrDefault();
            if (treeItem == null)
            {
                switch(selectedType)
                {
                    case ProcessCell.Name:
                        return new ProcessCell(instance);

                    case UserObject.Name:
                        return new UserObject(instance);

                    default:
                        return new S88Object(selectedType, instance);
                }
            }
            else
            {
                return treeItem;
            }
        }

        public override bool Delete(object child)
        {
            var treeViewItem = child as ITreeViewItem;
            if (treeViewItem != null && treeObjects.Contains(treeViewItem))
            {
                foreach (var item in treeViewItem.Items)
                {
                    treeViewItem.Delete(item);
                }

                if (treeViewItem.Items.Count() == 0)
                {
                    treeObjects.Remove(child as ITreeViewItem);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Корректировка объектов при их удалении или перемещении
        public void CheckRestriction(int oldNum, int newNum)
        {
            foreach (TechObject techObject in TechObjects)
            {
                techObject.CheckRestriction(oldNum, newNum);
            }
        }

        public void ChangeAttachedObjectsAfterDelete(int deletedObjectNum)
        {
            foreach (var obj in TechObjects)
            {
                ChangeAttachedObjectAfterDelete(obj.AttachedObjects,
                    deletedObjectNum);

                if (obj.BaseTechObject?.ObjectGroupsList != null)
                {
                    foreach (var group in obj.BaseTechObject.ObjectGroupsList)
                    {
                        ChangeAttachedObjectAfterDelete(group,
                            deletedObjectNum);
                    }
                }
            }
        }

        /// <summary>
        /// Изменение привязки объекта при удалении объекта из дерева 
        /// </summary>
        /// <param name="attachedObjects">Элемент для обработки</param>
        /// <param name="deletedObjectNum">Номер удаленного объекта</param>
        private void ChangeAttachedObjectAfterDelete(
            AttachedObjects attachedObjects, int deletedObjectNum)
        {
            if (attachedObjects?.Value == string.Empty)
            {
                return;
            }

            string attachingObjectsStr = attachedObjects.Value;
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
            attachedObjects.SetValue(string.Join(" ", attachingObjectsArr));
        }

        public void ChangeAttachedObjectsAfterMove(int oldNum, int newNum)
        {
            foreach (var obj in TechObjects)
            {
                ChangeAttachedObjectAfterMove(obj.AttachedObjects, oldNum,
                    newNum);

                if (obj.BaseTechObject?.ObjectGroupsList != null)
                {
                    foreach (var group in obj.BaseTechObject.ObjectGroupsList)
                    {
                        ChangeAttachedObjectAfterMove(group, oldNum, newNum);
                    }
                }
            }
        }

        /// <summary>
        /// Изменение привязки объекта при перемещении объекта по дереву
        /// </summary>
        /// <param name="attachedObjects">Элемент с объектами</param>
        /// <param name="oldNum">Старый номер объекта</param>
        /// <param name="newNum">Новый номер объекта</param>
        private void ChangeAttachedObjectAfterMove(
            AttachedObjects attachedObjects, int oldNum, int newNum)
        {
            string attachingObjectsStr = attachedObjects.Value;
            string[] attachingObjectsArr = attachingObjectsStr.Split(' ');
            for (int index = 0; index < attachingObjectsArr.Length; index++)
            {
                if (attachingObjectsArr[index] == newNum.ToString())
                {
                    attachingObjectsArr[index] = oldNum.ToString();
                }
                else if (attachingObjectsArr[index] == oldNum.ToString())
                {
                    attachingObjectsArr[index] = newNum.ToString();
                }
            }
            attachedObjects.SetValue(string.Join(" ", attachingObjectsArr));
        }

        public void RemoveAttachingToUnits(TechObject techObject)
        {
            int objNum = TechObjectManager.GetInstance()
                .GetTechObjectN(techObject);
            foreach (var obj in TechObjects)
            {
                RemoveAttachingToUnit(obj.AttachedObjects, objNum);

                if (obj.BaseTechObject?.ObjectGroupsList != null)
                {
                    foreach (var group in obj.BaseTechObject.ObjectGroupsList)
                    {
                        RemoveAttachingToUnit(group, objNum);
                    }
                }
            }
        }

        /// <summary>
        /// Удалить объект из привязки объекта
        /// </summary>
        /// <param name="attachedObjects">Элемент с привязанными объектами
        /// </param>
        /// <param name="objNum">Номер удаляемого объекта</param>
        private void RemoveAttachingToUnit(AttachedObjects attachedObjects,
            int objNum)
        {
            if (attachedObjects?.Value == string.Empty)
            {
                return;
            }

            List<int> attachedObjectsNums = attachedObjects.Value.Split(' ')
                .Select(int.Parse).ToList();
            if (attachedObjectsNums.Contains(objNum))
            {
                attachedObjectsNums.Remove(objNum);
                attachedObjects
                    .SetNewValue(string.Join(" ", attachedObjectsNums));
            }
        }
        #endregion

        /// <summary>
        /// Вставка базового объекта в редактор по LUA-имени базового объекта
        /// </summary>
        /// <param name="luaName"></param>
        public void InsertBaseObject(string luaName)
        {
            var baseObjectManager = BaseTechObjectManager.GetInstance();
            BaseTechObject foundBaseObject = baseObjectManager
                .GetTechObject(luaName);
            if (foundBaseObject != null)
            {
                string baseObjectTypeName = baseObjectManager
                    .GetS88Name(foundBaseObject.S88Level);
                var treeItem = GetTreeItem(baseObjectTypeName);
                if (treeItem != null)
                {
                    if (treeItem is S88Object s88Obj)
                    {
                        s88Obj.Insert(foundBaseObject.Name);
                    }
                    else
                    {
                        treeItem.Insert();
                    }

                    Editor.Editor.GetInstance().RefreshEditor();
                }
            }
        }

        /// <summary>
        /// Изменить базовый объект у объекта
        /// </summary>
        /// <param name="techObject"></param>
        public bool ChangeBaseObject(TechObject techObject)
        {
            return false;

            // 1. Удалить из S88Obj/ProcessCell/UserObject
            // 2. Если надо, удалить S88Obj/UserObject/ProcessCell
            // 3. Сбросить базовый объект
            // 4. Вставить в Unidentified

            // TODO: Создать форму, которая поможет изменить базовый объект
            // указывая различия
        }

        /// <summary>
        /// Объект мастера проекта.
        /// </summary>
        public TechObject ProcessCellObject
        {
            get
            {
                var masterObject = treeObjects.Where(x => x is ProcessCell)
                    .FirstOrDefault() as ProcessCell;
                if (masterObject != null)
                {
                    return masterObject.ProcessCellObject;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Получить количество аппаратов в проекте
        /// </summary>
        public int UnitsCount
        {
            get
            {
                int unitS88Level = (int)BaseTechObjectManager.ObjectType.Unit;
                var unitsCount = TechObjects
                    .Where(x => x.BaseTechObject != null)
                    .Where(x => x.BaseTechObject.S88Level == unitS88Level)
                    .Count();
                return unitsCount;
            }
        }

        /// <summary>
        /// Получить количество агрегатов в проекте
        /// </summary>
        public int EquipmentModulesCount
        {
            get
            {
                int aggregateS88Level = (int)BaseTechObjectManager.ObjectType
                    .Aggregate;
                var aggregatesCount = TechObjects
                    .Where(x => x.BaseTechObject != null)
                    .Where(x => x.BaseTechObject.S88Level == aggregateS88Level)
                    .Count();
                return aggregatesCount;
            }
        }

        /// <summary>
        /// Имя проекта.
        /// </summary>
        private string ProjectName { get;set; }

        /// <summary>
        /// Список всех технологических объектов в дереве.
        /// </summary>
        public List<TechObject> TechObjects
        {
            get
            {
                return techObjects;
            }
        }

        /// <summary>
        /// Единственный объект менеджера объектов.
        /// </summary>
        private static TechObjectManager instance;

        /// <summary>
        /// Список объектов дерева.
        /// </summary>
        private List<ITreeViewItem> treeObjects;

        /// <summary>
        /// Список всех технологических объектов в дереве.
        /// </summary>
        private List<TechObject> techObjects;

        /// <summary>
        /// Экземпляр LUA.
        /// </summary>
        private Lua lua;

        /// <summary>
        /// Класс для проверки технологических объектов.
        /// </summary>
        private TechObjectChecker techObjectManagerChecker;

        /// <summary>
        /// Класс, генерирующий информацию для базы каналов по объектам
        /// </summary>
        private TechObjectXMLMaker techObjectXMLMaker;
    }
}