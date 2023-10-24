﻿using BrightIdeasSoftware;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor
{
    /// <summary>
    /// Редактируемый объект - дерево элемента.
    /// </summary>
    public abstract class TreeViewItem : ITreeViewItem, IHelperItem
    {
        private ITreeViewItem parent;

        #region Реализация ITreeViewItem
        public ITreeViewItem Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        virtual public string[] DisplayText
        {
            get
            {
                return new string[] { "", "" };
            }
        }

        virtual public ITreeViewItem[] Items
        {
            get
            {
                return null;
            }
        }

        virtual public bool SetNewValue(string newName)
        {
            return false;
        }

        virtual public bool SetNewValue(IDictionary<int, List<int>> newDict)
        {
            return false;
        }

        virtual public bool SetNewValue(string newVal, bool isExtraValue)
        {
            return false;
        }

        virtual public bool IsEditable
        {
            get
            {
                return false;
            }
        }

        virtual public int[] EditablePart
        {
            get
            {
                return new int[] { -1, -1 };
            }
        }

        virtual public string[] EditText
        {
            get
            {
                return new string[] { "", "" };
            }
        }

        virtual public bool IsDeletable
        {
            get
            {
                return false;
            }
        }

        virtual public bool IsCopyable
        {
            get
            {
                return false;
            }
        }

        virtual public object Copy()
        {
            if(IsCopyable)
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        virtual public bool IsMoveable
        {
            get
            {
                return false;
            }
        }

        virtual public bool CanMoveUp(object child) 
            => (child as ITreeViewItem).IsMoveable;

        virtual public bool CanMoveDown(object child) 
            => (child as ITreeViewItem).IsMoveable;

        virtual public bool IsReplaceable
        {
            get
            {
                return false;
            }
        }

        virtual public ITreeViewItem MoveDown(object child)
        {
            return null;
        }

        virtual public ITreeViewItem MoveUp(object child)
        {
            return null;
        }

        virtual public bool IsInsertableCopy
        {
            get
            {
                return false;
            }
        }

        virtual public ITreeViewItem InsertCopy(object obj)
        {
            return null;
        }

        virtual public ITreeViewItem Replace(object child, object copyObject)
        {
            return null;
        }

        virtual public bool Delete(object child)
        {
            return false;
        }

        virtual public bool IsInsertable
        {
            get
            {
                return false;
            }
        }

        virtual public ITreeViewItem Insert()
        {
            return null;
        }

        virtual public bool IsUseDevList
        {
            get
            {
                return false;
            }
        }

        virtual public bool IsLocalRestrictionUse
        {
            get
            {
                return false;
            }
        }

        virtual public bool IsDrawOnEplanPage
        {
            get { return false; }
        }

        virtual public List<DrawInfo> GetObjectToDrawOnEplanPage()
        {
            return null;
        }

        virtual public void GetDisplayObjects(out EplanDevice.DeviceType[] devTypes,
            out EplanDevice.DeviceSubType[] devSubTypes, out bool displayParameters)
        {
            devTypes = null;
            devSubTypes = null;
            displayParameters = false;
        }

        public virtual bool NeedRebuildParent
        {
            get { return false; }
        }

        public virtual bool IsBoolParameter
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsMainObject
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsMode
        {
            get
            {
                return false;
            }
        }

        public virtual bool ShowWarningBeforeDelete
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Установка родителя для элемента
        /// </summary>
        /// <param name="parent">Родительский элемент</param>
        public void AddParent(ITreeViewItem parent)
        {
            Parent = parent;
            if (Items != null)
            {
                foreach (ITreeViewItem item in Items)
                {
                    item.AddParent(this);
                }
            }
        }

        /// <summary>
        /// Нужно ли отключить элемент
        /// </summary>
        /// <returns></returns>
        public virtual bool NeedDisable
        {
            get
            {
                return needDisable;
            }
            set
            {
                needDisable = value;
            }
        }

        private bool needDisable = false;

        /// <summary>
        /// Заполнен или нет элемент дерева
        /// True - отображать
        /// False - скрывать
        /// </summary>
        public virtual bool IsFilled
        {
            get
            {
                return (EditText[1].Length > 0 && EditText[1] != CommonConst.EmptyValue) ||
                    (Items != null && Array.Exists(Items, item => item.IsFilled));
            }
        }

        public virtual bool Contains(string value)
        {
            value = value.Trim().ToUpper();
            MarkedAsFound = DisplayText[0].ToUpper().Contains(value) ||
                DisplayText[1].ToUpper().Contains(value) ||
                EditText[0].ToUpper().Contains(value) ||
                EditText[1].ToUpper().Contains(value) ||
                ((Parent as TreeViewItem)?.MarkedAsFound ?? false);

            return MarkedAsFound || (Items != null && Array.Exists(Items, item => item.Contains(value)));
        }

        public virtual bool ContainsAndIsFilled(string value)
        {
            value = value.Trim().ToUpper();
            MarkedAsFound = DisplayText[0].ToUpper().Contains(value) ||
                DisplayText[1].ToUpper().Contains(value) ||
                EditText[0].ToUpper().Contains(value) ||
                EditText[1].ToUpper().Contains(value) ||
                ((Parent as TreeViewItem)?.MarkedAsFound ?? false);
            MarkedAsFound = MarkedAsFound && IsFilled;

            return MarkedAsFound || (Items != null && Array.Exists(Items, item => item.ContainsAndIsFilled(value)));
        }

        /// <summary>
        /// Флаг, указывающий что данный элемент или его родительские элементы,
        /// содержат искомую строку
        /// </summary>
        public bool MarkedAsFound { get; set; }
           

        public virtual ImageIndexEnum ImageIndex { get; set; } =
            ImageIndexEnum.NONE;

        public virtual bool ContainsBaseObject
        {
            get
            {
                return false;
            }
        }

        public virtual List<string> BaseObjectsList
        {
            get
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Отключено или нет свойство
        /// </summary>
        public bool Disabled { get; set; }

        public bool MarkToCut { get; set; }

        public virtual ITreeViewItem Cut(ITreeViewItem item)
        {
            return null;
        }

        public virtual bool IsCuttable { get; } = false;

        private OnValueChanged valueChanged;
        public event OnValueChanged ValueChanged
        {
            add
            {
                valueChanged = null;
                valueChanged += value;
            }
            remove
            {
                valueChanged -= value;
            }
        }

        public virtual IRenderer[] CellRenderer => new IRenderer[] { null, null };

        public void OnValueChanged(object sender)
        {
            valueChanged?.Invoke(sender);
        }
        #endregion

        public virtual void UpdateOnGenericTechObject(ITreeViewItem genericObject)
        {

        }

        #region реализация IHelperItem
        /// <summary>
        /// Получить ссылку на страницу с справкой
        /// </summary>
        /// <returns></returns>
        public virtual string GetLinkToHelpPage()
        {
            return null;
        }
        #endregion
    }

    public delegate void OnValueChanged(object sender);
}
