using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEditor;

namespace NewTechObject
{
    /// <summary>
    /// Объект агрегатов проекта.
    /// </summary>
    public class Aggregate : TreeViewItem
    {
        public Aggregate()
        {
            objects = new List<ITreeViewItem>();
        }

        /// <summary>
        /// Добавление объекта при загрузке из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        public void AddObjectWhenLoadFromLua(TechObject obj)
        {
            var findedBaseObject = objects
                .Where(x => x.EditText[0] == obj.BaseTechObject.Name)
                .FirstOrDefault() as BaseObject;
            if(findedBaseObject == null)
            {
                findedBaseObject = new BaseObject(obj.BaseTechObject.Name);
                objects.Add(findedBaseObject);
            }

            findedBaseObject.AddObjectWhenLoadFromLua(obj);
        }

        #region реализация ITreeViewItem
        public override ITreeViewItem[] Items
        {
            get
            {
                return objects.ToArray();
            }
        }

        public override string[] DisplayText
        {
            get
            {
                if (Items.Length > 0)
                {
                    return new string[] { $"{name} ({Items.Length})", "" };
                }
                else
                {
                    return new string[] { name, "" };
                }
            }
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
            string selectedSubType = ObjectsAdder.LastSelectedSubType;
            if (selectedSubType == null)
            {
                var objectsAdderForm = new ObjectsAdder(name);
                objectsAdderForm.ShowDialog();
                string subType = ObjectsAdder.LastSelectedSubType;
                if (subType != null)
                {
                    var insertedItem = InsertSubType(subType);
                    return insertedItem;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var insertedItem = InsertSubType(selectedSubType);
                return insertedItem;
            }
        }

        override public bool Delete(object child)
        {
            var baseObject = child as BaseObject;
            if (baseObject != null)
            {
                objects.Remove(baseObject);
                if (objects.Count == 0)
                {
                    Parent.Delete(this);
                }
                return true;
            }

            return false;
        }
        #endregion

        /// <summary>
        /// Вставить подтип в дерево.
        /// </summary>
        /// <param name="selectedSubType">Выбранный подтип на форме</param>
        /// <returns></returns>
        private ITreeViewItem InsertSubType(string selectedSubType)
        {
            ITreeViewItem baseTreeItem = GetTreeItem(selectedSubType);
            var currentObject = baseTreeItem.Insert();
            if (currentObject != null)
            {
                if (!objects.Contains(baseTreeItem))
                {
                    objects.Add(baseTreeItem);
                }

                return baseTreeItem;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Получить объект дерева, описывающий базу по S88.
        /// </summary>
        /// <param name="selectedSubType">Выбранный на форме подтип объекта
        /// </param>
        /// <returns></returns>
        private ITreeViewItem GetTreeItem(string selectedSubType)
        {
            var item = objects
                .Where(x => x.DisplayText[0].Contains(selectedSubType))
                .FirstOrDefault();
            if (item != null)
            {
                return item;
            }
            else
            {
                return new BaseObject(selectedSubType);
            }
        }

        string name = "Агрегат";
        List<ITreeViewItem> objects;
    }
}