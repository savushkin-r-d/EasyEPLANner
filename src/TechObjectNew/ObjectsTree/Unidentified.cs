using NewEditor;
using System.Collections.Generic;

namespace NewTechObject
{
    /// <summary>
    /// Неопознанные объекты
    /// </summary>
    public class Unidentified : TreeViewItem
    {
        public Unidentified() 
        {
            localObjects = new List<TechObject>();
            globalObjectsList = TechObjectManager.GetInstance().Objects;
        }

        /// <summary>
        /// Проверка и исправление ограничений при удалении/перемещении объекта
        /// </summary>
        public void CheckRestriction(int oldNum, int newNum)
        {
            foreach (TechObject techObject in globalObjectsList)
            {
                techObject.CheckRestriction(oldNum, newNum);
            }
        }

        /// <summary>
        /// Изменение номеров владельцев ограничений
        /// </summary>
        public void SetRestrictionOwner()
        {
            foreach (TechObject techObject in localObjects)
            {
                techObject.SetRestrictionOwner();
            }
        }

        /// <summary>
        /// Добавляем неопознанный объект в список
        /// </summary>
        /// <param name="obj">Объект</param>
        public void AddUnidentifiedObject(TechObject obj)
        {
            obj.SetGetLocalN(GetTechObjectLocalNum);
            localObjects.Add(obj);
        }

        #region реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                if(Items.Length > 0)
                {
                    return new string[] { $"{name} ({localObjects.Count})", "" };
                }
                else
                {
                    return new string[] { name, "" };
                }
            }
        }

        public override ITreeViewItem[] Items
        {
            get
            {
                return localObjects.ToArray();
            }
        }

        override public bool Delete(object child)
        {
            const int markAsDelete = -1;
            var techObject = child as TechObject;
            if (techObject != null)
            {
                int globalIndex = globalObjectsList.IndexOf(techObject) + 1;
                CheckRestriction(globalIndex, markAsDelete);

                // Работа со списком в дереве и общим списком объектов.
                localObjects.Remove(techObject);
                globalObjectsList.Remove(techObject);

                // Обозначение начального номера объекта для ограничений.
                SetRestrictionOwner();

                if(localObjects.Count == 0)
                {
                    Parent.Delete(this);
                }
                return true;
            }

            return false;
        }

        public override bool IsCuttable
        {
            get
            {
                return true;
            }
        }

        public override ITreeViewItem Cut(ITreeViewItem item)
        {
            var techObject = item as TechObject;
            if (techObject != null)
            {
                localObjects.Remove(techObject);

                if(localObjects.Count == 0)
                {
                    Parent.Delete(this);
                }

                return techObject;
            }

            return null;
        }
        #endregion

        /// <summary>
        /// Получить локальный номер технологического объекта
        /// </summary>
        /// <param name="searchingObject">Искомый объект</param>
        /// <returns></returns>
        private int GetTechObjectLocalNum(object searchingObject)
        {
            var techObject = searchingObject as TechObject;
            int num = localObjects.IndexOf(techObject) + 1;
            return num;
        }

        string name = "Неопознанные объекты";
        List<TechObject> localObjects;
        List<TechObject> globalObjectsList;
    }
}
