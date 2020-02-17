using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Класс, содержащий оборудование технологического объекта
    /// </summary>
    public class Equipment : Editor.TreeViewItem
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="owner">Владелец</param>
        public Equipment(TechObject owner)
        {
            this.owner = owner;
            items = new List<Editor.ITreeViewItem>();
        }

        /// <summary>
        /// Очистить список оборудования
        /// </summary>
        public void Clear()
        {
            items.Clear();
        }
        
        /// <summary>
        /// Добавить оборудование
        /// </summary>
        /// <param name="properties">Список оборудования</param>
        public void AddItems(BaseProperty[] properties)
        {
            foreach(BaseProperty property in properties)
            {
                items.Add(property);
            }
        }

        /// <summary>
        /// Добавить оборудование
        /// </summary>
        /// <param name="property">Оборудование</param>
        public void AddItem(BaseProperty property)
        {
            items.Add(property);
        }

        /// <summary>
        /// Копировать оборудование
        /// </summary>
        /// <param name="clone">Новый владелец</param>
        /// <returns></returns>
        public Equipment Clone(TechObject clone)
        {
            var equipment = new Equipment(clone);

            foreach(Editor.ITreeViewItem item in items)
            {
                var property = item as BaseProperty;
                equipment.AddItem(property.Clone());
            }

            return equipment;
        }

        #region Реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                if (items.Count() > 0)
                {
                    string res = string.Format("Оборудование ({0})",
                        items.Count());
                    return new string[] { res, "" };
                }
                else
                {
                    string res = string.Format("Оборудование");
                    return new string[] { res, "" };
                }
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }
        #endregion

        private TechObject owner;
        private List<Editor.ITreeViewItem> items;
    }
}
